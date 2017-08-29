using System.IO;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.Tools.hbm2ddl.SchemaExportTests
{
    public partial class ExportToFileFixture
    {
        [Test]
        public void ExportToFileUsingSetOutputFileAndCreate()
        {
            var configuration = TestConfigurationHelper.GetDefaultConfiguration();

            configuration.AddResource("NHibernate.Test.Tools.hbm2ddl.SchemaMetadataUpdaterTest.HeavyEntity.hbm.xml",
                                                                GetType().Assembly);
            
            var outputFileName = Path.GetTempFileName();
            var export = new SchemaExport(configuration);
            export.SetOutputFile(outputFileName);

            export.Create(false, false);

            Assert.IsTrue(File.Exists(outputFileName));
            Assert.IsTrue(new FileInfo(outputFileName).Length > 0);
        }

        [Test]
        public void ExportToFileUsingExecute()
        {
            var configuration = TestConfigurationHelper.GetDefaultConfiguration();

            configuration.AddResource("NHibernate.Test.Tools.hbm2ddl.SchemaMetadataUpdaterTest.HeavyEntity.hbm.xml",
                                                                GetType().Assembly);
            
            var outputFileName = Path.GetTempFileName();
            var export = new SchemaExport(configuration);

            export.Execute(null, false, false, new StreamWriter(outputFileName));

            Assert.IsTrue(File.Exists(outputFileName));
            Assert.IsTrue(new FileInfo(outputFileName).Length > 0);
        }
    }
}
