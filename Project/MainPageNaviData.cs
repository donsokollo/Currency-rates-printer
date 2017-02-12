using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    public class MainPageNaviData
    {

        public MainPageNaviData(string initDate, string endDate, string currency)
        {
            InitDate = initDate;
            EndDate = endDate;
            Currency = currency;
            
        }
        public string InitDate
        {
            get; set;
        }

        public string EndDate{ get;  set;
        }
        public string Currency { get;  set;
        }

        
}
}
