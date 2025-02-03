using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFileAnalyzer
{
    internal class LogFileAnalyzer
    {
        //properties: Logs (List of LogEntry); ProcessingQueue (Queue of LogEntry)        
        //methods: Analyze (override); ReadLogsAsync; ProcessLogsParallel;ParseLogEntry; ApplyDynamicFilter; ManageMemory
        public string Logs { get; }
        public string ProcessingQueue { get; }
    }
}
