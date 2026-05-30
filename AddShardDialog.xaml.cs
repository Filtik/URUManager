using System.Windows;
using URUManager.Models;

namespace URUManager
{
    public partial class AddShardDialog : Window
    {
        public Shard Result { get; private set; }
        public bool DeleteRequested { get; private set; }

        public AddShardDialog()
        {
            InitializeComponent();
        }

        public AddShardDialog(Shard existing) : this()
        {
            Title = Loc("DialogTitle_Edit", "Edit Shard");
            NameBox.Text = existing.Name;
            PathBox.Text = existing.Path;
            ArgumentsBox.Text = existing.Arguments ?? "";
            InternalCheck.IsChecked = existing.IsInternal;
            DeleteTosCheck.IsChecked = existing.DeleteTos;
            DeleteButton.Visibility = Visibility.Visible;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = Loc("BrowseDescription", "Select URU shard directory");
            if (!string.IsNullOrEmpty(PathBox.Text))
                dialog.SelectedPath = PathBox.Text;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                PathBox.Text = dialog.SelectedPath;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show(Loc("ErrorNoName", "Please enter a name."), "URU Manager",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                NameBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(PathBox.Text))
            {
                MessageBox.Show(Loc("ErrorNoPath", "Please enter a path."), "URU Manager",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                PathBox.Focus();
                return;
            }

            Result = new Shard
            {
                Name = NameBox.Text.Trim(),
                Path = PathBox.Text.Trim(),
                IsInternal = InternalCheck.IsChecked == true,
                Arguments = ArgumentsBox.Text.Trim(),
                DeleteTos = DeleteTosCheck.IsChecked == true
            };
            DialogResult = true;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var msg = Loc("DeleteConfirm", "Are you sure you want to delete this shard?");
            var result = MessageBox.Show(msg, "URU Manager", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DeleteRequested = true;
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private static string Loc(string key, string fallback = "")
        {
            return Application.Current.TryFindResource(key) as string ?? fallback;
        }
    }
}
