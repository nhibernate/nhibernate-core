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
		[Test]
		public void TestBlocking()
		{
			var l = new AsyncReaderWriterLock();
			for (var i = 0; i < 2; i++)
			{
				var readReleaser = l.ReadLock();
				Assert.That(l.CurrentReaders, Is.EqualTo(1));

				var readReleaserTask = Task.Run(() => l.ReadLock());
				AssertEqualValue(() => l.CurrentReaders, 2, readReleaserTask);
				AssertTaskCompleted(readReleaserTask);

				var writeReleaserTask = Task.Run(() => l.WriteLock());
				AssertEqualValue(() => l.AcquiredWriteLock, true, writeReleaserTask);
				AssertEqualValue(() => l.Writing, false, writeReleaserTask);
				Assert.That(writeReleaserTask.IsCompleted, Is.False);

				readReleaser.Dispose();
				Assert.That(l.CurrentReaders, Is.EqualTo(1));
				Assert.That(writeReleaserTask.IsCompleted, Is.False);

				readReleaserTask.Result.Dispose();
				Assert.That(l.CurrentReaders, Is.EqualTo(0));
				AssertEqualValue(() => l.Writing, true, writeReleaserTask);
				AssertTaskCompleted(writeReleaserTask);

				readReleaserTask = Task.Run(() => l.ReadLock());
				AssertEqualValue(() => l.ReadersWaiting, 1, readReleaserTask);
				Assert.That(readReleaserTask.IsCompleted, Is.False);

				var writeReleaserTask2 = Task.Run(() => l.WriteLock());
				AssertEqualValue(() => l.WritersWaiting, 1, writeReleaserTask2);
				Assert.That(writeReleaserTask2.IsCompleted, Is.False);

				writeReleaserTask.Result.Dispose();
				AssertEqualValue(() => l.WritersWaiting, 0, writeReleaserTask2);
				AssertEqualValue(() => l.Writing, true, writeReleaserTask2);
				Assert.That(readReleaserTask.IsCompleted, Is.False);
				AssertTaskCompleted(writeReleaserTask2);

				writeReleaserTask2.Result.Dispose();
				AssertEqualValue(() => l.Writing, false, writeReleaserTask2);
				AssertEqualValue(() => l.ReadersWaiting, 0, readReleaserTask);
				AssertEqualValue(() => l.CurrentReaders, 1, readReleaserTask);
				AssertTaskCompleted(readReleaserTask);

				readReleaserTask.Result.Dispose();
				Assert.That(l.ReadersWaiting, Is.EqualTo(0));
				Assert.That(l.WritersWaiting, Is.EqualTo(0));
				Assert.That(l.CurrentReaders, Is.EqualTo(0));
				Assert.That(l.Writing, Is.False);
			}
		}

		[Test]
		public void TestBlockingAsync()
		{
			var l = new AsyncReaderWriterLock();
			for (var i = 0; i < 2; i++)
			{
				var readReleaserTask = l.ReadLockAsync();
				AssertEqualValue(() => l.CurrentReaders, 1, readReleaserTask);
				AssertTaskCompleted(readReleaserTask);

				var readReleaserTask2 = l.ReadLockAsync();
				AssertEqualValue(() => l.CurrentReaders, 2, readReleaserTask2);
				AssertTaskCompleted(readReleaserTask2);

				var writeReleaserTask = l.WriteLockAsync();
				AssertEqualValue(() => l.AcquiredWriteLock, true, writeReleaserTask);
				AssertEqualValue(() => l.Writing, false, writeReleaserTask);
				Assert.That(writeReleaserTask.IsCompleted, Is.False);

				readReleaserTask.Result.Dispose();
				Assert.That(l.CurrentReaders, Is.EqualTo(1));
				Assert.That(writeReleaserTask.IsCompleted, Is.False);

				readReleaserTask2.Result.Dispose();
				Assert.That(l.CurrentReaders, Is.EqualTo(0));
				AssertEqualValue(() => l.Writing, true, writeReleaserTask);
				AssertTaskCompleted(writeReleaserTask);

				readReleaserTask = l.ReadLockAsync();
				AssertEqualValue(() => l.ReadersWaiting, 1, readReleaserTask);
				Assert.That(readReleaserTask.IsCompleted, Is.False);

				var writeReleaserTask2 = l.WriteLockAsync();
				AssertEqualValue(() => l.WritersWaiting, 1, writeReleaserTask2);
				Assert.That(writeReleaserTask2.IsCompleted, Is.False);

				writeReleaserTask.Result.Dispose();
				AssertEqualValue(() => l.WritersWaiting, 0, writeReleaserTask2);
				AssertEqualValue(() => l.Writing, true, writeReleaserTask2);
				Assert.That(readReleaserTask.IsCompleted, Is.False);
				AssertTaskCompleted(writeReleaserTask2);

				writeReleaserTask2.Result.Dispose();
				AssertEqualValue(() => l.Writing, false, writeReleaserTask2);
				AssertEqualValue(() => l.ReadersWaiting, 0, readReleaserTask);
				AssertEqualValue(() => l.CurrentReaders, 1, readReleaserTask);
				AssertTaskCompleted(readReleaserTask);

				readReleaserTask.Result.Dispose();
				Assert.That(l.ReadersWaiting, Is.EqualTo(0));
				Assert.That(l.WritersWaiting, Is.EqualTo(0));
				Assert.That(l.CurrentReaders, Is.EqualTo(0));
				Assert.That(l.Writing, Is.False);
			}
		}

		[Test, Explicit]
		public void TestConcurrentReadWrite()
		{
			var l = new AsyncReaderWriterLock();
			for (var i = 0; i < 2; i++)
			{
				var writeReleaser = l.WriteLock();
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
				AssertEqualValue(() => secondWriteThread.ThreadState == ThreadState.WaitSleepJoin, true);

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

				AssertEqualValue(() => secondReadThreads.All(o => o.ThreadState == ThreadState.WaitSleepJoin), true);

				var firstReadReleaserTasks = new Task[30];
				var firstReadStopSemaphore = new SemaphoreSlim(0);
				for (var j = 0; j < firstReadReleaserTasks.Length; j++)
				{
					firstReadReleaserTasks[j] = Task.Run(() =>
					{
						var releaser = l.ReadLock();
						firstReadStopSemaphore.Wait();
						releaser.Dispose();
					});
				}

				AssertEqualValue(() => l.ReadersWaiting, firstReadReleaserTasks.Length, waitDelay: 60000);

				writeReleaser.Dispose();
				secondWriteSemaphore.Release();
				secondReadSemaphore.Release(secondReadThreads.Length);
				Thread.Sleep(1000);
				firstReadStopSemaphore.Release(firstReadReleaserTasks.Length);

				AssertEqualValue(() => firstReadReleaserTasks.All(o => o.IsCompleted), true);
				Assert.That(l.ReadersWaiting, Is.EqualTo(secondReadThreads.Length));
				Assert.That(l.CurrentReaders, Is.EqualTo(0));
				AssertEqualValue(() => secondWriteThread.IsAlive, false);
				AssertEqualValue(() => secondReadThreads.All(o => o.IsAlive), true);

				secondWriteReleaser.Dispose();
				AssertEqualValue(() => secondReadThreads.All(o => !o.IsAlive), true);

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
		public void TestInvaildExitReadLockUsage()
		{
			var l = new AsyncReaderWriterLock();
			var readReleaser = l.ReadLock();
			var readReleaser2 = l.ReadLock();

			readReleaser.Dispose();
			readReleaser2.Dispose();
			Assert.Throws<InvalidOperationException>(() => readReleaser.Dispose());
			Assert.Throws<InvalidOperationException>(() => readReleaser2.Dispose());
		}

		[Test]
		public void TestOperationAfterDispose()
		{
			var l = new AsyncReaderWriterLock();
			l.Dispose();

			Assert.Throws<ObjectDisposedException>(() => l.ReadLock());
			Assert.Throws<ObjectDisposedException>(() => l.WriteLock());
		}

		[Test]
		public void TestInvaildExitWriteLockUsage()
		{
			var l = new AsyncReaderWriterLock();
			var writeReleaser = l.WriteLock();

			writeReleaser.Dispose();
			Assert.Throws<InvalidOperationException>(() => writeReleaser.Dispose());
		}

		[Test]
		public void TestMixingSyncAndAsync()
		{
			var l = new AsyncReaderWriterLock();
			var readReleaser = l.ReadLock();
			Assert.That(l.CurrentReaders, Is.EqualTo(1));

			var readReleaserTask = l.ReadLockAsync();
			AssertEqualValue(() => l.CurrentReaders, 2, readReleaserTask);
			AssertTaskCompleted(readReleaserTask);

			readReleaser.Dispose();
			Assert.That(l.CurrentReaders, Is.EqualTo(1));

			readReleaserTask.Result.Dispose();
			Assert.That(l.CurrentReaders, Is.EqualTo(0));

			var writeReleaser = l.WriteLock();
			Assert.That(l.AcquiredWriteLock, Is.True);

			var writeReleaserTask = l.WriteLockAsync();
			AssertEqualValue(() => l.WritersWaiting, 1, writeReleaserTask);
			Assert.That(writeReleaserTask.IsCompleted, Is.False);

			readReleaserTask = Task.Run(() => l.ReadLock());
			AssertEqualValue(() => l.ReadersWaiting, 1, readReleaserTask);
			Assert.That(readReleaserTask.IsCompleted, Is.False);

			var readReleaserTask2 = l.ReadLockAsync();
			AssertEqualValue(() => l.ReadersWaiting, 2, readReleaserTask2);
			Assert.That(readReleaserTask2.IsCompleted, Is.False);

			writeReleaser.Dispose();
			AssertEqualValue(() => l.WritersWaiting, 0, writeReleaserTask);
			AssertEqualValue(() => l.Writing, true, writeReleaserTask);
			AssertTaskCompleted(writeReleaserTask);
			Assert.That(readReleaserTask.IsCompleted, Is.False);
			Assert.That(readReleaserTask2.IsCompleted, Is.False);

			writeReleaserTask.Result.Dispose();
			AssertEqualValue(() => l.CurrentReaders, 2, readReleaserTask);
			AssertEqualValue(() => l.ReadersWaiting, 0, readReleaserTask2);
			AssertTaskCompleted(readReleaserTask);
			AssertTaskCompleted(readReleaserTask2);
		}

		[Test]
		public void TestWritePriorityOverReadAsync()
		{
			var l = new AsyncReaderWriterLock();
			for (var i = 0; i < 2; i++)
			{
				var writeReleaser = l.WriteLock();
				Assert.That(l.AcquiredWriteLock, Is.True);

				var readReleaserTask = l.ReadLockAsync();
				AssertEqualValue(() => l.ReadersWaiting, 1, readReleaserTask);

				var writeReleaserTask = l.WriteLockAsync();
				AssertEqualValue(() => l.WritersWaiting, 1, writeReleaserTask);

				writeReleaser.Dispose();
				AssertEqualValue(() => l.WritersWaiting, 0, writeReleaserTask);
				AssertEqualValue(() => l.ReadersWaiting, 1, readReleaserTask);
				AssertTaskCompleted(writeReleaserTask);

				writeReleaserTask.Result.Dispose();
				AssertEqualValue(() => l.ReadersWaiting, 0, readReleaserTask);
				AssertTaskCompleted(readReleaserTask);

				readReleaserTask.Result.Dispose();
			}
		}

		[Test]
		public void TestPartialReleasingReadLockAsync()
		{
			var l = new AsyncReaderWriterLock();
			var readReleaserTask = l.ReadLockAsync();
			AssertEqualValue(() => l.CurrentReaders, 1, readReleaserTask);
			AssertTaskCompleted(readReleaserTask);

			var readReleaserTask2 = l.ReadLockAsync();
			AssertEqualValue(() => l.CurrentReaders, 2, readReleaserTask);
			AssertTaskCompleted(readReleaserTask2);

			var writeReleaserTask = l.WriteLockAsync();
			AssertEqualValue(() => l.AcquiredWriteLock, true, writeReleaserTask);
			AssertEqualValue(() => l.Writing, false, writeReleaserTask);
			Assert.That(writeReleaserTask.IsCompleted, Is.False);

			var readReleaserTask3 = l.ReadLockAsync();
			AssertEqualValue(() => l.ReadersWaiting, 1, readReleaserTask3);
			Assert.That(readReleaserTask3.IsCompleted, Is.False);

			readReleaserTask.Result.Dispose();
			Assert.That(writeReleaserTask.IsCompleted, Is.False);
			Assert.That(readReleaserTask3.IsCompleted, Is.False);

			readReleaserTask2.Result.Dispose();
			AssertEqualValue(() => l.Writing, true, writeReleaserTask);
			AssertEqualValue(() => l.ReadersWaiting, 1, readReleaserTask3);
			AssertTaskCompleted(writeReleaserTask);
			Assert.That(readReleaserTask3.IsCompleted, Is.False);

			writeReleaserTask.Result.Dispose();
			AssertEqualValue(() => l.ReadersWaiting, 0, readReleaserTask3);
			AssertTaskCompleted(readReleaserTask3);
		}

		[Test, Explicit]
		public async Task TestLoadSyncAndAsync()
		{
			var l = new AsyncReaderWriterLock();
			int readLockCount = 0, writeLockCount = 0;
			var incorrectLockCount = false;
			var tasks = new Task[20];
			var masterRandom = new Random();
			var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

			for (var i = 0; i < tasks.Length; i++)
			{
				// Ensure that each random has its own unique seed
				var random = new Random(masterRandom.Next());
				tasks[i] = i % 2 == 0
					? Task.Run(() => SyncLock(random, cancellationTokenSource.Token))
					: AsyncLock(random, cancellationTokenSource.Token);
			}

			await Task.WhenAll(tasks);
			Assert.That(incorrectLockCount, Is.False);


			void CheckLockCount()
			{
				var countIsCorrect = readLockCount == 0 && writeLockCount == 0 ||
				                     readLockCount > 0 && writeLockCount == 0 ||
				                     readLockCount == 0 && writeLockCount == 1;

				if (!countIsCorrect)
				{
					Volatile.Write(ref incorrectLockCount, true);
				}
			}

			void SyncLock(Random random, CancellationToken cancellationToken)
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					var isRead = random.Next(100) < 80;
					var releaser = isRead ? l.ReadLock() : l.WriteLock();
					lock (l)
					{
						if (isRead)
						{
							readLockCount++;
						}
						else
						{
							writeLockCount++;
						}
					}

					Thread.Sleep(10);

					lock (l)
					{
						releaser.Dispose();
						if (isRead)
						{
							readLockCount--;
						}
						else
						{
							writeLockCount--;
						}
					}
				}
			}

			async Task AsyncLock(Random random, CancellationToken cancellationToken)
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					var isRead = random.Next(100) < 80;
					var releaser = isRead
						? await l.ReadLockAsync().ConfigureAwait(false)
						: await l.WriteLockAsync().ConfigureAwait(false);
					lock (l)
					{
						if (isRead)
						{
							readLockCount++;
						}
						else
						{
							writeLockCount++;
						}

						CheckLockCount();
					}

					await Task.Delay(10).ConfigureAwait(false);

					lock (l)
					{
						releaser.Dispose();
						if (isRead)
						{
							readLockCount--;
						}
						else
						{
							writeLockCount--;
						}

						CheckLockCount();
					}
				}
			}
		}

		private static void AssertEqualValue<T>(Func<T> getValueFunc, T value, Task task = null, int waitDelay = 5000)
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
					Thread.Sleep(step);
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

		private static void AssertTaskCompleted(Task task)
		{
			AssertEqualValue(() => task.IsCompleted, true, task);
		}
	}
}
