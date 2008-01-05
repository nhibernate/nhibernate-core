using System;
using System.Data;

using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.UserTypes
{
	/// <summary>
	/// Abstract type used for implementing NHibernate <see cref="IType"/>s for 
	/// the Nullables library.
	/// </summary>
	public abstract class NullableTypesType : ImmutableType
	{
		public NullableTypesType(SqlType type) : base(type)
		{
		}

		public override object NullSafeGet(IDataReader rs, string name)
		{
			object value = base.NullSafeGet(rs, name);
			if (value == null)
			{
				return NullValue;
			}
			else
			{
				return value;
			}
		}

		public override object Get(IDataReader rs, string name)
		{
			return Get(rs, rs.GetOrdinal(name));
		}

		public override string ToString(object value)
		{
			return value.ToString();
		}

		public override string Name
		{
			get { return ReturnedClass.Name; }
		}

		public abstract object NullValue { get; }
	}
}