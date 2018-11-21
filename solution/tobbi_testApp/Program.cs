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
        //it will be use to process tasks 
        static TaskProcessor<int> taskProcc;

        //it will be use to display message from first processor events
        static TaskProcessor<string> consoleLogger;

        //list with data to proces - source list
        static List<TaskData<int>> tasksDataList;

        //store results of processing task data - result list
        static List<TaskData<int>> listOfProccessedTaskData;

        static void Main(string[] args)
        {
            listOfProccessedTaskData = new List<TaskData<int>>();

            //to set time of work simulation
            Random r = new Random(DateTime.Now.Second);

            //main processor
            taskProcc = new TaskProcessor<int>();

            taskProcc.TaskComplited += TaskProcc_TaskComplited;
            taskProcc.TaskStarting += TaskProcc_TaskStarting;

            //log messages processor
            consoleLogger = new TaskProcessor<string>(500);

            #region test processor start/stop methods in chaotic way

            taskProcc.Start();
            taskProcc.Start();
            taskProcc.Start();
            taskProcc.Stop();
            taskProcc.Stop();
            taskProcc.Start();
            taskProcc.Start();
            taskProcc.Start();
            taskProcc.Stop();
            taskProcc.Stop();
            taskProcc.Start();
            taskProcc.Start();
            taskProcc.Start();
            taskProcc.Stop();
            taskProcc.Stop();
            taskProcc.Start();
            taskProcc.Start();
            taskProcc.Start();


            taskProcc.StopAndClear();
            taskProcc.StopAndClear();
            taskProcc.StopAndClear();

            taskProcc.Stop();
            taskProcc.Stop();
            taskProcc.Start();
            taskProcc.Start();

            taskProcc.StopAndClear();

            taskProcc.Start();
            taskProcc.Stop();
            taskProcc.Stop();
            taskProcc.Start();
            taskProcc.Start();
            taskProcc.Start();
            taskProcc.Stop();
            taskProcc.Stop();
            taskProcc.Start();
            taskProcc.Start();
            taskProcc.Start();
            taskProcc.Stop();
            taskProcc.Stop();
            taskProcc.Start();

            #endregion

            consoleLogger.Start();

            #region Add tasks to processor

            tasksDataList = new List<TaskData<int>>();

            for (int i = 0; i < 100; i++)
            {
                int k = i;
                int r_val = r.Next(1, 9);

                tasksDataList.Add(
                    new TaskData<int>(
                         //task delegate
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
                                 //intV.ToString();
                             }
                         },
                         //incomeData data for task
                         k,
                         //task name
                         $"t_{k}"));
            }



            //logMsg("all tasks were added.", ConsoleColor.Magenta);
            Console.WriteLine("all tasks were added.");

            #endregion
            

            #region Test start/stop processor with no empty queue
            
            Thread.Sleep(10000);

            taskProcc.Stop();

            logMsg("Processor was stoped.", ConsoleColor.Magenta);
            
            Thread.Sleep(3000);

            logMsg("Processor was started again.", ConsoleColor.Magenta);
            
            taskProcc.Start();

            #endregion
            
            Console.ReadLine();
        }

        #region Additional methods

        static void TaskProcc_TaskStarting(object sender, TaskProcessingStateEventArgs<int> e)
        {            
            if (e.TaskData.Ex == null)

                consoleLogger.AddTask(
                    msg => {
                        logMsg($" ({e.TaskData.IdWorkTask}) Task: {e.TaskData.Name} was starting ", ConsoleColor.Blue, false);
                    },
                    null,
                    string.Empty);
        }

        static void TaskProcc_TaskComplited(object sender, TaskProcessingStateEventArgs<int> e)
        {
            string msgToShow = string.Empty;
            ConsoleColor consCol = Console.ForegroundColor;

            listOfProccessedTaskData.Add(e.TaskData);

            if (e.TaskData.Ex == null)
            {
                msgToShow = $" ({e.TaskData.IdWorkTask}) Task {e.TaskData.Name} complited succsesfull";
                consCol = ConsoleColor.Green;
            }
            else
            {
                msgToShow = $" ({e.TaskData.IdWorkTask}) Current task ({e.TaskData.Name}) processing error. {e.TaskData.Ex.Message}";
                consCol = ConsoleColor.Red;
            }

            consoleLogger.AddTask(
                msg =>
                {
                    logMsg(msgToShow, consCol);
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

        #endregion
    }
}
