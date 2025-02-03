using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void ParseLogEntry()
        {

        }

        public void ProcessLogsParallel()
        {

        }

        public override void Analyze()
        {
            
        }
        
        public void ApplyDynamicFilter()
        {

        }

        public void ManageMemory()
        {
            //optimization logic
        }
    }
}
