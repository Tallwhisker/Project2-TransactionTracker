﻿namespace Project2_TransactionTracker
{
    internal class Program
    {
        private static void Main(string[] args)
        {           
            string filePath = @"./TransactionHistory.xml";
            TransactionHistory History = new TransactionHistory(filePath);

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("-------------\n" +
                "| KittyCash |\n" +
                "-------------"
            );
            Console.WriteLine("\nCommands:\n" +
                "> Add: Add transaction.\n" +
                "> FastAdd: Shortened Add method.\n" +
                "> Edit: Edit or Remove transaction. \n" +
                "> Sort: Sort transactions by category.\n" +
                "> >Sort Methods: Date, Name, Value (Ascending/Descending)\n" +
                "> List: Display transactions by category.\n" +
                "> >List Methods: All, Expense, Income.\n" +
                "> Reset: Reset transaction history.\n" +
                "> Quit: Quit the program.\n" +
                "List & Sort have shortened versions. Append by [method].\n" +
                "Example: \"sort date\" to sort by date ascending."
            );
            Console.WriteLine($"\nCurrent balance: {History.CalculateBalance()}");

            string? input;
            do 
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("\nMain Commands: Add, Fastadd, Edit, List, Sort, Reset, Quit, Help");
                Console.Write("Input main: ");
                input = Console.ReadLine();

                switch (input.ToLower()) 
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

                    //Sort has 1 required and 1 optional argument.
                    case "sort":
                        Console.WriteLine("Sort methods: Date / Name / Value");
                        Console.Write("Sort by: ");
                        string? sortInput = Console.ReadLine().Trim();
                        History.Sort(sortInput);
                        break;

                    case "sort date":
                        History.Sort("date", "a");
                        break;

                    case "sort name":
                        History.Sort("name", "a");
                        break;

                    case "sort value":
                        History.Sort("value", "a");
                        break;


                    case "reset":
                        History.ResetHistory();
                        break;

                    case "help":
                        Console.WriteLine("\nCommands:\n" +
                            "> Add: Add transaction.\n" +
                            "> FastAdd: Shortened Add method.\n" +
                            "> Edit: Edit or Remove transaction. \n" +
                            "> Sort: Sort transactions by category.\n" +
                            "> >Sort Methods: Date, Name, Value (Ascending/Descending)\n" +
                            "> List: Display transactions by category.\n" +
                            "> >List Methods: All, Expense, Income.\n" +
                            "> Reset: Reset transaction history.\n" +
                            "> Quit: Quit the program.\n" +
                            "List & Sort have shortened versions. Append by [method].\n" +
                            "Example: \"sort date\" to sort by date ascending."
                        );
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("> Unknown command.");
                        break;
                }
                //Save after each change
                History.WriteHistory();
            }
            while (input != "quit");
        }
    }
}