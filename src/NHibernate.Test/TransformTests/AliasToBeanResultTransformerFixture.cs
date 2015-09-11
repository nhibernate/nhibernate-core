using System.Collections;
using System.Collections.Generic;
using NHibernate.Transform;
using NUnit.Framework;

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

		public class PublicParameterLessCtor
		{
			private string something;

			public string Something
			{
				get { return something; }
				set { something = value; }
			}
		}

		public struct TestStruct
		{
			public string Something { get; set; }
		}

		public class PublicPropertiesSimpleDTO
		{
			public object Id { get; set; }
			public string Name { get; set; }
		}

		public class PrivateFieldsSimpleDTO
		{
			private object id;
			private string name;

			public object Id { get { return id; } }
			public string Name { get { return name; } }
		}

		public class BaseSimpleDTO
		{
			public object Id { get; set; }
		}

		public class PublicInheritedPropertiesSimpleDTO : BaseSimpleDTO
		{
			public string Name { get; set; }
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
			try
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
			}
			finally
			{
				Cleanup();	
			}
		}

		[Test]
		public void ToPublicProperties_WithoutAnyProjections()
		{
			try
			{
				Setup();

				using (ISession s = OpenSession())
				{
					var transformer = Transformers.AliasToBean<PublicPropertiesSimpleDTO>();
					IList<PublicPropertiesSimpleDTO> l = s.CreateSQLQuery("select * from Simple")
						.SetResultTransformer(transformer)
						.List<PublicPropertiesSimpleDTO>();
					Assert.That(l.Count, Is.EqualTo(2));
					Assert.That(l, Has.All.Not.Null);
				}
			}
			finally
			{
				Cleanup();
			}			
		}

		[Test]
		public void ToPrivateFields_WithoutAnyProjections()
		{
			try
			{
				Setup();

				using (ISession s = OpenSession())
				{
					var transformer = Transformers.AliasToBean<PrivateFieldsSimpleDTO>();
					IList<PrivateFieldsSimpleDTO> l = s.CreateSQLQuery("select * from Simple")
						.SetResultTransformer(transformer)
						.List<PrivateFieldsSimpleDTO>();
					Assert.That(l.Count, Is.EqualTo(2));
					Assert.That(l, Has.All.Not.Null);
				}
			}
			finally
			{
				Cleanup();
			}
		}

		[Test]
		public void ToInheritedPublicProperties_WithoutProjections()
		{
			try
			{
				Setup();

				using (ISession s = OpenSession())
				{
					var transformer = Transformers.AliasToBean<PublicInheritedPropertiesSimpleDTO>();
					IList<PublicInheritedPropertiesSimpleDTO> l = s.CreateSQLQuery("select * from Simple")
						.SetResultTransformer(transformer)
						.List<PublicInheritedPropertiesSimpleDTO>();
					Assert.That(l.Count, Is.EqualTo(2));
					Assert.That(l, Has.All.Not.Null);
				}
			}
			finally
			{
				Cleanup();
			}
		}

		[Test]
		public void WorkWithPublicParameterLessCtor()
		{
			try
			{
				Setup();

				var queryString = "select s.Name as something from Simple s";
				AssertAreWorking(queryString); // working for field access

				queryString = "select s.Name as Something from Simple s";
				AssertAreWorking(queryString); // working for property access
			}
			finally
			{
				Cleanup();
			}
		}

		[Test]
		public void WorksWithStruct()
		{
			try
			{
				Setup();

				IList<TestStruct> result;
				using (ISession s = OpenSession())
				{
					result = s.CreateSQLQuery("select s.Name as something from Simple s")
						.SetResultTransformer(Transformers.AliasToBean<TestStruct>())
						.List<TestStruct>();
				}
				Assert.AreEqual(2, result.Count);
			}
			finally
			{
				Cleanup();
			}
		}

		private void AssertAreWorking(string queryString)
		{
			using (ISession s = OpenSession())
			{
				IList<PublicParameterLessCtor> l =
					s.CreateSQLQuery(queryString).SetResultTransformer(
						Transformers.AliasToBean<PublicParameterLessCtor>()).List<PublicParameterLessCtor>();
				Assert.That(l.Count, Is.EqualTo(2));
				Assert.That(l, Has.All.Not.Null);
			}
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