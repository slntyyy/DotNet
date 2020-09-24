using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerService
{
    /// <summary>
    /// 轮询视图（Redis、SqlLite、其他等）采集服务
    /// </summary>
    public class PollingWorker : BackgroundService
    {
        private readonly ILogger<PollingWorker> _logger;
        public PollingWorker(ILogger<PollingWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Do();
                // 任务计时器
                await Task.Delay(1000 * 60, stoppingToken);
            }
        }
        public class PerformanceCounterListener : EventListener
        {
            private static HashSet<string> _keys = new HashSet<string> { "Count", "Min", "Max", "Mean", "Increment" };
            private static DateTimeOffset? _lastSampleTime;
            protected override void OnEventSourceCreated(EventSource eventSource)
            {
                base.OnEventSourceCreated(eventSource);
                if (eventSource.Name == "System.Runtime")
                {
                    EnableEvents(eventSource, EventLevel.Critical, (EventKeywords)(-1), new Dictionary<string, string> { ["EventCounterIntervalSec"] = "10" });
                }
            }
            protected override void OnEventWritten(EventWrittenEventArgs eventData)
            {
                if (_lastSampleTime != null && DateTimeOffset.UtcNow - _lastSampleTime.Value > TimeSpan.FromSeconds(1))
                {
                    Console.WriteLine();
                }
                _lastSampleTime = DateTimeOffset.UtcNow;
                var metrics = (IDictionary<string, object>)eventData.Payload[0];
                var name = metrics["Name"];
                var values = metrics
                    .Where(it => _keys.Contains(it.Key))
                    .Select(it => $"{it.Key} = {it.Value}");
                var timestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd hh:mm::ss");
                Console.WriteLine($"[{timestamp}]{name,-32}: {string.Join("; ", values.ToArray())}");
            }
        }
        private void Do()
        {
            //_ = new PerformanceCounterListener();
            //Console.Read();

            var sysinfo = new SystemInfo();
            Console.WriteLine("系统：" + sysinfo.GetOS());
            Console.WriteLine("版本：" + sysinfo.GetOSVersion());
            Console.WriteLine();
            var memory = sysinfo.GetMemory();
            Console.WriteLine("内存：" + memory.Total + "MB");
            Console.WriteLine("已用：" + memory.Used + "MB");
            Console.WriteLine("剩余：" + memory.Free + "MB");
            Console.WriteLine();

            Console.WriteLine(Directory.GetCurrentDirectory());

            foreach (var item in sysinfo.GetDrive())
            {

            }
        }
    }
}
