﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;

namespace Project
{
    public class PlotModel : INotifyPropertyChanged
    {


        Windows.Storage.ApplicationDataContainer localSettings;
        Windows.Storage.ApplicationDataCompositeValue composite;


        private ObservableCollection<CurrencyModel> currency = new ObservableCollection<CurrencyModel>();
        private string currCode;

        public ObservableCollection<CurrencyModel> Currency
        {
            get { return this.currency; }
            set
            {
                currency = value;
                StoreLocalSettings();
                this.OnPropertyChanged();
            }
        }

        public string CurrCode
        {
            get { return this.currCode; }
            set
            {
                currCode = value;
                StoreLocalSettings();
                this.OnPropertyChanged();
            }
        }
        //public PlotModel()
        //{
        //}

        public void SortItem(ObservableCollection<CurrencyModel> currency)
        {
            IEnumerable<CurrencyModel> enumerable = currency.OrderBy(o => o.Date);
            this.currency.Clear();
            foreach(CurrencyModel element in enumerable)
            {
                this.currency.Add((CurrencyModel)element);
            }
           // System.Diagnostics.Debug.WriteLine("oooooo" + currency.First<CurrencyModel>().Date + "oooooo" +currency.Last<CurrencyModel>().Date);
        }


        private static PlotModel myInstance;

        public static PlotModel getInstance()
        {
            return myInstance;
        }


        public PlotModel()
        {
            myInstance = this;
           currency = new ObservableCollection<CurrencyModel>();
            currCode = "";           
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            composite = (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["DataBindingPlotModel"];
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

               
                //if (currency.Count != 0)
                {
                    string currencySS = (String)composite["currency"];
                    string[] currencyS = currencySS.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string element in currencyS)
                    {
                        string date;
                        string rate ="0";
                        string[] currencyE = element.Split(separators2, StringSplitOptions.RemoveEmptyEntries);
                        date = currencyE[0];  
                        rate = currencyE[1];
                         
                        CurrencyModel component = new CurrencyModel(date, rate);
                        currency.Add(component);
                    }
                }
                currCode = (String)composite["currCode"];

            }
        }



        public void StoreLocalSettings()
        {
            composite["currency"] = "";
            
            
            foreach (CurrencyModel element in currency)
            {
                composite["currency"] += element.Date + "|";
                composite["currency"] += element.Rate + "|";
                composite["currency"] += "@";
            }
            composite["currCode"] = currCode;
            localSettings.Values["DataBindingPlotModel"] = composite;


        }






        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
   
}
