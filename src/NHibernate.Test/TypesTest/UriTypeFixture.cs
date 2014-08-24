using System;
using NHibernate.Type;
using NUnit.Framework;
using SharpTestsEx;

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
				entity.Url.Should().Not.Be.Null();
				entity.Url.OriginalString.Should().Be("http://www.fabiomaulo.blogspot.com/");
				entity.Url = new Uri("http://fabiomaulo.blogspot.com/2010/10/nhibernate-30-cookbook.html");
				s.Save(entity);
				s.Flush();
			}
			using (var s = OpenSession())
			{
				var entity = s.Get<UriClass>(1);
				entity.Url.OriginalString.Should().Be("http://fabiomaulo.blogspot.com/2010/10/nhibernate-30-cookbook.html");
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
				entity.Url.Should().Be.Null();
				s.Delete(entity);
				s.Flush();
			}
		}

		[Test]
		public void AutoDiscoverFromNetType()
		{
			// integration test to be 100% sure
			var propertyType = sessions.GetEntityPersister(typeof(UriClass).FullName).GetPropertyType("AutoUri");
			propertyType.Should().Be.InstanceOf<UriType>();
		}

	}
}