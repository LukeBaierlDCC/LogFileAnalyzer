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
        public string LogPath { get; }

        public virtual void Analyze()
        {

        }

    }
}
