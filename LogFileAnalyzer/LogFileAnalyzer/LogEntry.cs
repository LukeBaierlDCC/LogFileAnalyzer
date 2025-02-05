﻿using System;
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

        public void ParseFromLogLine(string logLine)
        {
            //used to parse log line string into properties of LogEntry...looking into RegEx or string manipulation
            if (string.IsNullOrEmpty(logLine))
            {
                throw new ArgumentException("Log line cannot be null or empty.", nameof(logLine));
            }

            string[] parts = logLine.Split(' ');
            if (parts.Length < 7)
            {
                throw new FormatException("Log line does not match the expected format.");
            }

            if (DateTime.TryParse(parts[0] + " " + parts[1], out DateTime dt))
            {
                Timestamp = dt;
            }
            else
            {
                //invalid date format handling will go here
            }

            //more logic for parsing and splitting currently under research
            //separate by array data
            //parse and trim
            //parse out data
            //join message strings
            Level = parts[2];
            ThreadId = int.Parse(parts[3].Trim('[', ']'));
            IPAddress = parts[4];
            Request = parts[5];
            StatusCode = int.TryParse(parts[7], out int status) ? status : -1;
            Message = string.Join(" ", parts.Skip(8));
            //source help https://stackoverflow.com/questions/54923764/parsing-string-data-in-c-net
            //https://stackoverflow.com/questions/17903885/c-sharp-string-trimming
            //https://learn.microsoft.com/en-us/dotnet/csharp/how-to/parse-strings-using-split (stackoverflow clarifies what MS docs generalizes)
        }

        public override string ToString()
        {
            //provides a formatted string representation of log entry for display or logging
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level} [Thread-{ThreadId}] {IPAddress} {Request} - {StatusCode} {Message}";
        }

        public override bool Equals(object obj)
        {
            //may be needed for deduplication or comparison operations
            if (obj == null || GetType() != obj.GetType())
                return false;

            LogEntry other = (LogEntry)obj;
            return Id == other.Id &&
                Timestamp == other.Timestamp &&
                Level == other.Level &&
                Message == other.Message &&
                IPAddress == other.IPAddress &&
                Request == other.Request &&
                StatusCode == other.StatusCode &&
                ThreadId == other.ThreadId &&
                Source == other.Source &&
                User == other.User;
        }

        //public override int GetHashCode()
        //{
        //    //overrides support hash-based collections, potentially incorporating LINQ operations

        //}

        //public bool IsError()
        //{
        //    //method returning a boolean indicating if this log entry represents an error

        //}

        //public string GetFormattedTimestamp()
        //{
        //    //for returning the timestamp in a specific format, I am looking into CultureInfo

        //}

        public void CalculateTimeDifference(LogEntry otherEntry)
        {
            //computes the time difference from another Log Entry, looking into Math.Abs

        }

        //public bool MatchesFilter(Func<LogEntry, bool> filter)
        //{
        //    //checks if this log entry matches specific criteria, useful for dynamic filtering. Incorporating lambda expressions for conditions.

        //}

        //public string ToJson()
        //{
        //    //serializes Log Entry, looking into JSON incorporation

        //}

        public void FromJson(string json)
        {
            //deserializes JSON string into object

        }
    }
}
