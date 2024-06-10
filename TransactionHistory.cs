using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Project2_TransactionTracker
{

    public class TransactionHistory
    {
        public TransactionHistory(string FilePath)
        {
            this.FilePath = FilePath;

            //ReadHistory checks FilePath and reads history, or returns empty List.
            this._transactions = this.ReadHistory();
            this._balance = 0;
        }

        public string FilePath { get; set; }
        private List<Transaction> _transactions;
        private decimal _balance;


        internal decimal CalculateBalance()
        {
            this._balance = (decimal)this._transactions.Sum(value => value.TransactionValue);
            return _balance;
        }

        internal void Add(Transaction transaction) 
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Added transaction.");
            _transactions.Add(transaction);
        }


        public void Remove(int index)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                this._transactions.RemoveAt(index);
                Console.WriteLine($"Removed transaction at index [{index}].");
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: Index {index} does not exist.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error processing device index.");
                Console.WriteLine(e);
            }
        }


        public void EditTransaction(int index, Transaction transaction)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                this._transactions[index] = transaction;
                Console.WriteLine($"Edited transaction at index [{index}].");
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: Index [{index}] does not exist.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error editing index [{index}].");
                Console.WriteLine(e);
            }
        }


        public void Sort(string method)
        {
            switch (method.ToLower())
            {
                case "name":
                    this._transactions = this._transactions.OrderBy(name => name.TransactionName).ToList();
                    break;

                case "cost":
                    this._transactions = this._transactions.OrderBy(value => value.TransactionValue).ToList();
                    break;

                case "date":
                    this._transactions = this._transactions.OrderBy(date => date.TransactionTime).ToList();
                    break;

                default:
                    Console.WriteLine("Input not recognized. Defaulting to Date.");
                    this.Sort("date");
                    break;
            }
        }


        public void ShowTransactions(string method)
        {
            Console.WriteLine("Listing transactions.");
            Console.WriteLine($"" +
                "Index".PadRight(13) +
                "Date".PadRight(17) +
                "Name".PadRight(25) +
                "Value".PadRight(15)
            );

            int index = 0;
            foreach (var item in _transactions)
            {
                string type = item.TransactionType.ToString();
                Console.WriteLine(type);
                switch (method.ToLower())
                {
                    case item.TransactionType.ToString():
                        Console.WriteLine($"" +
                            $"{index,-13}" +
                            $"{item.TransactionTime,-17}" +
                            $"{item.TransactionName,-25}" +
                            $"{item.TransactionValue,-15}"
                        );
                        break;

                    case "expense":
                        Console.WriteLine($"" +
                            $"{index,-13}" +
                            $"{item.TransactionTime,-17}" +
                            $"{item.TransactionName,-25}" +
                            $"{item.TransactionValue,-15}"
                        );
                        break;

                    case "income":
                        Console.WriteLine($"" +
                            $"{index,-13}" +
                            $"{item.TransactionTime,-17}" +
                            $"{item.TransactionName,-25}" +
                            $"{item.TransactionValue,-15}"
                        );
                        break;

                    default:
                        Console.WriteLine("List method not recognized, using All.");
                        this.ShowTransactions("All");
                        break;
                }
                index++;
            }
        }


        //Check FilePath for history, else return empty List.
        public List<Transaction> ReadHistory()
        {
            List<Transaction> transactions = new List<Transaction>();
            try
            {
                if (File.Exists(FilePath))
                {
                    XmlReader reader = XmlReader.Create(FilePath);
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Transaction>));

                    using (reader)
                    {
                        transactions = (List<Transaction>)serializer.Deserialize(reader);
                    }
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine($"Error loading file: {e}");
                File.Replace(FilePath, FilePath, @"./backup.xml");
                Console.WriteLine($"Data file at \"{FilePath}\" has been replaced with an empty file due to this error.");
            }

            return transactions;
        }


        internal void WriteHistory()
        {
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent = true;
            writerSettings.OmitXmlDeclaration = true;
            XmlWriter writer = XmlWriter.Create(this.FilePath, writerSettings);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlSerializer serializer = new XmlSerializer(typeof(List<Transaction>));

            using (writer)
            {
                serializer.Serialize(writer, this._transactions, namespaces);
            }
        }

        internal void ResetHistory()
        {
            Console.WriteLine("Reset history, you sure? Y/N");
            string input = Console.ReadLine();

            switch (input)
            {
                case "y":
                    if (File.Exists(FilePath)) 
                    {
                        Console.WriteLine($"Deleting history at {FilePath}");
                        File.Delete(FilePath);
                    }
                    break;

                default:
                    break;
            }
        }


        //public void ReadHistory()
        //{
        //    List<string> storage = File.ReadAllLines(FilePath).ToList();

        //    foreach (string line in storage)
        //    {
        //        string[] items = line.Split(',');
        //        decimal amount = Convert.ToDecimal(items[3]);

        //        DateOnly dateOnly = DateOnly.Parse(items[0]);

        //        this._transactions.Add(new Transaction(
        //            dateOnly, //Date
        //            items[1], //Type
        //            items[2], //Name
        //            amount //Value
        //            )
        //        );
        //    }
        //}


        //public void WriteStorage(string filePath)
        //{
        //    this.Sort("date");
        //    string[] strings = new string[this._transactions.Count];
        //    int i = 0;
        //    foreach (var item in this._transactions)
        //    {
        //        strings[i] =
        //            $"{item.TransactionTime}," +
        //            $"{item.TypeOfTransaction}," +
        //            $"{item.TransactionName}," +
        //            $"{item.TransactionValue}";
        //        i++;
        //    }
        //    File.WriteAllLines(filePath, strings);
        //}
    }
}
