using System;
using System.ComponentModel;

using Nullables.TypeConverters;

namespace Nullables
{
	/// <summary>
	/// An <see cref="INullableType"/> that wraps an <see cref="Int16"/> value.
	/// </summary>
	[TypeConverter(typeof(NullableInt16Converter)), Serializable()]
	public struct NullableInt16 : INullableType, IFormattable, IComparable
	{
		public static readonly NullableInt16 Default = new NullableInt16();

		private Int16 _value;
		private bool hasValue;

		#region Constructors

		public NullableInt16(Int16 value)
		{
			_value = value;
			hasValue = true;
		}

		#endregion

		#region INullableType Members

		object INullableType.Value
		{
			get { return Value; }
		}

		public bool HasValue
		{
			get { return hasValue; }
		}

		#endregion

		public Int16 Value
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

		public static explicit operator Int16(NullableInt16 nullable)
		{
			if (!nullable.HasValue)
				throw new NullReferenceException();

			return nullable.Value;
		}

		public static implicit operator NullableInt16(Int16 value)
		{
			return new NullableInt16(value);
		}

		public static implicit operator NullableInt16(DBNull value)
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
			else if (obj is NullableInt16)
				return Equals((NullableInt16) obj);
			else
				return false;
					//if this is reached, it is either some other type, or DBnull is compared with this and we have a Value.
		}

		public bool Equals(NullableInt16 x)
		{
			return Equals(this, x);
		}

		public static bool Equals(NullableInt16 x, NullableInt16 y)
		{
			if (x.HasValue != y.HasValue) //one is null
				return false;
			else if (x.HasValue) //therefor y also HasValue
				return x.Value == y.Value;
			else //both are null
				return true;
		}

		public static bool operator ==(NullableInt16 x, NullableInt16 y)
		{
			return x.Equals(y);
		}

		public static bool operator ==(NullableInt16 x, object y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(NullableInt16 x, NullableInt16 y)
		{
			return !x.Equals(y);
		}

		public static bool operator !=(NullableInt16 x, object y)
		{
			return !x.Equals(y);
		}

		public static NullableInt32 operator +(NullableInt16 x, NullableInt16 y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableInt32.Default;

			return new NullableInt32(x.Value + y.Value);
		}

		public static NullableInt32 operator -(NullableInt16 x, NullableInt16 y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableInt32.Default;

			return new NullableInt32(x.Value - y.Value);
		}

		public static NullableInt32 operator *(NullableInt16 x, NullableInt16 y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableInt32.Default;

			return new NullableInt32(x.Value * y.Value);
		}

		public static NullableInt32 operator /(NullableInt16 x, NullableInt16 y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableInt32.Default;

			return new NullableInt32(x.Value / y.Value);
		}

		public override int GetHashCode()
		{
			if (HasValue)
				return Value.GetHashCode();
			else
				return 0; //GetHashCode() doesn't garantee uniqueness, and neither do we.
		}

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
			if (obj is NullableInt16) //chack and unbox
			{
				NullableInt16 value = (NullableInt16) obj;

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
			else if (obj is Int16)
			{
				Int16 value = (Int16) obj;

				if (HasValue) //not null, so compare the real values.
					return Value.CompareTo(value);
				else
					return -1; //this is null, so less that the real value;
			}

			throw new ArgumentException("NullableInt16 can only compare to another NullableInt16 or a System.Int16");
		}

		#endregion

		#region Parse Members

		public static NullableInt16 Parse(string s)
		{
			if ((s == null) || (s.Trim().Length == 0))
			{
				return new NullableInt16();
			}
			else
			{
				try
				{
					return new NullableInt16(Int16.Parse(s));
				}
				catch (Exception ex)
				{
					throw new FormatException("Error parsing '" + s + "' to NullableInt16.", ex);
				}
			}
		}


		// TODO: implement the rest of the Parse overloads found in Int16

		#endregion
	}
}