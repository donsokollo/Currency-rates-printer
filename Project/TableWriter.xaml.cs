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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Project
{
    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TableWriter : Page
    {
        public TableWriter()
        {
            this.InitializeComponent();
            this.plotModel = new PlotModel();
            MinDateOk.MinDate = new DateTime( 2002, 10, 10);
            MaxDateOk.MaxDate = System.DateTime.Now.Date;

        }
        MainPageNaviData currencyL;

        private string basicUrl = "http://api.nbp.pl/api/exchangerates/rates/A/";

        // Declare a System.Threading.CancellationTokenSource.  
        CancellationTokenSource cts;
        public PlotModel plotModel{ get; set; }


        private void plotGraph_Click(object sender, RoutedEventArgs e)
        {
            if ( !MinDateOk.Date.Equals(null) && !MaxDateOk.Date.Equals(null))
            {
                this.currencyL.InitDate = MinDateOk.Date.Value.Date.ToString("yyyy-MM-dd");
                this.currencyL.EndDate = MaxDateOk.Date.Value.Date.ToString("yyyy-MM-dd");
                System.Diagnostics.Debug.WriteLine(this.currencyL.InitDate+"staaaaalo sieeee");
            }
            getCurrencyValue();
        }

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


        private async  void getCurrencyValue()
        {
            this.plotModel.Currency.Clear();

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

            // ***Use ToList to execute the query and start the tasks.   
            List<Task<string>> downloadTasks = downloadTasksQuery.ToList();

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
                parseXML(xml);
                resultsTextBox.Text = String.Format("");
                resultsTextBox.Text += String.Format("\r\nLength of the download:  {0}", xml.Length);
            }
        }


        private void parseXML(string xml)
        {

            System.Diagnostics.Debug.WriteLine(xml);

            // Create an XmlReader
            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            {
                string date;
                string rate;
                System.Diagnostics.Debug.WriteLine("!!!!!!!!!!!!!1111111!!!!!!!!!!!");
                reader.MoveToContent();
                System.Diagnostics.Debug.WriteLine(xml);
                
                while (reader.ReadToFollowing("Rates"))
                {
                    reader.ReadToDescendant("Rate");
                    while (reader.ReadToFollowing("Rate"))
                    {
                        reader.ReadToFollowing("EffectiveDate");
                        date = reader.ReadElementContentAsString();
                        //System.Diagnostics.Debug.WriteLine(date);
                        reader.ReadToFollowing("Mid");
                        rate = reader.ReadElementContentAsString();
                        //System.Diagnostics.Debug.WriteLine(rate);
                        CurrencyModel cur = new CurrencyModel(date, rate);
                        plotModel.Currency.Add(cur);
                    }
                }

            }
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


            List<string> urls = new List<string>
            {
                basicUrl+currencyL.Currency+"/"+currencyL.InitDate+"/"+currencyL.EndDate+"/?format=xml",
                basicUrl+"EUR/2016-10-10/2017-01-01/?format=xml"
                
            };
            System.Diagnostics.Debug.WriteLine(urls[1]);
            System.Diagnostics.Debug.WriteLine(urls[0]);
            //System.Diagnostics.Debug.WriteLine(urls[2]);
            return urls;
        }

        async Task<String> ProcessURL(string url, HttpClient client, CancellationToken ct)
        {
            // GetAsync returns a Task<HttpResponseMessage>.   
            string response = await client.GetStringAsync(url);
            System.Diagnostics.Debug.WriteLine("GETTING THERE!!!!!!!!!!!!!!!!!");
            HttpResponseMessage res = await client.GetAsync(url, ct);
            
            System.Diagnostics.Debug.WriteLine("");

            // System.Diagnostics.Debug.WriteLine("00000000000000000000!!!!!!!!!!!!!!!!!");
            // System.Diagnostics.Debug.WriteLine(response);
            //System.Diagnostics.Debug.WriteLine(date);
            return response;
        }


        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
        void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
        }
        void GoForward(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null && this.Frame.CanGoForward) this.Frame.GoForward();
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
            this.currencyL = (MainPageNaviData)e.Parameter;
            getCurrencyValue();
        }

        private void MaxDateOk_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            MinDateOk.MaxDate = (DateTimeOffset)MaxDateOk.Date;
                
        }

        private void MinDateOk_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            MaxDateOk.MinDate = (DateTimeOffset)MinDateOk.Date;
        }
    }
}
