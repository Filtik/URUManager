using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using URUManager.Services;
using URUManager.ViewModels;

namespace URUManager
{
    public partial class MainWindow : Window
    {
        private bool _languageInitializing;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            LoadBackground();
            InitLanguageCombo();
            Loaded += (s, e) => UpdateColumns(ActualWidth);
        }

        private void InitLanguageCombo()
        {
            _languageInitializing = true;
            foreach (ComboBoxItem item in LanguageCombo.Items)
            {
                if ((string)item.Tag == LanguageManager.CurrentLanguage)
                {
                    LanguageCombo.SelectedItem = item;
                    break;
                }
            }
            _languageInitializing = false;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateColumns(e.NewSize.Width);
        }

        private void UpdateColumns(double windowWidth)
        {
            const double minCardWidth = 320;
            double available = windowWidth - 24;
            int cols = Math.Max(1, (int)(available / minCardWidth));
            var grid = FindVisualChild<UniformGrid>(ShardsList);
            if (grid != null)
                grid.Columns = cols;
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                    return (T)child;
                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void LanguageCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_languageInitializing) return;
            var item = LanguageCombo.SelectedItem as ComboBoxItem;
            if (item != null)
                LanguageManager.Apply((string)item.Tag);
        }

        private void LoadBackground()
        {
            try
            {
                var image = new BitmapImage(new Uri("pack://application:,,,/Resources/Background.png", UriKind.Absolute));
                var brush = new ImageBrush(image) { Stretch = Stretch.UniformToFill };
                Background = brush;
            }
            catch { }
        }
    }

    public class ExeIconConverter : System.Windows.Data.IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2) return null;
            if (!(values[0] is string)) return null;
            if (!(values[1] is bool)) return null;

            string shardPath = (string)values[0];
            bool isInternal = (bool)values[1];

            if (string.IsNullOrEmpty(shardPath)) return null;

            string exeName = isInternal ? "plClient.exe" : "URULauncher.exe";
            string exePath = Path.Combine(shardPath, exeName);

            if (!File.Exists(exePath)) return null;

            try
            {
                using (var icon = System.Drawing.Icon.ExtractAssociatedIcon(exePath))
                {
                    var bitmap = Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                    bitmap.Freeze();
                    return bitmap;
                }
            }
            catch
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
