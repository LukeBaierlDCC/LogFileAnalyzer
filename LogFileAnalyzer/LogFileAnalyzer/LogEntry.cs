using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFileAnalyzer
{
    public class LogEntry
    {
        //properties: Timestamp; Level; Message; Id (GUID)
        //methods: GetSeverityAndCount
        public string IPAddress { get; set; }
        public string Request { get; set; }
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; } //update: date needed
        public string Level { get; set; }
        public string Message { get; set; }
        public Guid Id { get; private set; }
        public string Source { get; set; }
        public string User { get; set; }
        public int ThreadId { get; set; }

        public LogEntry()
        {
            Id = Guid.NewGuid();
        }

        public (string Severity, int Count) GetSeverityAndCount()
        {
            string severity = Level switch
            {
                "ERROR" => "High",
                "WARNING" => "Medium",
                _ => "Low"
            };

            int count = 1;

            return (severity, count);
        }

        public void ParseFromLogLine()
        {
            //used to parse log line string into properties of LogEntry...looking into RegEx or string manipulation
        }

        public void Equals()
        {
            //may be needed for deduplication or comparison operations
        }

        public void ToString()
        {
            //provides a formatted string representation of log entry for display or logging
        }

        public void IsError()
        {
            //method returning a boolean indicating if this log entry represents an error
        }

        public void GetFormattedTimestamp()
        {
            //for returning the timestamp in a specific format, I am looking into CultureInfo
        }

        public void CalculateTimeDifference()
        {
            //computes the time difference from another Log Entry, looking into Math.Abs
        }

        public void MatchesFilter()
        {
            //checks if this log entry matches specific criteria, useful for dynamic filtering. Incorporating lambda expressions for conditions.
        }

        public void ToJson()
        {
            //serializes Log Entry, looking into JSON incorporation
        }

        public void FromJson()
        {
            //deserializes JSON string into object
        }

        public void GetHashCode()
        {
            //overrides support hash-based collections, potentially incorporating LINQ operations
        }
    }
}
