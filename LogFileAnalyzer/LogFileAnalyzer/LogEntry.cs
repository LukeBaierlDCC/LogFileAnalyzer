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
    }
}
