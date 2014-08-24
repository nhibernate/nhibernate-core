using System;
using System.Collections.Generic;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps the Year, Month, and Day of a <see cref="System.DateTime"/> Property to a 
	/// <see cref="DbType.Date"/> column
	/// </summary>
	[Serializable]
	public class DateType : PrimitiveType, IIdentifierType, ILiteralType, IParameterizedType
	{
		public const string BaseValueParameterName = "BaseValue";
		public static readonly DateTime BaseDateValue = new DateTime(1753, 01, 01);
		private DateTime customBaseDate = BaseDateValue;

		/// <summary></summary>
		public DateType() : base(SqlTypeFactory.Date)
		{
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "Date"; }
		}

		public override object Get(IDataReader rs, int index)
		{
			try
			{
				DateTime dbValue = Convert.ToDateTime(rs[index]);
				return dbValue.Date;
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
			}
		}

		public override object Get(IDataReader rs, string name)
		{
			return Get(rs, rs.GetOrdinal(name));
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(DateTime); }
		}

		public override void Set(IDbCommand st, object value, int index)
		{
			var parm = st.Parameters[index] as IDataParameter;
			var dateTime = (DateTime)value;
			if (dateTime < customBaseDate)
			{
				parm.Value = DBNull.Value;
			}
			else
			{
				parm.DbType = DbType.Date;
				parm.Value = dateTime.Date;
			}
		}

		public override bool IsEqual(object x, object y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}

			DateTime date1 = (DateTime)x;
			DateTime date2 = (DateTime)y;
			if (date1.Equals(date2))
				return true;

			return date1.Day == date2.Day
						 && date1.Month == date2.Month
						 && date1.Year == date2.Year;
		}

		public override int GetHashCode(object x, EntityMode entityMode)
		{
			DateTime date = (DateTime)x;
			int hashCode = 1;
			unchecked
			{
				hashCode = 31 * hashCode + date.Day;
				hashCode = 31 * hashCode + date.Month;
				hashCode = 31 * hashCode + date.Year;
			}
			return hashCode;
		}

		public override string ToString(object val)
		{
			return ((DateTime) val).ToShortDateString();
		}

		public override object FromStringValue(string xml)
		{
			return DateTime.Parse(xml);
		}

		public object StringToObject(string xml)
		{
			return string.IsNullOrEmpty(xml) ? null : FromStringValue(xml);
		}

		public override System.Type PrimitiveClass
		{
			get { return typeof(DateTime); }
		}

		public override object DefaultValue
		{
			get { return customBaseDate; }
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return '\'' + ((DateTime)value).ToShortDateString() + '\'';
		}

		public void SetParameterValues(IDictionary<string, string> parameters)
		{
			if(parameters == null)
			{
				return;
			}
			string value;
			if (parameters.TryGetValue(BaseValueParameterName, out value))
			{
				customBaseDate = DateTime.Parse(value);
			}
		}
	}
}