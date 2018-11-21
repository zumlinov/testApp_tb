using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tobbi_pc.Classes;

namespace tobbi_pc
{
    public class TaskProcessor<T,K>
    {
        #region privet fields

        object console_locker;
        //bool doWork;

        AutoResetEvent loopTriger;

        ConcurrentQueue<TaskData<T, K>> tasksQ;

        Task workTask;

        Thread currWorkTaskThread;

        CancellationTokenSource cancelationTS;
        
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public int WaitForCurrentTaskTimeOut { get; }

        #region Ctors

        public TaskProcessor(int waitForCurrentTaskTimeOut)
        {
            console_locker = new object();

            WaitForCurrentTaskTimeOut = waitForCurrentTaskTimeOut;

            if(WaitForCurrentTaskTimeOut<0)
            {
                WaitForCurrentTaskTimeOut = 0;
            }

            loopTriger = new AutoResetEvent(false);            
            cancelationTS = new CancellationTokenSource();
            tasksQ = new ConcurrentQueue<TaskData<T, K>>();
        }

        #endregion

        #region Public methods

        public Guid AddTAsk(Func<T,K> taskMethod, T incomeData, string taskName)
        {
            TaskData<T, K> taskData = new TaskData<T, K>(taskMethod, incomeData, taskName);

            tasksQ.Enqueue(taskData);

            loopTriger.Set();

            return taskData.Id;
        }

        /// <summary>
        /// Start tasks propcessing 
        /// </summary>
        public void Start()
        {
            //task already started
            if(workTask!=null)
            {
                if(workTask.Status == TaskStatus.Running)
                {
                    return;
                }
                else
                {
                    Stop();
                }
            }

            cancelationTS = new CancellationTokenSource();
            workTask = Task.Run(action: workMethod);                 
        }

        /// <summary>
        /// Stop task processing 
        /// </summary>
        public void Stop()
        {
            if(workTask!=null && workTask.Status == TaskStatus.Running)
            {
                //cancelation request
                cancelationTS.Cancel();

                loopTriger.Set();

                //wait for task finishing
                workTask.Wait(WaitForCurrentTaskTimeOut);

                #region Forece way to abort task

                if(workTask.Status==TaskStatus.Running)
                {
                    try
                    {
                        currWorkTaskThread?.Abort();
                    }
                    catch (Exception)
                    {
                        
                    }
                }

                #endregion
            }

            workTask = null;
            currWorkTaskThread = null;
        }

        #endregion

        #region private methods

        void workMethod()
        {
            currWorkTaskThread = Thread.CurrentThread;

            TaskData<T, K> currentTD = null;

            while (true)
            {
                #region Check cancelation 

                if (cancelationTS.Token.IsCancellationRequested)
                {
                    logMsg("Processing was canceled", ConsoleColor.Magenta);
                    break;
                }

                #endregion

                if(!tasksQ.IsEmpty)
                {
                    if(tasksQ.TryDequeue(out currentTD))
                    {
                        try
                        {
                            logMsg($"Task: {currentTD.Name} was started.", ConsoleColor.Blue, false);
                            currentTD.Result = currentTD.TaskMethod(currentTD.IncomeData);
                            logMsg($"Task: {currentTD.Name} succsessfully processed.", ConsoleColor.Green);
                        }
                        catch (Exception ex)
                        {
                            logMsg($"Current task ({currentTD.Name}) processing error. {ex.Message}", ConsoleColor.Red);
                            currentTD.Ex = ex;
                        }
                    }
                    else
                    {
                        logMsg($"Task Dequeuing failure.", ConsoleColor.Yellow);
                    }
                }
                else
                {
                    //Console.WriteLine("Work loop paused");
                    loopTriger.WaitOne();
                }                
            }
        }

        void logMsg(string msg, ConsoleColor color, bool changeLineAfterText = true)
        {
            lock (console_locker)
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

        #endregion

    }
}
