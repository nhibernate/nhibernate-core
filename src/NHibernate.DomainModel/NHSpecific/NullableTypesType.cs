using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Abstract type used for implementing NHibernate <see cref="IType"/>s for 
	/// the Nullables library.
	/// </summary>
	[Serializable]
	public abstract class NullableTypesType : ImmutableType
	{
		public NullableTypesType(SqlType type) : base(type)
		{
		}

		public override object NullSafeGet(DbDataReader rs, string name, ISessionImplementor session)
		{
			object value = base.NullSafeGet(rs, name, session);
			if (value == null)
			{
				return NullValue;
			}
			else
			{
				return value;
			}
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Get(rs, rs.GetOrdinal(name), session);
		}

		public override string ToString(object value)
		{
			return value.ToString();
		}

		public override string Name
		{
			get { return ReturnedClass.Name; }
		}

		public override bool IsEqual(object x, object y)
		{
			return Equals(x, y);
		}

		public abstract object NullValue { get; }
	}
}