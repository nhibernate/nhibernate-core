using System;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	public class UriTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "Uri"; }
		}

		[Test]
		public void ReadWrite()
		{
			using (var s = OpenSession())
			{
				var entity = new UriClass { Id = 1 };
				entity.Url = new Uri("http://www.fabiomaulo.blogspot.com/");
				s.Save(entity);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				var entity = s.Get<UriClass>(1);
				Assert.That(entity.Url, Is.Not.Null);
				Assert.That(entity.Url.OriginalString, Is.EqualTo("http://www.fabiomaulo.blogspot.com/"));
				entity.Url = new Uri("http://fabiomaulo.blogspot.com/2010/10/nhibernate-30-cookbook.html");
				s.Save(entity);
				s.Flush();
			}
			using (var s = OpenSession())
			{
				var entity = s.Get<UriClass>(1);
				Assert.That(entity.Url.OriginalString, Is.EqualTo("http://fabiomaulo.blogspot.com/2010/10/nhibernate-30-cookbook.html"));
				s.Delete(entity);
				s.Flush();
			}
		}

		[Test]
		public void InsertNullValue()
		{
			using (ISession s = OpenSession())
			{
				var entity = new UriClass { Id = 1 };
				entity.Url = null;
				s.Save(entity);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				var entity = s.Get<UriClass>(1);
				Assert.That(entity.Url, Is.Null);
				s.Delete(entity);
				s.Flush();
			}
		}

		[Test]
		public void AutoDiscoverFromNetType()
		{
			// integration test to be 100% sure
			var propertyType = sessions.GetEntityPersister(typeof(UriClass).FullName).GetPropertyType("AutoUri");
			Assert.That(propertyType, Is.InstanceOf<UriType>());
		}

	}
}