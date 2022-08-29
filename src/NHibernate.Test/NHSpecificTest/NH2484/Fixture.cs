using System;
using System.Drawing;
using System.Reflection;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2484
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return (dialect is Dialect.MsSql2008Dialect);
		}

		[Test]
		public void TestPersistenceOfClassWithUnknownSerializableType()
		{
			var stream = typeof(Fixture).Assembly.GetManifestResourceStream("NHibernate.Test.NHSpecificTest.NH2484.food-photo.jpg");
			var image = Image.FromStream(stream);

			var model = new ClassWithImage() { Image = image };
			var imageSize = model.Image.Size;
			int id = -1;

			using (ISession session = OpenSession())
			{
				session.SaveOrUpdate(model);
				session.Flush();
				id = model.Id;
				Assert.That(id, Is.GreaterThan(-1));
			}
			using (ISession session = OpenSession())
			{
				model = session.Get<ClassWithImage>(id);
				Assert.That(model.Image.Size, Is.EqualTo(imageSize)); // Ensure type is not truncated
			}
			using (ISession session = OpenSession())
			{
				session.CreateQuery("delete from ClassWithImage").ExecuteUpdate();
				session.Flush();
			}

			stream.Dispose();
		}

		[Test]
		public void TestPersistenceOfClassWithSerializableType()
		{
			var stream = typeof(Fixture).Assembly.GetManifestResourceStream("NHibernate.Test.NHSpecificTest.NH2484.food-photo.jpg");
			var image = Image.FromStream(stream);

			var model = new ClassWithSerializableType() { Image = image };
			var imageSize = ((Image) model.Image).Size;
			int id = -1;

			using (ISession session = OpenSession())
			{
				session.SaveOrUpdate(model);
				session.Flush();
				id = model.Id;
				Assert.That(id, Is.GreaterThan(-1));
			}
			using (ISession session = OpenSession())
			{
				model = session.Get<ClassWithSerializableType>(id);
				Assert.That(((Image) model.Image).Size, Is.EqualTo(imageSize)); // Ensure type is not truncated
			}
			using (ISession session = OpenSession())
			{
				session.CreateQuery("delete from ClassWithSerializableType").ExecuteUpdate();
				session.Flush();
			}

			stream.Dispose();
		}
	}
}
