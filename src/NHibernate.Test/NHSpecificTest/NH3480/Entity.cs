using System;
using System.Collections.Generic;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3480
{
	class Entity
	{
		public Entity()
		{
			Children = new HashSet<Child>();
		}

		public virtual Key Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int OtherId { get; set; }
		public virtual int YetAnotherOtherId { get; set; }
		public virtual ISet<Child> Children { get; set; }
		public virtual ISet<int> Elements { get; set; } = new HashSet<int>();

		public override bool Equals(object obj)
		{
			if (obj is Entity)
			{
				var otherEntity = (Entity) obj;
				return otherEntity.Id.Equals(this.Id);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public class Key
		{
			public virtual Guid Id { get; set; }

			public override bool Equals(object obj)
			{
				if (obj is Key)
				{
					var otherEntity = (Key) obj;
					return otherEntity.Id.Equals(this.Id);
				}
				return false;
			}

			public override int GetHashCode()
			{
				// Needed to reproduce the problem
				return 20.GetHashCode();
			}
		}
	}

	class Child
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Entity Parent { get; set; }
	}

	// This user type is the Elements collection element type
	public class SimpleCustomType : IUserType
	{
		private static readonly SqlType[] ReturnSqlTypes = { SqlTypeFactory.Int32 };

		#region IUserType Members

		public new bool Equals(object x, object y)
		{
			if (ReferenceEquals(x, y))
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}

			return x.Equals(y);
		}

		public int GetHashCode(object x)
		{
			return x?.GetHashCode() ?? 0;
		}

		public SqlType[] SqlTypes => ReturnSqlTypes;

		public object DeepCopy(object value)
		{
			return value;
		}

		public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			cmd.Parameters[index].Value = value ?? DBNull.Value;
		}

		public System.Type ReturnedType => typeof(int);

		public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			// Detect if the root cause of the bug is still there: if yes, the owner will be null.
			Assert.That(owner, Is.Not.Null);

			var index = rs.GetOrdinal(names[0]);
			if (rs.IsDBNull(index))
			{
				return null;
			}
			return rs.GetInt32(index);
		}

		public bool IsMutable => false;

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

		#endregion
	}
}
