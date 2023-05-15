using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Core;
using Windows.Storage.Streams;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using System.Threading;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Test_UWP_1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Geolocator geolocator;
        private MapIcon locationIcon;

        private bool isSelectingStartPoint = false;
        private bool isSelectingEndPoint = false;
        private Geopoint startPoint;
        private Geopoint endPoint;

        private NotificationManager notificationManager;
        
        // Цвета
        public SolidColorBrush RED_Notification = new SolidColorBrush(Colors.Red);
        public SolidColorBrush GREEN_Notification = new SolidColorBrush(Colors.Green);
        public SolidColorBrush BLUE_Notification = new SolidColorBrush(Colors.Blue);

        private CancellationTokenSource locationUpdateTokenSource;


        public MainPage()
        {
            this.InitializeComponent();
            mapControl.MapServiceToken = "35wISN8sOtCWorow7xE8~kFqkQNroOLGF4n0qIdTLfA~AqSmv4QThH7uxnbScEHHguCNdvVVsHlvfRiZzMqgtPJAAGrIlKaJn0SEKAMizS9q";
            SetCurrentLocation();
            mapControl.ZoomLevel = 12; // Установите желаемый уровень масштабирования карты
            mapControl.Style = MapStyle.Road;
            this.Loaded += MainPage_Loaded;

            geolocator = new Geolocator();
            locationIcon = new MapIcon();
            
            // Настройка метки
            locationIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/LocationIcon.png")); // Измените путь к изображению на свое
            locationIcon.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 0.5);

            // Добавление метки на карту
            mapControl.MapElements.Add(locationIcon);

            // Подписка на событие изменения местоположения
            geolocator.PositionChanged += Geolocator_PositionChanged;


            // Запуск получения местоположения
            GetLocation();
            menuGrid.SizeChanged += MenuGrid_SizeChanged;
            
        }

        private void MenuGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Получение текущей ширины menuGrid
            double menuGridWidth = menuGrid.ActualWidth;

            // Установка минимальной ширины окна
            var currentView = ApplicationView.GetForCurrentView();
            currentView.SetPreferredMinSize(new Size(menuGridWidth, 0));
        }


        private async void GetLocation()
        {
            Geoposition geoposition = await geolocator.GetGeopositionAsync();
            UpdateLocation(geoposition.Coordinate.Point.Position);
        }

        private void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            UpdateLocation(args.Position.Coordinate.Point.Position);
        }

        private async void UpdateLocation(BasicGeoposition position)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // mapControl.Center = new Geopoint(position);
                locationIcon.Location = new Geopoint(position);
                locationIcon.Title = $"Моё местоположение\n{position.Latitude}°, {position.Longitude}°";
            });       
        }

        public async void SetCurrentLocation()
        {
            Geolocator geolocator = new Geolocator();
            Geoposition geoposition = await geolocator.GetGeopositionAsync();

            double latitude = geoposition.Coordinate.Point.Position.Latitude;
            double longitude = geoposition.Coordinate.Point.Position.Longitude;

            mapControl.Center = new Geopoint(new BasicGeoposition { Latitude = latitude, Longitude = longitude });
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = searchTextBox.Text;
            SearchCity(searchQuery, "simple");
        }

        public async Task SearchCity(string searchQuery, string typeSearch)
        {
            if (!string.IsNullOrEmpty(searchQuery))
            {
                MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(searchQuery, null);
        
                if (result.Status == MapLocationFinderStatus.Success && result.Locations.Count > 0)
                {
                    BasicGeoposition location = new BasicGeoposition
                    {
                        Latitude = result.Locations[0].Point.Position.Latitude,
                        Longitude = result.Locations[0].Point.Position.Longitude
                    };

                    if (typeSearch == "simple") // Обычный поиск
                    {
                        mapControl.Center = new Geopoint(location);
                        mapControl.ZoomLevel = 10;
                        notificationManager.ShowNotification("Результат поиска.", $"Город {searchQuery} найден!\nКоординаты: {location.Latitude}, {location.Longitude}", GREEN_Notification);
                        
                    }
                    else if (typeSearch == "startPoint") // Начальная точка
                    {
                        startPoint = new Geopoint(location);
                    }
                    else if (typeSearch == "endPoint") // Конечная точка
                    {
                        endPoint = new Geopoint(location);
                    }
                }
                else
                {
                    notificationManager.ShowNotification("Ошибка поиска!", "Кажется вы ввели неправильное название, или у вас нет интернет соединения!", RED_Notification);
                    searchTextBox.Text = "";
                }
            }
            else
            {
                // Пустой запрос поиска
                notificationManager.ShowNotification("Поиск невозможен.", "Введите название города!", RED_Notification);
            }
        }
        
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedStyle = (ComboBoxItem)mapStyleComboBox.SelectedItem;
            if (selectedStyle != null)
            {
                string style = selectedStyle.Content.ToString();
                ChangeMapStyle(mapControl, style);
            }
        }

        /// <summary>
        /// Изменяет стиль карты.
        /// </summary>
        /// <param name="mapControl">Элемент управления картой.</param>
        /// <param name="mapStyle">Новый стиль карты.</param>
        public void ChangeMapStyle(MapControl mapControl, string mapStyle)
        {
            switch (mapStyle)
            {
                case "Aerial":
                    mapControl.Style = MapStyle.Aerial;
                    notificationManager.ShowNotification("Обновление карты.", "Стиль Aerial установлен.", GREEN_Notification);
                    break;
                case "AerialWithRoads":
                    mapControl.Style = MapStyle.AerialWithRoads;
                    notificationManager.ShowNotification("Обновление карты.", "Стиль AerialWithRoads установлен.", GREEN_Notification);
                    break;
                case "Road":
                    mapControl.Style = MapStyle.Road;
                    notificationManager.ShowNotification("Обновление карты.", "Стиль Road установлен.", GREEN_Notification);
                    break;
                case "Terrain":
                    mapControl.Style = MapStyle.Terrain;
                    notificationManager.ShowNotification("Обновление карты.", "Стиль Terrain установлен.", GREEN_Notification);
                    break;
                case "Aerial3D":
                    mapControl.Style = MapStyle.Aerial3D;
                    notificationManager.ShowNotification("Обновление карты.", "Стиль Aerial3D установлен.", GREEN_Notification);
                    break;
                case "Aerial3DWithRoads":
                    mapControl.Style = MapStyle.Aerial3DWithRoads;
                    notificationManager.ShowNotification("Обновление карты.", "Стиль Aerial3DWithRoads установлен.", GREEN_Notification);
                    break;
                default:
                    // По умолчанию устанавливаем стиль "Road"
                    mapControl.Style = MapStyle.Road;
                    break;
            }
        }    

        // Создание и добавление MapIcon на карту
        private void AddMapIcon(MapControl mapControl, BasicGeoposition location, string title, string description)
        {
            // Создание нового MapIcon
            MapIcon mapIcon = new MapIcon
            {
                Location = new Geopoint(location),
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1.0) // Нормализованная точка привязки
            };

            // Создание пользовательского шаблона метки
            string template = $"{title}\n"
                              + $"{description}\n"
                              + $"{location.Latitude}, {location.Longitude}";

            // Присоединение пользовательского шаблона к метке
            mapIcon.Title = template;

            // Добавление MapIcon на карту
            mapControl.MapElements.Add(mapIcon);
        }
        
       
        
        private async Task<string> ReverseGeocodeAsync(BasicGeoposition location)
        {
            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAtAsync(new Geopoint(location));

            if (result.Status == MapLocationFinderStatus.Success && result.Locations.Count > 0)
            {
                MapLocation mapLocation = result.Locations[0];
                return mapLocation.DisplayName;
            }

            return string.Empty;
        }
        
        private async void MapControl_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            if (isSelectingStartPoint)
            {
                startPoint = args.Location;
                isSelectingStartPoint = false;
            }
            else if (isSelectingEndPoint)
            {
                endPoint = args.Location;
                isSelectingEndPoint = false;
            }
        }

        private async Task BuildRoute()
        {
            await SearchCity(startRoutePoint.Text, "startPoint");
            await SearchCity(endRoutePoint.Text, "endPoint");

            if (startPoint != null && endPoint != null)
            {
                // Создаем объект маршрута
                MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteAsync(startPoint, endPoint);

                // Проверяем, удалось ли построить маршрут
                if (routeResult.Status == MapRouteFinderStatus.Success)
                {
                    // Очищаем предыдущие маршруты на карте
                    mapControl.Routes.Clear();

                    // Отображаем маршрут на карте
                    MapRouteView routeView = new MapRouteView(routeResult.Route);
                    mapControl.Routes.Add(routeView);

                    // Задаем область отображения карты, чтобы охватить весь маршрут
                    await mapControl.TrySetViewBoundsAsync(routeResult.Route.BoundingBox, null, MapAnimationKind.None);

                    // Показываем длину маршрута и примерное время езды
                    double lengthInKilometers = routeResult.Route.LengthInMeters / 1000.0;
                    TimeSpan estimatedDuration = routeResult.Route.EstimatedDuration;

                    ShowRouteInformation(lengthInKilometers, estimatedDuration);
                }
            }
        }

        private void ShowRouteInformation(double lengthInKilometers, TimeSpan estimatedDuration)
        {
            try
            {
                string lengthString = string.Format("Длина маршрута: {0} км", lengthInKilometers.ToString("0.00"));
                string durationString = string.Format("Примерное время езды: {0} день, {1} часов, {2} минут ", estimatedDuration.ToString(@"dd"), estimatedDuration.ToString(@"hh"), estimatedDuration.ToString(@"mm"));

                // Выводим информацию о маршруте
                notificationManager.ShowNotification("Маршрут построен.", $"{lengthString}\n{durationString}", GREEN_Notification);
            }
            catch (Exception ex)
            {
                notificationManager.ShowNotification("Ошибка!", $"Непредвиденная ошибка!\n ERROR({ex.Message})", RED_Notification);
            }
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Создание экземпляра NotificationManager и передача пользовательского контрола
            notificationManager = new NotificationManager(notificationControl);
        }

        private async void CreateRoute_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(startRoutePoint.Text) && !string.IsNullOrEmpty(endRoutePoint.Text))
            {
                await BuildRoute();
                await UpdateMap();
            }
            else
            {
                notificationManager.ShowNotification("Ошибка.", $"Заполните все поля!", RED_Notification);
            }
        }

        private async Task UpdateMap()
        {
            // Получаем текущий маршрут, если он доступен
            if (mapControl.Routes.Count > 0)
            {
                MapRouteView routeView = mapControl.Routes[0];

                // Получаем границы маршрута
                GeoboundingBox routeBounds = routeView.Route.BoundingBox;

                // Задаем область отображения карты, чтобы охватить маршрут
                await mapControl.TrySetViewBoundsAsync(routeBounds, null, MapAnimationKind.None);
            }
        }

    }
}