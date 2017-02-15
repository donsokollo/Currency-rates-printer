using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;

namespace Project
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.viewModel = new ViewModel();
            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);
            Application.Current.Resuming += new EventHandler<Object>(App_Resuming);
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        async void App_Suspending(Object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            //GetNavigationState();
            // TODO: This is the time to save app data in case the process is terminated
        }
        private void App_Resuming(Object sender, Object e)
        {
            // TODO: Refresh network data, perform UI updates, and reacquire resources like cameras, I/O devices, etc.
        }

        public ViewModel viewModel { get; set; }

        private void getDatesBtn_Click(object sender, RoutedEventArgs e)
        {
            getDatesFromServer();    
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Func<string> itemId = sender.ToString;
            this.Frame.Navigate(typeof(TableWriter), itemId);
        }


        private string basicUrl = "http://www.nbp.pl/kursy/xml/";
        private string datesUrl = "http://www.nbp.pl/kursy/xml/dir.txt";
        

        private async Task<int> getDatesFromServer()
        {
            HttpClient client = new HttpClient();

            Task<string> getDatesTask = client.GetStringAsync(datesUrl);
            statusText.Text = "Getting dates ... Please wait";

            formatDatesResult(await getDatesTask);

            statusText.Text = "Dates recieved";

            return 1;
        }

        private void formatDatesResult(string input)
        {
            viewModel.RealFileNames.Clear();
            viewModel.Dates.Clear();
            string[] dates = input.Split('\n');
            char[] charsToTrim = { '\r', ' ' };
            foreach (string date in dates)
            {
                if (date.StartsWith("a"))
                {
                    viewModel.RealFileNames.Add(date);
                    string newDate = date.Trim(charsToTrim).Substring(5);
                    string formatedDate = "20" + newDate.Substring(0, 2) + '-' + newDate.Substring(2, 2) + '-' + newDate.Substring(4) ;
                    viewModel.Dates.Add(formatedDate);
                }
            }
        }

        private void datesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getInfoByDate((string) viewModel.RealFileNames[datesListView.SelectedIndex]);
        }



        private async Task<int> getInfoByDate(string filename)
        {
            HttpClient client = new HttpClient();

            Task<string> getDatesTask = client.GetStringAsync(basicUrl + filename + ".xml");
            statusText.Text = "Getting data...";

            viewModel.Currency.Clear();

            string resultXML = await getDatesTask;

            parseXML(resultXML);

            statusText.Text = "Data recieved";

            return 1;
        }

        private void parseXML(string xml)
        {

            System.Diagnostics.Debug.WriteLine(xml);
            // Create an XmlReader
            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            {
                string name;
                string converter;
                string code;
                string rate;

                reader.MoveToContent();
                while (reader.ReadToFollowing("pozycja"))
                {
                    reader.ReadToFollowing("nazwa_waluty");
                    name = reader.ReadElementContentAsString();
                    reader.ReadToFollowing("przelicznik");
                    converter = reader.ReadElementContentAsString();
                    reader.ReadToFollowing("kod_waluty");
                    code = reader.ReadElementContentAsString();
                    reader.ReadToFollowing("kurs_sredni");
                    rate = reader.ReadElementContentAsString();
                    CurrencyModel cur = new CurrencyModel(code, name, rate, converter);
                    viewModel.Currency.Add(cur);
                }
            }
        }
        
        private void currencyListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selDate = (string)viewModel.Dates[currencyListView.SelectedIndex];

            string currDate = System.DateTime.Today.ToString("yyyy-MM-dd");

            string selCurrency = (string)viewModel.Currency[currencyListView.SelectedIndex].Code;
           MainPageNaviData data = new MainPageNaviData(selDate, currDate, selCurrency);
            System.Diagnostics.Debug.WriteLine(currDate);
            //System.Diagnostics.Debug.WriteLine(itemId);
            this.Frame.Navigate(typeof(TableWriter), data);
        }


    }

}
