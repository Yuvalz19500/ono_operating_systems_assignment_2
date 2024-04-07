using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class AddressSpace
    {
        private Dictionary<string, double> m_dVariables;
        public int ProcessId { get; private set; }
        public Code Code { get; set; }
        public AddressSpace(int iProcessId)
        {
            ProcessId = iProcessId;
            m_dVariables = new Dictionary<string, double>();
            m_dVariables["EOF"] = double.NaN;
        }
        public double this[string sVariable]
        {
            get
            {
                return m_dVariables[sVariable.Trim()];
            }
            set
            {
                m_dVariables[sVariable.Trim()] = value;
            }
        }
    }
}
