using AppMvp.Presentation.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AppMvp.UI.Views
{
    public partial class EmptyView : UserControl, IViewWithParameter, AppMvp.Presentation.Abstractions.IAsyncView
    {
        public EmptyView()
        {
            InitializeComponent();
        }

        public Task ActivateAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void ReceiveParameter(object parameter)
        {
            // Handle the received parameter
        }
    }
}
