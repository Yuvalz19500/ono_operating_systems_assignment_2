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
                outgoingProcess = m_dProcessTable[CPU.ActiveProcess];
            }

            ProcessTableEntry enteringProcess = m_dProcessTable[iEnteringProcessId];
            CPU.ActiveProcess = iEnteringProcessId;
            CPU.ActiveAddressSpace = enteringProcess.AddressSpace;
            CPU.ActiveConsole = enteringProcess.Console;
            CPU.ProgramCounter = enteringProcess.ProgramCounter;

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
                bool bOnlyIdleRemains = iNextProcessId == IDLE_PROCESS_ID;

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
            //Compute the average time from the moment that a process enters the system until it terminates.
            throw new NotImplementedException();
        }
        public int MaximalStarvation()
        {
            //Compute the maximal time that some project has waited in a ready stage without receiving CPU time.
            throw new NotImplementedException();
        }
    }
}
