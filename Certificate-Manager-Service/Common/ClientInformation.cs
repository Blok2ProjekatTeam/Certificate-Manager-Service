using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ClientInformation
    {
        private int textId = 0;
        private int logId = 0;
        public string TimeStamp { get; set; }
        public string OU { get; set; }
        public string CN { get; set; }
        public int TextId { get => textId; set => textId = value; }
        public int LogId { get => logId; set => logId = value; }
    }
}
