using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    public class CurrencyModel
    {
        public CurrencyModel(string code, string name, string rate, string converter)
        {
            Code = code;
            Name = name;
            Rate = rate;
            Converter = converter;
        }

        public CurrencyModel(string date,  string rate)
        {
            Date = date;
            Rate = rate;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Rate { get; set; }
        public string Converter { get; set; }
        public string Date { get; set; }

        public string FirstLineTable
        {
            get
            {
                return $"{this.Date}";
            }
        }

        public string SecondLineTable
        {
            get
            {
                return $"{this.Rate}";
            }
        }
        public string FirstLine
        {
            get
            {
                return $"{this.Code} - {this.Name}";
            }
        }

        public string SecondLine
        {
            get
            {
                return $"{this.Rate} - Converter: {this.Converter}";
            }
        }
    }
}
