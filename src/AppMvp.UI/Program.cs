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

        //// Resolve MainViewModel from DI
        //var vm = host.Services.GetRequiredService<MainViewModel>();

        //// Create the form manually, injecting the ViewModel
        //var form = new Form1(vm);

        //Application.Run(form);

        var mainForm = host.Services.GetRequiredService<MainForm>();
        Application.Run(mainForm);

        host.StopAsync().Wait();
    }
}
