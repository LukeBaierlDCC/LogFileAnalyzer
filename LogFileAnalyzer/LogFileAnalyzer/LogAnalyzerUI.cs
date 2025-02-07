using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFileAnalyzer
{
    public class LogAnalyzerUI
    {
        //properties: analyzer(LogFileAnalyzer)
        public LogFileAnalyzer Analyzer { get; set; }
        //methods: StartAnalysis; DisplayResults; GetUserInput

        public void StartAnalysis()
        {
            if (Analyzer == null)
            {
                Console.WriteLine("Analyzer not initialized. Please provide input first.");
                return;
            }
            Console.WriteLine("Analysis started.");
            try
            {
                Analyzer.Analyze();
                Console.WriteLine("Analysis completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during analysis: {ex.Message}");
            }
        }

        public void DisplayResults()
        {
            if (Analyzer == null)
            {
                Console.WriteLine("No analysis has been performed yet. Please start an analysis first.");
                return;
            }

            try
            {
                Console.WriteLine("Analysis Results:");
                string report = File.ReadAllText(Analyzer.OutputPath);
                Console.WriteLine(report);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("No report file found. Analysis might not have completed or save correctly.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while displaying results: {ex.Message}");
            }
        }

        public void GetUserInput()
        {
            //self-explanatory
            Console.WriteLine("Please specify the path to the log file:");
            string logFilePath = Console.ReadLine();

            Console.WriteLine("Enter the output path for the analysis report:");
            string outputPath = Console.ReadLine();

            Console.WriteLine("Choose a log level for analysis (Info, Warning, Error, Debug):");
            string logLevelInput = Console.ReadLine();
            LogLevel logLevel = Enum.TryParse<LogLevel>(logLevelInput, true, out LogLevel parsedLevel) ? parsedLevel : LogLevel.Info;

            DateTime? fromDate = null, toDate = null;
            Console.WriteLine("Do you want to filter by time range? (y/n)");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                Console.WriteLine("Enter start date (MM/dd/yyyy HH:mm:ss):");
                fromDate = DateTime.TryParse(Console.ReadLine(), out DateTime start) ? start : (DateTime?)null;
                Console.WriteLine("Enter end date (MM/dd/yyyy HH:mm:ss):");
                toDate = DateTime.TryParse(Console.ReadLine(), out DateTime end) ? end : (DateTime?)null;
            }

            string filterCriteria = "";
            Console.WriteLine("Do you want to apply specific filters? (y/n)");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                Console.WriteLine("Enter filter criteria (e.g., 'Level=Error', 'IP=192.168.1.1'):");
                filterCriteria = Console.ReadLine();
            }

            Analyzer = new LogFileAnalyzer(logFilePath);
            if (Analyzer != null)
            {
                Analyzer.OutputPath = outputPath;
                Analyzer.LogLevel = logLevel;

                if (fromDate.HasValue && toDate.HasValue)
                {
                    Analyzer.SetTimeRange(fromDate.Value, toDate.Value);
                }

                if (!string.IsNullOrEmpty(filterCriteria))
                {
                    Analyzer.ApplyDynamicFilter(filterCriteria);
                }
            }
            else
            {
                Console.WriteLine("Failed to initialize Analyzer.");
            }
        }
    }
}
