using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using log4net;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class ThreadSafeDictionaryFixture
	{
		public ThreadSafeDictionaryFixture()
		{
			log4net.Config.XmlConfigurator.Configure(LogManager.GetRepository(typeof(ThreadSafeDictionaryFixture).Assembly));
		}

		private static readonly ILog log = LogManager.GetLogger(typeof(ThreadSafeDictionaryFixture));

		private readonly Random rnd = new Random();
		private int read, write;

		[Test, Explicit]
		public void MultiThreadAccess()
		{
			MultiThreadRunner<ConcurrentDictionary<int, int>>.ExecuteAction[] actions =
				new MultiThreadRunner<ConcurrentDictionary<int, int>>.ExecuteAction[]
					{
						delegate(ConcurrentDictionary<int, int> d)
							{
								log.DebugFormat("T{0} Add", Thread.CurrentThread.Name);
								write++;
								d.TryAdd(rnd.Next(), rnd.Next());
							}, 
						delegate(ConcurrentDictionary<int, int> d)
						 	{
								log.DebugFormat("T{0} ContainsKey", Thread.CurrentThread.Name);
					   		read++;
					   		d.ContainsKey(rnd.Next());
					   	}, 
						delegate(ConcurrentDictionary<int, int> d)
			   	   	{
								log.DebugFormat("T{0} Remove", Thread.CurrentThread.Name);
			   	   		write++;
			   	   		int value;
			   	   		d.TryRemove(rnd.Next(), out value);
			   	   	}, 
						delegate(ConcurrentDictionary<int, int> d)
	   	   	   	{
								log.DebugFormat("T{0} TryGetValue", Thread.CurrentThread.Name);
	   	   	   		read++;
	   	   	   		int val;
	   	   	   		d.TryGetValue(rnd.Next(), out val);
	   	   	   	}, 
						delegate(ConcurrentDictionary<int, int> d)
 	   	   	   	{
 	   	   	   		try
 	   	   	   		{
									log.DebugFormat("T{0} get_this[]", Thread.CurrentThread.Name);
 	   	   	   			read++;
 	   	   	   			int val = d[rnd.Next()];
 	   	   	   		}
 	   	   	   		catch (KeyNotFoundException)
 	   	   	   		{
 	   	   	   			// not foud key
 	   	   	   		}
 	   	   	   	}, 
						delegate(ConcurrentDictionary<int, int> d)
 	   	   	   	{
								log.DebugFormat("T{0} set_this[]", Thread.CurrentThread.Name);
 	   	   	   		write++;
 	   	   	   		d[rnd.Next()] = rnd.Next();
 	   	   	   	},
						delegate(ConcurrentDictionary<int, int> d)
							{
								log.DebugFormat("T{0} Keys", Thread.CurrentThread.Name);
								read++;
								IEnumerable<int> e = d.Keys;
							},
						delegate(ConcurrentDictionary<int, int> d)
					   	{
								log.DebugFormat("T{0} Values", Thread.CurrentThread.Name);
					   		read++;
					   		IEnumerable<int> e = d.Values;
					   	}, 
						delegate(ConcurrentDictionary<int, int> d)
			   	   	{
								log.DebugFormat("T{0} GetEnumerator", Thread.CurrentThread.Name);
			   	   		read++;
			   	   		foreach (KeyValuePair<int, int> pair in d)
			   	   		{
			   	   			
			   	   		}
			   	   	},
					};
			MultiThreadRunner<ConcurrentDictionary<int, int>> mtr = new MultiThreadRunner<ConcurrentDictionary<int, int>>(20, actions);
			ConcurrentDictionary<int, int> wrapper = new ConcurrentDictionary<int, int>();
			mtr.EndTimeout = 2000;
			mtr.Run(wrapper);
			log.DebugFormat("{0} reads, {1} writes -- elements {2}", read, write, wrapper.Count);
		}
	}
}
