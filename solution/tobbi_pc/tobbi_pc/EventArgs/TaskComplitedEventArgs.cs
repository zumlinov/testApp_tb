using System;
using tobbi_pc.Classes;

namespace tobbi_pc
{
    /// <summary>
    /// Contain data for TaskComplite event
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    public class TaskProcessingStateEventArgs<T,K> : EventArgs
    {
        #region Public properties

        /// <summary>
        /// Contain all info about task
        /// </summary>
        public TaskData<T,K> TaskData { get; }

        #endregion

        #region Ctors

        public TaskProcessingStateEventArgs(TaskData<T, K> taskData)
        {
            if(taskData==null)
            {
                throw new ArgumentNullException("Task data can't be null.");
            }

            TaskData = taskData;
        }

        #endregion
    }
}
