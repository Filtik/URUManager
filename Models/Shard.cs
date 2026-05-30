using System.ComponentModel;
using System.Runtime.Serialization;

namespace URUManager.Models
{
    [DataContract]
    public class Shard : INotifyPropertyChanged
    {
        private string _name;
        private string _path;
        private bool _isInternal;
        private string _arguments;
        private bool _deleteTos;

        [DataMember]
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); OnPropertyChanged("DisplayName"); }
        }

        [DataMember]
        public string Path
        {
            get { return _path; }
            set { _path = value; OnPropertyChanged("Path"); }
        }

        [DataMember]
        public bool IsInternal
        {
            get { return _isInternal; }
            set { _isInternal = value; OnPropertyChanged("IsInternal"); OnPropertyChanged("DisplayName"); }
        }

        [DataMember]
        public string Arguments
        {
            get { return _arguments; }
            set { _arguments = value; OnPropertyChanged("Arguments"); }
        }

        [DataMember]
        public bool DeleteTos
        {
            get { return _deleteTos; }
            set { _deleteTos = value; OnPropertyChanged("DeleteTos"); }
        }

        public string DisplayName
        {
            get { return _name; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
