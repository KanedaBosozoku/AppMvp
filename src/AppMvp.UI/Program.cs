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
                services.AddTransient<PeopleViewModel>();
                // MainViewModel(IMediator mediator) is resolved automatically

                // ----------------------------
                // FORMS
                // ----------------------------
                services.AddSingleton<MainForm>();
                //services.AddSingleton<MainForm>(sp =>
                //    new MainForm(
                //        sp.GetRequiredService<MainFormViewModel>(),
                //        sp.GetRequiredService<IRegionNavigationPresenter>(),
                //        sp.GetRequiredService<IRegionHost>(),
                //        sp.GetRequiredService<IBusyIndicator>()
                //    ));


                // ----------------------------
                // VIEWS
                // ----------------------------         
                services.AddTransient<PeopleView>();


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
                // PRESENTERS
                // ----------------------------
                //services.AddSingleton<IPeoplePresenter, PeoplePresenter>();
                // PeoplePresenter(MainViewModel vm) is resolved automatically

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
                });

                // Optional hosted services
                // services.AddHostedService<WorkerService>();
            })
            .Build();

        // Start background services (if any)
        host.Start();

        ApplicationConfiguration.Initialize();

        // Create a WindowsFormsSynchronizationContext now and register it with the busy indicator
        // so UI components don't need to set it manually in their Shown handlers.
        var wfContext = new System.Windows.Forms.WindowsFormsSynchronizationContext();
        System.Threading.SynchronizationContext.SetSynchronizationContext(wfContext);

        // Resolve MainForm and set the busy indicator's synchronization context before running the message loop
        var busy = host.Services.GetRequiredService<AppMvp.Presentation.Abstractions.IBusyIndicator>();
        busy.SetSynchronizationContext(System.Threading.SynchronizationContext.Current);

        var mainForm = host.Services.GetRequiredService<MainForm>();
        Application.Run(mainForm);

        host.StopAsync().Wait();
    }
}
