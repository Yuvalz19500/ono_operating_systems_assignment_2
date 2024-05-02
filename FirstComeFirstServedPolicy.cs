using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class FirstComeFirstServedPolicy : SchedulingPolicy
    {
        protected readonly Queue<int> _proccessQueue = new();

        public override int NextProcess(Dictionary<int, ProcessTableEntry> dProcessTable)
        {
            for (int i = 0; i < _proccessQueue.Count; i++)
            {
                int processId = _proccessQueue.Dequeue();
                _proccessQueue.Enqueue(processId);
                ProcessTableEntry e = dProcessTable[processId];
                if (!e.Done && !e.Blocked)
                {
                    return processId;
                }
            }

            return -1;
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
