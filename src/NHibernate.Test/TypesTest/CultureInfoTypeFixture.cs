using System;
using System.Globalization;
using NHibernate.Dialect;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	[TestFixture]
	public class CultureInfoTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "CultureInfo"; }
		}

		[Test]
		public void ReadWriteBasicCulture()
		{
			Guid id;
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var entity = new CultureInfoClass { BasicCulture = CultureInfo.GetCultureInfo("en-US") };
				s.Save(entity);
				id = entity.Id;
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var entity = s.Get<CultureInfoClass>(id);
				Assert.That(entity.BasicCulture, Is.Not.Null);
				Assert.That(entity.BasicCulture.Name, Is.EqualTo("en-US"));
				Assert.That(entity.BasicCulture, Is.EqualTo(CultureInfo.GetCultureInfo("en-US")));
				entity.BasicCulture = CultureInfo.GetCultureInfo("fr-BE");
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var entity = s.Get<CultureInfoClass>(id);
				Assert.That(entity.BasicCulture.Name, Is.EqualTo("fr-BE"));
				Assert.That(entity.BasicCulture, Is.EqualTo(CultureInfo.GetCultureInfo("fr-BE")));
				entity.BasicCulture = null;
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var entity = s.Get<CultureInfoClass>(id);
				Assert.That(entity.BasicCulture, Is.Null);
				t.Commit();
			}
		}

		[Test]
		public void ReadWriteExtendedCulture()
		{
			Guid id;
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var entity = new CultureInfoClass { ExtendedCulture = CultureInfo.GetCultureInfo("en-US-posix") };
				s.Save(entity);
				id = entity.Id;
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var entity = s.Get<CultureInfoClass>(id);
				Assert.That(entity.ExtendedCulture, Is.Not.Null);
				Assert.That(entity.ExtendedCulture.Name, Is.EqualTo("en-US-posix"));
				Assert.That(entity.ExtendedCulture, Is.EqualTo(CultureInfo.GetCultureInfo("en-US-posix")));
				t.Commit();
			}
		}

		[Test]
		public void WriteTooLongCulture()
		{
			if (Dialect is SQLiteDialect)
				Assert.Ignore("SQLite has no length limited string type.");
			using var s = OpenSession();
			using var t = s.BeginTransaction();
			var entity = new CultureInfoClass { BasicCulture = CultureInfo.GetCultureInfo("en-US-posix") };
			s.Save(entity);
			Assert.That(t.Commit, Throws.Exception);
		}

		[Test]
		public void AutoDiscoverFromNetType()
		{
			// integration test to be 100% sure
			var propertyType = Sfi.GetEntityPersister(typeof(CultureInfoClass).FullName).GetPropertyType(nameof(CultureInfoClass.BasicCulture));
			Assert.That(propertyType, Is.InstanceOf<CultureInfoType>());
		}
	}
}
