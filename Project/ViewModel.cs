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
    public class ViewModel : INotifyPropertyChanged
    {

        Windows.Storage.ApplicationDataContainer localSettings;
        Windows.Storage.ApplicationDataCompositeValue composite;


        private ObservableCollection<string> dates = new ObservableCollection<string>();
        private ObservableCollection<CurrencyModel> currency = new ObservableCollection<CurrencyModel>();
        private ArrayList realFileNames = new ArrayList();
        private string lifeHistory;
        private string lastDate;


        public ArrayList RealFileNames
        {
            get { return this.realFileNames; }
            set {
                this.realFileNames = value;
                StoreLocalSettings();
            }
        }

        public ObservableCollection<string> Dates
        {
            get { return this.dates; }
            set {
                this.dates = value;
                StoreLocalSettings();
            }
        }

        public ObservableCollection<CurrencyModel> Currency
        {
            get { return this.currency; }
            set {
                this.currency = value;
                StoreLocalSettings();
            }
        }

        public string LastDate
        {
            get { return this.lastDate; }
            set
            {
                this.lastDate = value;
                StoreLocalSettings();
                this.OnPropertyChanged();
            }
        }

        public string LifeHistory
        {
            get { return "LifeHistory: " + this.lifeHistory; }
            set
            {
                if (value.CompareTo("launched") == 0)
                    this.lifeHistory += " 1";
                else if (value.CompareTo("restored") == 0)
                    this.lifeHistory += " 2";
                else if (value.CompareTo("suspended") == 0)
                    this.lifeHistory += " 3";
                else if (value.CompareTo("resumed") == 0)
                    this.lifeHistory += " 4";
                else if (value.CompareTo("") == 0)
                    this.lifeHistory = "";
                StoreLocalSettings();
                this.OnPropertyChanged();
            }
        }

       

        // public ViewModel()
        // {
        // }

        public void StoreLocalSettings()
        {
            composite["dates"] = "";
            composite["currency"] = "";
            composite["realFileNames"] = "";
            composite["lastDate"] = "";

            foreach (string element in dates)
            {
                composite["dates"] += element+"@";
            }
            foreach (CurrencyModel element in currency)
            {
                composite["currency"] += element.Code + "|";
                composite["currency"] += element.Name + "|";
                composite["currency"] += element.Rate + "|";
                composite["currency"] += element.Converter + "|";
                composite["currency"] += "@";
            }
            foreach (string element in realFileNames)
            {
                composite["realFileNames"] += element + "@";
            }
            composite["lifeHistory"] = lifeHistory;
            composite["lastDate"] = lastDate;
            localSettings.Values["DataBindingViewModel"] = composite;
            

        }



        private static ViewModel myInstance;

        public static ViewModel getInstance()
        {
            return myInstance;
        }

        
        public ViewModel()
        {
            myInstance = this;
            dates = new ObservableCollection<string>();
        currency = new ObservableCollection<CurrencyModel>();
        realFileNames = new ArrayList();

            lastDate = "";
        lifeHistory = "";

            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            composite = (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["DataBindingViewModel"];
            if (composite == null)
            {
                composite = new Windows.Storage.ApplicationDataCompositeValue();
                StoreLocalSettings();
            }
            else
            {
                String[] separators = { "@" };
                String[] separators2 = { "|" };
                //dates = (String)composite["fname"];

                string datesSS = (String)composite["dates"];
                string[] datesS = datesSS.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                foreach (string element in datesS)
                {
                    dates.Add(element);
                }
                
                //if (currency.Count != 0)
                {
                    string currencySS = (String)composite["currency"];
                    string[] currencyS = currencySS.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string element in currencyS)
                    {
                        string code;
                        string rate;
                        string name;
                        string converter = "0";
                        string[] currencyE = element.Split(separators2, StringSplitOptions.RemoveEmptyEntries);
                        code = currencyE[0];
                        name = currencyE[1];
                        rate = currencyE[2];
                        if (currencyE.Length > 3) converter = currencyE[3];
                        CurrencyModel component = new CurrencyModel(code, name, rate, converter);
                        currency.Add(component);
                    }
                }
                //if (realFileNames.Count != 0)
                {
                    string realFileNamesSS = (String)composite["realFileNames"];
                    string[] realFileNamesS = realFileNamesSS.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string element in realFileNamesS)
                    {
                        realFileNames.Add(element);
                    }
                }
                

                lifeHistory = (String)composite["lifeHistory"];
                lastDate = (String)composite["lastDate"];
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
