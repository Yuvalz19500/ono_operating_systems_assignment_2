using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Scheduling
{
    class Disk
    {
        public ReadTokenRequest ActiveRequest
        {
            get
            {
                return m_rActiveRequest;
            }
            set
            {

                if (m_rActiveRequest != null)
                    throw new InvalidOperationException("Disk cannot handle multiple requests at the same time.");
                m_rActiveRequest = value;
                m_srActiveFile = new StreamReader(value.FileName);
                m_cOverallTokenPointer = 0;
                m_bEndOfStream = false;
                ReadLine();                    
            }
        }
        public OperatingSystem OperatingSystem { get; set; }

        private ReadTokenRequest m_rActiveRequest;
        private StreamReader m_srActiveFile;
        private int m_iCurrentLinePointer, m_cOverallTokenPointer;
        private bool m_bEndOfStream;
        private string[] m_asTokens;

        public void ProcessRequest()
        {
            if (ActiveRequest != null)
            {
                if (m_bEndOfStream)
                {
                    ActiveRequest.EndOfStreamReached = true;
                    EndRequestHandling();
                }
                else if (m_cOverallTokenPointer == ActiveRequest.TokenNumber)
                {
                    ActiveRequest.Token = m_asTokens[m_iCurrentLinePointer];
                    EndRequestHandling();
                }
                else
                {
                    m_iCurrentLinePointer++;
                    m_cOverallTokenPointer++;
                    while (!m_bEndOfStream && m_iCurrentLinePointer == m_asTokens.Length)
                        ReadLine();
                }
            }
        }

        private void EndRequestHandling()
        {
            m_srActiveFile.Close();
            m_srActiveFile = null;
            ReadTokenRequest r = m_rActiveRequest;
            m_rActiveRequest = null;
            OperatingSystem.Interrupt(r);
        }

        private void ReadLine()
        {
            if (m_srActiveFile.EndOfStream)
            {
                m_asTokens = null;
                m_bEndOfStream = true;
            }
            else
            {
                string sLine = m_srActiveFile.ReadLine();
                m_asTokens = sLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                m_iCurrentLinePointer = 0;
            }
        }
    }
}
