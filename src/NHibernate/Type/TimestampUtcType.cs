using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Type
{
    [Serializable]
    public class TimestampUtcType : TimestampType
    {
        /// <summary>
        /// Returns the DateTimeKind for type <see cref="TimestampUtcType"/>.
        /// </summary>
        /// <value>Returns DateTimeKind.Utc</value>
        protected override DateTimeKind Kind { get { return System.DateTimeKind.Utc; } }

        /// <summary>
        /// Retrieve the current system Utc time.
        /// </summary>
        /// <value>DateTime.UtcNow</value>
        protected override DateTime Now { get { return DateTime.UtcNow; } }

        public override string Name
        {
            get
            {
                return "TimestampUtc";
            }
        }

        /// <summary>
        /// Parse the value by the base implementation and convert it to Utc as the .net framework by default parse to a DateTime value with Kind set to Local.
        /// </summary>
        /// <returns>DateTime value where Kind is Utc</returns>
        public override object FromStringValue(string xml)
        {
            return ((DateTime)base.FromStringValue(xml)).ToUniversalTime();
        }

        /// <summary>
        /// Validate the passed DateTime value if Kind is set to Utc and passes value to base implementation (<see cref="TimestampType.Set(System.Data.IDbCommand, object, int )"/>).
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when Kind is NOT Utc.</exception>
        public override void Set(System.Data.IDbCommand st, object value, int index)
        {
            if (value is DateTime)
            {
                var v = (DateTime)value;
                if (v.Kind != System.DateTimeKind.Utc) throw new ArgumentException("Kind is NOT Utc", "value");
            }

            base.Set(st, value, index);
        }

        /// <summary>
        /// Compares two DateTime object and also compare its Kind which is not used by the .net framework DateTime.Equals implementation.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool IsEqual(object x, object y)
        {
            return base.IsEqual(x, y) && ((DateTime)x).Kind == ((DateTime)y).Kind;
        }

        /// <summary>
        /// Retrieve the string representation of the timestamp object. This is in the following format:
        /// <code>
        /// 2011-01-27T14:50:59.6220000Z
        /// </code>
        /// </summary>
        public override string ToString(object val)
        {
            return ((DateTime)val).ToString("o");
        }
    }
}
