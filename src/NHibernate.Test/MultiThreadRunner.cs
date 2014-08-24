using System;
using System.Threading;

namespace NHibernate.Test
{
	public class MultiThreadRunner<T>
	{
		public delegate void ExecuteAction(T subject);
		private readonly int numThreads;
		private readonly ExecuteAction[] actions;
		private readonly Random rnd = new Random();
		private bool running;
		private int timeout = 1000;
		private int timeoutBetweenThreadStart = 30;

		public MultiThreadRunner(int numThreads, ExecuteAction[] actions)
		{
			if(numThreads < 1)
			{
				throw new ArgumentOutOfRangeException("numThreads",numThreads,"Must be GT 1");
			}
			if (actions == null || actions.Length == 0)
			{
				throw new ArgumentNullException("actions");
			}
			foreach (ExecuteAction action in actions)
			{
				if(action==null)
					throw new ArgumentNullException("actions", "null delegate");
			}
			this.numThreads = numThreads;
			this.actions = actions;
		}

		public int EndTimeout
		{
			get { return timeout; }
			set { timeout = value; }
		}

		public int TimeoutBetweenThreadStart
		{
			get { return timeoutBetweenThreadStart; }
			set { timeoutBetweenThreadStart = value; }
		}

		public void Run(T subjectInstance)
		{
			running = true;
			Thread[] t = new Thread[numThreads];
			for (int i = 0; i < numThreads; i++)
			{
				t[i] = new Thread(ThreadProc);
				t[i].Name = i.ToString();
				t[i].Start(subjectInstance);
				if (i > 2)
					Thread.Sleep(timeoutBetweenThreadStart);
			}

			Thread.Sleep(timeout);

			// Tell the threads to shut down, then wait until they all
			// finish.
			running = false;
			for (int i = 0; i < numThreads; i++)
			{
				t[i].Join();
			}
		}

		private void ThreadProc(object arg)
		{
			T subjectInstance = (T) arg;
			while (running)
			{
				int actionIdx = rnd.Next(0, actions.Length);
				actions[actionIdx](subjectInstance);
			}
		}
	}
}
