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
        
        //public string LogPath { get; set; } //sets the property from outside the class (allows for the property to be both read and set from outside the class,
        //might not be the best approach if working with encapsulation -- saving this for later in case I prefer this approach

        public virtual void Analyze()
        {

        }

    }
}
