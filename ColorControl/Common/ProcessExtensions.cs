using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ColorControl.Common
{
    public static class ProcessExtensions
    {
        private static string FindIndexedProcessName(int pid, string processName, IEnumerable<Process> processes = null)
        {
            var processesByName = processes == null ? Process.GetProcessesByName(processName) : processes.Where(p => p.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase)).ToArray();
            string processIndexedName = null;

            for (var index = 0; index < processesByName.Length; index++)
            {
                processIndexedName = index == 0 ? processName : processName + "#" + index;
                var processId = new PerformanceCounter("Process", "ID Process", processIndexedName);
                if ((int)processId.NextValue() == pid)
                {
                    return processIndexedName;
                }
            }

            return processIndexedName;
        }

        private static Process FindPidFromIndexedProcessName(string indexedProcessName, IEnumerable<Process> processes = null)
        {
            var parentId = new PerformanceCounter("Process", "Creating Process ID", indexedProcessName);
            var parentProcessId = (int)parentId.NextValue();

            try
            {
                return processes == null ? Process.GetProcessById(parentProcessId) : processes.FirstOrDefault(p => p.Id == parentProcessId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Process Parent(this Process process, IEnumerable<Process> processes = null)
        {
            var indexedProcessName = FindIndexedProcessName(process.Id, process.ProcessName, processes);
            if (indexedProcessName == null)
            {
                return null;
            }

            return FindPidFromIndexedProcessName(indexedProcessName, processes);
        }
    }
}
