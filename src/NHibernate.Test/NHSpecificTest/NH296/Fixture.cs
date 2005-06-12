using System;
using System.Collections;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH296
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "NHSpecificTest.NH296.Mappings.hbm.xml" }; }
		}


		[Test]
		public void CRUD()
		{
			Stock stock = new Stock();
			stock.ProductPK = new ProductPK();
			stock.ProductPK.Number = 1;
			stock.ProductPK.Type   = 1;

			using( ISession s = OpenSession() )
			{
				s.Save( stock );
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				stock = (Stock) s.Get( typeof( Stock ), stock.ProductPK );
				Assert.IsNotNull( stock );
			}

			using( ISession s = OpenSession() )
			{
				stock = (Stock) s.Get( typeof( Product ), stock.ProductPK );
				Assert.IsNotNull( stock );

				stock.Property = 10;
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				s.Delete( stock );
				s.Flush();
			}
		}
	}
}
