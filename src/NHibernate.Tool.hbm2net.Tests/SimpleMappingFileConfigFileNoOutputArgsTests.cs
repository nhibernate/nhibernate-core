using System;
using System.IO;

using NUnit.Framework;

namespace NHibernate.Tool.hbm2net.Tests
{
	/// <summary>
	///  <para>Test Fixture on using a configuration file with hbm2net and with no output
	///  arguments specified.</para>
	/// </summary>
	[TestFixture, Category("Functional Tests")]
	public class SimpleMappingFileConfigFileNoOutputArgsTests
	{
		private const string VelocityRenderer = "NHibernate.Tool.hbm2net.VelocityRenderer";

		private FileInfo mappingFile;
		private FileInfo configFile;
		private FileInfo templateFile;
		private const string MappingFileResourceName = "Simple.hbm.xml";
		private const string ExpectedFileResourceName = "Simple.csharp";
		private const string TemplateFileResourceName = "convert.vm";

// TODO: Need to move this into method as it will depend on the supplied options in the config file e.g. the
// supplied package and renderer.
		private static string ExpectedFileName = Path.Combine(TestHelper.DefaultOutputDirectory.FullName, @"Simple.cs");

		[SetUp]
		public void Init()
		{
		}

		[TearDown]
		public void Destroy()
		{
			if (TestHelper.DefaultOutputDirectory.Exists) TestHelper.DefaultOutputDirectory.Delete(true);
			if (mappingFile != null && mappingFile.Exists) mappingFile.Delete();
			if (templateFile != null && templateFile.Exists) templateFile.Delete();
			if (configFile != null && configFile.Exists) configFile.Delete();
		}

		private static void AssertFile()
		{
			Assert.IsTrue(File.Exists(ExpectedFileName), "File not found: {0}", ExpectedFileName);
			using (StreamReader sr = File.OpenText(ExpectedFileName))
			{
				Assert.AreEqual(ResourceHelper.GetResource(ExpectedFileResourceName), sr.ReadToEnd());
			}
		}

		/// <summary>
		///  <para>Test that the template and mapping file path is relative to the location of the 
		///  config file.</para>
		/// </summary>
		/// <remarks>
		///  <para>Using to resolve NH-242.</para>
		/// </remarks>
		[Test]
		public void TemplateFileRelativeToConfig()
		{
			configFile = new FileInfo(Path.GetTempFileName());

			// the template file needs to be written to the same 
			// directory as the config file.	
			templateFile = new FileInfo(Path.Combine(configFile.DirectoryName, "convert.vm"));
			ResourceHelper.WriteToFileFromResource(templateFile, TemplateFileResourceName);

			mappingFile = new FileInfo(Path.Combine(configFile.DirectoryName, MappingFileResourceName));
			ResourceHelper.WriteToFileFromResource(mappingFile, MappingFileResourceName);

			TestHelper.CreateConfigFile(configFile, templateFile.Name, VelocityRenderer, "");

			// ensure that test is setup correctly
			Assert.IsTrue(configFile.Exists && configFile.Length != 0);
			Assert.IsTrue(templateFile.Exists && templateFile.Length != 0);
			Assert.IsTrue(mappingFile.Exists && mappingFile.Length != 0);
			Assert.AreEqual(templateFile.DirectoryName, configFile.DirectoryName);
			Assert.AreEqual(mappingFile.DirectoryName, configFile.DirectoryName);

			string[] args = new string[] {"--config=" + configFile.FullName, mappingFile.FullName};
			CodeGenerator.Main(args);
			AssertFile();
		}

		public void TemplateFileAbsolutePath()
		{
			configFile = new FileInfo(Path.GetTempFileName());

			templateFile = new FileInfo(Path.GetTempFileName());
			ResourceHelper.WriteToFileFromResource(templateFile, TemplateFileResourceName);

			mappingFile = new FileInfo(Path.Combine(configFile.DirectoryName, MappingFileResourceName));
			ResourceHelper.WriteToFileFromResource(mappingFile, MappingFileResourceName);

			TestHelper.CreateConfigFile(configFile, templateFile.FullName, VelocityRenderer, "");

			// ensure that test is setup correctly
			Assert.IsTrue(configFile.Exists && configFile.Length != 0);
			Assert.IsTrue(templateFile.Exists && templateFile.Length != 0);
			Assert.IsTrue(mappingFile.Exists && mappingFile.Length != 0);
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

		/// <summary>
		///  <para>Test that <see cref="IOException"/> gets thrown when the NVelocity template
		///  supplied in the config.xml does not exist.</para>
		/// </summary>
		[Test, ExpectedException(typeof(IOException))]
		public void TemplateFileDoesNotExist()
		{
			configFile = new FileInfo(Path.GetTempFileName());

			templateFile = new FileInfo("non-existant-file.vm");

			mappingFile = new FileInfo(Path.Combine(configFile.DirectoryName, MappingFileResourceName));
			ResourceHelper.WriteToFileFromResource(mappingFile, MappingFileResourceName);

			TestHelper.CreateConfigFile(configFile, templateFile.Name, VelocityRenderer, "");

			// ensure that test is setup correctly
			Assert.IsTrue(configFile.Exists && configFile.Length != 0);
			Assert.IsFalse(templateFile.Exists);
			Assert.IsTrue(mappingFile.Exists && mappingFile.Length != 0);
			Assert.AreEqual(mappingFile.DirectoryName, configFile.DirectoryName);

			string[] args = new string[] {"--config=" + configFile.FullName, mappingFile.FullName};
			CodeGenerator.Main(args);
			AssertFile();
		}
	}
}