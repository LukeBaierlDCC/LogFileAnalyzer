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
        private List<string> Logs { get; set; } = new List<string>();
        //public string ProcessingQueue { get; set;  }
        public ConcurrentQueue<string> ProcessingQueue { get; set; } = new ConcurrentQueue<string>();

        public LogFileAnalyzer(string logPath) : base(logPath)  //preserving this as back-up method in case public BaseAnalyzer() : this(string.Empty) { } fails
        {

        }

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
                throw new IOException($"Error reading logs from {filePath}: {ex.Message}", ex);
            }
        }

        private LogEntry ParseLogEntry(string logLine)
        {
            if (string.IsNullOrWhiteSpace(logLine))
                return null;

            var logEntry = new LogEntry();
            string pattern = @"(?<ip>\d+\.\d+\.\d+\.\d+) - - \[(?<timestamp>.*?)\] ""(?<request>.*?)"" (?<status>\d+) (?<size>\d+|-) (?<level>.*?)$";

            Match match = Regex.Match(logLine, pattern);

            if (match.Success)
            {
                logEntry.IPAddress = match.Groups["ip"].Value;
                logEntry.Request = match.Groups["request"].Value;
                logEntry.StatusCode = int.TryParse(match.Groups["status"].Value, out int status) ? status : 0;
                logEntry.Timestamp = DateTime.TryParse(match.Groups["timestamp"].Value, out DateTime timestamp) ? timestamp : DateTime.MinValue;
                logEntry.Level = match.Groups["level"].Value;
            }

            return logEntry;
        }

        private void ProcessLog(string logLine)
        {
            var log = ParseLogEntry(logLine);

            if (log != null)
            {
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
        }

        public async Task ProcessLogsParallel()
        {
            var tasks = Logs.Select(log => Task.Run(() => ProcessLog(log))).ToList();
            await Task.WhenAll(tasks);
        }

        private Tuple<DateTime, DateTime> CalculateTimeRange(List<LogEntry> logs)
        {
            if (logs == null || !logs.Any())
            {
                return new Tuple<DateTime, DateTime>(DateTime.MinValue, DateTime.MinValue);
            }

            var minTime = logs.Min(l => l.Timestamp);
            var maxTime = logs.Max(l => l.Timestamp);

            return new Tuple<DateTime, DateTime>(minTime, maxTime);
        }

        private Tuple<DateTime, DateTime> CalculateAverageResponseTime(List<LogEntry> logs)
        {
            if (logs == null || !logs.Any())
            {
                return new Tuple<DateTime, DateTime>(DateTime.MinValue, DateTime.MinValue);
            }

            var minTime = logs.Min(l => l.Timestamp);
            var maxTime = logs.Max(l => l.Timestamp);

            return new Tuple<DateTime, DateTime>(minTime, maxTime);
        }

        public override void Analyze()
        {
            try
            {
                string filePath = LogPath;
                //string filePath = "path/to/your/logfile.log";
                ReadLogsAsync(filePath).Wait();

                var logEntries = Logs.Select(ParseLogEntry).Where(l => l != null).ToList();
                
                ProcessLogsParallel().Wait();

                string filterCriteria = "Level=Error";
                var filter = ApplyDynamicFilter(filterCriteria);

                //var logEntries = Logs.Select(ParseLogEntry).ToList();

                var filteredLogs = logEntries.Where(filter).ToList();

                int errorCount = filteredLogs.Count(log => log.Level == "Error");

                var (minTime, maxTime) = CalculateTimeRange(filteredLogs);
                var averageResponseTime = CalculateAverageResponseTime(filteredLogs);

                Console.WriteLine($"Total errors found: {errorCount}");
                Console.WriteLine($"Time range of logs: {minTime} to {maxTime}");
                Console.WriteLine($"Average response time: {averageResponseTime}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during analysis: {ex.Message}");
            }
        }

        public Func<LogEntry, bool> ApplyDynamicFilter(string filterCriteria)
        {
            //string[] parts = filterCriteria.Split('=');
            //if (parts.Length != 2)
            //{
            //    throw new ArgumentException("Invalid filter criteria format", nameof(filterCriteria));
            //}

            //string propertyName = parts[0].Trim();
            //string value = parts[1].Trim();

            //var parameter = Expression.Parameter(typeof(LogEntry), "log");

            //if (propertyName == "Level")
            //{
            //    var property = Expression.Property(parameter, propertyName);
            //    var constant = Expression.Constant(value);
            //    var body = Expression.Equal(property, constant);

            //    var lambda = Expression.Lambda<Func<LogEntry, bool>>(body, parameter);
            //    return lambda.Compile();
            //}
            //else
            //{
            //    throw new NotSupportedException($"Filtering by {propertyName} is not supported.");
            //}

            var criteria = filterCriteria.Split(new[] { "AND", "OR" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim())
                .ToArray();

            Func<LogEntry, bool>[] filters = criteria.Select(BuildFilter).ToArray();

            return entry => filters.Aggregate(true, (current, filter) => current && filter(entry));
        }

        private Func<LogEntry, bool> BuildFilter(string singleCriteria)
        {
            string[] parts = singleCriteria.Split('=');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid filter criteria format", nameof(singleCriteria));
            }

            string propertyName = parts[0].Trim();
            string value = parts[1].Trim();

            var parameter = Expression.Parameter(typeof(LogEntry), "log");
            var property = Expression.Property(parameter, propertyName);

            var propertyType = typeof(LogEntry).GetProperty(propertyName).PropertyType;

            object parsedValue;
            if (propertyType == typeof(string))
            {
                parsedValue = value;
            }
            else if (propertyType == typeof(int))
            {
                if (!int.TryParse(value, out int intValue))
                    throw new ArgumentException($"Failed to parse '{value}' to int for property '{propertyName}'");
                parsedValue = intValue;
            }
            else if (propertyType == typeof(DateTime))
            {
                if (!DateTime.TryParse(value, out DateTime dateTimeValue))
                    throw new ArgumentException($"Failed to parse '{value}' to int for property '{propertyName}'");
                parsedValue = dateTimeValue;
            }
            else
            {
                throw new NotSupportedException($"Filtering by property type {propertyType.Name} is not supported.");
            }

            var constant = Expression.Constant(parsedValue, propertyType);
            var body = Expression.Equal(property, constant);

            return Expression.Lambda<Func<LogEntry, bool>>(body, parameter).Compile();
        }

        public void ManageMemory()
        {
            //optimization logic
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            object boxedInt = 42;
            if (boxedInt is int unboxedInt)
            {
                Console.WriteLine($"Unboxed: {unboxedInt}");
            }

            if (Logs != null && Logs.Count > 10000)
            {
                Logs.Clear();
                Logs = null;
                Logs = new List<string>();
            }

            using (var tempStream = new MemoryStream())
            {

            }

            Console.WriteLine("Memory management tasks performed.");
        }

        //public override void SpecificAnalysis()
        //{
        //    throw new NotImplementedException("This method should be overridden in derived class.");
        //}
    }
}
