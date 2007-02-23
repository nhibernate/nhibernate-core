using System;
using System.ComponentModel;

using Nullables.TypeConverters;

namespace Nullables
{
	/// <summary>
	/// An <see cref="INullableType"/> that wraps a <see cref="Decimal"/> value.
	/// </summary>
	[TypeConverter(typeof(NullableDecimalConverter)), Serializable()]
	public struct NullableDecimal : INullableType, IFormattable, IComparable
	{
		public static readonly NullableDecimal Default = new NullableDecimal();

		private Decimal _value;
		private bool hasValue;

		#region Constructors

		public NullableDecimal(Decimal value)
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

		public Decimal Value
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

		public static explicit operator Decimal(NullableDecimal nullable)
		{
			if (!nullable.HasValue)
				throw new NullReferenceException();

			return nullable.Value;
		}

		public static implicit operator NullableDecimal(Decimal value)
		{
			return new NullableDecimal(value);
		}

		public static implicit operator NullableDecimal(DBNull value)
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
			else if (obj is NullableDecimal)
				return Equals((NullableDecimal) obj);
			else
				return false;
					//if this is reached, it is either some other type, or DBnull is compared with this and we have a Value.
		}

		public bool Equals(NullableDecimal x)
		{
			return Equals(this, x);
		}

		public static bool Equals(NullableDecimal x, NullableDecimal y)
		{
			if (x.HasValue != y.HasValue) //one is null
				return false;
			else if (x.HasValue) //therefor y also HasValue
				return x.Value == y.Value;
			else //both are null
				return true;
		}

		public static bool operator ==(NullableDecimal x, NullableDecimal y)
		{
			return x.Equals(y);
		}

		public static bool operator ==(NullableDecimal x, object y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(NullableDecimal x, NullableDecimal y)
		{
			return !x.Equals(y);
		}

		public static bool operator !=(NullableDecimal x, object y)
		{
			return !x.Equals(y);
		}

		public static NullableDecimal operator +(NullableDecimal x, NullableDecimal y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return Default;

			return new NullableDecimal(x.Value + y.Value);
		}

		public static NullableDecimal operator -(NullableDecimal x, NullableDecimal y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return Default;

			return new NullableDecimal(x.Value - y.Value);
		}

		public static NullableDecimal operator *(NullableDecimal x, NullableDecimal y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return Default;

			return new NullableDecimal(x.Value * y.Value);
		}

		public static NullableDecimal operator /(NullableDecimal x, NullableDecimal y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return Default;

			return new NullableDecimal(x.Value / y.Value);
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
			if (obj is NullableDecimal) //chack and unbox
			{
				NullableDecimal value = (NullableDecimal) obj;

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
			else if (obj is Decimal)
			{
				Decimal value = (Decimal) obj;

				if (HasValue) //not null, so compare the real values.
					return Value.CompareTo(value);
				else
					return -1; //this is null, so less that the real value;
			}

			throw new ArgumentException("NullableDecimal can only compare to another NullableDecimal or a System.Decimal");
		}

		#endregion

		#region Parse Members

		public static NullableDecimal Parse(string s)
		{
			if ((s == null) || (s.Trim().Length == 0))
			{
				return new NullableDecimal();
			}
			else
			{
				try
				{
					return new NullableDecimal(Decimal.Parse(s));
				}
				catch (Exception ex)
				{
					throw new FormatException("Error parsing '" + s + "' to NullableDecimal.", ex);
				}
			}
		}

		// TODO: implement the rest of the Parse overloads found in Decimal

		#endregion
	}
}