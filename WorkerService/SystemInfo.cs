using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace WorkerService
{
    public class MemoryMetrics
    {
        public double Total;
        public double Used;
        public double Free;
    }
    public class DriveMetrics
    {
        public string Name;
        public double TotalFreeSpace;
        public double TotalSize;
    }
    public class SystemInfo
    {

        public OSPlatform GetOS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }
            return OSPlatform.Windows;
        }
        public OperatingSystem GetOSVersion()
        {
            return Environment.OSVersion;
        }
        public bool Is64Bit()
        {
            return Environment.Is64BitOperatingSystem;
        }
        public DriveMetrics[] GetDrive()
        {
            var driveInfo = DriveInfo.GetDrives();
            return driveInfo.Select(s => new DriveMetrics
            {
                Name = s.Name,
                TotalFreeSpace = s.TotalFreeSpace,
                TotalSize = s.TotalSize,
            }).ToArray();
        }
        /// <summary>
        /// 获取总内存(单位:M)
        /// </summary>
        /// <returns></returns>
        public MemoryMetrics GetMemory()
        {
            try
            {
                if (GetOS() == OSPlatform.Windows)
                    return GetWindowsMetrics();
                return GetUnixMetrics();
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 获取Windows的内存指标
        /// </summary>
        /// <returns></returns>
        public MemoryMetrics GetWindowsMetrics()
        {
            var output = "";

            var info = new ProcessStartInfo
            {
                FileName = "wmic",
                Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value",
                RedirectStandardOutput = true
            };

            using (var process = Process.Start(info))
            {
                output = process.StandardOutput.ReadToEnd();
            }

            var lines = output.Trim().Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var freeMemoryParts = lines[0].Split(new string[1] { "=" }, StringSplitOptions.RemoveEmptyEntries);
            var totalMemoryParts = lines[1].Split(new string[1] { "=" }, StringSplitOptions.RemoveEmptyEntries);

            var metrics = new MemoryMetrics();
            metrics.Total = Math.Round(double.Parse(totalMemoryParts[1]) / 1024, 0);
            metrics.Free = Math.Round(double.Parse(freeMemoryParts[1]) / 1024, 0);
            metrics.Used = metrics.Total - metrics.Free;

            return metrics;
        }
        /// <summary>
        /// 获取Unix的内存指标
        /// </summary>
        /// <returns></returns>
        public MemoryMetrics GetUnixMetrics()
        {
            var output = "";

            var info = new ProcessStartInfo("free -m")
            {
                FileName = "/bin/bash",
                Arguments = "-c \"free -m\"",
                RedirectStandardOutput = true
            };

            using (var process = Process.Start(info))
            {
                output = process.StandardOutput.ReadToEnd();
                Console.WriteLine(output);
            }

            var lines = output.Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var memory = lines[1].Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);

            var metrics = new MemoryMetrics();
            metrics.Total = double.Parse(memory[1]);
            metrics.Used = double.Parse(memory[2]);
            metrics.Free = double.Parse(memory[3]);

            return metrics;
        }

        /// <summary>
        /// 获取某个进程的内存(单位:M)
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public long GetMemory(string processName)
        {
            Process[] p = Process.GetProcesses();
            Int64 totalMemory = 0;
            totalMemory = p.Where(x => x.ProcessName == processName).Sum(s => s.WorkingSet64);
            return totalMemory / 1024 / 1024;
        }
        public string GetCPU()
        {
            //Environment
            return null;
        }
        public Process GetProcess(string processName)
        {
            Process[] p = Process.GetProcesses();
            return p.Where(x => x.ProcessName == processName).FirstOrDefault();
        }
        public string GetNetWork()
        {
            var data = NetworkInterface.GetAllNetworkInterfaces()
                .SelectMany(sm => sm.GetIPProperties().UnicastAddresses)
                .Select(s => s.Address)
                .Where(x => x.IsIPv6LinkLocal)
                .ToList();
            return null;
        }
    }
}
