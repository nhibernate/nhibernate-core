using System;

using NHibernate.Type;
using NHibernate.SqlTypes;

namespace NHibernate.DomainModel.NHSpecific
{
	public class NullableInt32Type : NullableTypesType
	{
		public NullableInt32Type() : base(new Int32SqlType())
		{
		}

		public override bool Equals(object x, object y)
		{
			//get boxed values.
			NullableInt32 xTyped = (NullableInt32)x;
			return xTyped.Equals(y);
		}

		public override object NullValue
		{
			get { return NullableInt32.Default; }
		}
		
		public override bool HasNiceEquals
		{
			get { return true; }
		}
		
		public override bool IsMutable
		{
			get { return true; }
		}

		public override string Name
		{
			get { return "NullableInt32"; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(NullableInt32); }
		}

		public override object DeepCopyNotNull(object val)
		{
			return val;
		}

		public override object Get(System.Data.IDataReader rs, int index)
		{
			//TODO: perhaps NullableInt32 has a method/operator/contructor that will take an object.
			object value = rs[index];

			if( value==DBNull.Value )
			{
				return NullableInt32.Default;
			}
			else 
			{
				return new NullableInt32(Convert.ToInt32(value));
			}
		}

		public override void Set(System.Data.IDbCommand cmd, object value, int index)
		{
			System.Data.IDataParameter parameter = (System.Data.IDataParameter)cmd.Parameters[index];
			NullableInt32 nullableValue = (NullableInt32)value;

			if( nullableValue.HasValue ) 
			{
				parameter.Value = nullableValue.Value;
			}
			else 
			{
				parameter.Value = DBNull.Value;
			}
		}

		public override string ToXML(object val)
		{
			return val.ToString();
		} 
	}
}
