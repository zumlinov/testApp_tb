using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tobbi_pc;
using tobbi_pc.Classes;

namespace tobbi_testApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Random r = new Random(DateTime.Now.Second);

            TaskProcessor<int,string> taskProcc = new TaskProcessor<int, string>(3000);

            try
            {
                taskProcc.Start();
                //taskProcc.Start();
                //taskProcc.Stop();
                //taskProcc.Start();
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Starting tasks processor error. {ex.Message}");
                throw;
            }

            List<Action> tds = new List<Action>();

            for (int i = 0; i < 100; i++)
            {
                int k = i;
                int r_val = r.Next(1, 9);

                taskProcc.AddTAsk(
                 intV =>
                 {
                     if (r_val == 6)
                     {
                         throw new Exception("Test exception inside a task");
                     }
                     else
                     {
                         Thread.Sleep(r_val * 50);
                         return intV.ToString();
                     }
                 },
                 k,
                 $"t_{k}");

                Thread.Sleep(r_val*10);
            }

            //Parallel.Invoke(tds.ToArray());

            Console.WriteLine("all tasks were added.");

            Thread.Sleep(10000);

            taskProcc.Stop();
            Console.WriteLine("Processor was stoped");

            Thread.Sleep(3000);
            Console.WriteLine("Processor was started again");

            taskProcc.Start();

            //Console.WriteLine("the end");
            Console.ReadLine();
        }
    }
}
