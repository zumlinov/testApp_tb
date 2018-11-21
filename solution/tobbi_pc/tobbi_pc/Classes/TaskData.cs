﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tobbi_pc.Classes
{
    /// <summary>
    /// Contain data is needed to start client task  
    /// </summary>
    /// <typeparam name="T">Type of data produced by client task</typeparam>
    /// <typeparam name="K">Income data for client task</typeparam>
    public class TaskData<T>
    {
        #region Public properties

        /// <summary>
        ///TaskMethod user want to be done
        /// </summary>
        public Action<T> TaskMethod { get; protected set; }

        /// <summary>
        /// Income data need to be processed by Task
        /// </summary>
        public T IncomeData { get; protected set; }        

        /// <summary>
        /// Exception if it happaned
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

        //temp
        public int IdWorkTask { get; set; }

        #endregion

        #region Ctors

        public TaskData(Action<T> taskMethod, T incomeData, string name)
        {
            if(taskMethod==null)
            {
                throw new ArgumentNullException("Task can't be null.");
            }

            TaskMethod = taskMethod;
            IncomeData = incomeData;
            Name = name;

            Id = new Guid();
        }

        #endregion
    }
}
