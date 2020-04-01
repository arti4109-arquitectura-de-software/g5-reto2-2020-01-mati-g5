using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace parseCsvs
{
    class TableResume
    {

    }
    class pair
    {
        public string Time { get; set; }
        public int CountOk { get; set; }
        public int CountError { get; set; }
        public double Latency { get; set; }
    }
    class Program
    {
        static string csv1 = "D:/results/results_test_100_per_second.csv";
        static string csv2 = "D:/results/results_test_200_per_second.csv";
        static string csv3 = "D:/results/results_test_300_per_second.csv";

        static void Main(string[] args)
        {
            using var reader = new StreamReader(csv3);
            var buffer = new StringBuilder();

            var a = new StringBuilder();
            var b = new StringBuilder();

            var listA = new List<KeyValuePair<string, string>>();
            var listB = new List<KeyValuePair<string, string>>();
            var listC = new List<KeyValuePair<string, string>>();

            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');


                if (values[4].Contains("OK"))
                {
                    listA.Add(new KeyValuePair<string, string>(values[0], values[4]));
                }
                else
                {
                    listB.Add(new KeyValuePair<string, string>(values[0], values[4]));
                }

                listC.Add(new KeyValuePair<string, string>(values[0], values[14]));
            }

            var a1 = listA.GroupBy(g => g.Key).Select(s => new pair()
            {
                Time = s.Key,
                CountOk = s.Count()
            }).ToList();

            var b1 = listB.GroupBy(g => g.Key).Select(s => new pair()
            {
                Time = s.Key,
                CountError = s.Count()
            }).ToList();

            var c1 = listC.GroupBy(g => g.Key).Select(s => new pair()
            {
                Time = s.Key,
                Latency = s.Select(t => Convert.ToInt32(t.Value)).Average()
            }).ToList();

            a1.AddRange(b1);
            a1.AddRange(c1);

            var f = a1.GroupBy(g => g.Time).Select(s => new pair()
            {
                Time = s.Key,
                CountOk = s.Sum(t => t.CountOk),
                CountError = s.Sum(t => t.CountError),
                Latency = s.Max(m => m.Latency)
            }).OrderBy(o => o.Time)
              .Select(s => s.Time + "\t" + s.CountOk + "\t" + s.CountError + "\t" + Convert.ToInt32(s.Latency))
              .ToList();

            var fa1 = string.Join("\n", f).Replace("2020/03/28", "2020/03/18");

        }
    }
}
