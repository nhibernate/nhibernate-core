using System;
using System.IO;
using NUnit.Framework;

namespace NHibernate.Tool.hbm2net.Tests
{
	[TestFixture, Category("Functional Tests")]
	public class SimpleMappingFileNoConfigNoOutputArgsTests 
	{
		FileInfo mappingFile;
		const string MappingFileResourceName = "Simple.hbm.xml";
		const string ExpectedFileResourceName = "DomainModel.csharp";
		static string ExpectedFileName = Path.Combine(TestHelper.DefaultOutputDirectory.FullName, @"NHibernate\DomainModel\Simple, NHibernate\DomainModel.cs");
		

		[SetUp]
		public void Init() {}

		[TearDown]
		public void Destroy()
		{
			if (TestHelper.DefaultOutputDirectory.Exists) TestHelper.DefaultOutputDirectory.Delete(true);
			if (mappingFile.Exists) mappingFile.Delete();
		}


		private static void AssertFile()
		{
			Assert.IsTrue(File.Exists(ExpectedFileName));
			using(StreamReader sr = File.OpenText(ExpectedFileName))
			{
				Assert.AreEqual(ResourceHelper.GetResource(ExpectedFileResourceName), sr.ReadToEnd());
			}
		}

		/// <summary>
		///  <para>Test when the mapping file resides in same folder as hbm2net but the full path is <b>not</b> supplied.</para>
		/// </summary>
		[Test]
		public void MappingFileNoPathSameFolderAsHbm2Net()
		{
			mappingFile = new FileInfo("Simple.hbm.xml");	
			ResourceHelper.WriteToFileFromResource(mappingFile, MappingFileResourceName);
			string[] args = new string[] {mappingFile.Name};
			CodeGenerator.Main(args);
			AssertFile();
		}

		/// <summary>
		///  <para>Test when the mapping file resides in same folder as hbm2net but the full path is supplied.</para>
		/// </summary>
		[Test]
		public void MappingFileInSameFolderAsHbm2Net()
		{
			mappingFile = new FileInfo("Simple.hbm.xml");	
			ResourceHelper.WriteToFileFromResource(mappingFile, MappingFileResourceName);
			string[] args = new string[] {mappingFile.FullName};
			CodeGenerator.Main(args);	
			AssertFile();
		}

		/// <summary>
		///  <para>Test when the mapping file doesn't reside in the same directory as hbm2net.</para>
		/// </summary>
		[Test]
		public void MappingFileInDifferentFolderThanHbm2Net()
		{ 
			mappingFile = ResourceHelper.CreateFileFromResource(MappingFileResourceName);
			string[] args = new string[] {mappingFile.FullName};
			CodeGenerator.Main(args);	
			AssertFile();
		}
	}
}
