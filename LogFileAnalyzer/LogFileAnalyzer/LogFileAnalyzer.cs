using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogFileAnalyzer
{
    public class LogFileAnalyzer : BaseAnalyzer
    {
        //properties: Logs (List of LogEntry); ProcessingQueue (Queue of LogEntry)        
        //methods: Analyze (override); ReadLogsAsync; ProcessLogsParallel;ParseLogEntry; ApplyDynamicFilter; ManageMemory
        //public string Logs { get; set; }
        public List<string> Logs { get; set; } = new List<string>();
        //public string ProcessingQueue { get; set;  }
        public ConcurrentQueue<string> ProcessingQueue { get; set; } = new ConcurrentQueue<string>();        

        public async Task ReadLogsAsync(string filePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string content = await reader.ReadToEndAsync();
                    Logs = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading logs: {ex.Message}");
            }
        }

        private LogEntry ParseLogEntry(string logLine)
        {
            if (string.IsNullOrWhiteSpace(logLine))
                return null;

            var logEntry = new LogEntry();
            string pattern = @"(?<ip>\d+\.\d+\.\d+\.\d+) - - \[(?<timestamp>.*?)\] ""(?<request>.*?)"" (?<status>\d+) (?<size>\d+|-)";

            Match match = Regex.Match(logLine, pattern);

            if (match.Success)
            {
                logEntry.IPAddress = match.Groups["ip"].Value;
                logEntry.Request = match.Groups["request"].Value;
                logEntry.StatusCode = int.TryParse(match.Groups["status"].Value, out int status) ? status : 0;
                logEntry.Timestamp = DateTime.TryParse(match.Groups["timestamp"].Value, out DateTime timestamp) ? timestamp : DateTime.MinValue;
            }

            return logEntry;
        }

        private void ProcessLog(string logLine)
        {
            var log = ParseLogEntry(logLine);

            switch (log.Level)
            {
                case "Error":
                    break;
                case "Info":
                    break;
                default:
                    break;
            }
        }

        public async Task ProcessLogsParallel()
        {
            var tasks = Logs.Select(log => Task.Run(() => ProcessLog(log))).ToList();
            await Task.WhenAll(tasks);
        }

        public override void Analyze()
        {
            
        }

        public Func<LogEntry, bool> ApplyDynamicFilter(string filterCriteria)
        {
            string[] parts = filterCriteria.Split('=');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid filter criteria format", nameof(filterCriteria));
            }

            string propertyName = parts[0].Trim();
            string value = parts[1].Trim();

            var parameter = Expression.Parameter(typeof(LogEntry), "log");

            if (propertyName == "Level")
            {
                var property = Expression.Property(parameter, propertyName);
                var constant = Expression.Constant(value);
                var body = Expression.Equal(property, constant);

                var lambda = Expression.Lambda<Func<LogEntry, bool>>(body, parameter);
                return lambda.Compile();
            }
            else
            {
                throw new NotSupportedException($"Filtering by {propertyName} is not supported.");
            }
        }

        public void ManageMemory()
        {
            //optimization logic
        }
    }
}
