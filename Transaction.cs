using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2_TransactionTracker
{
    internal class Transaction
    {
        public Transaction(DateOnly TransactionTime, string? typeOfTransaction, string TransactionName, decimal transactionValue)
        {
            this.TransactionTime = TransactionTime;
            this.TransactionName = TransactionName;
            this.TypeOfTransaction = typeOfTransaction;
            this.TransactionValue = transactionValue;
        }

        public DateOnly TransactionTime { get; set; }
        public string TransactionName { get; set; }
        public string ?TypeOfTransaction { get; set; }
        public decimal TransactionValue { get; set; }

    }
}
