using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFileAnalyzer
{
    public class BaseAnalyzer
    {
        public string LogPath { get; private set; } //this is a Read-Only after initialization by using the private set
        public string OutputPath { get; set; }
        public Dictionary<string, string> Config { get; set; }
        public string AnalyzerName { get; set; }
        
        //public string LogPath { get; set; } //sets the property from outside the class (allows for the property to be both read and set from outside the class,
        //might not be the best approach if working with encapsulation -- saving this for later in case I prefer this approach

        public virtual void Analyze()
        {
            //entry point for analysis, allows derived classes to override with specific analysis logic
        }

        public virtual void ReadLogs()
        {
            //reads the logs from the source
        }

        public void ParseLogLine()
        {
            //parses each line into log entry objects after reading logs
        }

        public void FilterLogs()
        {
            //filters logs based on criteria
        }

        public void ProcessLogs()
        {
            //processes the logs when applicable
        }

        public void GenerateReport()
        {
            //creates a summary or detailed report
        }

        public void SaveReport()
        {
            //saves report to file for output
        }

        public void CleanUp()
        {
            //ensures all resources are managed accordingly post-analysis
        }

        public void ValidateConfig()
        {
            //validates the config if applicable
        }
    }
}
