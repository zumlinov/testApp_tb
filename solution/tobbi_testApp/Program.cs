using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using tobbi_pc;
using tobbi_pc.Classes;

namespace tobbi_testApp
{
    class Program
    {
        static bool resultListWasChecked = false;
        static object list_locker = new object();

        //it will be use to process tasks 
        static TasksProcessor<string> taskProcc;

        //it will be use to display message from first processor events
        static TasksProcessor<string> consoleLogger;

        //list with data to proces - source list
        static List<TaskData<string>> listOfSourceTaskData;

        //store results of processing task data - result list
        static List<TaskData<string>> listOfProccessedTaskData;

        static void Main(string[] args)
        {
            //contain processed tasks
            listOfProccessedTaskData = new List<TaskData<string>>();

            //contain source tasks
            listOfSourceTaskData = new List<TaskData<string>>();

            //to set time of work simulation
            Random r = new Random(DateTime.Now.Second);

            //main processor 
            taskProcc = new TasksProcessor<string>();

            taskProcc.TaskComplited += TaskProcc_TaskComplited;
            taskProcc.TaskStarting += TaskProcc_TaskStarting;
            taskProcc.AllTasksProcessed += TaskProcc_AllTasksProcessed;

            //log messages processor
            consoleLogger = new TasksProcessor<string>(500);

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

            #region add client tasks from different threads with different dalays      

            Parallel.For(0, 50,
                it =>
                {
                    int r_val = r.Next(1, 9);

                    TaskData<string> currTask = new TaskData<string>(
                         //async task delegate
                         async incData =>
                         {
                             //simulate exception inside task
                             if (r_val == 6)
                             {
                                 throw new Exception("Test exception inside a task");
                             }
                             //right way to perform
                             else
                             {
                                 await Task.Run(() =>
                                 {
                                     //do some work ...
                                     

                                     Thread.Sleep(r_val * 20);
                                 });                                 
                             }
                         },
                         //incomeData data for task
                         it.ToString(),
                         //task name
                         $"t_{it}");

                    //add task to the processor queue
                    lock (list_locker)
                    {
                        listOfSourceTaskData.Add(currTask);
                        taskProcc.AddTask(currTask);
                    }

                    Thread.Sleep(r_val * 10);
                });

            //logMsg("all tasks were added.", ConsoleColor.Magenta);
            Console.WriteLine("all tasks were added.");

            #endregion


            #region Test start/stop processor with no empty queue

            Thread.Sleep(2000);

            taskProcc.Stop();

            logMsg("Processor was stoped.", ConsoleColor.Magenta);

            Thread.Sleep(3000);

            logMsg("Processor was started again.", ConsoleColor.Magenta);

            taskProcc.Start();

            #endregion

            Console.ReadLine();
        }
        
        #region Event handlers

        static void TaskProcc_AllTasksProcessed(object sender, EventArgs e)
        {
            if (resultListWasChecked)
            {
                return;
            }

            if (!(listOfProccessedTaskData?.Count > 0
                && listOfSourceTaskData?.Count > 0))
            {
                return;
            }

            bool somethingIsWrong = false;

            logMsg("\n\nSource and Result lists COMPARATION:\n\n", ConsoleColor.Red);

            if (listOfSourceTaskData?.Count != listOfProccessedTaskData?.Count)
            {
                logMsg("\nSource and Result lists have different count of items.", ConsoleColor.Red);

                somethingIsWrong = true;
            }

            if (!somethingIsWrong)
            {
                for (int i = 0; i < listOfSourceTaskData.Count; i++)
                {
                    var source_it = listOfSourceTaskData[i];
                    var processed_it = listOfProccessedTaskData[i];

                    if (source_it.Id != processed_it.Id)
                    {
                        logMsg($"Items num_{i} are not same. \n{source_it.Id} - {processed_it.Id}", ConsoleColor.Red);

                        somethingIsWrong = true;
                        break;
                    }

                    logMsg($"Source: {source_it.Name} - Result: {processed_it.Name}", ConsoleColor.DarkYellow);
                }
            }

            if (!somethingIsWrong)
            {
                logMsg("\nSource and result lists have the same count and order of items", ConsoleColor.Yellow);
            }

            resultListWasChecked = true;
        }

        static void TaskProcc_TaskStarting(object sender, TaskProcessingStateEventArgs<string> e)
        {
            //add start processing item to result list
            lock (list_locker)
            {
                listOfProccessedTaskData.Add(e.TaskData);
            }

            if (e.TaskData.Ex == null)

                consoleLogger.AddTask(
                    async msg =>
                    {
                        logMsg($"Client task: {e.TaskData.Name} was starting ", ConsoleColor.Blue);
                    },
                    null,
                    string.Empty);
        }

        static void TaskProcc_TaskComplited(object sender, TaskProcessingStateEventArgs<string> e)
        {
            string msgToShow = string.Empty;
            ConsoleColor consCol = Console.ForegroundColor;

            if (e.TaskData.Ex == null)
            {
                msgToShow = $"Client task {e.TaskData.Name} complited succsesfull";
                consCol = ConsoleColor.Green;
            }
            else
            {
                msgToShow = $"!!! Current task ({e.TaskData.Name}) processing error. {e.TaskData.Ex.Message}";
                consCol = ConsoleColor.Red;
            }

            consoleLogger.AddTask(
                async msg =>
                {
                    logMsg(msgToShow, consCol);
                },
                null,
                string.Empty);
        }

        #endregion
        
        #region Additional methods

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
