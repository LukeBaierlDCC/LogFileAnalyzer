using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFileAnalyzer
{
    public class BaseAnalyzer
    {
        //properties: LogPath
        //methods: Analyze (virtual)
        public string LogPath { get; private set; } //this is a Read-Only after initialization by using the private set
        
        

        public virtual void Analyze()
        {

        }

    }
}
