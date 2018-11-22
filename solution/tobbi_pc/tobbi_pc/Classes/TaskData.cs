using System;
using System.Threading.Tasks;

namespace tobbi_pc.Classes
{
    /// <summary>
    /// Contain data is needed to start client task  
    /// </summary>
    /// <typeparam name="T">Type of income data will be processed by client task</typeparam>   
    public class TaskData<T>
    {
        #region Public properties

        /// <summary>
        ///Async TaskMethod user want to be done. 
        /// </summary>
        public Func<T, Task> TaskMethod { get; protected set; }

        /// <summary>
        /// Income data need to be processed by TaskMethod
        /// </summary>
        public T IncomeData { get; set; }

        /// <summary>
        /// Exception if something went wrong
        /// </summary>
        public Exception Ex { get; set; }

        /// <summary>
        /// Id for task idintification 
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Name of task ( mostly for people)
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region Ctors

        public TaskData(Func<T, Task> taskMethod, T incomeData, string name)
        {
            TaskMethod = taskMethod ?? throw new ArgumentNullException("Task can't be null.");

            IncomeData = incomeData;
            Name = name;

            Id = Guid.NewGuid();
        }

        #endregion
    }
}
