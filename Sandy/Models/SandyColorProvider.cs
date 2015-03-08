using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace AV.Cyclone.Sandy.Models
{
    public class SandyColorProvider : INotifyPropertyChanged
    {
        public static readonly SolidColorBrush DefaultKeywordBrush = new SolidColorBrush(Colors.Blue);
        public static readonly SolidColorBrush DefaultNumberBrush = new SolidColorBrush(Colors.Black);
        public static readonly SolidColorBrush DefaultOperatorBrush = new SolidColorBrush(Colors.Black);
        public static readonly SolidColorBrush DefaultIdentifierBrush = new SolidColorBrush(Colors.Black);

        private Brush keywordBrush = DefaultKeywordBrush;
        private Brush identifierBrush = DefaultNumberBrush;
        private Brush operatorBrush = DefaultOperatorBrush;
        private Brush numberBrush = DefaultIdentifierBrush;

        public event PropertyChangedEventHandler PropertyChanged;

        public Brush KeywordBrush
        {
            get { return keywordBrush; }
            set
            {
                if (Equals(value, keywordBrush)) return;
                keywordBrush = value;
                OnPropertyChanged();
            }
        }

        public Brush IdentifierBrush
        {
            get { return identifierBrush; }
            set
            {
                if (Equals(value, identifierBrush)) return;
                identifierBrush = value;
                OnPropertyChanged();
            }
        }

        public Brush OperatorBrush
        {
            get { return operatorBrush; }
            set
            {
                if (Equals(value, operatorBrush)) return;
                operatorBrush = value;
                OnPropertyChanged();
            }
        }

        public Brush NumberBrush
        {
            get { return numberBrush; }
            set
            {
                if (Equals(value, numberBrush)) return;
                numberBrush = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}