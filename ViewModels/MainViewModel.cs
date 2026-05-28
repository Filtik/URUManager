using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using URUManager.Models;
using URUManager.Services;

namespace URUManager.ViewModels
{
    public class MainViewModel
    {
        private readonly ShardStorageService _storage = new ShardStorageService();

        public ObservableCollection<Shard> Shards { get; } = new ObservableCollection<Shard>();

        public ICommand AddShardCommand { get; }
        public ICommand EditShardCommand { get; }
        public ICommand StartShardCommand { get; }

        public MainViewModel()
        {
            foreach (var s in _storage.Load())
                Shards.Add(s);

            AddShardCommand = new RelayCommand(_ => AddShard());
            EditShardCommand = new RelayCommand(s => EditShard((Shard)s));
            StartShardCommand = new RelayCommand(s => StartShard((Shard)s));
        }

        private void AddShard()
        {
            var dialog = new URUManager.AddShardDialog();
            dialog.Owner = Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
            {
                Shards.Add(dialog.Result);
                Save();
            }
        }

        private void EditShard(Shard shard)
        {
            var dialog = new URUManager.AddShardDialog(shard);
            dialog.Owner = Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
            {
                if (dialog.DeleteRequested)
                {
                    Shards.Remove(shard);
                }
                else
                {
                    shard.Name = dialog.Result.Name;
                    shard.Path = dialog.Result.Path;
                    shard.IsInternal = dialog.Result.IsInternal;
                    shard.Arguments = dialog.Result.Arguments;
                }
                Save();
            }
        }

        private void StartShard(Shard shard)
        {
            string exe = shard.IsInternal ? "plClient.exe" : "URULauncher.exe";
            string exePath = Path.Combine(shard.Path, exe);

            if (!File.Exists(exePath))
            {
                string label = Application.Current.TryFindResource("ErrorFileNotFound") as string ?? "File not found:";
                MessageBox.Show(
                    label + "\n" + exePath,
                    "URU Manager",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                WorkingDirectory = shard.Path,
                Arguments = shard.Arguments ?? ""
            });
        }

        private void Save()
        {
            _storage.Save(new List<Shard>(Shards));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
