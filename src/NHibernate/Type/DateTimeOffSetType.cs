using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
    /// <summary>
    /// Maps a <see cref="System.DateTimeOffset" /> Property to a <see cref="DbType.DateTimeOffset"/>
    /// </summary>
    [Serializable]
    public class DateTimeOffsetType : DateTimeType
    {
        /// <summary></summary>
        public DateTimeOffsetType()
            : base(SqlTypeFactory.DateTimeOffSet)
        {
        }

        public override string Name
        {
            get { return "DateTimeOffset"; }
        }

        public override System.Type ReturnedClass
        {
            get
            {
                return typeof(DateTimeOffset);
            }
        }

        public override System.Type PrimitiveClass
        {
            get { return typeof(DateTimeOffset); }
        }

        public override void Set(IDbCommand st, object value, int index)
        {
            DateTimeOffset dateValue = (DateTimeOffset)value;
            ((IDataParameter) st.Parameters[index]).Value = 
                new DateTimeOffset(dateValue.Year, dateValue.Month, dateValue.Day, dateValue.Hour, dateValue.Minute, dateValue.Second,dateValue.Offset);
        }
        
        public override object Get(IDataReader rs, int index)
        {
            try
            {
                DateTimeOffset dbValue = (DateTimeOffset)rs[index];;
                return new DateTimeOffset(dbValue.Year, dbValue.Month, dbValue.Day, dbValue.Hour, dbValue.Minute, dbValue.Second,dbValue.Offset);
            }
            catch (Exception ex)
            {
                throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
            }
        }

        public override IComparer Comparator
        {
            get { return Comparer<DateTimeOffset>.Default; }
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

            DateTimeOffset date1 = (DateTimeOffset)x;
            DateTimeOffset date2 = (DateTimeOffset)y;

            if (date1.Equals(date2))
                return true;

            return (date1.Year == date2.Year &&
                    date1.Month == date2.Month &&
                    date1.Day == date2.Day &&
                    date1.Hour == date2.Hour &&
                    date1.Minute == date2.Minute &&
                    date1.Second == date2.Second &&
                    date1.Offset == date2.Offset);
        }

        public override int GetHashCode(object x, EntityMode entityMode)
        {
            // Custom hash code implementation because DateTimeType is only accurate
            // up to seconds.
            DateTimeOffset date = (DateTimeOffset)x;
            int hashCode = 1;
            unchecked
            {
                hashCode = 31 * hashCode + date.Second;
                hashCode = 31 * hashCode + date.Minute;
                hashCode = 31 * hashCode + date.Hour;
                hashCode = 31 * hashCode + date.Day;
                hashCode = 31 * hashCode + date.Month;
                hashCode = 31 * hashCode + date.Year;
            }
            return hashCode;
        }

        public override string ToString(object val)
        {
            return ((DateTimeOffset)val).ToString();
        }
      
        public override object FromStringValue(string xml)
        {
            return DateTimeOffset.Parse(xml);
        }

        public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
        {
            return "'" + ((DateTimeOffset)value) + "'";
        }
    }
}