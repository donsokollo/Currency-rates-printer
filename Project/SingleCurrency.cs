using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Defines an info requested for a single download package of one currency for plotting purposes

namespace Project
{
    public class SingleCurrency
    {

        public SingleCurrency(string initDate, string endDate, string currency)
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
