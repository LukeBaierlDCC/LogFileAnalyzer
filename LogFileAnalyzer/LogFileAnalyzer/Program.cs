using LogFileAnalyzer;
using System;

namespace LogFileAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            LogAnalyzerUI ui = new LogAnalyzerUI();
            
            Console.WriteLine("Welcome to Log File Analyzer.");
            Console.WriteLine("Commands: 'setup' to configure analysis, 'analyze' to start analysis, 'show' to display results, 'exit' to quit.");

            string command = "";
            do
            {
                try
                {
                    Console.Write("Enter command: ");
                    command = Console.ReadLine().ToLower();

                    switch (command)
                    {
                        case "setup":
                            ui.GetUserInput();
                            break;
                        case "analyze":
                            ui.StartAnalysis();
                            break;
                        case "show":
                            ui.DisplayResults();
                            break;
                        case "exit":
                            Console.WriteLine("Exiting application.");
                            break;
                        default:
                            Console.WriteLine("Invalid option. The only commands are 'setup', 'analyze', 'show' or 'exit'.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            } while (command != "exit");

            Console.WriteLine("Thank you for choosing Log File Analyzer for your organizational needs.");
        }
    }
}