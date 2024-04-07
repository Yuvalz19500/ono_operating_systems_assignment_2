using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class FirstComeFirstServedPolicy : SchedulingPolicy
    {

        public override int NextProcess(Dictionary<int, ProcessTableEntry> dProcessTable)
        {
            throw new NotImplementedException();
        }

        public override void AddProcess(int iProcessId)
        {
            throw new NotImplementedException();
        }

        public override bool RescheduleAfterInterrupt()
        {
            throw new NotImplementedException();
        }
    }
}
