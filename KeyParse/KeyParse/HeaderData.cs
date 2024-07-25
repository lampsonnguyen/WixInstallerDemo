using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyParse
{
    public class HeaderData
    {
        public byte[] HeaderFlag { get; set; }
        public int HeaderVersion { get; set; }

        public byte[] Reserved { get; set; }

        public int NumberOfKeys { get; set; }
    }
}
