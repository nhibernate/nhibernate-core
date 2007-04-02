using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using NHibernate.Search.Storage;
using NUnit.Framework;

namespace NHibernate.Search.Tests
{
	[TestFixture]
	public class FileHleperTestCase
	{
		[SetUp]
		public void SetUp()
		{
			DirectoryInfo dir = new DirectoryInfo("./filehelpersrc");
			dir.Create();
			WriteFile(dir, "a");
			WriteFile(dir, "b");
			dir = new DirectoryInfo(Path.Combine(dir.FullName, "subdir"));
			dir.Create();
			WriteFile(dir, "c");
		}

		private void WriteFile(DirectoryInfo dir, String name)
		{
			FileInfo a = new FileInfo(Path.Combine(dir.FullName, name));
			StreamWriter os = a.CreateText();
			os.WriteLine(1);
			os.WriteLine(2);
			os.WriteLine(3);
			os.Flush();
			os.Close();
		}

		[TearDown]
		protected void TearDown()
		{
			DirectoryInfo dir = new DirectoryInfo("./filehelpersrc");
			dir.Delete(true);
			dir = new DirectoryInfo("./filehelperdest");
			dir.Delete(true);
		}

		[Test]
		public void Synchronize()
		{
			DirectoryInfo src = new DirectoryInfo("./filehelpersrc");
			DirectoryInfo dest = new DirectoryInfo("./filehelperdest");
			FileHelper.Synchronize(src, dest, true);
			Assert.IsTrue(File.Exists(Path.Combine(dest.FullName, "b")));

			string path = Path.Combine(dest.FullName, Path.Combine("subdir", "c"));
			Assert.IsTrue(File.Exists(path));

			//change
			Thread.Sleep(2*2000);
			StreamWriter os = File.CreateText(Path.Combine(src.FullName, "c"));
			os.WriteLine(1);
			os.WriteLine(2);
			os.WriteLine(3);
			os.Flush();
			os.Close();
			FileInfo test = new FileInfo(Path.Combine(src.FullName, "c"));
			FileInfo destTest = new FileInfo(Path.Combine(dest.FullName, "c"));
			Assert.AreNotSame(test.LastWriteTime, destTest.LastWriteTime);
			FileHelper.Synchronize(src, dest, true);
			destTest.Refresh();
			Assert.AreEqual(test.LastWriteTime, destTest.LastWriteTime);
			Assert.AreEqual(test.Length, destTest.Length);

			//delete file
			test.Delete();
			FileHelper.Synchronize(src, dest, true);
			destTest.Refresh();
			Assert.IsTrue(! destTest.Exists);
		}
	}
}