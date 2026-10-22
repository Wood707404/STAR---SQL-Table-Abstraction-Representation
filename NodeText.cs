using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFSamples.Samples.HousePlan
{
    public class NodeText : PropertyChangedBase
    {
        string name;
        string type;

        public NodeText(string name, string type)
        {
            this.name = name;
            this.type = type;
        }

        public string Name
        {
            get { return name; }
            set 
            { 
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public string Type
        {
            get { return type; }
            set 
            { 
                type = value;
                OnPropertyChanged("Type");
            }
        }

    }
}
