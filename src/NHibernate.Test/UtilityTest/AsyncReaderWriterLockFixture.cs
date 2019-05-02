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
			for (var i = 0; i < 10; i++)
			{
				var readReleaser = l.ReadLock();
				var readReleaserTask = Task.Run(() => l.ReadLock());
				Assert.That(readReleaserTask.Wait(_delay), Is.True);

				var writeReleaserTask = Task.Run(() => l.WriteLock());
				Assert.That(writeReleaserTask.Wait(_delay), Is.False);

				readReleaser.Dispose();
				Assert.That(writeReleaserTask.Wait(_delay), Is.False);

				readReleaserTask.Result.Dispose();
				Assert.That(writeReleaserTask.Wait(_delay), Is.True);

				readReleaserTask = Task.Run(() => l.ReadLock());
				Assert.That(readReleaserTask.Wait(_delay), Is.False);

				var writeReleaserTask2 = Task.Run(() => l.WriteLock());
				Assert.That(writeReleaserTask2.Wait(_delay), Is.False);

				writeReleaserTask.Result.Dispose();
				Assert.That(readReleaserTask.Wait(_delay), Is.False);
				Assert.That(writeReleaserTask2.Wait(_delay), Is.True);

				writeReleaserTask2.Result.Dispose();
				Assert.That(readReleaserTask.Wait(_delay), Is.True);
				readReleaserTask.Result.Dispose();
			}
		}

		[Test]
		public void TestBlockingAsync()
		{
			var l = new AsyncReaderWriterLock();
			for (var i = 0; i < 10; i++)
			{
				var readReleaserTask = l.ReadLockAsync();
				var readReleaserTask2 = l.ReadLockAsync();
				Assert.That(readReleaserTask.Wait(_delay), Is.True);
				Assert.That(readReleaserTask2.Wait(_delay), Is.True);

				var writeReleaserTask = l.WriteLockAsync();
				Assert.That(writeReleaserTask.Wait(_delay), Is.False);

				readReleaserTask.Result.Dispose();
				Assert.That(writeReleaserTask.Wait(_delay), Is.False);

				readReleaserTask2.Result.Dispose();
				Assert.That(writeReleaserTask.Wait(_delay), Is.True);

				readReleaserTask = l.ReadLockAsync();
				Assert.That(readReleaserTask.Wait(_delay), Is.False);

				var writeReleaserTask2 = l.WriteLockAsync();
				Assert.That(writeReleaserTask2.Wait(_delay), Is.False);

				writeReleaserTask.Result.Dispose();
				Assert.That(readReleaserTask.Wait(_delay), Is.False);
				Assert.That(writeReleaserTask2.Wait(_delay), Is.True);

				writeReleaserTask2.Result.Dispose();
				Assert.That(readReleaserTask.Wait(_delay), Is.True);
				readReleaserTask.Result.Dispose();
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
			var readReleaserTask = l.ReadLockAsync();
			Assert.That(readReleaserTask.Wait(_delay), Is.True);

			readReleaser.Dispose();
			readReleaserTask.Result.Dispose();

			var writeReleaser = l.WriteLock();
			var writeReleaserTask = l.WriteLockAsync();
			Assert.That(writeReleaserTask.Wait(_delay), Is.False);

			readReleaserTask = Task.Run(() => l.ReadLock());
			var readReleaserTask2 = l.ReadLockAsync();
			Assert.That(readReleaserTask.Wait(_delay), Is.False);
			Assert.That(readReleaserTask2.Wait(_delay), Is.False);

			writeReleaser.Dispose();
			Assert.That(writeReleaserTask.Wait(_delay), Is.True);
			Assert.That(readReleaserTask.Wait(_delay), Is.False);
			Assert.That(readReleaserTask2.Wait(_delay), Is.False);

			writeReleaserTask.Result.Dispose();
			Assert.That(readReleaserTask.Wait(_delay), Is.True);
			Assert.That(readReleaserTask2.Wait(_delay), Is.True);
		}

		[Test]
		public void TestWritePriorityOverReadAsync()
		{
			var l = new AsyncReaderWriterLock();
			for (var i = 0; i < 10; i++)
			{
				var writeReleaser = l.WriteLock();
				var readReleaserTask = l.ReadLockAsync();
				var writeReleaserTask = l.WriteLockAsync();

				writeReleaser.Dispose();
				Assert.That(writeReleaserTask.Wait(_delay), Is.True);

				writeReleaserTask.Result.Dispose();
				Assert.That(readReleaserTask.Wait(_delay), Is.True);
				readReleaserTask.Result.Dispose();
			}
		}

		[Test]
		public void TestPartialReleasingReadLockAsync()
		{
			var l = new AsyncReaderWriterLock();
			var readReleaserTask = l.ReadLockAsync();
			var readReleaserTask2 = l.ReadLockAsync();
			Assert.That(readReleaserTask.Wait(_delay), Is.True);
			Assert.That(readReleaserTask2.Wait(_delay), Is.True);

			var writeReleaserTask = l.WriteLockAsync();
			Assert.That(writeReleaserTask.Wait(_delay), Is.False);

			var readReleaserTask3 = l.ReadLockAsync();
			Assert.That(readReleaserTask3.Wait(_delay), Is.False);

			readReleaserTask.Result.Dispose();
			Assert.That(writeReleaserTask.Wait(_delay), Is.False);
			Assert.That(readReleaserTask3.Wait(_delay), Is.False);

			readReleaserTask2.Result.Dispose();
			Assert.That(writeReleaserTask.Wait(_delay), Is.True);
			Assert.That(readReleaserTask3.Wait(_delay), Is.False);

			writeReleaserTask.Result.Dispose();
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
	}
}
