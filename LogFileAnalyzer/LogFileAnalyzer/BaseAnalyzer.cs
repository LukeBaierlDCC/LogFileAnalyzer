using System;
using System.Collections.Generic;
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

    public class BaseAnalyzer
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

        public virtual void FilterLogs()
        {
            //filters logs based on criteria

        }

        public virtual void ProcessLogs()
        {
            //processes the logs when applicable

        }

        public virtual string GenerateReport()
        {
            //creates a summary or detailed report
            LastAnalysisTime = DateTime.Now;
            return "";
        }

        public void SaveReport(string report)
        {
            //saves report to file for output

        }

        public virtual void Dispose()
        {
            //ensures all resources are managed accordingly post-analysis

        }

        public void ValidateConfig()
        {
            //validates the config if applicable

        }

        public virtual void Analyze()
        {
            //entry point for analysis, allows derived classes to override with specific analysis logic
            try
            {
                this.ReadLogs();
                this.FilterLogs();
                this.ProcessLogs();
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

        protected virtual void SpecificAnalysis()
        {
            throw new NotImplementedException("This method should be overridden in derived class.");
        }
    }
}
