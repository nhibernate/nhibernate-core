using System;
using System.Collections;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.GenericTest.EnumGeneric
{
	/// <summary>
	/// http://nhibernate.jira.com/browse/NH-1236
	/// </summary>
	[TestFixture]
	public class EnumGenericFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new String[] {"GenericTest.EnumGeneric.EnumGenericFixture.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void MapsToEnum()
		{
			using (ISession s = OpenSession())
			{
				A a = new A();
				SessionImpl impl = (SessionImpl) s;
				IEntityPersister persister = impl.GetEntityPersister(typeof(A).FullName, a);

				int index = -1;
				for (int i = 0; i < persister.PropertyNames.Length; i++)
				{
					if (persister.PropertyNames[i] == "NullableValue")
					{
						index = i;
						break;
					}
				}

				if (index == -1) Assert.Fail("Property NullableValue not found.");

				Assert.That(persister.PropertyTypes[index], Is.AssignableTo<PersistentEnumType>());
			}
		}

		[Test]
		public void Persists()
		{
			A a1 = new A();

			using (ISession s = OpenSession())
			{
				s.Save(a1);
				s.Flush();
			}

			//Verify initial null
			using (ISession s = OpenSession())
			{
				A a2 = s.Load<A>(a1.Id);
				Assert.IsNull(a2.NullableValue);
				a2.NullableValue = B.Value3;
				s.Save(a2);
				s.Flush();
			}

			//Verify set to non-null
			using (ISession s = OpenSession())
			{
				A a3 = s.Load<A>(a1.Id);
				Assert.AreEqual(B.Value3, a3.NullableValue);
				a3.NullableValue = null;
				s.Save(a3);
				s.Flush();
			}

			//Verify set to null
			using (ISession s = OpenSession())
			{
				A a4 = s.Load<A>(a1.Id);
				Assert.IsNull(a4.NullableValue);
				s.Delete(a4);
				s.Flush();
			}
		}
	}
}
