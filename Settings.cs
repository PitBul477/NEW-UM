using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEW_UM
{
    public class Settings
    {
        private static Settings instance;
        public static Settings Instance => instance ?? (instance = new Settings());

        public int Interval1r { get; set; }
        public int Interval2r { get; set; }
        //public int CountFinal { get; set; }
        public string IPadd { get; set; }
        public int PortsAdd { get; set; }

        private Settings() { }
    }

}
