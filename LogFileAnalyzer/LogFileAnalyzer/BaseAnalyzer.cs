using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LogFileAnalyzer
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Debug
    }

    public class BaseAnalyzer : IDisposable
    {
        public string LogPath { get; private set; } //this is a Read-Only after initialization by using the private set
        public string OutputPath { get; set; }
        public Dictionary<string, string> Config { get; private set; }
        public string AnalyzerName { get; set; }
        public LogLevel LogLevel { get; set; } = LogLevel.Info;
        public DateTime LastAnalysisTime { get; private set; }

        //public string LogPath { get; set; } //sets the property from outside the class (allows for the property to be both read and set from outside the class,
        //might not be the best approach if working with encapsulation -- saving this for later in case I prefer this approach
        public BaseAnalyzer() : this(string.Empty) { }

        public BaseAnalyzer(string logPath)
        {
            LogPath = logPath;
            Config = new Dictionary<string, string>();
        }

        protected List<LogEntry> logs = new List<LogEntry>();

        public virtual void ReadLogs()
        {
            //reads the logs from the source
            if (string.IsNullOrEmpty(this.LogPath))
            {
                throw new InvalidOperationException("LogPath cannot be null or empty.");
            }

            try
            {
                using (StreamReader sr = new StreamReader(this.LogPath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        LogEntry logEntry = this.ParseLogLine(line);
                        if (logEntry != null)
                        {
                            logs.Add(logEntry);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                throw new Exception("Error reading log file", ex);
            }            
        }

        protected virtual LogEntry ParseLogLine(string line)
        {
            //parses each line into log entry objects after reading logs
            if (string.IsNullOrEmpty(line))
            {
                return null;
            }

            string[] parts = line.Split(new[] { ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 4)
            {
                return null;
            }

            DateTime timestamp;
            if (!DateTime.TryParse(parts[0] + " " + parts[1], out timestamp))
            {
                //handle parsing errors
                return null;
            }

            //LogLevel level;
            //if (!Enum.TryParse<LogLevel>(parts[2], true, out level))
            //{
            //    return null;
            //}

            string level = parts[2];
            string message = string.Join(" ", parts.Skip(3));

            return new LogEntry
            {
                Timestamp = timestamp,
                Level = level,
                Message = message,
                Id = Guid.NewGuid()
            };
        }

        public virtual void FilterLogs(List<LogEntry> logs)
        {
            //filters logs based on criteria
            if (logs == null || logs.Count == 0)
            {
                throw new ArgumentException("Log collection cannot be null or empty", nameof(logs));
            }

            logs = logs.Where(log =>
                Enum.Parse<LogLevel>(log.Level) >= this.LogLevel).ToList();

            this.SpecificFilterLogs(logs);
        }

        protected virtual void SpecificFilterLogs(List<LogEntry> logs)
        {
            //override in derived classes for custom processing logic
        }

        public virtual void ProcessLogs(List<LogEntry> logs)
        {
            //processes the logs when applicable
            if (logs == null || logs.Count == 0)
            {
                throw new ArgumentException("Log collection cannot be null or empty", nameof(logs));
            }

            var logLevelCounts = logs.GroupBy(log => log.Level).ToDictionary(g => g.Key, g => g.Count());

            this.SpecificProcessLogs(logs);
        }

        protected virtual void SpecificProcessLogs(List<LogEntry> logs)
        {
            //override in derived classes for custom processing logic
        }

        public virtual string GenerateReport()
        {
            //creates a summary or detailed report
            LastAnalysisTime = DateTime.Now;

            var logLevelCounts = logs.GroupBy(log => log.Level).ToDictionary(g => g.Key, g => g.Count());

            int errorCount = logLevelCounts.TryGetValue("Error", out int errors) ? errors : 0;
            int warningCount = logLevelCounts.TryGetValue("Warning", out int warnings) ? warnings : 0;

            string timeRange = logs.Any()
                ? $"from {logs.Min(l => l.Timestamp)} to {logs.Max(l => l.Timestamp)}"
                : "No logs available";

            StringBuilder report = new StringBuilder();
            report.AppendLine($"Log Analysis Report as of {LastAnalysisTime}");
            report.AppendLine($"Time Range: {timeRange}");
            report.AppendLine($"Total Errors: {errorCount}");
            report.AppendLine($"Total Warnings: {warningCount}");
            report.AppendLine("Log Level Distribution:");
            foreach (var pair in logLevelCounts)
            {
                report.AppendLine($"- {pair.Key}: {pair.Value}");
            }

            return report.ToString();
        }

        public void SaveReport(string report)
        {
            //saves report to file for output
            if (string.IsNullOrEmpty(OutputPath))
            {
                throw new InvalidOperationException("OutputPath has not been set.");
            }
            try
            {
                System.IO.File.WriteAllText(OutputPath, report);
                Console.WriteLine($"Report successfully saved to {OutputPath}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save report to {OutputPath}", ex);
            }
        }

        public virtual void Dispose()
        {
            //ensures all resources are managed accordingly post-analysis
            this.logs.Clear();
        }

        public void ValidateConfig()
        {
            //validates the config if applicable
            if (Config == null)
            {
                throw new InvalidOperationException("Configuration dictionary is null.");
            }

            if (!Config.ContainsKey("logLevel") || !Enum.TryParse<LogLevel>(Config["logLevel"], true, out _))
            {
                throw new ArgumentException("Invalid or missing logLevel in configuration.");
            }

            if (!Config.ContainsKey("outputPath") || string.IsNullOrWhiteSpace(Config["outputPath"]))
            {
                throw new ArgumentException("Invalid or missing outputPath in configuration.");
            }

            OutputPath = Config["outputPath"];
            LogLevel = Enum.Parse<LogLevel>(Config["logLevel"], true);
        }

        public virtual void Analyze()
        {
            //entry point for analysis, allows derived classes to override with specific analysis logic
            try
            {               
                this.ReadLogs();

                List<LogEntry> logs = this.GetLogs();

                this.FilterLogs(logs);
                this.ProcessLogs(logs);
                string report = this.GenerateReport();
                this.SaveReport(report);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred during analysis", ex);
            }
            finally
            {
                this.Dispose();
            }

        }

        protected List<LogEntry> GetLogs()
        {
            return this.logs;
        }

        protected virtual void SpecificAnalysis()
        {
            throw new NotImplementedException("This method should be overridden in derived class.");
        }
    }
}
