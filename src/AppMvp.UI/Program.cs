using AppMvp.ApplicationCore.CommandBus;
using Microsoft.Extensions.Logging;
using AppMvp.ApplicationCore.CommandHandlers;
using AppMvp.ApplicationCore.EventBus;

using AppMvp.Domain.Repositories;
using AppMvp.Infrastructure.Persistence;
using AppMvp.Presentation;
using AppMvp.Presentation.Abstractions;
using AppMvp.Presentation.Navigation;
using AppMvp.Presentation.ViewModels;
using AppMvp.UI.Forms;
using AppMvp.UI.Navigation;
using AppMvp.UI.Registry;
using AppMvp.UI.Views;
using AppMvp.Presentation.Services;
using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows.Forms;

namespace AppMvp.UI;

static class Program
{
    [STAThread]
    static void Main()
    {
        // Load configuration (appsettings.json + environment variables) and configure Serilog
        var baseDir = AppContext.BaseDirectory;
        var configPath = System.IO.Path.Combine(baseDir, "appsettings.json");
        var devConfigPath = System.IO.Path.Combine(baseDir, "appsettings.Development.json");
        var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .AddJsonFile(configPath, optional: true, reloadOnChange: true)
            .AddJsonFile(devConfigPath, optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        var host = Host.CreateDefaultBuilder()
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                // ----------------------------
                // VIEWMODEL
                // ----------------------------
                services.AddSingleton<MainFormViewModel>();
                // Use a single shared PeopleViewModel instance so UI handlers and the view operate on the same viewmodel
                services.AddSingleton<PeopleViewModel>();
                // Ensure logging services are available to views and services resolved from DI
                services.AddLogging();
                // MainViewModel(IMediator mediator) is resolved automatically

                // ----------------------------
                // FORMS
                // ----------------------------
                services.AddSingleton<MainForm>();

                // ----------------------------
                // VIEWS
                // ----------------------------         
                services.AddTransient<PeopleView>(sp =>
                {
                    var vm = sp.GetRequiredService<PeopleViewModel>();
                    var busy = sp.GetRequiredService<AppMvp.Presentation.Abstractions.IBusyIndicator>();
                    var logger = sp.GetService<Microsoft.Extensions.Logging.ILogger<PeopleView>>();
                    return new PeopleView(vm, busy, logger);
                });
                services.AddTransient<PersonEditForm>();


                // ----------------------------
                // VIEW REGISTRY
                // ----------------------------
                services.AddSingleton<IViewRegistry, ViewRegistry>(sp =>
                {
                    var registry = new ViewRegistry();
                    registry.Register<PeopleView>("PeopleView");
                    //registry.Register<OrdersView>("OrdersView");
                    //registry.Register<SettingsView>("SettingsView");
                    return registry;
                });


                // ----------------------------
                // NAVIGATION
                // ----------------------------
                services.AddSingleton<IFormNavigator, WinFormsNavigator>();
                services.AddSingleton<IRegionHost, WinFormsRegionHost>();
                services.AddSingleton<IRegionNavigator, WinFormsRegionNavigator>();
                services.AddSingleton<IRegionNavigationPresenter, RegionNavigationPresenter>();

                // ----------------------------
                // UI BUSY INDICATOR
                // ----------------------------
                services.AddSingleton<IBusyIndicator, BusyIndicatorService>();
                // UI Dispatcher (lazy capture of SynchronizationContext)
                services.AddSingleton<AppMvp.Presentation.Abstractions.IUiDispatcher, AppMvp.UI.Services.UiDispatcher>();
                // Error dialog implementation
                services.AddSingleton<AppMvp.Presentation.Abstractions.IErrorDialog, AppMvp.UI.Services.ErrorDialog>();


                // ----------------------------
                // APPLICATION SERVICES
                // ----------------------------
                services.AddSingleton<ICommandBus, MediatrCommandBus>();
                services.AddSingleton<IApplicationEventBus, MediatrApplicationEventBus>();

                // ----------------------------
                // REPOSITORIES
                // ----------------------------
                services.AddSingleton<IPersonRepository, PersonRepository>();

                // ----------------------------
                // MEDIATR v12+
                // ----------------------------
                services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(typeof(LoadPeopleCommandHandler).Assembly);
                    // Also register handlers from the Presentation and UI assemblies (handlers)
                    cfg.RegisterServicesFromAssembly(typeof(AppMvp.Presentation.ViewModels.PeopleViewModel).Assembly);
                    cfg.RegisterServicesFromAssembly(typeof(MainForm).Assembly);
                });

                // Optional hosted services
                // services.AddHostedService<WorkerService>();
            })
            .Build();

        // Start background services (if any)
        host.Start();

        ApplicationConfiguration.Initialize();

        // Create a WindowsFormsSynchronizationContext now and set it as the current context
        var wfContext = new System.Windows.Forms.WindowsFormsSynchronizationContext();
        System.Threading.SynchronizationContext.SetSynchronizationContext(wfContext);

        // Resolve and configure services that depend on the UI synchronization context
        var busy = host.Services.GetRequiredService<AppMvp.Presentation.Abstractions.IBusyIndicator>();
        busy.SetSynchronizationContext(System.Threading.SynchronizationContext.Current);

        var uiDispatcher = host.Services.GetRequiredService<AppMvp.Presentation.Abstractions.IUiDispatcher>();
        // Configure telemetry client for error reporting (unhandled exceptions)
        // Prefer Serilog-based telemetry client so reports flow through configured sinks (file/seq/sentry)
        var serilogTelemetry = new AppMvp.UI.Services.SerilogTelemetryClient();
        AppMvp.UI.Services.ErrorReporter.SetTelemetryClient(serilogTelemetry);
        // Register the DI-created error dialog with ErrorReporter so it can be reused
        var errorDialog = host.Services.GetRequiredService<AppMvp.Presentation.Abstractions.IErrorDialog>();
        AppMvp.UI.Services.ErrorReporter.SetErrorDialog(errorDialog);

        var loggerFactory = host.Services.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>();
        var globalLogger = loggerFactory.CreateLogger("AppMvp.Global");

        // Global UI thread exception handler -> show error dialog via ErrorReporter
        Application.ThreadException += (s, e) =>
        {
            try
            {
                globalLogger.LogError(e.Exception, "Unhandled UI thread exception");
                _ = AppMvp.UI.Services.ErrorReporter.ShowErrorAndLogAsync("An unexpected error occurred.", e.Exception, globalLogger, null);
            }
            catch { }
        };

        // Non-UI thread exceptions
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            try
            {
                var ex = e.ExceptionObject as Exception;
                globalLogger.LogCritical(ex, "Unhandled AppDomain exception");
                _ = AppMvp.UI.Services.ErrorReporter.ShowErrorAndLogAsync("A fatal error occurred.", ex, globalLogger, null);
            }
            catch { }
        };

        // Task scheduler unobserved exceptions
        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            try
            {
                globalLogger.LogError(e.Exception, "Unobserved task exception");
                _ = AppMvp.UI.Services.ErrorReporter.ShowErrorAndLogAsync("An error occurred in background work.", e.Exception, globalLogger, null);
                e.SetObserved();
            }
            catch { }
        };

        var mainForm = host.Services.GetRequiredService<MainForm>();
        try
        {
            Application.Run(mainForm);
        }
        finally
        {
            // Stop host and flush logs
            host.StopAsync().Wait();
            Log.CloseAndFlush();
        }
    }
}
