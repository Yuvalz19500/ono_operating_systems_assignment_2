using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class FirstComeFirstServedPolicy : SchedulingPolicy
    {
        private readonly Queue<int> _proccessQueue = new();

        public override int NextProcess(Dictionary<int, ProcessTableEntry> dProcessTable)
        {
            if (_proccessQueue.Count == 0)
                return -1;

            return _proccessQueue.Dequeue();
        }

        public override void AddProcess(int iProcessId)
        {
            _proccessQueue.Enqueue(iProcessId);
        }

        public override bool RescheduleAfterInterrupt()
        {
            return false;
        }
    }
}
