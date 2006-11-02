using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace NHibernate.Tool.hbm2net.Tests
{
	[TestFixture]
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

		private static string FilterRuntimeVersion(string contents)
		{
			string result = Regex.Replace(contents, "^//     Runtime Version:.*$", "", RegexOptions.Multiline);
			return result;
		}

		private static void AssertFile()
		{
			Assert.IsTrue(File.Exists(ExpectedFileName));
			using(StreamReader sr = File.OpenText(ExpectedFileName))
			{
				string filteredExpected = FilterRuntimeVersion(ResourceHelper.GetResource(ExpectedFileResourceName));
				string filteredActual = FilterRuntimeVersion(sr.ReadToEnd());
				Assert.AreEqual(filteredExpected, filteredActual);
			}
		}

		/// <summary>
		///  <p>Test when the mapping file resides in same folder as hbm2net put the full path is <b>not</b> supplied.</p>
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
		///  <p>Test when the mapping file resides in same folder as hbm2net put the full path is supplied.</p>
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
		///  <p>Test when the mapping file doesn't reside in the same directory as hbm2net.</p>
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
