using System;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	public partial class AsyncReaderWriterLockFixture
	{
		private readonly int _delay = 50;

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
				Assert.That(readReleaserTask.Wait(_delay), Is.True);

				var writeReleaserTask = Task.Run(() => l.WriteLock());
				AssertEqualValue(() => l.AcquiredWriteLock, true, writeReleaserTask);
				AssertEqualValue(() => l.Writing, false, writeReleaserTask);
				Assert.That(writeReleaserTask.Wait(_delay), Is.False);

				readReleaser.Dispose();
				Assert.That(l.CurrentReaders, Is.EqualTo(1));
				Assert.That(writeReleaserTask.Wait(_delay), Is.False);

				readReleaserTask.Result.Dispose();
				Assert.That(l.CurrentReaders, Is.EqualTo(0));
				AssertEqualValue(() => l.Writing, true, writeReleaserTask);
				Assert.That(writeReleaserTask.Wait(_delay), Is.True);

				readReleaserTask = Task.Run(() => l.ReadLock());
				AssertEqualValue(() => l.ReadersWaiting, 1, readReleaserTask);
				Assert.That(readReleaserTask.Wait(_delay), Is.False);

				var writeReleaserTask2 = Task.Run(() => l.WriteLock());
				AssertEqualValue(() => l.WritersWaiting, 1, writeReleaserTask2);
				Assert.That(writeReleaserTask2.Wait(_delay), Is.False);

				writeReleaserTask.Result.Dispose();
				AssertEqualValue(() => l.WritersWaiting, 0, writeReleaserTask2);
				AssertEqualValue(() => l.Writing, true, writeReleaserTask2);
				Assert.That(readReleaserTask.Wait(_delay), Is.False);
				Assert.That(writeReleaserTask2.Wait(_delay), Is.True);

				writeReleaserTask2.Result.Dispose();
				AssertEqualValue(() => l.Writing, false, writeReleaserTask2);
				AssertEqualValue(() => l.ReadersWaiting, 0, readReleaserTask);
				AssertEqualValue(() => l.CurrentReaders, 1, readReleaserTask);
				Assert.That(readReleaserTask.Wait(_delay), Is.True);

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
				Assert.That(readReleaserTask.Wait(_delay), Is.True);

				var readReleaserTask2 = l.ReadLockAsync();
				AssertEqualValue(() => l.CurrentReaders, 2, readReleaserTask2);
				Assert.That(readReleaserTask2.Wait(_delay), Is.True);

				var writeReleaserTask = l.WriteLockAsync();
				AssertEqualValue(() => l.AcquiredWriteLock, true, writeReleaserTask);
				AssertEqualValue(() => l.Writing, false, writeReleaserTask);
				Assert.That(writeReleaserTask.Wait(_delay), Is.False);

				readReleaserTask.Result.Dispose();
				Assert.That(l.CurrentReaders, Is.EqualTo(1));
				Assert.That(writeReleaserTask.Wait(_delay), Is.False);

				readReleaserTask2.Result.Dispose();
				Assert.That(l.CurrentReaders, Is.EqualTo(0));
				AssertEqualValue(() => l.Writing, true, writeReleaserTask);
				Assert.That(writeReleaserTask.Wait(_delay), Is.True);

				readReleaserTask = l.ReadLockAsync();
				AssertEqualValue(() => l.ReadersWaiting, 1, readReleaserTask);
				Assert.That(readReleaserTask.Wait(_delay), Is.False);

				var writeReleaserTask2 = l.WriteLockAsync();
				AssertEqualValue(() => l.WritersWaiting, 1, writeReleaserTask2);
				Assert.That(writeReleaserTask2.Wait(_delay), Is.False);

				writeReleaserTask.Result.Dispose();
				AssertEqualValue(() => l.WritersWaiting, 0, writeReleaserTask2);
				AssertEqualValue(() => l.Writing, true, writeReleaserTask2);
				Assert.That(readReleaserTask.Wait(_delay), Is.False);
				Assert.That(writeReleaserTask2.Wait(_delay), Is.True);

				writeReleaserTask2.Result.Dispose();
				AssertEqualValue(() => l.Writing, false, writeReleaserTask2);
				AssertEqualValue(() => l.ReadersWaiting, 0, readReleaserTask);
				AssertEqualValue(() => l.CurrentReaders, 1, readReleaserTask);
				Assert.That(readReleaserTask.Wait(_delay), Is.True);

				readReleaserTask.Result.Dispose();
				Assert.That(l.ReadersWaiting, Is.EqualTo(0));
				Assert.That(l.WritersWaiting, Is.EqualTo(0));
				Assert.That(l.CurrentReaders, Is.EqualTo(0));
				Assert.That(l.Writing, Is.False);
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
			Assert.That(readReleaserTask.Wait(_delay), Is.True);

			readReleaser.Dispose();
			Assert.That(l.CurrentReaders, Is.EqualTo(1));

			readReleaserTask.Result.Dispose();
			Assert.That(l.CurrentReaders, Is.EqualTo(0));

			var writeReleaser = l.WriteLock();
			Assert.That(l.AcquiredWriteLock, Is.True);

			var writeReleaserTask = l.WriteLockAsync();
			AssertEqualValue(() => l.WritersWaiting, 1, writeReleaserTask);
			Assert.That(writeReleaserTask.Wait(_delay), Is.False);

			readReleaserTask = Task.Run(() => l.ReadLock());
			AssertEqualValue(() => l.ReadersWaiting, 1, readReleaserTask);
			Assert.That(readReleaserTask.Wait(_delay), Is.False);

			var readReleaserTask2 = l.ReadLockAsync();
			AssertEqualValue(() => l.ReadersWaiting, 2, readReleaserTask2);
			Assert.That(readReleaserTask2.Wait(_delay), Is.False);

			writeReleaser.Dispose();
			AssertEqualValue(() => l.WritersWaiting, 0, writeReleaserTask);
			AssertEqualValue(() => l.Writing, true, writeReleaserTask);
			Assert.That(writeReleaserTask.Wait(_delay), Is.True);
			Assert.That(readReleaserTask.Wait(_delay), Is.False);
			Assert.That(readReleaserTask2.Wait(_delay), Is.False);

			writeReleaserTask.Result.Dispose();
			AssertEqualValue(() => l.CurrentReaders, 2, readReleaserTask);
			AssertEqualValue(() => l.ReadersWaiting, 0, readReleaserTask2);
			Assert.That(readReleaserTask.Wait(_delay), Is.True);
			Assert.That(readReleaserTask2.Wait(_delay), Is.True);
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
				Assert.That(writeReleaserTask.Wait(_delay), Is.True);

				writeReleaserTask.Result.Dispose();
				AssertEqualValue(() => l.ReadersWaiting, 0, readReleaserTask);
				Assert.That(readReleaserTask.Wait(_delay), Is.True);

				readReleaserTask.Result.Dispose();
			}
		}

		[Test]
		public void TestPartialReleasingReadLockAsync()
		{
			var l = new AsyncReaderWriterLock();
			var readReleaserTask = l.ReadLockAsync();
			AssertEqualValue(() => l.CurrentReaders, 1, readReleaserTask);
			Assert.That(readReleaserTask.Wait(_delay), Is.True);

			var readReleaserTask2 = l.ReadLockAsync();
			AssertEqualValue(() => l.CurrentReaders, 2, readReleaserTask);
			Assert.That(readReleaserTask2.Wait(_delay), Is.True);

			var writeReleaserTask = l.WriteLockAsync();
			AssertEqualValue(() => l.AcquiredWriteLock, true, writeReleaserTask);
			AssertEqualValue(() => l.Writing, false, writeReleaserTask);
			Assert.That(writeReleaserTask.Wait(_delay), Is.False);

			var readReleaserTask3 = l.ReadLockAsync();
			AssertEqualValue(() => l.ReadersWaiting, 1, readReleaserTask3);
			Assert.That(readReleaserTask3.Wait(_delay), Is.False);

			readReleaserTask.Result.Dispose();
			Assert.That(writeReleaserTask.Wait(_delay), Is.False);
			Assert.That(readReleaserTask3.Wait(_delay), Is.False);

			readReleaserTask2.Result.Dispose();
			AssertEqualValue(() => l.Writing, true, writeReleaserTask);
			AssertEqualValue(() => l.ReadersWaiting, 1, readReleaserTask3);
			Assert.That(writeReleaserTask.Wait(_delay), Is.True);
			Assert.That(readReleaserTask3.Wait(_delay), Is.False);

			writeReleaserTask.Result.Dispose();
			AssertEqualValue(() => l.ReadersWaiting, 0, readReleaserTask3);
			Assert.That(readReleaserTask3.Wait(_delay), Is.True);
		}

		[Test, Explicit]
		public async Task TestLoadSyncAndAsync()
		{
			var l = new AsyncReaderWriterLock();
			int readLockCount = 0, writeLockCount = 0;
			var incorrectLockCount = false;
			var threads = new Thread[10];
			var tasks = new Task[10];
			var masterRandom = new Random();
			var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

			for (var i = 0; i < threads.Length; i++)
			{
				// Ensure that each random has its own unique seed
				var random = new Random(masterRandom.Next());
				threads[i] = new Thread(() => SyncLock(random, cancellationTokenSource.Token));
				threads[i].Start();
			}

			for (var i = 0; i < tasks.Length; i++)
			{
				// Ensure that each random has its own unique seed
				var random = new Random(masterRandom.Next());
				tasks[i] = Task.Run(() => AsyncLock(random, cancellationTokenSource.Token));
			}

			foreach (var thread in threads)
			{
				thread.Join();
			}

			await Task.WhenAll(tasks).ConfigureAwait(false);
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

		private static void AssertEqualValue<T>(Func<T> getValueFunc, T value, Task task, int waitDelay = 500)
		{
			var currentTime = 0;
			const int step = 5;
			while (currentTime < waitDelay)
			{
				task.Wait(step);
				currentTime += step;
				if (getValueFunc().Equals(value))
				{
					return;
				}
			}

			Assert.That(getValueFunc(), Is.EqualTo(value));
		}
	}
}
