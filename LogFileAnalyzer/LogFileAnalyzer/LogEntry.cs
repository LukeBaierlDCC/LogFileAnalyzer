using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

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

        public override int GetHashCode()
        {
            //overrides support hash-based collections, potentially incorporating LINQ operations
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + Timestamp.GetHashCode();
                hash = hash * 23 + (Level?.GetHashCode() ?? 0);
                hash = hash * 23 + (Message?.GetHashCode() ?? 0);
                hash = hash * 23 + (IPAddress?.GetHashCode() ?? 0);
                hash = hash * 23 + (Request?.GetHashCode() ?? 0);
                hash = hash * 23 + StatusCode.GetHashCode();
                hash = hash * 23 + ThreadId.GetHashCode();
                hash = hash * 23 + (Source?.GetHashCode() ?? 0);
                hash = hash * 23 + (User?.GetHashCode() ?? 0);
                return hash;
            }
        }

        public bool IsError()
        {
            //method returning a boolean indicating if this log entry represents an error
            return Level.Equals("ERROR", StringComparison.OrdinalIgnoreCase);
        }

        public string GetFormattedTimestamp()
        {
            //for returning the timestamp in a specific format, I am looking into CultureInfo
            return Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

        public TimeSpan CalculateTimeDifference(LogEntry otherEntry)
        {
            //computes the time difference from another Log Entry
            if (otherEntry == null)
            {
                throw new ArgumentException(nameof(otherEntry), "The other log entry cannot be null.");
            }

            TimeSpan difference = this.Timestamp.ToUniversalTime() - otherEntry.Timestamp.ToUniversalTime();

            //self-note...if this line below is not necessary, then just comment out
            double absoluteMilliseconds = Math.Abs(difference.TotalMilliseconds);

            return TimeSpan.FromMilliseconds(absoluteMilliseconds);
            //return TimeSpan.FromMilliseconds(Math.Abs(difference.TotalMilliseconds)); //<- in case the 'double' line is not needed
        }

        public bool MatchesFilter(Func<LogEntry, bool> filter)
        {
            //checks if this log entry matches specific criteria, useful for dynamic filtering. Incorporating lambda expressions for conditions.
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter), "The filter cannot be null.");
            }

            return filter(this);
        }

        public string ToJson()
        {
            //serializes Log Entry, looking into JSON incorporation
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        public void FromJson(string json)
        {
            //deserializes JSON string into object
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("JSON string cannot be null or empty.", nameof(json));
            }

            LogEntry deserialized = JsonSerializer.Deserialize<LogEntry>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            this.Timestamp = deserialized.Timestamp;
            this.Level = deserialized.Level;
            this.Message = deserialized.Message;
            this.Id = deserialized.Id;
            this.IPAddress = deserialized.IPAddress;
            this.Request = deserialized.Request;
            this.StatusCode = deserialized.StatusCode;
            this.ThreadId = deserialized.ThreadId;
            this.Source = deserialized.Source;
            this.User = deserialized.User;
        }
    }
}
