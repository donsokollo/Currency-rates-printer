using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    public class PlotModel
    {
        
        private ObservableCollection<CurrencyModel> currency = new ObservableCollection<CurrencyModel>();
        
       
        public ObservableCollection<CurrencyModel> Currency
        {
            get { return this.currency; }
            set { this.currency = value; }
        }

        public PlotModel()
        {
        } 

    }
}
