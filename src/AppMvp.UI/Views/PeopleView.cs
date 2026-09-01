using AppMvp.Presentation.Abstractions;
using AppMvp.Presentation.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppMvp.UI.Views
{
    public partial class PeopleView : UserControl, IViewWithParameter
    {
        private readonly PeopleViewModel _vm;
        private readonly CancellationTokenSource _cts = new();

        public PeopleView(PeopleViewModel vm)
        {
            InitializeComponent();
            _vm = vm;

            _ = LoadPeopleAsync();   // fire-and-forget async load
            this.Disposed += (s, e) => Cleanup(); // Cancel any ongoing operations when the view is disposed
        }

        private async Task LoadPeopleAsync()
        {
            try
            {
                await _vm.LoadPeopleAsync(_cts.Token);

                lstPeople.DataSource = _vm.People;
                lstPeople.DisplayMember = nameof(PersonViewModel.DisplayName);
            }
            catch (OperationCanceledException)
            {
                // ignore — view was disposed
            }
        }

        public async void ReceiveParameter(object parameter)
        {
            if (parameter is int personId)
            {
                try
                {
                    await _vm.LoadPersonAsync(personId, _cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // ignore — view was disposed
                }
            }
        }

        private void Cleanup()
        {
            // 🔥 Your cleanup logic here
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}