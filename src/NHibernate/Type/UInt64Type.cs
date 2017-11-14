using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.UInt64"/> Property 
	/// to a <see cref="DbType.UInt64"/> column.
	/// </summary>
	[Serializable]
	public partial class UInt64Type : PrimitiveType, IDiscriminatorType, IVersionType
	{
		/// <summary></summary>
		public UInt64Type() : base(SqlTypeFactory.UInt64)
		{
		}

		public override string Name
		{
			get { return "UInt64"; }
		}

		private static readonly UInt32 ZERO = 0;
		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			try
			{
				return Convert.ToUInt64(rs[index]);
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
			}
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			try
			{
				return Convert.ToUInt64(rs[name]);
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[name]), ex);
			}
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(UInt64); }
		}

		public override void Set(DbCommand rs, object value, int index, ISessionImplementor session)
		{
			rs.Parameters[index].Value = value;
		}

		public object StringToObject(string xml)
		{
			return FromStringValue(xml);
		}

		public override object FromStringValue(string xml)
		{
			return UInt64.Parse(xml);
		}

		#region IVersionType Members

		public virtual object Next(object current, ISessionImplementor session)
		{
			return (UInt64)current + 1;
		}

		public virtual object Seed(ISessionImplementor session)
		{
			return 1;
		}

		public IComparer Comparator
		{
			get { return Comparer<UInt64>.Default; }
		}

		#endregion

		public override System.Type PrimitiveClass
		{
			get { return typeof(UInt64); }
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