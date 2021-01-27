using AmongServers.Launcher.Coordinator.Entities;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography;
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

namespace AmongServers.Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private ApiClient _client;

        class ServerItem
        {
            public string Name { get; set; }
            public string IPAddressPort { get; set; }
            public bool IsSaved { get; set; }
            public int CountPlayers { get; set; }
            public int CountLobbies { get; set; }

            public IPEndPoint Endpoint { get; set; }
        }

        public MainWindow()
        {
            // setup client
            _client = new ApiClient(Constants.ApiUrl);

            // initialise
            InitializeComponent();
        }

        /// <summary>
        /// Refreshes the banner image.
        /// </summary>
        /// <returns></returns>
        public async Task RefreshBannerAsync(CancellationToken cancellationToken = default)
        {
            BannerEntity[] banners = null;
            BannerEntity activeBanner = new BannerEntity() {
                Link = "https://amongservers.com",
                Text = "Visit AmongServers.com!"
            };

            // load the banners from the API
            try {
                banners = await _client.ListBannersAsync(Constants.ApplicationVersion, cancellationToken);
            } catch (Exception ex) {
                Debug.Fail(ex.ToString());
            }

            // pick a banner from the service or use the default
            if (banners != null && banners.Length > 0) {
                activeBanner = banners[RandomNumberGenerator.GetInt32(banners.Length)];
            }

            // load the image
            if (!string.IsNullOrEmpty(activeBanner.ImageUrl)) {
                imgBanner.Source = new BitmapImage(new Uri(activeBanner.ImageUrl));
            }

            imgBanner.Cursor = Cursors.Hand;
            btnBanner.Cursor = Cursors.Hand;
            imgBanner.DataContext = activeBanner;
            btnBanner.DataContext = activeBanner;
        }

        /// <summary>
        /// Refresh the server list.
        /// </summary>
        /// <returns></returns>
        public async Task RefreshServersAsync(CancellationToken cancellationToken = default)
        {
            // load the servers from the API
            ServerEntity[] servers = Array.Empty<ServerEntity>();

            try {
                servers = await _client.ListServersAsync(cancellationToken);
            } catch (Exception ex) {
                Debug.Fail(ex.ToString());
                return;
            }

            servers = servers.Concat(new[] {
                new ServerEntity() {
                    Name = "My Among Server",
                    Endpoint = "144.56.23.9:32323",
                    LastSeenAt = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(RandomNumberGenerator.GetInt32(5)),
                    Games = Array.Empty<GameEntity>()
                },
                 new ServerEntity() {
                    Name = "Potato lobby, come join!",
                    Endpoint = "141.2.134.1:5623",
                    LastSeenAt = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(RandomNumberGenerator.GetInt32(10)),
                    Games = new[] {
                        new GameEntity() {
                            State = "started",
                            CountPlayers = 10,
                            NumImposters = 2,
                            HostPlayer = null,
                            IsPublic = false,
                            Map = "polus",
                            MaxPlayers = 10,
                            Players = Array.Empty<PlayerEntity>()
                        }
                    }
                }
            }).ToArray();

            // set the item source
            listServers.ItemsSource = servers.Where(s => IPEndPoint.TryParse(s.Endpoint, out IPEndPoint _))
            .OrderBy(s=> s.Name).Select(s => new ServerItem() {
                Name = s.Name,
                IPAddressPort = $"{s.Endpoint ?? "N/A"}",
                IsSaved = false,
                CountPlayers = s.Games == null ? 0 : s.Games.Sum(g => g.CountPlayers),
                CountLobbies = s.Games == null ? 0 : s.Games.Length,
                Endpoint = IPEndPoint.Parse(s.Endpoint)
            });

            // setup filter
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listServers.ItemsSource);
            view.Filter = listServers_Filter;
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            lblVersion.Text = $"Version {Constants.ApplicationVersion}";
            _ = RefreshBannerAsync();
            btnRefresh.IsEnabled = false;
           await RefreshServersAsync();
            btnRefresh.IsEnabled = true;
        }

        private void btnBanner_Click(object sender, RoutedEventArgs e)
        {
            BannerEntity banner = (BannerEntity)btnBanner.DataContext;

            if (banner == null)
                return;
            if (string.IsNullOrEmpty(banner.Link))
                return;

            Process.Start(new ProcessStartInfo(banner.Link) {
                UseShellExecute = true
            });
        }

        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            btnRefresh.IsEnabled = false;
            await RefreshServersAsync();
            btnRefresh.IsEnabled = true;
        }

        private void btnOpenWebsite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(Constants.WebsiteUrl) {
                UseShellExecute = true
            });
        }

        private async void btnDirectPlay_Click(object sender, RoutedEventArgs e)
        {
            // validate the server endpoint is correct
            if (!IPEndPoint.TryParse(txtDirectPlay.Text, out IPEndPoint serverEndpoint) || serverEndpoint.Port == 0) {
                MessageBox.Show("The direct play address is invalid", "AmongServers", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // change the region file and launch the game
            btnDirectPlay.IsEnabled = false;

            try {
                await Bootstrapper.ReplaceRegionInfoAsync("AmongServers", serverEndpoint);
                await Bootstrapper.LaunchGameAsync();
            } catch(Exception ex) {
                MessageBox.Show($"An error occured launching the game{Environment.NewLine}{Environment.NewLine}{ex.ToString()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            btnDirectPlay.IsEnabled = true;
        }

        private void txtDirectPlay_TextChanged(object sender, TextChangedEventArgs e)
        {
            // check if the endpoint is valid
            bool isValidIp = IPEndPoint.TryParse(txtDirectPlay.Text, out IPEndPoint _);

            // if it's empty or valid we leave the default border brush
            if (string.IsNullOrEmpty(txtDirectPlay.Text) || isValidIp) {

            } else {
            }

            // enable the play button
            btnDirectPlay.IsEnabled = isValidIp;
        }

        private async void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            // get the button instance
            Button button = (Button)sender;

            // get the data context
            if (button.DataContext is ServerItem serverItem) {
                button.IsEnabled = false;

                try {
                    await Bootstrapper.ReplaceRegionInfoAsync(serverItem.Name, serverItem.Endpoint);
                    await Bootstrapper.LaunchGameAsync();
                } catch (Exception ex) {
                    MessageBox.Show($"An error occured launching the game{Environment.NewLine}{Environment.NewLine}{ex.ToString()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                button.IsEnabled = true;
            }
        }

        private bool listServers_Filter(object o)
        {
            if (!string.IsNullOrWhiteSpace(txtFilter.Text) && o is ServerItem serverItem) {
                return serverItem.Name.Contains(txtFilter.Text, StringComparison.OrdinalIgnoreCase);
            } else {
                return true;
            }
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(listServers.ItemsSource).Refresh();
        }
    }
}
