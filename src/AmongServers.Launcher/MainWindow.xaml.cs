using AmongServers.Launcher.Coordinator.Entities;
using AmongServers.Launcher.Utilities;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
        private List<FavouriteServer> _favouriteServers = new List<FavouriteServer>();

        class ServerItem : INotifyPropertyChanged
        {
            private bool _isSaved;

            public bool IsSaved {
                get {
                    return _isSaved;
                } set {
                    _isSaved = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FavouriteIcon)));
                }
            }

            public string Name { get; set; }
            public string Endpoint { get; set; }
            public bool IsAvailable { get; set; }
            public string CountPlayers { get; set; }
            public string CountPublicLobbies { get; set; }
            public string CountPrivateLobbies { get; set; }
            public string FavouriteIcon => IsSaved ? "Heart" : "HeartOutline";

            public event PropertyChangedEventHandler PropertyChanged;
        }

        public MainWindow()
        {
            // setup client
            _client = new ApiClient(Constants.ApiUrl);

            // initialise
            InitializeComponent();
        }

        /// <summary>
        /// Loads the favourites.
        /// </summary>
        /// <returns></returns>
        private async Task LoadFavouritesAsync()
        {
            if (File.Exists(System.IO.Path.Combine(App.DataPath, "Favourites.json"))) {
                string favouritesStr = await File.ReadAllTextAsync(System.IO.Path.Combine(App.DataPath, "Favourites.json"));
                _favouriteServers = new List<FavouriteServer>(JsonConvert.DeserializeObject<FavouriteServer[]>(favouritesStr));
            }
        }

        /// <summary>
        /// Saves the favourites.
        /// </summary>
        /// <returns></returns>
        private Task SaveFavouritesAsync()
        {
            return File.WriteAllTextAsync(System.IO.Path.Combine(App.DataPath, "Favourites.json"), JsonConvert.SerializeObject(_favouriteServers));
        }

        /// <summary>
        /// Refreshes the banner image.
        /// </summary>
        /// <returns></returns>
        private async Task RefreshBannerAsync(CancellationToken cancellationToken = default)
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
                    Endpoint = "144.56.23.9:32021",
                    LastSeenAt = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(RandomNumberGenerator.GetInt32(5)),
                    Games = Array.Empty<GameEntity>()
                },
                new ServerEntity() {
                    Name = "EU Central | The UK is an Impostor",
                    Endpoint = "141.2.134.1:32021",
                    LastSeenAt = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(RandomNumberGenerator.GetInt32(10)),
                    Games = new[] {
                        new GameEntity() {
                            State = "started",
                            CountPlayers = 10,
                            NumImposters = 2,
                            HostPlayer = new PlayerEntity() {
                                Name = "Baconator"
                            },
                            IsPublic = true,
                            Map = "polus",
                            MaxPlayers = 10,
                            Players = new[] {
                                new PlayerEntity() {
                                    Name = "Cheesenator"
                                },
                                new PlayerEntity() {
                                    Name = "Breadantor"
                                },
                                new PlayerEntity() {
                                    Name = "Baconator"
                                }
                            }
                        }
                    }
                },
                new ServerEntity() {
                    Name = "OnlyPrivate Servers",
                    Endpoint = "8.34.179.44:32022",
                    LastSeenAt = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(RandomNumberGenerator.GetInt32(10)),
                    Games = new[] {
                        new GameEntity() {
                            State = "starting",
                            CountPlayers = 3,
                            NumImposters = 2,
                            HostPlayer = null,
                            IsPublic = false,
                            Map = "polus",
                            MaxPlayers = 5,
                            Players = Array.Empty<PlayerEntity>()
                        },
                         new GameEntity() {
                            State = "started",
                            CountPlayers = 4,
                            NumImposters = 2,
                            HostPlayer = null,
                            IsPublic = false,
                            Map = "polus",
                            MaxPlayers = 5,
                            Players = Array.Empty<PlayerEntity>()
                        }
                    }
                }
            }).ToArray();

            // get the favourited servers
            IEnumerable<ServerItem> favouriteItems = _favouriteServers.Select(f => {
                ServerEntity s = servers.SingleOrDefault(s => s.Endpoint == f.Endpoint.ToString());

                if (s == null) {
                    return new ServerItem() {
                        Name = f.Name,
                        CountPlayers = "?",
                        CountPrivateLobbies = "?",
                        CountPublicLobbies = "?",
                        Endpoint = f.Endpoint,
                        IsSaved = true,
                        IsAvailable = false
                    };
                } else {
                    // update the name
                    f.Name = s.Name;

                    return new ServerItem() {
                        Name = s.Name,
                        CountPlayers = s.Games == null ? "!" : s.Games.Sum(g => g.CountPlayers).ToString(),
                        CountPublicLobbies = s.Games == null ? "!" : s.Games.Count(g => g.IsPublic).ToString(),
                        CountPrivateLobbies = s.Games == null ? "!" : s.Games.Count(g => !g.IsPublic).ToString(),
                        Endpoint = f.Endpoint,
                        IsSaved = true,
                        IsAvailable = true
                    };
                }
            });

            // save favourites incase we updated any "last-seen" names
            await SaveFavouritesAsync();

            // get the regular servers
            IEnumerable<ServerItem> regularServers = servers.Where(s => IPEndPoint.TryParse(s.Endpoint, out IPEndPoint _))
            .Where(s => !favouriteItems.Any(f => f.Endpoint == s.Endpoint))
            .Select(s => new ServerItem() {
                Name = s.Name,
                Endpoint = s.Endpoint.ToString(),
                IsAvailable = true,
                IsSaved = false,
                CountPlayers = s.Games == null ? "!" : s.Games.Sum(g => g.CountPlayers).ToString(),
                CountPublicLobbies = s.Games == null ? "!" : s.Games.Count(g => g.IsPublic).ToString(),
                CountPrivateLobbies = s.Games == null ? "!" : s.Games.Count(g => !g.IsPublic).ToString()
            });

            // set the item source
            listServers.ItemsSource = new ObservableCollection<ServerItem>(favouriteItems.Concat(regularServers));

            // setup filter
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listServers.ItemsSource);
            view.Filter = listServers_Filter;

            if (view.SortDescriptions.Count == 0) {
                view.SortDescriptions.Add(new SortDescription(nameof(ServerItem.IsSaved), ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription(nameof(ServerItem.Name), ListSortDirection.Ascending));
            }
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // block for first load
            LoadFavouritesAsync().Wait();

            // set the version text
            lblVersion.Text = $"Version {Constants.ApplicationVersion}";

            // refresh the banner
            _ = RefreshBannerAsync();

            // refresh the server list
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
            if (!IPEndPoint.TryParse(txtDirectPlay.Text, out IPEndPoint serverEndpoint)) {
                MessageBox.Show("The direct play address is invalid", "AmongServers", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // add the default port
            if (serverEndpoint.Port == 0)
                serverEndpoint = new IPEndPoint(serverEndpoint.Address, 22023);

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
            bool isValidIp = IPEndPoint.TryParse(txtDirectPlay.Text, out IPEndPoint endpoint) && !endpoint.Address.Equals(IPAddress.Any);

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
                    await Bootstrapper.ReplaceRegionInfoAsync(serverItem.Name, IPEndPoint.Parse(serverItem.Endpoint));
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
            CollectionViewSource.GetDefaultView(listServers.ItemsSource)?.Refresh();
        }

        private async void btnFavourite_Click(object sender, RoutedEventArgs e)
        {
            // get the button instance
            Button button = (Button)sender;

            // get the data context
            if (button.DataContext is ServerItem serverItem) {
                button.IsEnabled = false;

                if (serverItem.IsSaved) {
                    // remove from favourites
                    _favouriteServers.Remove(_favouriteServers.Single(f => f.Endpoint == serverItem.Endpoint));

                    // if this server was not available anymore we need to remove it now it's not pinned
                    if (!serverItem.IsAvailable) {
                        ((ObservableCollection<ServerItem>)listServers.ItemsSource).Remove(serverItem);
                    }

                    // mark as not saved then save
                    serverItem.IsSaved = false;
                    await SaveFavouritesAsync();
                } else {
                    // add to favourites
                    _favouriteServers.Add(new FavouriteServer() {
                        AddedAt = DateTimeOffset.UtcNow,
                        Endpoint = serverItem.Endpoint,
                        Name = serverItem.Name
                    });

                    // mark as saved then save
                    serverItem.IsSaved = true;
                    await SaveFavouritesAsync();
                }

                // refresh view for new sorting
                CollectionViewSource.GetDefaultView(listServers.ItemsSource)?.Refresh();

                button.IsEnabled = true;
            }
        }
    }
}
