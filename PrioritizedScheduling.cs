using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class PrioritizedScheduling : RoundRobin
    {
        public PrioritizedScheduling(int iQuantum) : base(iQuantum)
        {
        }

        public override int NextProcess(Dictionary<int, ProcessTableEntry> dProcessTable)
        {
            int selectedProcess = -1;

            Queue<int> orderedQueue = new(_proccessQueue.OrderByDescending((procId) => dProcessTable[procId].Priority));

            for (int i = 0; i < orderedQueue.Count; i++)
            {
                int processId = orderedQueue.Dequeue();
                orderedQueue.Enqueue(processId);
                ProcessTableEntry e = dProcessTable[processId];
                if (!e.Done && !e.Blocked)
                {
                    selectedProcess = processId;
                    break;
                }
            }

            return selectedProcess;
        }

        public override void AddProcess(int iProcessId)
        {
            base.AddProcess(iProcessId);
        }

        public override bool RescheduleAfterInterrupt()
        {
            return true;
        }
    }
}
