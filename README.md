There is solution for tobbi pro.

sln file: solution\tobbi_pc 

There are two projects: 
	tobbi_pc - classes and provider consumer class; ( TasksProcessor<T> )
	tobbi_testApp - test application to test classes code.
	
Client task class TaskData<T> contain next properties:
            
        /// <summary>
        ///TaskMethod user want to be done. 
        /// </summary>
        public Action<T> TaskMethod { get; protected set; }

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
		
Fill all this properties and pass it to a taskProcessor.		
	
Testing process :

1. Create tested consumer class. (TasksProcessor<T>)
2. Test Start/Stop methods of this class in unusual way 	
3. Add 50 tasks from different threads 
4. Wait for tasks processing 
5. Compare source list of tasks with result list of tasks
	Both of them must have the same count and order of items.
