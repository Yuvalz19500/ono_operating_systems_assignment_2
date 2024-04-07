using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    abstract class SchedulingPolicy
    {
        //returns the pid of the next process to be executed on the CPU
        public abstract int NextProcess(Dictionary<int, ProcessTableEntry> dProcessTable);
        //notifies the scheduler that a process has been added
        public abstract void AddProcess(int iProcessId);
        //returns true if scheduling is needed after an interrupt
        public abstract bool RescheduleAfterInterrupt();
    }
}
