namespace Project2_TransactionTracker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Create instance of history & read file if it exists.
            TransactionHistory transactionHistory = new TransactionHistory();

            string filePath = @"../TransactionHistory.txt";
            if (File.Exists(filePath))
            {
                transactionHistory.ReadStorage(filePath);
                Console.WriteLine($"Imported data from {filePath}");
            }

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine($"Current balance: {transactionHistory.CalculateBalance()}");
            Console.WriteLine("");

            string input;
            do 
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Commands: 'add' 'edit' 'remove' 'list' 'quit'");
                input = Console.ReadLine();

                switch (input) 
                {
                    case "quit":
                        break;

                    case "add":
                        transactionHistory.AddTransaction();
                        break;

                    case "edit":
                        transactionHistory.EditTransaction();
                        break;

                    case "remove":
                        transactionHistory.RemoveTransaction();
                        break;

                    case "list":
                        transactionHistory.ShowTransactions();
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Unknown command");
                        break;

                }

            } while (input != "quit");
        }
    }
}