using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using System.Collections.Generic;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Int16"/> Property 
	/// to a <see cref="DbType.Int16"/> column.
	/// </summary>
	[Serializable]
	public class Int16Type : PrimitiveType, IDiscriminatorType, IVersionType
	{
		/// <summary></summary>
		public Int16Type() : base(SqlTypeFactory.Int16)
		{
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "Int16"; }
		}

		private static readonly Int16 ZERO = 0;
		public override object Get(IDataReader rs, int index)
		{
			try
			{
				return Convert.ToInt16(rs[index]);
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
				return Convert.ToInt16(rs[name]);
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[name]), ex);
			}
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(Int16); }
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
			return Int16.Parse(xml);
		}

		#region IVersionType Members

		public virtual object Next(object current, ISessionImplementor session)
		{
			return (Int16)((Int16)current + 1);
		}

		public virtual object Seed(ISessionImplementor session)
		{
			return (Int16)1;
		}

		public IComparer Comparator
		{
			get { return Comparer<Int16>.Default; }
		}

		#endregion

		public override System.Type PrimitiveClass
		{
			get { return typeof (Int16); }
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