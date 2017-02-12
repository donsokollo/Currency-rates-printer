using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    public class ViewModel
    {
        private ObservableCollection<string> dates = new ObservableCollection<string>();
        private ObservableCollection<CurrencyModel> currency = new ObservableCollection<CurrencyModel>();
        private ArrayList realFileNames = new ArrayList();

        public ArrayList RealFileNames
        {
            get { return this.realFileNames; }
            set { this.realFileNames = value; }
        }

        public ObservableCollection<string> Dates
        {
            get { return this.dates; }
            set { this.dates = value; }
        }

        public ObservableCollection<CurrencyModel> Currency
        {
            get { return this.currency; }
            set { this.currency = value; }
        }

        public ViewModel()
        {
        } 

    }
}
