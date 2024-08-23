using System.ComponentModel;

namespace To_Do.Models
{
    public class CustomNavigationViewItemModel
    {
        private string idTag;
        public string IdTag
        {
            get { return idTag; }
            set
            {
                idTag = value;
                OnPropertyChanged("IdTag");
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private string glyph;
        public string Glyph
        {
            get { return glyph; }
            set
            {
                glyph = value;
                OnPropertyChanged("Glyph");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
