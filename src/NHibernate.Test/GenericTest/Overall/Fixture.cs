using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

namespace NHibernate.Test.GenericTest.Overall
{
	[TestFixture]
	[Ignore( "Generic entities not supported" )]
	public class Fixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] { "GenericTest.Overall.Mappings.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void CRUD()
		{
			A<int> entity = new A<int>();
			entity.Property = 10;
			entity.Collection = new List<int>();
			entity.Collection.Add( 20 );

			using( ISession session = OpenSession() )
			using( ITransaction transaction = session.BeginTransaction() )
			{
				session.Save( entity );
				transaction.Commit();
			}

			using( ISession session = OpenSession() )
			using( ITransaction transaction = session.BeginTransaction() )
			{
				session.Delete( entity );
				transaction.Commit();
			}
		}
	}
}
