namespace Project2_TransactionTracker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string filePath = @"../TransactionHistory.xml";
            //Create instance of history & read file if it exists.
            TransactionHistory History = new TransactionHistory(filePath);

            Console.WriteLine($"\nCurrent balance: {History.CalculateBalance()}");

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
                        TransactionManager.New(History);
                        break;

                    case "edit":
                    case"remove":
                        TransactionManager.Edit(History);
                        break;

                    case "list":
                        Console.WriteLine("All / Expense / Income");
                        string listInput = Console.ReadLine().Trim();
                        History.ShowTransactions(listInput);
                        break;

                    case "sort":
                        Console.WriteLine("Sort by 'date' 'name' 'cost");
                        string sortInput = Console.ReadLine().Trim();
                        History.Sort(sortInput);
                        break;

                    case "dev":
                        History.Add(new Transaction(
                            TransactionTypes.Income,
                            DateOnly.Parse("2024-05-01"),
                            "Lön",
                            9000
                        ));

                        History.Add(new Transaction(
                            TransactionTypes.Expense,
                            DateOnly.Parse("2024-05-02"),
                            "Willys",
                            1324
                        ));
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Unknown command");
                        break;

                }

                //Save after each change
                History.WriteHistory();
            }
            while (input != "quit");
        }
    }
}