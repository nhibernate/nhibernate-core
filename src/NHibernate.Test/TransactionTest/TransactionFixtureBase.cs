using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.TransactionTest
{
	public abstract class TransactionFixtureBase : TestCase
	{
		protected override IList Mappings => new[] { "TransactionTest.Person.hbm.xml" };

		protected override string MappingsAssembly => "NHibernate.Test";

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from System.Object");
				t.Commit();
			}
		}

		public class TestInterceptor : EmptyInterceptor
		{
			private readonly int _numero;
			private readonly List<int> _flushOrder;

			public TestInterceptor(int numero, List<int> flushOrder)
			{
				_numero = numero;
				_flushOrder = flushOrder;
			}

			public override void PreFlush(ICollection entitites)
			{
				_flushOrder.Add(_numero);
			}
		}
	}
}