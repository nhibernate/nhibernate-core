using System.Collections;
using System.Collections.Generic;
using NHibernate.Transform;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NHibernate.Test.TransformTests
{
	[TestFixture]
	public class AliasToBeanResultTransformerFixture : TestCase
	{
		public class WithOutPublicParameterLessCtor
		{
			private string something;
			protected WithOutPublicParameterLessCtor() {}

			public WithOutPublicParameterLessCtor(string something)
			{
				this.something = something;
			}

			public string Something
			{
				get { return something; }
			}
		}

		#region Overrides of TestCase

		protected override IList Mappings
		{
			get { return new[] {"TransformTests.Simple.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		#endregion

		[Test]
		public void WorkWithOutPublicParameterLessCtor()
		{
			Setup();

			using (ISession s = OpenSession())
			{
				IList<WithOutPublicParameterLessCtor> l =
					s.CreateSQLQuery("select s.Name as something from Simple s").SetResultTransformer(
						Transformers.AliasToBean<WithOutPublicParameterLessCtor>()).List<WithOutPublicParameterLessCtor>();
				Assert.That(l.Count, Is.EqualTo(2));
				Assert.That(l, Has.All.Not.Null);
			}

			Cleanup();
		}

		private void Cleanup()
		{
			using (ISession s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					s.Delete("from Simple");
					s.Transaction.Commit();
				}
			}
		}

		private void Setup()
		{
			using (ISession s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					s.Save(new Simple {Name = "Name1"});
					s.Save(new Simple {Name = "Name2"});
					s.Transaction.Commit();
				}
			}
		}
	}
}