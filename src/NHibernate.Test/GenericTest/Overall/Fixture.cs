using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

namespace NHibernate.Test.GenericTest.Overall
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "GenericTest.Overall.Mappings.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void CRUD()
		{
			var entity = new A<int> { Property = 10, Collection = new List<int> { 20 } };

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Save(entity);
				transaction.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete(entity);
				transaction.Commit();
			}
		}

		[Test]
		public void CRUDAB()
		{
			var entity = new A<B>
			{
				Property = new B { Prop = 2 },
				Collection = new List<B> { new B { Prop = 3 } }
			};

			Console.WriteLine(entity.GetType().FullName);

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Save(entity);
				transaction.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete(entity);
				transaction.Commit();
			}
		}
	}

}
