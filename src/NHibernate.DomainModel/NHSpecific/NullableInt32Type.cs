using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableInt32"/>.
	/// </summary>
	[Serializable]
	public class NullableInt32Type : NullableTypesType
	{
		public NullableInt32Type() : base(SqlTypeFactory.Int32)
		{
		}

		public override object NullValue
		{
			get { return NullableInt32.Default; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(NullableInt32); }
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			return new NullableInt32(Convert.ToInt32(rs[index]));
		}

		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			var parameter = cmd.Parameters[index];
			NullableInt32 nullableValue = (NullableInt32) value;

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
			return NullableInt32.Parse(xml);
		}

		public override bool[] ToColumnNullness(object value, Engine.IMapping mapping)
		{
			return value == null || NullableInt32.Default.Equals(value) ? new bool[] { false } : new bool[] { true };
		}
	}
}