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

		[SetUp]
		public void Init() {}

		[TearDown]
		public void Destroy()
		{
			if (TestHelper.DefaultOutputDirectory.Exists) TestHelper.DefaultOutputDirectory.Delete(true);
			if (mappingFile != null && mappingFile.Exists) mappingFile.Delete();
		}


		private static void AssertFile()
		{
			string expectedFileName = Path.Combine(TestHelper.DefaultOutputDirectory.FullName, @"NHibernate\DomainModel\Simple, NHibernate\DomainModel.cs");
			Assert.IsTrue(File.Exists(expectedFileName));
			using(StreamReader sr = File.OpenText(expectedFileName))
			{
				Assert.AreEqual(ResourceHelper.GetResource(ExpectedFileResourceName), sr.ReadToEnd());
			}
		}

		/// <summary>
		///  <para>Test the mapping file resides in current directory but the full path is <b>not</b> supplied.</para>
		/// </summary>
		[Test]
		public void MappingFileNoPathSameFolderAsCurrentDirectory()
		{
			Environment.CurrentDirectory = Path.GetTempPath();
			mappingFile = new FileInfo(Path.Combine(Environment.CurrentDirectory,"Simple.hbm.xml"));	
			ResourceHelper.WriteToFileFromResource(mappingFile, MappingFileResourceName);
			string[] args = new string[] {mappingFile.Name};
			CodeGenerator.Main(args);
			AssertFile();
		}

		/// <summary>
		///  <para>Test when the mapping file resides in same folder as current directory but the full path is supplied.</para>
		/// </summary>
		[Test]
		public void MappingFileInSameFolderAsCurrentDirectory()
		{
			mappingFile = new FileInfo("Simple.hbm.xml");
			Assert.AreEqual(Environment.CurrentDirectory, mappingFile.DirectoryName);
			ResourceHelper.WriteToFileFromResource(mappingFile, MappingFileResourceName);
			string[] args = new string[] {mappingFile.FullName};
			CodeGenerator.Main(args);	
			AssertFile();
		}
		
		[Test, ExpectedException(typeof(System.IO.FileNotFoundException))]
		public void MappingFileDoesNotExist()
		{
			string[] args = new string[] {"non-existant-file.hbm.xml"};
			CodeGenerator.Main(args);			
		}

		/// <summary>
		///  <para>Test when the mapping file doesn't reside in the same directory as hbm2net.</para>
		/// </summary>
		[Test]
		public void MappingFileInDifferentFolderThanCurrentDirectory()
		{ 
			mappingFile = ResourceHelper.CreateFileFromResource(MappingFileResourceName);
			string[] args = new string[] {mappingFile.FullName};
			CodeGenerator.Main(args);	
			AssertFile();
		}
	}
}
