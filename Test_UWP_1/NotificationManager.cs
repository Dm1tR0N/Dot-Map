using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Test_UWP_1
{
    public class NotificationManager
    {
        private UserControl notificationControl;

        public NotificationManager(UserControl control)
        {
            notificationControl = control;
        }

        public void ShowNotification(string title, string content, SolidColorBrush color)
        {
            // Установка текстовых значений в контрол уведомления
            TextBlock titleTextBlock = notificationControl.FindName("titleTextBlock") as TextBlock;
            TextBlock contentTextBlock = notificationControl.FindName("contentTextBlock") as TextBlock;
            Grid grid = notificationControl.FindName("BoxNotification") as Grid;

            titleTextBlock.Text = title;
            contentTextBlock.Text = content;
            grid.Background = color;


            // Отображение контрола уведомления
            notificationControl.Visibility = Windows.UI.Xaml.Visibility.Visible;

            // Здесь вы можете добавить анимацию или другие эффекты для плавного отображения уведомления

            // Задержка на отображение уведомления
            Task.Delay(5000).ContinueWith(t =>
            {
                // Скрытие уведомления
                Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    notificationControl.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                });
            });
        }
    }
}
