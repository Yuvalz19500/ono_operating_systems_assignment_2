using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class OperatingSystem
    {
        public Disk Disk { get; private set; }
        public CPU CPU { get; private set; }
        private Dictionary<int, ProcessTableEntry> m_dProcessTable;
        private List<ReadTokenRequest> m_lReadRequests;
        private int m_cProcesses;
        private SchedulingPolicy m_spPolicy;
        private static int IDLE_PROCESS_ID = 0;

        public OperatingSystem(CPU cpu, Disk disk, SchedulingPolicy sp)
        {
            CPU = cpu;
            Disk = disk;
            m_dProcessTable = new Dictionary<int, ProcessTableEntry>();
            m_lReadRequests = new List<ReadTokenRequest>();
            cpu.OperatingSystem = this;
            disk.OperatingSystem = this;
            m_spPolicy = sp;

            Code idleCode = new IdleCode();
            m_dProcessTable[IDLE_PROCESS_ID] = new ProcessTableEntry(IDLE_PROCESS_ID, "IdleProcess", idleCode);
            m_dProcessTable[IDLE_PROCESS_ID].StartTime = CPU.TickCount;
            m_dProcessTable[IDLE_PROCESS_ID].Priority = int.MinValue;
            m_spPolicy.AddProcess(IDLE_PROCESS_ID);
            m_cProcesses++;
        }


        public void CreateProcess(string sCodeFileName)
        {
            Code code = new Code(sCodeFileName);
            m_dProcessTable[m_cProcesses] = new ProcessTableEntry(m_cProcesses, sCodeFileName, code);
            m_dProcessTable[m_cProcesses].StartTime = CPU.TickCount;
            m_spPolicy.AddProcess(m_cProcesses);
            m_cProcesses++;
        }

        public void CreateProcess(string sCodeFileName, int iPriority)
        {
            Code code = new Code(sCodeFileName);
            m_dProcessTable[m_cProcesses] = new ProcessTableEntry(m_cProcesses, sCodeFileName, code);
            m_dProcessTable[m_cProcesses].Priority = iPriority;
            m_dProcessTable[m_cProcesses].StartTime = CPU.TickCount;
            m_spPolicy.AddProcess(m_cProcesses);
            m_cProcesses++;
        }

        public void ProcessTerminated(Exception e)
        {
            if (e != null)
                Console.WriteLine("Process " + CPU.ActiveProcess + " terminated unexpectedly. " + e);
            m_dProcessTable[CPU.ActiveProcess].Done = true;
            m_dProcessTable[CPU.ActiveProcess].Console.Close();
            m_dProcessTable[CPU.ActiveProcess].EndTime = CPU.TickCount;
            ActivateScheduler();
        }

        public void TimeoutReached()
        {
            ActivateScheduler();
        }

        public void ReadToken(string sFileName, int iTokenNumber, int iProcessId, string sParameterName)
        {
            ReadTokenRequest request = new ReadTokenRequest();
            request.ProcessId = iProcessId;
            request.TokenNumber = iTokenNumber;
            request.TargetVariable = sParameterName;
            request.Token = null;
            request.FileName = sFileName;
            m_dProcessTable[iProcessId].Blocked = true;
            if (Disk.ActiveRequest == null)
                Disk.ActiveRequest = request;
            else
                m_lReadRequests.Add(request);
            CPU.ProgramCounter = CPU.ProgramCounter + 1;
            ActivateScheduler();
        }

        public void Interrupt(ReadTokenRequest rFinishedRequest)
        {
            // a. Translate the token to double
            double tokenParsedValue = Double.NaN;

            if (rFinishedRequest.Token != null) 
            {
                if (!Double.TryParse(rFinishedRequest.Token, out tokenParsedValue))
                {
                    Console.WriteLine("Warning: Read token '" + rFinishedRequest.Token + "' could not be parsed as a double.");
                    return;
                }
            }

            // b. Save the value in the appropriate variable
            if (m_dProcessTable.TryGetValue(rFinishedRequest.ProcessId, out var entry))
            {
                entry.AddressSpace[rFinishedRequest.TargetVariable] = tokenParsedValue;
                entry.Blocked = false;
                entry.LastCPUTime = CPU.TickCount;
                m_dProcessTable[rFinishedRequest.ProcessId] = entry;
            }
            else
            {
                Console.WriteLine("Error: Process ID " + rFinishedRequest.ProcessId + " not found in process table.");
                return;
            }

            // c. Activate the next read request if any
            if (m_lReadRequests.Count > 0)
            {
                ReadTokenRequest nextRequest = m_lReadRequests[0];
                m_lReadRequests.RemoveAt(0);
                Disk.ActiveRequest = nextRequest;
            }

            // d. Call the scheduler if required
            if (m_spPolicy.RescheduleAfterInterrupt())
                ActivateScheduler();
        }

        private ProcessTableEntry? ContextSwitch(int iEnteringProcessId)
        {
            ProcessTableEntry? outgoingProcess = null;

            if (CPU.ActiveProcess != -1) {
                m_dProcessTable[CPU.ActiveProcess].ProgramCounter = CPU.ProgramCounter;
                m_dProcessTable[CPU.ActiveProcess].AddressSpace = CPU.ActiveAddressSpace;
                m_dProcessTable[CPU.ActiveProcess].Console = CPU.ActiveConsole;
                m_dProcessTable[CPU.ActiveProcess].LastCPUTime = CPU.TickCount;
                outgoingProcess = m_dProcessTable[CPU.ActiveProcess];
            }

            ProcessTableEntry enteringProcess = m_dProcessTable[iEnteringProcessId];
            int maxTime = enteringProcess.MaxStarvation;
            if (maxTime < CPU.TickCount - enteringProcess.LastCPUTime) 
            {
                m_dProcessTable[iEnteringProcessId].MaxStarvation = CPU.TickCount - enteringProcess.LastCPUTime;
            }

            CPU.ActiveProcess = iEnteringProcessId;
            CPU.ActiveAddressSpace = enteringProcess.AddressSpace;
            CPU.ActiveConsole = enteringProcess.Console;
            CPU.ProgramCounter = enteringProcess.ProgramCounter;

            if (m_spPolicy is RoundRobin roundRobinPolicy) 
            {
                CPU.RemainingTime = roundRobinPolicy.m_iQuantum;
            }

            return outgoingProcess;
        }

        public void ActivateScheduler()
        {
            int iNextProcessId = m_spPolicy.NextProcess(m_dProcessTable);
            if (iNextProcessId == -1)
            {
                Console.WriteLine("All processes terminated or blocked.");
                CPU.Done = true;
            }
            else
            {
                bool bOnlyIdleRemains = iNextProcessId == IDLE_PROCESS_ID && m_dProcessTable.Values.All(entry => entry.Done || entry.ProcessId == IDLE_PROCESS_ID);

                if(bOnlyIdleRemains)
                {
                    Console.WriteLine("Only idle remains.");
                    CPU.Done = true;
                }
                else
                    ContextSwitch(iNextProcessId);
            }
        }

        public double AverageTurnaround()
        {
            double sum = 0;
            foreach (ProcessTableEntry e in m_dProcessTable.Values) 
            {
                sum += e.EndTime - e.StartTime;
            }

            return sum / m_dProcessTable.Count;
        }
        public int MaximalStarvation()
        {
            int max = int.MinValue;

            foreach (ProcessTableEntry e in m_dProcessTable.Values) 
            {
                if (e.MaxStarvation > max) 
                {
                    max = e.MaxStarvation;
                }
            }

            return max;
        }
    }
}
