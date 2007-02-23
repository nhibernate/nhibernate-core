using System;
using System.Data;

using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Nullables.NHibernate
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableInt64"/>.
	/// </summary>
	[Serializable]
	public class NullableInt64Type : NullableTypesType
	{
		public NullableInt64Type() : base(SqlTypeFactory.Int64)
		{
		}

		public override object NullValue
		{
			get { return NullableInt64.Default; }
		}

		public override Type ReturnedClass
		{
			get { return typeof(NullableInt64); }
		}

		public override object Get(IDataReader rs, int index)
		{
			return new NullableInt64(Convert.ToInt64(rs[index]));
		}

		public override void Set(IDbCommand cmd, object value, int index)
		{
			IDataParameter parameter = (IDataParameter) cmd.Parameters[index];
			NullableInt64 nullableValue = (NullableInt64) value;

			if (nullableValue.HasValue)
			{
				parameter.Value = nullableValue.Value;
			}
			else
			{
				parameter.Value = DBNull.Value;
			}
		}

		public override object FromStringValue(string xml)
		{
			return NullableInt64.Parse(xml);
		}
	}
}