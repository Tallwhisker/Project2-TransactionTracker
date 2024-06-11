using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Project2_TransactionTracker
{
    public enum TransactionTypes
    {
        Expense = -1,
        Income = 1
    }

    [XmlType("Transaction")]
    public class Transaction
    {
        public Transaction() { }

        public Transaction(TransactionTypes TransactionType, DateOnly TransactionTime, string TransactionName, decimal transactionValue)
        {
            this.TransactionType = TransactionType;
            this.TransactionTime = TransactionTime;
            this.TransactionName = TransactionName;
            this.TransactionValue = transactionValue;
        }

        [XmlAttribute("TransactionType")]
        public TransactionTypes TransactionType
        { get; set; }

        [XmlIgnore]
        public DateOnly TransactionTime
        { get; set; }

        [XmlAttribute("TransactionName")]
        public string? TransactionName
        { get; set; }

        [XmlAttribute("TransactionValue")]
        public decimal TransactionValue
        { get; set; }

        //This exists because XmlReader can't handle DateOnly objects. Sad.
        [XmlAttribute("DateString")]
        public string TimeString 
        { 
            get { return this.TransactionTime.ToString("yyyy-MM-dd"); }
            set { this.TransactionTime = DateOnly.Parse(value); } 
        }
    }
}
