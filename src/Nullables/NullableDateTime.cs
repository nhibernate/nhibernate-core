using System;
using System.ComponentModel;

using Nullables.TypeConverters;

namespace Nullables
{
	/// <summary>
	/// An <see cref="INullableType"/> that wraps a <see cref="DateTime"/> value.
	/// </summary>
	/// <remarks>
	/// Please see the 
	/// <a href="http://msdn.microsoft.com/netframework/programming/bcl/faq/DateAndTimeFAQ.aspx">DateTime FAQ</a>
	/// on MSDN. 
	/// </remarks>
	[TypeConverter(typeof(NullableDateTimeConverter)), Serializable()]
	public struct NullableDateTime : INullableType, IFormattable, IComparable
	{
		public static readonly NullableDateTime Default = new NullableDateTime();

		private DateTime _value;
		private bool hasValue;

		#region Constructors

		public NullableDateTime(DateTime value)
		{
			_value = value;
			hasValue = true;
		}

		#endregion

		#region INullable Members

		object INullableType.Value
		{
			get { return Value; }
		}

		public bool HasValue
		{
			get { return hasValue; }
		}

		#endregion

		public DateTime Value
		{
			get
			{
				if (hasValue)
					return _value;
				else
					throw new InvalidOperationException("Nullable type must have a value.");
			}
		}

		#region Casts

		public static explicit operator DateTime(NullableDateTime nullable)
		{
			if (!nullable.HasValue)
				throw new NullReferenceException();

			return nullable.Value;
		}

		public static implicit operator NullableDateTime(DateTime value)
		{
			return new NullableDateTime(value);
		}

		public static implicit operator NullableDateTime(DBNull value)
		{
			return Default;
		}

		#endregion

		public override string ToString()
		{
			if (HasValue)
				return Value.ToString();
			else
				return string.Empty;
		}

		public override bool Equals(object obj)
		{
			if (obj is DBNull && !HasValue)
				return true;
			else if (obj is NullableDateTime)
				return Equals((NullableDateTime) obj);
			else
				return false;
					//if this is reached, it is either some other type, or DBnull is compared with this and we have a Value.
		}

		public bool Equals(NullableDateTime x)
		{
			return Equals(this, x);
		}

		public static bool Equals(NullableDateTime x, NullableDateTime y)
		{
			if (x.HasValue != y.HasValue) //one is null
				return false;
			else if (x.HasValue) //therefor y also HasValue
				return x.Value == y.Value;
			else //both are null
				return true;
		}

		public static bool operator ==(NullableDateTime x, NullableDateTime y)
		{
			return x.Equals(y);
		}

		public static bool operator ==(NullableDateTime x, object y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(NullableDateTime x, NullableDateTime y)
		{
			return !x.Equals(y);
		}

		public static bool operator !=(NullableDateTime x, object y)
		{
			return !x.Equals(y);
		}

		public override int GetHashCode()
		{
			if (HasValue)
				return Value.GetHashCode();
			else
				return 0; //GetHashCode() doesn't garantee uniqueness, and neither do we.
		}

		//TODO: Operators for DateTime (?)

		#region IFormattable Members

		string IFormattable.ToString(string format, IFormatProvider formatProvider)
		{
			if (HasValue)
				return Value.ToString(format, formatProvider);
			else
				return string.Empty;
		}

		#endregion

		#region IComparable Members

		public int CompareTo(object obj)
		{
			if (obj is NullableDateTime) //chack and unbox
			{
				NullableDateTime value = (NullableDateTime) obj;

				if (value.HasValue == this.HasValue) //both null or not null
				{
					if (this.HasValue) //this has a value, so they both do
						return Value.CompareTo(value.Value);
					else
						return 0; //both null, so they are equal;
				}
				else //one is null
				{
					if (HasValue) //he have a value, so we are greater.
						return 1;
					else
						return -1;
				}
			}
			else if (obj is DateTime)
			{
				DateTime value = (DateTime) obj;

				if (HasValue) //not null, so compare the real values.
					return Value.CompareTo(value);
				else
					return -1; //this is null, so less that the real value;
			}

			throw new ArgumentException("NullableDateTime can only compare to another NullableDateTime or a System.DateTime");
		}

		#endregion

		#region Parse Members

		public static NullableDateTime Parse(string s)
		{
			if ((s == null) || (s.Trim().Length == 0))
			{
				return new NullableDateTime();
			}
			else
			{
				try
				{
					return new NullableDateTime(DateTime.Parse(s));
				}
				catch (Exception ex)
				{
					throw new FormatException("Error parsing '" + s + "' to NullableDateTime.", ex);
				}
			}
		}

		// TODO: implement the rest of the Parse overloads found in DateTime

		#endregion
	}
}