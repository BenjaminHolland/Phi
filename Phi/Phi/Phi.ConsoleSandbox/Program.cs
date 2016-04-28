using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phi.Menus;
using Phi;
using Phi.IO;
using System.Diagnostics;
using System.IO;

namespace Phi.ConsoleSandbox
{

    public class DiskStats
    {
        class TransferRecord
        {
            public long TransferredBytes;
            public long ElapsedTicks;
            public long Frequency;
        }
        IPicker ConsolePicker = new ReaderWriterPicker(Console.In, Console.Out, false, false, "Drives", ">");
        NativeDrive CurrentDrive;
        string CurrentTag;
        public DiskStats()
        {

        }
        public void PickDrive()
        {
            var driveMenu = new HashMenu<NativeDriveRecord>();
            driveMenu.Add(new Item<NativeDriveRecord>("null", "None", 100, null));
            foreach (var record in NativeDrive.Drives.Select((v, i) => new { Record = v, Index = i }))
            {
                driveMenu.Add(new Item<NativeDriveRecord>(record.Index.ToString(), $"{record.Record.DrivePath} ({string.Join(",", record.Record.Volumes)})",record.Index,record.Record));
            }
            while (true)
            {
                var result = ConsolePicker.Pick(driveMenu);
                if (!result.Any())
                {
                    Console.WriteLine("Invalid Key");
                }
                else
                {
                    if (CurrentDrive != null) CurrentDrive.Dispose();
                    if (result.First() == null)
                    {
                        CurrentDrive = null;
                        break;
                    }
                    else
                    {
                        CurrentDrive = new NativeDrive(result.First().DrivePath);
                        break;
                    }
                }
            }
        }
        public void GenerateGigStats()
        {
            List<TransferRecord> records = new List<TransferRecord>();
            for (int i = 0; i < 3; i++)
            {
                int total = (int)Math.Pow(1024, 3);
                int remaining = total;
                int bufferSize = 67108864;
                int pos = 0;
                byte[] buffer = new byte[bufferSize];
                Stopwatch sw = Stopwatch.StartNew();
                while (remaining > 0)
                {
                    int count = Math.Min(remaining, bufferSize);

                    Console.WriteLine($"Reading {pos}-{pos + count}");
                    CurrentDrive.Read(pos, (uint)count, buffer);
                    pos += count;
                    remaining -= count;
                }
                sw.Stop();
                records.Add(new TransferRecord()
                {
                    TransferredBytes = total,
                    ElapsedTicks = sw.ElapsedTicks,
                    Frequency = Stopwatch.Frequency
                });
            }
            TransferRecord record = new TransferRecord()
            {
                TransferredBytes = (int)Math.Pow(1024, 3),
                Frequency = Stopwatch.Frequency,
                ElapsedTicks = (long)records.Average(x => (decimal)x.ElapsedTicks)
            };
            File.WriteAllLines($"Gig Read Stats-{CurrentTag}.csv",new string[] { "Buffer Size,Elapsed Seconds,Transfer Speed (MBPS)", $"{record.TransferredBytes},{record.ElapsedTicks / (double)record.Frequency},{record.TransferredBytes / 1000000.0 / (record.ElapsedTicks / (double)record.Frequency)}" });
        }
        public void GenerateBufferSizeStats()
        {
            List<TransferRecord> stats = new List<TransferRecord>();
            for (int bp = 12; bp <= 27; bp++)
            {

                int bufferSize = (int)Math.Pow(2, bp);
                byte[] buffer = new byte[bufferSize];
                for (int i = 0; i < 5; i++)
                {

                    Console.WriteLine($"Read Stats: Buffer Size {bufferSize}, round {i}");
                    TransferRecord currentRecord = new TransferRecord();
                    Stopwatch sw = Stopwatch.StartNew();
                    CurrentDrive.Read(0, (uint)bufferSize, buffer);
                    currentRecord.ElapsedTicks = sw.ElapsedTicks;
                    currentRecord.Frequency = Stopwatch.Frequency;
                    currentRecord.TransferredBytes = buffer.Length;
                    stats.Add(currentRecord);
                }
            }
            List<string> lines = new List<string>();
            lines.Add("Buffer Size,Elapsed Seconds,Transfer Speed (MBPS)");
            var groups = stats.GroupBy(record => record.TransferredBytes);
            foreach(var group in groups)
            {
                TransferRecord record = new TransferRecord()
                {
                    ElapsedTicks = (long)group.Average(x => x.ElapsedTicks),
                    TransferredBytes = group.Key,
                    Frequency = Stopwatch.Frequency
                };
                lines.Add($"{record.TransferredBytes},{record.ElapsedTicks / (double)record.Frequency},{record.TransferredBytes / 1000000.0 / (record.ElapsedTicks / (double)record.Frequency)}");
            }
           
            File.WriteAllLines($"Buffer Read Statistics - {CurrentTag}.csv",lines);
        }
        public void RunStats()
        {
            Console.WriteLine("File Tag>");
            CurrentTag = Console.ReadLine();
            GenerateBufferSizeStats();
            GenerateGigStats();
        }
        public void Run()
        {
            bool done = false;
            while (!done)
            {
                var menu = new HashMenu<Action>();
                menu.Add(new Item<Action>("b", "Back", 100, () => done = true));
                menu.Add(new Item<Action>("1", "Pick Disk", 0, PickDrive));
                if (CurrentDrive != null)
                {
                    menu.Add(new Item<Action>("2", "Run Stats", 1, RunStats));
                }
                var result = Enumerable.Empty<Action>();
                while (!result.Any())
                {
                    result = ConsolePicker.Pick(menu);
                    if (!result.Any())
                    {
                        Console.WriteLine("Invalid Key");
                    }
                }
                result.First()();
            }
        }
    }
    class Program
    {
       
        static void Main(string[] args)
        {
            bool done = false;
            DiskStats stats = new DiskStats();
            HashMenu<Action> menu = new HashMenu<Action>();
            menu.Add(new Item<Action>("x", "Exit", 100, () => done = true));
            menu.Add(new Item<Action>("1", "Native Drive", 1, stats.Run));
            IPicker picker = new ReaderWriterPicker(Console.In, Console.Out, false, false, "Actions", ">");
            while (!done)
            {
                var result = picker.Pick(menu).SingleOrDefault();
                if (result == null)
                {
                    Console.WriteLine("Invalid key.");
                }
                else
                {
                    result();
                }
            }

        }
    }
}
