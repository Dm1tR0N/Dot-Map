using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Test_UWP_1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Geolocator geolocator;
        private MapIcon locationMarker;
        
        private bool isSelectingStartPoint = false;
        private bool isSelectingEndPoint = false;
        private Geopoint startPoint;
        private Geopoint endPoint;

        private NotificationManager notificationManager;
        
        // Цвета
        public SolidColorBrush RED_Notification = new SolidColorBrush(Colors.Red);
        public SolidColorBrush GREEN_Notification = new SolidColorBrush(Colors.Green);
        public MainPage()
        {
            this.InitializeComponent();
            mapControl.MapServiceToken = "35wISN8sOtCWorow7xE8~kFqkQNroOLGF4n0qIdTLfA~AqSmv4QThH7uxnbScEHHguCNdvVVsHlvfRiZzMqgtPJAAGrIlKaJn0SEKAMizS9q";
            mapControl.Center = new Geopoint(new BasicGeoposition { Latitude = 56.493473, Longitude = 84.9493429 }); // Установите желаемые координаты центра карты
            mapControl.ZoomLevel = 12; // Установите желаемый уровень масштабирования карты
            mapControl.Style = MapStyle.Road;
            this.Loaded += MainPage_Loaded;

            //AddMapIcon(mapControl, new BasicGeoposition { Latitude = 56.455483, Longitude = 84.951491 }, "Название", "Что то");
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
        
        
        
        // Обработчик события нажатия на элемент карты
        // С 150 по 236 строки реализация работы с метками! Нужно доработать
        // private void MapControl_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        // {
        //     if (args.MapElements.Count > 0)
        //     {
        //         // Получение выбранного элемента карты
        //         MapElement selectedElement = args.MapElements[0];
        //
        //         // Проверка, является ли элемент точкой интереса
        //         if (selectedElement is MapIcon mapIcon)
        //         {
        //             // Создание объекта с данными о метке
        //             MapMarkerData markerData = new MapMarkerData
        //             {
        //                 Title = "Название точки",
        //                 Description = "Описание точки",
        //                 ImageUrl = "URL изображения"
        //                 // Добавьте другие свойства метки
        //             };
        //
        //             // Отображение карточки с информацией о точке
        //             ShowInfoCard(markerData);
        //         }
        //     }
        // }

        private void ShowInfoCard(MapMarkerData markerData)
        {
            // Создание карточки
            TextBlock titleTextBlock = new TextBlock();
            titleTextBlock.Text = markerData.Title;

            TextBlock descriptionTextBlock = new TextBlock();
            descriptionTextBlock.Text = markerData.Description;

            Image image = new Image();
            image.Source = new BitmapImage(new Uri(markerData.ImageUrl));

            // Создание контейнера для карточки
            StackPanel infoPanel = new StackPanel();
            infoPanel.Background = new SolidColorBrush(Colors.White);
            infoPanel.BorderThickness = new Thickness(1);
            infoPanel.BorderBrush = new SolidColorBrush(Colors.Black);
            infoPanel.Margin = new Thickness(10);
            infoPanel.Padding = new Thickness(10);
            infoPanel.Children.Add(titleTextBlock);
            infoPanel.Children.Add(descriptionTextBlock);
            infoPanel.Children.Add(image);

            // Создание всплывающей панели
            Popup infoPopup = new Popup();
            infoPopup.Child = infoPanel;
            infoPopup.IsOpen = true;
            infoPopup.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            infoPopup.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;

            // Обработчик события закрытия всплывающей панели
            infoPopup.Closed += (sender, e) =>
            {
                // Очистка ресурсов после закрытия карточки
                infoPopup.Child = null;
                infoPopup = null;
            };
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
        
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (menuPanel.Visibility == Visibility.Collapsed)
            {
                // Показать меню с анимацией
                menuPanel.Visibility = Visibility.Visible;
                var showAnimation = new DoubleAnimation()
                {
                    From = 0,
                    To = menuPanel.ActualWidth,
                    Duration = TimeSpan.FromSeconds(0.3)
                };
        
                // Создание Storyboard и добавление анимации
                var storyboard = new Storyboard();
                storyboard.Children.Add(showAnimation);
                Storyboard.SetTarget(showAnimation, menuPanel);
                Storyboard.SetTargetProperty(showAnimation, "(FrameworkElement.Width)");

                // Запуск анимации
                storyboard.Begin();
            }
            else if (menuPanel.Visibility == Visibility.Visible)
            {
                // Скрыть меню с анимацией
                var hideAnimation = new DoubleAnimation()
                {
                    From = menuPanel.ActualWidth,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.3)
                };
        
                // Создание Storyboard и добавление анимации
                var storyboard = new Storyboard();
                storyboard.Children.Add(hideAnimation);
                Storyboard.SetTarget(hideAnimation, menuPanel);
                Storyboard.SetTargetProperty(hideAnimation, "(FrameworkElement.Width)");

                menuPanel.Visibility = Visibility.Collapsed;


                // Запуск анимации
                storyboard.Begin();
            }
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
            string lengthString = string.Format("Длина маршрута: {0} км", lengthInKilometers.ToString("0.00"));
            string durationString = string.Format("Примерное время езды: {0} часов, {1} минут ", estimatedDuration.ToString(@"hh"), estimatedDuration.ToString(@"mm"));

            // Выводим информацию о маршруте
            notificationManager.ShowNotification("Маршрут построен.", $"{lengthString}\n{durationString}", GREEN_Notification);
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