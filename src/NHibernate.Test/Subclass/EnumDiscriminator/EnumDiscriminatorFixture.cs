using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Subclass.EnumDiscriminator
{
	[TestFixture]
	public class EnumDiscriminatorFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new String[] {"Subclass.EnumDiscriminator.EnumDiscriminator.hbm.xml"}; }
		}

		[Test]
		public void PersistsDefaultDiscriminatorValue()
		{
			Foo foo = new Foo();
			foo.Id = 1;

			using (ISession s = OpenSession())
			{
				s.Save(foo);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				Baz baz = s.Load<Baz>(1L);
				Assert.AreEqual(Colors.Green, baz.Color);
			}
		}

		[Test]
		public void CanConvertOneTypeToAnother()
		{
			Foo foo = new Foo();
			foo.Id = 1;

			using (ISession s = OpenSession())
			{
				s.Save(foo);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				Baz baz = s.Load<Baz>(1L);
				baz.Color = Colors.Blue;
				s.Save(baz);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				Bar bar = s.Load<Bar>(1L);
			}
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				s.Delete("from Baz");
				s.Flush();
			}
		}
	}
}