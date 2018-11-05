using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NHibernate.Test
{
	public class MultiThreadRunner<T>
	{
		public delegate void ExecuteAction(T subject);

		private readonly int _numThreads;
		private readonly ExecuteAction[] _actions;
		private readonly Random _rnd = new Random();
		private volatile bool _running;
		private ConcurrentQueue<Exception> _errors = new ConcurrentQueue<Exception>();

		public MultiThreadRunner(int numThreads, params ExecuteAction[] actions)
		{
			if (numThreads < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(numThreads), numThreads, "Must be GTE 1");
			}
			if (actions == null || actions.Length == 0)
			{
				throw new ArgumentNullException(nameof(actions));
			}
			if (actions.Any(action => action == null))
			{
				throw new ArgumentNullException(nameof(actions), "null delegate");
			}
			_numThreads = numThreads;
			_actions = actions;
		}

		public int EndTimeout { get; set; } = 1000;

		public int TimeoutBetweenThreadStart { get; set; } = 30;

		public Exception[] GetErrors() => _errors.ToArray();
		public void ClearErrors() => _errors = new ConcurrentQueue<Exception>();

		public int Run(T subjectInstance)
		{
			var allThreads = new List<ThreadHolder<T>>();

			var launcher = new Thread(
				() =>
				{
					try
					{
						for (var i = 0; i < _numThreads; i++)
						{
							var threadHolder = new ThreadHolder<T>
							{
								Thread = new Thread(ThreadProc) { Name = i.ToString() },
								Subject = subjectInstance
							};
							threadHolder.Thread.Start(threadHolder);
							allThreads.Add(threadHolder);
							if (i > 2 && TimeoutBetweenThreadStart > 0)
								Thread.Sleep(TimeoutBetweenThreadStart);
						}
					}
					catch (Exception e)
					{
						_errors.Enqueue(e);
						throw;
					}
				});
			var totalLoops = 0;
			_running = true;
			// Use a separated thread for launching in case too many threads are asked: the inner Start will freeze
			// but would be able to resume once _running would have been set to false, causing first threads to stop.
			launcher.Start();
			// Sleep for the required timeout, taking into account the start delay (if all threads are launchable without
			// having to wait due to thread starvation).
			Thread.Sleep(TimeoutBetweenThreadStart * _numThreads + EndTimeout);
			// Tell the threads to shut down, then wait until they all finish.
			_running = false;
			launcher.Join();
			foreach (var threadHolder in allThreads.Where(t => t != null))
			{
				threadHolder.Thread.Join();
				totalLoops += threadHolder.LoopsDone;
			}
			return totalLoops;
		}

		private void ThreadProc(object arg)
		{
			try
			{
				var holder = (ThreadHolder<T>) arg;
				while (_running)
				{
					var actionIdx = _rnd.Next(0, _actions.Length);
					_actions[actionIdx](holder.Subject);
					holder.LoopsDone++;
				}
			}
			catch (Exception e)
			{
				_errors.Enqueue(e);
				throw;
			}
		}

		private class ThreadHolder<TH>
		{
			public Thread Thread { get; set; }
			public int LoopsDone { get; set; }
			public TH Subject { get; set; }
		}
	}
}
