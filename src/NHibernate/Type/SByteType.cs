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
	/// Maps a <see cref="System.SByte"/> Property 
	/// to a <see cref="DbType.SByte"/> column.
	/// </summary>
	[Serializable]
	public class SByteType : PrimitiveType, IDiscriminatorType
	{
		/// <summary></summary>
		public SByteType() : base(SqlTypeFactory.SByte)
		{
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "SByte"; }
		}

		private static readonly SByte ZERO = 0;
		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			try
			{
				return Convert.ToSByte(rs[index]);
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
				return Convert.ToSByte(rs[name]);
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[name]), ex);
			}
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(SByte); }
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
			return SByte.Parse(xml);
		}

		#region IVersionType Members

		public virtual object Next(object current, ISessionImplementor session)
		{
			return (SByte)((SByte)current + 1);
		}

		public virtual object Seed(ISessionImplementor session)
		{
			return (SByte)1;
		}

		public IComparer Comparator
		{
			get { return Comparer<SByte>.Default; }
		}

		#endregion

		public override System.Type PrimitiveClass
		{
			get { return typeof(SByte); }
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