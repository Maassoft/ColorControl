using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static ColorControl.Shared.Native.WinApi;

namespace ColorControl.Shared.Common
{
    public static class ProcessExtensions
    {
        private static string FindIndexedProcessName(int pid, string processName, IEnumerable<Process> processes = null)
        {
            var processesByName = processes == null ? Process.GetProcessesByName(processName) : processes.Where(p => p.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase)).ToArray();
            string processIndexedName = null;

            try
            {
                for (var index = 0; index < processesByName.Length; index++)
                {
                    processIndexedName = index == 0 ? processName : processName + "#" + index;
                    var processId = new PerformanceCounter("Process", "ID Process", processIndexedName);
                    if ((int)processId.NextValue() == pid)
                    {
                        return processIndexedName;
                    }
                }
            }
            catch (Exception) { }

            return processIndexedName;
        }

        private static Process FindPidFromIndexedProcessName(string indexedProcessName, IEnumerable<Process> processes = null)
        {
            try
            {
                var parentId = new PerformanceCounter("Process", "Creating Process ID", indexedProcessName);
                var parentProcessId = (int)parentId.NextValue();

                return processes == null ? Process.GetProcessById(parentProcessId) : processes.FirstOrDefault(p => p.Id == parentProcessId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Process Parent(this Process process, IEnumerable<Process> processes = null)
        {
            if (Utils.IsAdministrator())
            {
                try
                {
                    return GetParentProcess(process.Handle);
                }
                catch
                {
                    return null;
                }
            }

            var indexedProcessName = FindIndexedProcessName(process.Id, process.ProcessName, processes);
            if (indexedProcessName == null)
            {
                return null;
            }

            return FindPidFromIndexedProcessName(indexedProcessName, processes);
        }

        public static Process GetParentProcess(nint handle)
        {
            var pbi = new ParentProcessUtilities();
            int returnLength;
            int status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
            if (status != 0)
                throw new Win32Exception(status);

            try
            {
                return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
            }
            catch (ArgumentException)
            {
                // not found
                return null;
            }
        }

    }
}
