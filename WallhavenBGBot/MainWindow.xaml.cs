using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WallhavenBGBot.Helpers;
using WallhavenBGBot.Models;

namespace WallhavenBGBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BGBotViewModel _viewModel = BGBotViewModel.ViewModel;
        private CancellationTokenSource _tokenSource;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = _viewModel;
        }

        protected void BtnOrderClicked(object sender, EventArgs e)
        {
            _viewModel.SelectedOrder = _viewModel.SelectedOrder == WallhavenAPI.SortOrder.Asc ? WallhavenAPI.SortOrder.Desc : WallhavenAPI.SortOrder.Asc;
        }

        private Task StartSwitchLoop(TimeSpan interval, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                while(true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await SwitchBackground();
                    await Task.Delay(interval, cancellationToken);
                }
            });
        }

        private async Task SwitchBackground()
        {
            var query = BGBotViewModel.GetQuery();

            if (!WallhavenAPI.API.IsLoggedIn())
            {
                if (!String.IsNullOrEmpty(_viewModel.Username) && !String.IsNullOrEmpty(_viewModel.Password))
                    WallhavenAPI.API.Login(_viewModel.Username, _viewModel.Password);
            }

            var results = WallhavenAPI.API.Search(query);
            if (results.Any())
            {
                var img = WallhavenAPI.API.GetFile(results[0]);
                var filepath = @"F:\bgs\" + results[0].Split('/').Last();
                File.WriteAllBytes(filepath, img);

                BackgroundHelper.UpdateBackground(filepath);
            }
        }

        protected async void BtnSaveConfigClicked(object sender, EventArgs e)
        {
            var errors = BGBotViewModel.GetErrors();
            if (errors.Count > 0)
            {
                MessageBox.Show(String.Join("\n", errors), "Configuration error");
                return;
            }

            BGBotViewModel.Save();
            if (_viewModel.Interval > 0)
            {
                if (_tokenSource != null)
                    _tokenSource.Cancel();

                _tokenSource = new CancellationTokenSource();
                await StartSwitchLoop(TimeSpan.FromMinutes(_viewModel.Interval), _tokenSource.Token);
            }
        }

        protected async void BtnSetBackgroundClicked(object sender, EventArgs e)
        {
            var errors = BGBotViewModel.GetErrors();
            if (errors.Count > 0)
            {
                MessageBox.Show(String.Join("\n", errors), "Configuration error");
                return;
            }

            BGBotViewModel.Save();
            await SwitchBackground();
        }
    }
}
