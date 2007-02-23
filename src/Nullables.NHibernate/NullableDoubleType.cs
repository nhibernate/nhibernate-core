using System;
using System.Data;

using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Nullables.NHibernate
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableDouble"/>.
	/// </summary>
	[Serializable]
	public class NullableDoubleType : NullableTypesType
	{
		public NullableDoubleType() : base(SqlTypeFactory.Double)
		{
		}

		public override object NullValue
		{
			get { return NullableDouble.Default; }
		}

		public override Type ReturnedClass
		{
			get { return typeof(NullableDouble); }
		}

		public override object Get(IDataReader rs, int index)
		{
			return new NullableDouble(Convert.ToDouble(rs[index]));
		}

		public override void Set(IDbCommand cmd, object value, int index)
		{
			IDataParameter parameter = (IDataParameter) cmd.Parameters[index];
			NullableDouble nullableValue = (NullableDouble) value;

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
			return NullableDouble.Parse(xml);
		}
	}
}