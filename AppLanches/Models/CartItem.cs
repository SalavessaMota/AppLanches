using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AppLanches.Models
{
    public class CartItem : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public decimal TotalValue { get; set; }

        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                }
            }
        }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? UrlImage { get; set; }
        public string? PathImage => AppConfig.BaseUrl + "/images/" + UrlImage;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
