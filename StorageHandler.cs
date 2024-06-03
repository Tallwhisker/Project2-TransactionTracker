using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Transactions;

namespace Project2_TransactionTracker
{
    static class StorageHandler
    {
        public string filePath = @"../TransactionHistory.txt";

        public static List<Transaction> Read()
        {
            List<string> storage = File.ReadAllLines(filePath).ToList();
            List<Transaction> transactions = [];

            foreach (string line in storage)
            {
                string[] items = line.Split(',');
                decimal amount = Convert.ToDecimal(items[3]);

                string[] date = items[0].Split("-"); //Get and split date into array
                int year = Convert.ToInt32(date[0]);
                int month = Convert.ToInt32(date[1]);
                int day = Convert.ToInt32(date[2]);
                DateOnly dateOnly = new DateOnly(year, month, day);

                transactions.Add(new Transaction(
                    dateOnly, //Date
                    items[1], //Type
                    items[2], //Name
                    amount //Value
                    )
                );
            }
            return transactions;
        }

        public static void Save(List<Transaction> transactionList)
        {
            string[] strings = new string[transactionList.Count];
            int i = 0;
            foreach (var item in transactionList)
            {
                strings[i] = 
                    $"{item.TransactionTime}," +
                    $"{item.TypeOfTransaction}," +
                    $"{item.TransactionName}," +
                    $"{item.TransactionValue}";
                i++;
            }
            File.WriteAllLines(filePath, strings);
        }
    }
}
