using System;
using System.IO;
using NUnit.Framework;

namespace NHibernate.Tool.hbm2net.Tests
{
	[TestFixture, Category("Functional Tests")]
	public class SimpleMappingFileConfigFileNoOutputArgsTests 
	{
		private const string VelocityRenderer = "NHibernate.Tool.hbm2net.VelocityRenderer";

		FileInfo mappingFile;
		FileInfo configFile;
		FileInfo templateFile;
		const string MappingFileResourceName = "Simple.hbm.xml";
		const string ExpectedFileResourceName = "DomainModel.csharp";

// TODO: Need to move this into method as it will depend on the supplied options in the config file e.g. the
// supplied package and renderer.
		static string ExpectedFileName = Path.Combine(TestHelper.DefaultOutputDirectory.FullName, @"DomainModel.cs");

		[SetUp]
		public void Init() 
		{
		
		}

		[TearDown]
		public void Destroy()
		{
			if (TestHelper.DefaultOutputDirectory.Exists) TestHelper.DefaultOutputDirectory.Delete(true);
			if (mappingFile.Exists) mappingFile.Delete();
			if (templateFile != null && templateFile.Exists) templateFile.Delete();
			if (configFile.Exists) configFile.Delete();
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
		///  <para>Test that the template and mapping file path is relative to the location of the 
		///  hbm2net library.</para>
		/// </summary>
		/// <remarks>
		///  <para>Using to resolve NH-242.</para>
		/// </remarks>
		[Test]
		public void TemplateFileRelativeToHbm2NetDll()
		{			
			configFile = new FileInfo(Path.GetTempFileName());

			// the template file needs to be written to the same 
			// directory as the hbm2net.dll assembly for this test	
			string dir = new FileInfo(System.Reflection.Assembly.GetAssembly(typeof(CodeGenerator)).Location).DirectoryName;

			templateFile = new FileInfo(Path.Combine(dir, "convert.vm")); 
			ResourceHelper.WriteToFileFromResource(templateFile, "convert.vm");
			
			mappingFile = new FileInfo(Path.Combine(configFile.DirectoryName, MappingFileResourceName));
			ResourceHelper.WriteToFileFromResource(mappingFile, MappingFileResourceName);
			
			TestHelper.CreateConfigFile(configFile, templateFile.Name, VelocityRenderer, "");	

			// ensure that test is setup correctly
			Assert.IsTrue(configFile.Exists && configFile.Length != 0);
			Assert.IsTrue(templateFile.Exists && templateFile.Length != 0);
			Assert.IsTrue(mappingFile.Exists && mappingFile.Length != 0);
			Assert.AreEqual(templateFile.DirectoryName, dir);
			Assert.AreEqual(mappingFile.DirectoryName, configFile.DirectoryName);

			string[] args = new string[] {"--config=" + configFile.FullName, mappingFile.FullName};
			CodeGenerator.Main(args);
			AssertFile();
		}

		/// <summary>
		///  <para>Test providing a config file without an template parameter.</para>
		/// </summary>
		/// <remarks>
		///  <para>The default template convert.vm should be used to generate the source file.</para>
		/// </remarks>
		[Test]
		public void NoTemplateFileParam()
		{
			configFile = new FileInfo(Path.GetTempFileName());

			// the mapping file needs to be written to the same 
			// directory as the config file for this test			
			
			mappingFile = new FileInfo(Path.Combine(configFile.DirectoryName, MappingFileResourceName));
			ResourceHelper.WriteToFileFromResource(mappingFile, MappingFileResourceName);
			
			TestHelper.CreateConfigFile(configFile, null, VelocityRenderer, "");	

			// ensure that test is setup correctly
			Assert.IsTrue(configFile.Exists && configFile.Length != 0);
			Assert.IsTrue(mappingFile.Exists && mappingFile.Length != 0);
			Assert.AreEqual(mappingFile.DirectoryName, configFile.DirectoryName);

			string[] args = new string[] {"--config=" + configFile.FullName, mappingFile.FullName};
			CodeGenerator.Main(args);
			AssertFile();			
		}

	}
}
