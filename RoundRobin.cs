using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class RoundRobin : FirstComeFirstServedPolicy
    {
        protected int m_iQuantum;

        public RoundRobin(int iQuantum) : base()
        {
            m_iQuantum = iQuantum;
        }

        public override int NextProcess(Dictionary<int, ProcessTableEntry> dProcessTable)
        {
            throw new NotImplementedException();
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
