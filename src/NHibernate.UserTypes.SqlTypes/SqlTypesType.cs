using System;
using System.Data;
using System.Data.SqlTypes;
using System.Reflection;

using NHibernate.SqlTypes;

namespace NHibernate.UserTypes.SqlTypes
{
	public abstract class SqlTypesType : NullableTypesType
	{
		private object nullValue;

		public SqlTypesType(SqlType type) : base(type)
		{
		}

		private object GetNullValueUsingReflection()
		{
			FieldInfo nullField = ReturnedClass.GetField("Null", BindingFlags.Public | BindingFlags.Static);
			return nullField.GetValue(null);
		}

		public override object NullValue
		{
			get
			{
				if (nullValue == null)
				{
					nullValue = GetNullValueUsingReflection();
				}
				return nullValue;
			}
		}

		/// <summary>
		/// Get the wrapped value from an <see cref="INullable"/> (Int32 for SqlInt32, etc.)
		/// </summary>
		/// <param name="value">An INullable that is not null</param>
		/// <returns></returns>
		protected abstract object GetValue(INullable value);

		public override sealed void Set(IDbCommand cmd, object value, int index)
		{
			INullable nullableValue = (INullable) value;
			object parameterValue = nullableValue.IsNull
			                        	? DBNull.Value
			                        	: GetValue(nullableValue);

			((IDataParameter) cmd.Parameters[index]).Value = parameterValue;
		}
	}
}