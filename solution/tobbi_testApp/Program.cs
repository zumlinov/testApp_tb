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
        static TaskProcessor<int, string> taskProcc;
        static TaskProcessor<string, bool> consoleLogger;

        static void Main(string[] args)
        {
            Random r = new Random(DateTime.Now.Second);

            taskProcc = new TaskProcessor<int, string>(3000);

            taskProcc.TaskComplited += TaskProcc_TaskComplited;
            taskProcc.TaskStarting += TaskProcc_TaskStarting;

            consoleLogger = new TaskProcessor<string, bool>(3000);


                taskProcc.Start();
                consoleLogger.Start();

            #region Add tasks to processor

            for (int i = 0; i < 100; i++)
            {
                int k = i;
                int r_val = r.Next(1, 9);

                taskProcc.AddTAsk(
                     //task
                     intV =>
                     {
                         //simulate exception inside task
                         if (r_val == 6)
                         {
                             throw new Exception("Test exception inside a task");
                         }
                         //right way to perform
                         else
                         {
                             Thread.Sleep(r_val * 50);
                             return intV.ToString();
                         }
                     },
                     //incomeData data for task
                     k,
                     //task name
                     $"t_{k}");

                Thread.Sleep(r_val*10);
            }
            
            #endregion

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

        private static void TaskProcc_TaskStarting(object sender, TaskProcessingStateEventArgs<int, string> e)
        {            
            if (e.TaskData.Ex == null)

                consoleLogger.AddTAsk(
                    msg => {
                        logMsg($"Task: {e.TaskData.Name} was starting", ConsoleColor.Blue, false);
                        return true;
                    },
                    null,
                    string.Empty);
        }

        private static void TaskProcc_TaskComplited(object sender, TaskProcessingStateEventArgs<int, string> e)
        {
            string msgToShow = string.Empty;
            ConsoleColor consCol = Console.ForegroundColor;

            if (e.TaskData.Ex == null)
            {
                msgToShow = $"Task {e.TaskData.Name} complited succsesfull";
                consCol = ConsoleColor.Green;
            }
            else
            {
                msgToShow = $"Current task ({e.TaskData.Name}) processing error. {e.TaskData.Ex.Message}";
                consCol = ConsoleColor.Red;
            }

            consoleLogger.AddTAsk(
                msg =>
                {
                    logMsg(msgToShow, consCol);
                    return true;
                },
                null,
                string.Empty);
        }

        static void logMsg(string msg, ConsoleColor color, bool changeLineAfterText = true)
        {
            Console.ForegroundColor = color;

            if (changeLineAfterText)
            {
                Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : {msg}");
            }
            else
            {
                Console.Write($"{DateTime.Now.ToLongTimeString()} : {msg} ");
            }

            Console.ResetColor();
        }

    }
}
