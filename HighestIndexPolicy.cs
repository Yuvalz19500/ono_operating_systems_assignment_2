using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class HighestIndexPolicy : SchedulingPolicy
    {
        public override int NextProcess(Dictionary<int, ProcessTableEntry> dProcessTable)
        {
            foreach (ProcessTableEntry e in dProcessTable.Values.OrderByDescending(x=>x.ProcessId))
            {
                if (!e.Done && !e.Blocked)
                {
                    return e.ProcessId;
                }
            }
            return -1;
        }

        public override void AddProcess(int iProcessId)
        {
        }

        public override bool RescheduleAfterInterrupt()
        {
            return true;
        }
    }
}
