namespace Project2_TransactionTracker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string filePath = @"../TransactionHistory.xml";

            //Create instance of history & read file if it exists.
            TransactionHistory History = new TransactionHistory(filePath);
            Console.WriteLine("Commands:\n" +
                "> Add: Add transaction.\n" +
                "> Fastadd: Shortened Add method\n" +
                "> Edit: Edit or Remove transaction. \n" +
                "> Sort: Sort transactions by category.\n" +
                "> List: Display transactions by category.\n" +
                "> Reset: Reset transaction history.\n" +
                "> Quit: Quit the program.\n" +
                "List & Sort have shortened versions. Append by [method]."
            );
            Console.WriteLine($"\nCurrent balance: {History.CalculateBalance()}");

            string input;
            do 
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nMain Commands: Add, Fastadd, Edit, List, Sort, Reset, Quit.");
                Console.Write("Input main: ");
                input = Console.ReadLine();

                switch (input) 
                {
                    case "quit":
                        break;

                    case "add":
                        TransactionManager.New(History);
                        break;

                    case "fastadd":
                        TransactionManager.FastAdd(History);
                        break;

                    case "edit":
                        TransactionManager.Edit(History);
                        break;


                    case "list":
                        Console.WriteLine("List methods: All / Expense / Income");
                        Console.Write("List: ");
                        string? listInput = Console.ReadLine().Trim();
                        History.ShowTransactions(listInput);
                        break;

                    case "list all":
                        History.ShowTransactions("all");
                        break;

                    case "list expense":
                        History.ShowTransactions("expense");
                        break;

                    case "list income":
                        History.ShowTransactions("income");
                        break;


                    case "sort":
                        Console.WriteLine("Sort methods: Date / Name / Cost");
                        Console.Write("Sort by: ");
                        string sortInput = Console.ReadLine().Trim();
                        History.Sort(sortInput);
                        break;

                    case "sort date":
                        History.Sort("date");
                        break;

                    case "sort name":
                        History.Sort("name");
                        break;

                    case "sort cost":
                        History.Sort("cost");
                        break;

                    case "reset":
                        History.ResetHistory();
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("> Unknown command");
                        break;

                }

                //Save after each change
                History.WriteHistory();
            }
            while (input != "quit");
        }
    }
}