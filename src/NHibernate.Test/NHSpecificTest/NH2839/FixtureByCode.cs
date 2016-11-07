using System;
using System.Data;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2839
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual bool IsActive { get; set; }
	}

	public class MyBooleanType : IEnhancedUserType
	{
		public SqlType[] SqlTypes
		{
			get { return new[] {SqlTypeFactory.Int32}; }
		}

		public System.Type ReturnedType
		{
			get { return typeof (bool); }
		}

		public bool Equals(object x, object y)
		{
			if (x == y) return true;

			if (x is bool && y is bool)
				return (bool) x == (bool) y;
			return false;
		}

		public int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		public object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			var ordinal = rs.GetOrdinal(names[0]);
			if (rs.IsDBNull(ordinal))
				return false;
			return rs.GetInt32(ordinal) == 1;
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			((IDbDataParameter) cmd.Parameters[index]).Value = ((bool) value) ? 1 : -1;
		}

		public object DeepCopy(object value)
		{
			return (bool)value;
		}

		public bool IsMutable
		{
			get { return false; }
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public object Assemble(object cached, object owner)
		{
			return cached;
		}

		public object Disassemble(object value)
		{
			return value;
		}

		public object FromXMLString(string xml)
		{
			return xml;
		}

		public string ObjectToSQLString(object value)
		{
			return ((bool) value) ? "1" : "-1";
		}

		public string ToXMLString(object value)
		{
			return ((bool)value) ? "1" : "-1";
		}
	}

	public class FixtureByCode : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Property(x => x.IsActive, pm=>pm.Type<MyBooleanType>());
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob", IsActive = true};
				session.Save(e1);

				var e2 = new Entity { Name = "Sally",IsActive = false};
				session.Save(e2);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void YourTestName()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = from e in session.Query<Entity>()
							 where e.IsActive
							 select e;

				Assert.AreEqual(1, result.ToList().Count);
			}
		}
	}
}