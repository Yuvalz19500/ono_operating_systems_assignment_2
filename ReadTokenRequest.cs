using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class ReadTokenRequest
    {
        public string FileName { get; set; }
        public int TokenNumber { get; set; }
        public string Token { get; set; }
        public int ProcessId { get; set; }
        public string TargetVariable { get; set; }
        public bool EndOfStreamReached { get; set; }
    }
}
