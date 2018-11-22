using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using tobbi_pc.Classes;

namespace tobbi_pc
{
    /// <summary>
    /// Class to parallel add , store  and execute task one by one
    /// </summary>
    /// <typeparam name="T">Type of param will be passed to task delegat</typeparam>
    public class TasksProcessor<T>
    {
        #region privet fields
        
        //this instance will stop work loop if queue will be empty
        AutoResetEvent loopTriger;

        //queue to store tasks
        ConcurrentQueue<TaskData<T>> tasksQ;

        //task to process client tasks
        Task workTask;

        //thread of current task
        Thread currWorkTaskThread;

        //token to cancel working  loop processing
        CancellationTokenSource cancelationTS;

        #endregion

        #region Public props
        
        /// <summary>
        /// 
        /// </summary>
        public int WaitForCurrentTaskFinishingTimeOut { get; }

        #endregion

        #region Events

        /// <summary>
        /// This event is raised when task processing was complited
        /// </summary>
        public event EventHandler<TaskProcessingStateEventArgs<T>> TaskComplited;

        /// <summary>
        /// This event is raised when task processing begin
        /// </summary>
        public event EventHandler<TaskProcessingStateEventArgs<T>> TaskStarting;

        /// <summary>
        /// Inform subscribers that queue of tasks is empty again
        /// </summary>
        public event EventHandler<EventArgs> AllTasksProcessed;

        #endregion

        #region Ctors

        public TasksProcessor(int waitForCurrentTaskTimeOut = 300)
        {           
            WaitForCurrentTaskFinishingTimeOut = waitForCurrentTaskTimeOut;

            if(WaitForCurrentTaskFinishingTimeOut<0)
            {
                WaitForCurrentTaskFinishingTimeOut = 0;
            }

            loopTriger = new AutoResetEvent(false);            
            cancelationTS = new CancellationTokenSource();
            createQ();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// This method add data to queue
        /// </summary>
        /// <param name="taskMethod">method will be called </param>
        /// <param name="incomeData">data to process by <taskMethod></param>
        /// <param name="taskName">Just name for task data. Can be usefull for client</param>
        /// <returns>Id of created instance of TaskData  </returns>
        public Guid AddTask(Func<T,Task> taskMethod, T incomeData, string taskName)
        {
            //create task data to proces
            TaskData<T> taskData = new TaskData<T>(taskMethod, incomeData, taskName);

            AddTask(taskData);

            return taskData.Id;
        }

        /// <summary>
        /// Add TaskData instance 
        /// </summary>
        /// <param name="taskData">Item to add </param>
        public void AddTask(TaskData<T> taskData)
        {
            //add it to Q
            tasksQ.Enqueue(taskData);

            //scroll the loop
            loopTriger.Set();
        }

        /// <summary>
        /// Remove all TaskData from queue
        /// </summary>
        public void StopAndClear()
        {
            Stop();
            createQ();
        }
        

        /// <summary>
        /// Start tasks propcessing 
        /// </summary>
        public void Start()
        {
            //task already started - stop it
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
            
            workTask = Task.Factory.StartNew(workMethod);                 
        }

        /// <summary>
        /// Stop task processing 
        /// </summary>
        public void Stop()
        {
            if (workTask != null)
            {
                if (workTask.Status == TaskStatus.Running 
                        || workTask.Status == TaskStatus.WaitingToRun
                        || workTask.Status == TaskStatus.Created
                        || workTask.Status == TaskStatus.WaitingForActivation)
                {
                    //request cancelation
                    cancelationTS.Cancel();
                    
                    //scroll the working loop
                    loopTriger.Set();
                    
                    //wait for task finishing
                    workTask.Wait(WaitForCurrentTaskFinishingTimeOut);
                }

                #region Force way to abort a task - last chance

                if (workTask.Status == TaskStatus.Running)
                {
                    try
                    {
                        currWorkTaskThread?.Abort();
                    }
                    catch (Exception ex)
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

        /// <summary>
        /// Process TaskData from queue
        /// </summary>
        async Task workMethod()
        {            
            //may be used to emergency task abort
            currWorkTaskThread = Thread.CurrentThread;

            TaskData<T> currentTD = null;

            while (true)
            {
                #region Check cancelation 

                if (cancelationTS.Token.IsCancellationRequested)
                {
                    //logMsg("Processing was canceled", ConsoleColor.Magenta);
                    break;
                }

                #endregion

                //check queue length
                if(!tasksQ.IsEmpty)
                {
                    //try to get next task
                    if(tasksQ.TryDequeue(out currentTD))
                    {
                        try
                        {                            
                            onTaskStarting(currentTD);
                           
                            await currentTD.TaskMethod(currentTD.IncomeData);                                                     
                        }
                        catch (Exception ex)
                        {
                            currentTD.Ex = ex;
                        }

                        //notify subscribers
                        onTaskComplited(currentTD);
                    }                   
                }
                else
                {
                    //notifi subscribers
                    onAllTasksProcessed();

                    //waiting for new tasks will be added to the Q
                    loopTriger.WaitOne();
                }                
            }
        }
                
        void onTaskComplited(TaskData<T> taskData)
        {
            TaskComplited?.Invoke(this, new TaskProcessingStateEventArgs<T>(taskData));
        }

        void onTaskStarting(TaskData<T> taskData)
        {
            TaskStarting?.Invoke(this, new TaskProcessingStateEventArgs<T>(taskData));
        }

        void onAllTasksProcessed()
        {
            AllTasksProcessed?.Invoke(this, new EventArgs());
        }

        void createQ()
        {
            tasksQ = new ConcurrentQueue<TaskData<T>>();
        }

        #endregion

    }
}
