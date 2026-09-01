using AppMvp.ApplicationCore.Commands;
using AppMvp.ApplicationCore.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using AppMvp.Presentation.Abstractions;

namespace AppMvp.Presentation.ViewModels
{
    public sealed class MainFormViewModel
    {
        private readonly IRegionNavigationPresenter _nav;

        public MainFormViewModel(IRegionNavigationPresenter nav)
        {
            _nav = nav;
        }

        //public void ShowHome()
        //{
        //    _nav.NavigateToRegion("ContentRegion", typeof(HomeView));
        //}

        public void ShowPeople()
        {
            _nav.NavigateToRegion("ContentRegion", "PeopleView");
        }

        //public void ShowOrders()
        //{
        //    _nav.NavigateToRegion("ContentRegion", typeof(OrdersView));
        //}

        //public void ShowSettings()
        //{
        //    _nav.NavigateToRegion("ContentRegion", typeof(SettingsView));
        //}
    }

}
