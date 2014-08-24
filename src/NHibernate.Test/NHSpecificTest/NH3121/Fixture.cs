using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using NHibernate.Dialect;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3121
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect; // All MS dialects.
		}

		// Some notes:
		// Mappings for all three properties use either unspecified length (defaulting to 8000 bytes)
		// or a length specified to a value smaller than 8001 bytes. This is since for larger values
		// the driver will increase the parameter size to int.MaxValue/2.

		[Test]
		public void ShouldThrowWhenByteArrayTooLong()
		{
			// For SQL Server only the SqlClientDriver sets parameter lengths
			// even when there is no length specified in the mapping. The ODBC
			// driver won't cause the truncation issue and hence not the exception.
			if (!(sessions.ConnectionProvider.Driver is SqlClientDriver))
				Assert.Ignore("Test limited to drivers that sets parameter length even with no length specified in the mapping.");

			const int reportSize = 17158;
			var random = new Random();

			var reportImage = new Byte[reportSize];
			random.NextBytes(reportImage);

			var report = new Report { UnsizedArray = reportImage };

			var ex = Assert.Throws<PropertyValueException>(() => PersistReport(report));

			Assert.That(ex.Message, Is.StringContaining("Report.UnsizedArray"));
			Assert.That(ex.InnerException, Is.TypeOf<HibernateException>());
			Assert.That(ex.InnerException.Message,
						Is.EqualTo("The length of the byte[] value exceeds the length configured in the mapping/parameter."));
		}


		[Test]
		public void ShouldThrowWhenImageTooLarge()
		{
			Assembly assembly = Assembly.Load(MappingsAssembly);
			var stream = assembly.GetManifestResourceStream("NHibernate.Test.NHSpecificTest.NH2484.food-photo.jpg");
			var image = Bitmap.FromStream(stream);

			var report = new Report { Image = image };

			var ex = Assert.Throws<PropertyValueException>(() => PersistReport(report));

			Assert.That(ex.Message, Is.StringContaining("Report.Image"));
			Assert.That(ex.InnerException, Is.TypeOf<HibernateException>());
			Assert.That(ex.InnerException.Message,
						Is.EqualTo("The length of the byte[] value exceeds the length configured in the mapping/parameter."));
		}


		[Test]
		public void ShouldThrowWhenImageAsISerializableTooLarge()
		{
			Assembly assembly = Assembly.Load(MappingsAssembly);
			var stream = assembly.GetManifestResourceStream("NHibernate.Test.NHSpecificTest.NH2484.food-photo.jpg");
			var image = Bitmap.FromStream(stream);

			var report = new Report { SerializableImage = image };

			var ex = Assert.Throws<PropertyValueException>(() => PersistReport(report));

			Assert.That(ex.Message, Is.StringContaining("Report.SerializableImage"));
			Assert.That(ex.InnerException, Is.TypeOf<HibernateException>());
			Assert.That(ex.InnerException.Message,
						Is.EqualTo("The length of the byte[] value exceeds the length configured in the mapping/parameter."));
		}


		private void PersistReport(Report report)
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				session.Save(report);
				session.Flush();
				// No commit to avoid DB pollution (test success means we should throw and never insert anyway).
			}
		}
	}
}
