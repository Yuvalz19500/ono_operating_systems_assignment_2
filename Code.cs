using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Scheduling
{
    class Code
    {
        protected List<string> m_lLines;
        public int LineCount { get { return m_lLines.Count; } }
        protected Dictionary<string, int> m_dLables;
        public string this[int iLine]
        {
            get
            {
                return GetLine(iLine);
            }
        }

        public Code()
        {
            m_lLines = new List<string>();
            m_dLables = new Dictionary<string, int>();
        }

        public Code(string sCodeFile) : this()
        {
            StreamReader sr = new StreamReader(sCodeFile);
            string sLine = "";
            while (!sr.EndOfStream)
            {
                sLine = sr.ReadLine().Trim();
                if (!sLine.Contains(' ') && sLine.EndsWith(":"))
                {
                    sLine = sLine.Replace(":", "");
                    m_dLables.Add(sLine, m_lLines.Count);
                }
                else
                    m_lLines.Add(sLine);
            }
            sr.Close();
        }

        private string ReplaceLabels(string sLine)
        {
            string[] asTokens = sLine.Split(' ');
            string s = "";
            for (int i = 0; i < asTokens.Length; i++)
            {
                if (m_dLables.ContainsKey(asTokens[i]))
                    s += m_dLables[asTokens[i]];
                else
                    s += asTokens[i];
                s += " ";
            }
            return s.TrimEnd();
        }

        private string GetLine(int iLine)
        {
            string sLine = m_lLines[iLine];
            if (sLine.StartsWith("goto"))
            {
                foreach (KeyValuePair<string, int> p in m_dLables)
                    sLine = ReplaceLabels(sLine);
            }
            return sLine;
        }
    }
}
