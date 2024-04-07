using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace Scheduling
{
    class ProcessConsole
    {
        private StreamWriter m_swLog;
        private Thread m_tConsole;
        private OutputConsole m_ocOutput;
        private int m_iProcessId;
        private string m_sProcessName;
        public static int WindowsCount = 0;

        public ProcessConsole(int iProcessId, string sProcessName)
        {
            m_sProcessName = sProcessName;
            m_iProcessId = iProcessId;
            m_tConsole = new Thread(OpenConsole);
            m_tConsole.Start();
            while (m_ocOutput == null)
                Thread.Yield();
            Thread.Sleep(100);
            m_swLog = new StreamWriter("ProcessLog." + m_iProcessId + ".txt");
        }
        public void OpenConsole()
        {
            try
            {
                WindowsCount++;
                m_ocOutput = new OutputConsole();
                m_ocOutput.Text = m_sProcessName + "(pid " + m_iProcessId + ")";
                m_ocOutput.ShowDialog();
            }
            catch (Exception e)
            {
                m_ocOutput.Close();
                m_ocOutput = null;
            }
        }
        public void Close()
        {
            m_tConsole.Abort();
            m_tConsole = null;
            m_swLog.Close();
        }
        public void Write(string s)
        {
            m_ocOutput.Invoke(new MethodInvoker(m_ocOutput.BringToFront));
            //m_ocOutput.Write(s);
            m_ocOutput.Invoke(new OutputConsole.WriteDelegate(m_ocOutput.Write), new object[]{ s });
            Thread.Sleep(100);
            m_swLog.Write(s.Replace("\\n", "\n"));
        }       
    }
}
