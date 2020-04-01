using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;
using System.Timers;

namespace noiseInductor
{
    class Program
    {
        static Timer timer;
        static Random rnd;
        static int[] sampleScale = new int[] { 2, 5, 7 };
        static int sampleIndex = 0;
        static void Main(string[] args)
        {
            rnd = new Random();
            timer = new Timer();
            timer.Interval = 60 * 1000;
            timer.Elapsed += new ElapsedEventHandler(TimerEventProcessor);
            timer.Start();
            Console.ReadLine();
        }

        static List<int> RandomSample(int sample)
        {
            rnd = new Random();
            var response = new HashSet<int>();

            while (response.Count < sample)
            {
                response.Add(rnd.Next(0, 10));
            }

            return response.ToList();
        }

        public static void TimerEventProcessor(object sender, ElapsedEventArgs e)
        {
            var pods = GetPodsNames("soilder").ToArray();

            if (sampleIndex > 2) return;

            var scale = sampleScale[sampleIndex];

            var sample = RandomSample(scale);

            System.Threading.Interlocked.Increment(ref sampleIndex);

            sample.AsParallel().ForAll(i =>
            {
                DeletePod(pods[i]);
            });

            Console.WriteLine("Destroy Event with shutdown " + scale + " instances at");
            Console.WriteLine(DateTime.Now.ToString());
            Console.WriteLine();
        }

        private static void DeletePod(string v)
        {
            Runspace runspace = RunspaceFactory.CreateRunspace();

            // open it

            runspace.Open();

            // create a pipeline and feed it the script text

            Pipeline pipeline = runspace.CreatePipeline();

            pipeline.Commands.AddScript("kubectl delete pod " + v);
            pipeline.Commands.Add("Out-String");

            Collection<PSObject> results = pipeline.Invoke();

            runspace.Close();
        }

        static List<String> GetPodsNames(string prefix)
        {

            var response = new List<String>();

            Runspace runspace = RunspaceFactory.CreateRunspace();

            // open it

            runspace.Open();

            // create a pipeline and feed it the script text

            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript("kubectl get pods");

            pipeline.Commands.Add("Out-String");

            Collection<PSObject> results = pipeline.Invoke();

            // close the runspace

            runspace.Close();

            // convert the script result into a single string

            StringBuilder stringBuilder = new StringBuilder();
            foreach (PSObject obj in results)
            {
                response.AddRange(
                    obj.ToString()
                    .Split('\n')
                    .Where(w => w.StartsWith(prefix))
                    .Select(s => s.Split(' ')[0])
                    );
            }

            return response;
        }
    }
}
