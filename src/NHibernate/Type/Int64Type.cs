using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Int64"/> Property 
	/// to a <see cref="DbType.Int64"/> column.
	/// </summary>
	[Serializable]
	public class Int64Type : PrimitiveType, IDiscriminatorType, IVersionType
	{
		/// <summary></summary>
		internal Int64Type() : base(SqlTypeFactory.Int64)
		{
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "Int64"; }
		}

		private static readonly Int64 ZERO = 0;
		public override object Get(IDataReader rs, int index)
		{
			try
			{
				return Convert.ToInt64(rs[index]);
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
			}
		}

		public override object Get(IDataReader rs, string name)
		{
			try
			{
				return Convert.ToInt64(rs[name]);
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[name]), ex);
			}
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(Int64); }
		}

		public override void Set(IDbCommand rs, object value, int index)
		{
			((IDataParameter)rs.Parameters[index]).Value = value;
		}

		public object StringToObject(string xml)
		{
			return FromStringValue(xml);
		}

		public override object FromStringValue(string xml)
		{
			return Int64.Parse(xml);
		}

		#region IVersionType Members

		public virtual object Next(object current, ISessionImplementor session)
		{
			return (Int64)current + 1L;
		}

		public virtual object Seed(ISessionImplementor session)
		{
			return 1L;
		}

		public IComparer Comparator
		{
			get { return Comparer<Int64>.Default; }
		}

		#endregion

		public override System.Type PrimitiveClass
		{
			get { return typeof(Int64); }
		}

		public override object DefaultValue
		{
			get { return ZERO; }
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return value.ToString();
		}
	}
}