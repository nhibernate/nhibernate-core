using System;
using System.Data;
using System.Data.Common;

namespace NHibernate.Type
{
	/// <summary>
	/// Handles any type returned from a Coalesce projection
	/// </summary>
	[Serializable]
	public class CoalesceType : PrimitiveType
	{
		public CoalesceType() : base(new SqlTypes.SqlType(DbType.Object))
		{
		}

		public override string Name
		{
			get { return "Coalesce"; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(object); }
		}

		public override void Set(DbCommand cmd, object value, int index)
		{
			throw new System.NotImplementedException();
		}

		public override object Get(DbDataReader rs, int index)
		{
			throw new System.NotImplementedException();
		}

		public override object Get(DbDataReader rs, string name)
		{
			throw new System.NotImplementedException();
		}

		public override object FromStringValue(string xml)
		{
			throw new System.NotImplementedException();
		}

		public override System.Type PrimitiveClass
		{
			get { return typeof(object); }
		}

		public override object DefaultValue
		{
			get { return null; }
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return value.ToString();
		}
	}
}
