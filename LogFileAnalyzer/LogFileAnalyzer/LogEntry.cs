using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFileAnalyzer
{
    internal class LogEntry
    {
        //properties: Timestamp; Level; Message; Id (GUID)
        //methods: GetSeverityAndCount
        public string Timestamp { get; set; } //using string for more flexibility and brevity - date not needed
        public string Level { get; set; }
        public string Message { get; set; }
        public string Id { get; set; }


    }
}
