using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    public class PlotModel : INotifyPropertyChanged
    {

        private ObservableCollection<CurrencyModel> currency = new ObservableCollection<CurrencyModel>();


        public ObservableCollection<CurrencyModel> Currency
        {
            get { return this.currency; }
            set
            {
                currency = value;
                this.OnPropertyChanged();
            }
        }

        public PlotModel()
        {
        }
        
        public void SortItem()
        {
            IEnumerable<CurrencyModel> enumerable = this.currency.OrderBy(o => o.Date);
            ObservableCollection<CurrencyModel> currency = new ObservableCollection<CurrencyModel>(enumerable);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
   
}
