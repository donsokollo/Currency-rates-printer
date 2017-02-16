using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
// Add a using directive and a reference for System.Net.Http.  
using System.Net.Http;

// Add the following using directive.  
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Globalization;
using System.ComponentModel;
using System.Collections.ObjectModel;

using System.Runtime.InteropServices.ComTypes;
using Syncfusion.UI.Xaml.Charts;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using System.Runtime.CompilerServices;
using System.Text;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Project
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TableWriter : Page, INotifyPropertyChanged
    {
        public TableWriter()
        {
            this.InitializeComponent();
            this.plotModel = PlotModel.getInstance();
            this.plotModel1 = new PlotModel();
           
            MinDateOk.MinDate = new DateTime(2002, 10, 10);
            MaxDateOk.MaxDate = System.DateTime.Now.Date;
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            download_progressBar.Visibility = Visibility.Collapsed;
        }


        MainPageNaviData currencyL;
        public MainPageNaviData CurrencyL{
            get { return this.currencyL; }
            set
            {
                currencyL = value;
                this.OnPropertyChanged();
            }
        }

        private string basicUrl = "http://api.nbp.pl/api/exchangerates/rates/A/";

        // Declare a System.Threading.CancellationTokenSource.  
        CancellationTokenSource cts;
        public PlotModel plotModel { get; set; }

        public PlotModel plotModel1 { get; set; }
        int packageSpan = 50; // a span of one download package for currency plot, must be <=365 due to server limits at uri

        private void plotGraph_Click(object sender, RoutedEventArgs e)
        {
            if (!MinDateOk.Date.Equals(null) && !MaxDateOk.Date.Equals(null))
            {
                this.currencyL.InitDate = MinDateOk.Date.Value.Date.ToString("yyyy-MM-dd");
                this.currencyL.EndDate = MaxDateOk.Date.Value.Date.ToString("yyyy-MM-dd");
                System.Diagnostics.Debug.WriteLine(this.currencyL.InitDate + "staaaaalo sieeee");
            }
            getCurrencyValue();
            
            
            //List<Order> objListOrder = new List<Order>();

            // List<Order> SortedList = objListOrder.OrderBy(o => o.OrderDate).ToList();
        }



        


        private async void getCurrencyValue()
        {
            // clears the current view of table and list
            this.plotModel.Currency.Clear();
            resultsTextBox.Text = String.Format("");
            //resultsTextBox.ClearValue;

            // Instantiate the CancellationTokenSource.  
            cts = new CancellationTokenSource();

            try
            {
                await AccessTheWebAsync(cts.Token);
                resultsTextBox.Text += "\r\nDownloads complete.";
            }
            catch (OperationCanceledException)
            {
                resultsTextBox.Text += "\r\nDownloads canceled.\r\n";
            }
            catch (Exception)
            {
                resultsTextBox.Text += "\r\nDownloads failed.\r\n";
            }

            cts = null;
            
            //SortItem();
           //this.plotModel.SortItem(plotModel.Currency);
            // this.plotModel.Currency.Add(new CurrencyModel("2017-02-15", "0"));
            //System.Diagnostics.Debug.WriteLine("oooooo" + this.plotModel.Currency.First<CurrencyModel>().Date + "oooooo" + this.plotModel.Currency.Last<CurrencyModel>().Date);
           
            IEnumerable<CurrencyModel> enumerable = plotModel.Currency.OrderBy(o => o.Date);
            
            foreach (CurrencyModel element in enumerable)
            {
                this.plotModel.Currency.Add((CurrencyModel)element);
            }
            System.Diagnostics.Debug.WriteLine("oooooo" + this.plotModel.Currency.First<CurrencyModel>().Date);
            System.Diagnostics.Debug.WriteLine("oooooo" + this.plotModel.Currency.Last<CurrencyModel>().Date);
        }

        public void SortItem()
        {
            IEnumerable<CurrencyModel> enumerable = plotModel.Currency.OrderBy(o => o.Date);
            plotModel.Currency = new ObservableCollection<CurrencyModel>(enumerable);
            plotModel.Currency.Add(new CurrencyModel("2017-02-15", "1"));
            System.Diagnostics.Debug.WriteLine("oooooo" + this.plotModel.Currency.First<CurrencyModel>().Date);
           System.Diagnostics.Debug.WriteLine("oooooo" + this.plotModel.Currency.Last<CurrencyModel>().Date);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }






        async Task AccessTheWebAsync(CancellationToken ct)
        {
            HttpClient client = new HttpClient();

            // Make a list of web addresses.  
            List<string> urlList = SetUpURLList();

            // ***Create a query that, when executed, returns a collection of tasks.  
            IEnumerable<Task<string>> downloadTasksQuery =
                from url in urlList select ProcessURL(url, client, ct);
            download_progressBar.Visibility = Visibility.Visible;
            download_progressBar.Maximum = urlList.Count;

            // ***Use ToList to execute the query and start the tasks.   
            List<Task<string>> downloadTasks = downloadTasksQuery.ToList();
            int currentProgress = 0; //progress of downloading
            // ***Add a loop to process the tasks one at a time until none remain.  
            while (downloadTasks.Count > 0)
            {
                // Identify the first task that completes.  
                Task<string> firstFinishedTask = await Task.WhenAny(downloadTasks);



                // ***Remove the selected task from the list so that you don't  
                // process it more than once.  
                downloadTasks.Remove(firstFinishedTask);
                // Await the completed task.  
                string xml = await firstFinishedTask;
               // System.Diagnostics.Debug.WriteLine("!!!!!!!!!!!!!!!!!AccessTheWebAsync next xml is!!!!!!!!!!!");
               System.Diagnostics.Debug.WriteLine("what is it" + xml);
               // System.Diagnostics.Debug.WriteLine("!!!!!!!!!!!!!!!!!AccessTheWebAsync next xml is!!!!!!!!!!!");

              currentProgress +=  parseXML(await firstFinishedTask);
                // Await the completed task. 
              
                download_progressBar.Value = currentProgress;
                
                resultsTextBox.Text += String.Format("\r\nLength of the download:  {0}", xml.Length);
            }
            download_progressBar.Visibility = Visibility.Collapsed;
        }


        private int parseXML(string xml)
        {
           int  currentProgress = 1;

            //System.Diagnostics.Debug.WriteLine(xml);
            
            // Create an XmlReader
            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            {
                string date;
                string rate ="0";
                //System.Diagnostics.Debug.WriteLine("!!!!!!!!!!!!!1111111!!!!!!!!!!!");
                reader.MoveToContent();
                //System.Diagnostics.Debug.WriteLine(xml);

               // reader.ReadToFollowing("Rates");
                reader.ReadToDescendant("Rate");
                {
                    
                    while (reader.ReadToFollowing("Rate"))
                    {
                        reader.ReadToFollowing("EffectiveDate");
                        date = reader.ReadElementContentAsString();
                        //System.Diagnostics.Debug.WriteLine(date);
                        /* if (!reader.NodeType.Equals("None"))*/
                        //reader.ReadToFollowing("Mid");
                        rate = reader.ReadElementContentAsString();
                       // System.Diagnostics.Debug.WriteLine(rate);
                       // if (reader.ReadElementContentAsString.)
                            CurrencyModel cur = new CurrencyModel(date, rate);
                        plotModel.Currency.Add(cur);
                    }
                }

            }
            return currentProgress;
        }
        /////////////
        //  WebRequest request = HttpWebRequest.Create(url);
        //request.ContentType = "application/xml";
        //WebResponse response = await request.GetResponseAsync();
        //StreamReader reader = new StreamReader(response.GetResponseStream());
        //string content = reader.ReadToEnd();
        //reader.Dispose();
        //response.Dispose();
        //return content;
        ////////////////////////


        private List<string> SetUpURLList()
        {

            List<SingleCurrency> packageList = defineListNumber();
            List<string> urls = new List<string>();
            int iii = 0;
            foreach (SingleCurrency element in packageList)
            {
                urls.Add(basicUrl + element.Currency + "/" + element.InitDate + "/" + element.EndDate + "/?format=xml");
                System.Diagnostics.Debug.WriteLine(iii + "      " + urls[iii++]);
            }


            //System.Diagnostics.Debug.WriteLine(urls[1]);
            // System.Diagnostics.Debug.WriteLine(urls[0]);
            //System.Diagnostics.Debug.WriteLine(urls[2]);
            return urls;
        }
        private List<SingleCurrency> defineListNumber()
        {
            List<SingleCurrency> currList = new List<SingleCurrency>();
            DateTime initDate = DateTime.ParseExact(currencyL.InitDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(currencyL.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            int interval = endDate.Subtract(initDate).Days;
            int fullPackageNumber = interval / this.packageSpan;
            int remainder = interval - fullPackageNumber * this.packageSpan;
            for (int i = 0; i < fullPackageNumber; i++) //defines all packages except for the last one having different packageSpan (reminder)
            {
                string startDate;
                string toDate;
                string currency = currencyL.Currency;
                startDate = initDate.AddDays(i * this.packageSpan).ToString("yyyy-MM-dd");
                toDate = initDate.AddDays((i + 1) * this.packageSpan).ToString("yyyy-MM-dd");
                SingleCurrency data = new SingleCurrency(startDate, toDate, currency);
                System.Diagnostics.Debug.WriteLine("      " + i + "     " + data);
                currList.Add(data);

            }
            //makes and adds the last package with days span smaller than the packageSpan
            SingleCurrency lastData = new SingleCurrency(endDate.AddDays(-remainder).ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), currencyL.Currency);
            currList.Add(lastData);
            System.Diagnostics.Debug.WriteLine("  last" + "     " + lastData);
            return currList;
        }

        async Task<String> ProcessURL(string url, HttpClient client, CancellationToken ct)
        {
           
            HttpResponseMessage res = await client.GetAsync(url, ct);

            res.EnsureSuccessStatusCode();
            
            string response = await res.Content.ReadAsStringAsync();  

            System.Diagnostics.Debug.WriteLine("xml String retrieved from server");
          
            return response;
        }


        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
            ViewModel.getInstance();
        }
        void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
            ViewModel.getInstance();
        }
        void GoForward(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null && this.Frame.CanGoForward) this.Frame.GoForward();
            ViewModel.getInstance();
            PlotModel.getInstance();
        }
#if WINDOWS_PHONE_APP
        void HardwareButtons_BackPressed(object sender,
          Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
          if (this.Frame != null && this.Frame.CanGoBack)
          {
            e.Handled = true;
            this.Frame.GoBack();
          }
        }
#endif
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            String[] separators = { "@" };
            String[] dataToHandle = e.Parameter.ToString().Split(separators, StringSplitOptions.RemoveEmptyEntries);
            this.currencyL = new MainPageNaviData(dataToHandle[0], dataToHandle[1], dataToHandle[2]);
            plotModel.CurrCode = dataToHandle[2];

            getCurrencyValue();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        private void MaxDateOk_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            MinDateOk.MaxDate = (DateTimeOffset)MaxDateOk.Date;

        }

        private void MinDateOk_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            MaxDateOk.MinDate = (DateTimeOffset)MinDateOk.Date;
        }



        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void Save_Btn_Click(object sender, RoutedEventArgs e)
        {
           
            This_chart.Save();
                   
        }

        private void cloesApp_Click(object sender, RoutedEventArgs e)
        {

            Application.Current.Exit();

        }



    }
}
