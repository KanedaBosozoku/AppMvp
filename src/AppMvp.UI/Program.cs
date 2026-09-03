using AppMvp.ApplicationCore.CommandBus;
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
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // ----------------------------
                // VIEWMODEL
                // ----------------------------
                services.AddSingleton<MainFormViewModel>();
                // Use a single shared PeopleViewModel instance so UI handlers and the view operate on the same viewmodel
                services.AddSingleton<PeopleViewModel>();
                // MainViewModel(IMediator mediator) is resolved automatically

                // ----------------------------
                // FORMS
                // ----------------------------
                services.AddSingleton<MainForm>();

                // ----------------------------
                // VIEWS
                // ----------------------------         
                services.AddTransient<PeopleView>();
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

        var mainForm = host.Services.GetRequiredService<MainForm>();
        Application.Run(mainForm);

        host.StopAsync().Wait();
    }
}
