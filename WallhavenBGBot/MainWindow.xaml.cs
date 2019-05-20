using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WallhavenBGBot.Helpers;
using WallhavenBGBot.Models;

namespace WallhavenBGBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static object _lockObject = new object();

        private BGBotViewModel _viewModel;
        private System.Timers.Timer _timer;
        private System.Windows.Forms.NotifyIcon _notifyIcon;

        public MainWindow()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon() {
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location),
                Visible = true,
            };

            _notifyIcon.Click += (sender, e) => { Show(); WindowState = WindowState.Normal; };

            InitializeComponent();
            _viewModel = (BGBotViewModel)DataContext;

            if (_viewModel.Interval > 0)
                StartSwitchLoop(TimeSpan.FromMinutes(_viewModel.Interval));
        }

        private void StartSwitchLoop(TimeSpan interval)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }

            _timer = new System.Timers.Timer(interval.Minutes * 60 * 1000);
            _timer.Elapsed += (o,e) =>
            {
                SwitchBackground();
            };
            _timer.Start();
        }

        private void SwitchBackground()
        {
            lock(_lockObject)
            {
                var query = _viewModel.GetQuery();

                if (!WallhavenAPI.API.IsLoggedIn() && !String.IsNullOrEmpty(_viewModel.Username) && !String.IsNullOrEmpty(_viewModel.Password))
                {
                    if (!WallhavenAPI.API.Login(_viewModel.Username, _viewModel.Password))
                    {
                        MessageBox.Show("Couldn't log in with the credentials supplied, please try again, or clear the credential boxes.");
                        return;
                    }
                }

                var results = WallhavenAPI.API.Search(query);
                if (results.Any())
                {
                    var img = WallhavenAPI.API.GetFile(results[0]);
                    var imageFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WallhavenBGBot");

                    if (!Directory.Exists(imageFolder))
                        Directory.CreateDirectory(imageFolder);

                    var filepath = Path.Combine(imageFolder, results[0].Split('/').Last());
                    File.WriteAllBytes(filepath, img);

                    BackgroundHelper.UpdateBackground(filepath);

                    if (_viewModel.AutomaticallyCleanAppDataFolder)
                    {
                        var oldFiles = Directory.GetFiles(imageFolder).Where(x => x != filepath).ToList();
                        foreach (var item in oldFiles)
                            File.Delete(item);
                    }
                }
            }
        }

        protected void BtnSaveConfigClicked(object sender, EventArgs e)
        {
            var errors = _viewModel.GetErrors();
            if (errors.Count > 0)
            {
                MessageBox.Show(String.Join("\n", errors), "Configuration error");
                return;
            }

            _viewModel.Save();
            if (_viewModel.Interval > 0)
                StartSwitchLoop(TimeSpan.FromMinutes(_viewModel.Interval));
            else if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }

        protected void BtnSetBackgroundClicked(object sender, EventArgs e)
        {
            var errors = _viewModel.GetErrors();
            if (errors.Count > 0)
            {
                MessageBox.Show(String.Join("\n", errors), "Configuration error");
                return;
            }

            _viewModel.Save();
            SwitchBackground();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();

            base.OnStateChanged(e);
        }
    }
}
