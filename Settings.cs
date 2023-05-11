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

        public int Setting1 { get; set; }
        public string Setting2 { get; set; }

        private Settings() { }
    }

}
