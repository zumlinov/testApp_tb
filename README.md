There is solution for tobbi pro.

sln file: solution\tobbi_pc 

There are two projects: 
	tobbi_pc - classes and provider consumer class; ( TasksProcessor<T> )
	tobbi_testApp - test application to test classes code.
	
Test process :

1. Create tested consumer class. (TasksProcessor<T>)
2. Test Start/Stop methods of this class in unusual way 	
3. Add 50 tasks from different threads 
4. Wait for tasks processing 
5. Compare source list of tasks with result list of tasks
	Both of them must have the same count and order of items.
