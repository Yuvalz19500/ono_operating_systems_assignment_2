using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class RoundRobin : FirstComeFirstServedPolicy
    {
        public int m_iQuantum { get; }

        public RoundRobin(int iQuantum) : base()
        {
            m_iQuantum = iQuantum;
        }

        public override int NextProcess(Dictionary<int, ProcessTableEntry> dProcessTable)
        {
            return base.NextProcess(dProcessTable);
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
