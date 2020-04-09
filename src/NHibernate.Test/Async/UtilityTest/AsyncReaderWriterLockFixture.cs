﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	public partial class AsyncReaderWriterLockFixture
	{

		[Test, Explicit]
		public async Task TestConcurrentReadWriteAsync()
		{
			var l = new AsyncReaderWriterLock();
			for (var i = 0; i < 2; i++)
			{
				var writeReleaser = await (l.WriteLockAsync());
				Assert.That(l.Writing, Is.True);

				var secondWriteSemaphore = new SemaphoreSlim(0);
				var secondWriteReleaser = default(AsyncReaderWriterLock.Releaser);
				var secondWriteThread = new Thread(
					() =>
					{
						secondWriteSemaphore.Wait();
						secondWriteReleaser = l.WriteLock();
					});
				secondWriteThread.Priority = ThreadPriority.Highest;
				secondWriteThread.Start();
				await (AssertEqualValueAsync(() => secondWriteThread.ThreadState == ThreadState.WaitSleepJoin, true));

				var secondReadThreads = new Thread[20];
				var secondReadReleasers = new AsyncReaderWriterLock.Releaser[secondReadThreads.Length];
				var secondReadSemaphore = new SemaphoreSlim(0);
				for (var j = 0; j < secondReadReleasers.Length; j++)
				{
					var index = j;
					var thread = new Thread(
						() =>
						{
							secondReadSemaphore.Wait();
							secondReadReleasers[index] = l.ReadLock();
						});
					thread.Priority = ThreadPriority.Highest;
					secondReadThreads[j] = thread;
					thread.Start();
				}

				await (AssertEqualValueAsync(() => secondReadThreads.All(o => o.ThreadState == ThreadState.WaitSleepJoin), true));

				var firstReadReleaserTasks = new Task[30];
				var firstReadStopSemaphore = new SemaphoreSlim(0);
				for (var j = 0; j < firstReadReleaserTasks.Length; j++)
				{
					firstReadReleaserTasks[j] = Task.Run(async () =>
					{
						var releaser = await (l.ReadLockAsync());
						await (firstReadStopSemaphore.WaitAsync());
						releaser.Dispose();
					});
				}

				await (AssertEqualValueAsync(() => l.ReadersWaiting, firstReadReleaserTasks.Length, waitDelay: 60000));

				writeReleaser.Dispose();
				secondWriteSemaphore.Release();
				secondReadSemaphore.Release(secondReadThreads.Length);
				await (Task.Delay(1000));
				firstReadStopSemaphore.Release(firstReadReleaserTasks.Length);

				await (AssertEqualValueAsync(() => firstReadReleaserTasks.All(o => o.IsCompleted), true));
				Assert.That(l.ReadersWaiting, Is.EqualTo(secondReadThreads.Length));
				Assert.That(l.CurrentReaders, Is.EqualTo(0));
				await (AssertEqualValueAsync(() => secondWriteThread.IsAlive, false));
				await (AssertEqualValueAsync(() => secondReadThreads.All(o => o.IsAlive), true));

				secondWriteReleaser.Dispose();
				await (AssertEqualValueAsync(() => secondReadThreads.All(o => !o.IsAlive), true));

				Assert.That(l.ReadersWaiting, Is.EqualTo(0));
				Assert.That(l.CurrentReaders, Is.EqualTo(secondReadThreads.Length));

				foreach (var secondReadReleaser in secondReadReleasers)
				{
					secondReadReleaser.Dispose();
				}

				Assert.That(l.ReadersWaiting, Is.EqualTo(0));
				Assert.That(l.CurrentReaders, Is.EqualTo(0));
			}
		}

		[Test]
		public async Task TestInvaildExitReadLockUsageAsync()
		{
			var l = new AsyncReaderWriterLock();
			var readReleaser = await (l.ReadLockAsync());
			var readReleaser2 = await (l.ReadLockAsync());

			readReleaser.Dispose();
			readReleaser2.Dispose();
			Assert.Throws<InvalidOperationException>(() => readReleaser.Dispose());
			Assert.Throws<InvalidOperationException>(() => readReleaser2.Dispose());
		}

		[Test]
		public void TestOperationAfterDisposeAsync()
		{
			var l = new AsyncReaderWriterLock();
			l.Dispose();

			Assert.ThrowsAsync<ObjectDisposedException>(() => l.ReadLockAsync());
			Assert.ThrowsAsync<ObjectDisposedException>(() => l.WriteLockAsync());
		}

		[Test]
		public async Task TestInvaildExitWriteLockUsageAsync()
		{
			var l = new AsyncReaderWriterLock();
			var writeReleaser = await (l.WriteLockAsync());

			writeReleaser.Dispose();
			Assert.Throws<InvalidOperationException>(() => writeReleaser.Dispose());
		}

		private static async Task AssertEqualValueAsync<T>(Func<T> getValueFunc, T value, Task task = null, int waitDelay = 5000, CancellationToken cancellationToken = default(CancellationToken))
		{
			var currentTime = 0;
			var step = 5;
			while (currentTime < waitDelay)
			{
				if (task != null)
				{
					task.Wait(step);
				}
				else
				{
					await (Task.Delay(step, cancellationToken));
				}
				
				currentTime += step;
				if (getValueFunc().Equals(value))
				{
					return;
				}

				step *= 2;
			}

			Assert.That(getValueFunc(), Is.EqualTo(value));
		}

		private static Task AssertTaskCompletedAsync(Task task, CancellationToken cancellationToken = default(CancellationToken))
		{
			return AssertEqualValueAsync(() => task.IsCompleted, true, task, cancellationToken: cancellationToken);
		}
	}
}
