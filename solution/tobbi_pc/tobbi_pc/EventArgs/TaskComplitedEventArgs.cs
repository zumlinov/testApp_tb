using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tobbi_pc.Classes;

namespace tobbi_pc
{
    /// <summary>
    /// Contain data for TaskComplite event
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    public class TaskComplitedEventArgs<T,K> : EventArgs
    {
        #region Public properties

        /// <summary>
        /// Contain all info about task
        /// </summary>
        public TaskData<T,K> TaskData { get; }

        #endregion

        #region Ctors

        public TaskComplitedEventArgs(TaskData<T, K> taskData)
        {
            if(taskData==null)
            {
                throw new ArgumentNullException("Task data can't be null.");
            }

            TaskData = TaskData;
        }

        #endregion
    }
}
