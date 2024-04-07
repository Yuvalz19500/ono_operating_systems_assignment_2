using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class ProcessTableEntry
    {
        public bool Blocked { get; set; }
        public bool Done { get; set; }
        public string Name { get; private set; }
        public AddressSpace AddressSpace { get; set; }
        public int ProcessId { get; private set; }
        public int ProgramCounter { get; set; }
        public ProcessConsole Console { get; set; }
        public int Quantum { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int LastCPUTime { get; set; }
        public int MaxStarvation { get; set; }
        public int Priority { get; set; }

        public ProcessTableEntry(int iProcessId, string sName, Code code)
        {
            ProcessId = iProcessId;
            AddressSpace = new AddressSpace(ProcessId);
            AddressSpace.Code = code;
            Console = new ProcessConsole(iProcessId, sName);
            Name = sName;
            LastCPUTime = 0;
            StartTime = 0;
            EndTime = -1;
            MaxStarvation = 0;
        }
    }
}
