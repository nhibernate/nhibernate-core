using System;
using System.ComponentModel;

using Nullables.TypeConverters;

//Contributed by Sergey Koshcheyev

namespace Nullables
{
	/// <summary>
	/// An <see cref="INullableType"/> that wraps a <see cref="SByte"/> value.
	/// </summary>
	[TypeConverter(typeof(NullableSByteConverter)), Serializable(), CLSCompliant(false)]
	public struct NullableSByte : INullableType, IFormattable, IComparable
	{
		public static readonly NullableSByte Default = new NullableSByte();

		private SByte _value;
		private bool hasValue;

		#region Constructors

		public NullableSByte(SByte value)
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

		public SByte Value
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

		public static explicit operator SByte(NullableSByte nullable)
		{
			if (!nullable.HasValue)
				throw new NullReferenceException();

			return nullable.Value;
		}

		public static implicit operator NullableSByte(SByte value)
		{
			return new NullableSByte(value);
		}

		public static implicit operator NullableSByte(DBNull value)
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
			else if (obj is NullableSByte)
				return Equals((NullableSByte) obj);
			else
				return false;
					//if this is reached, it is either some other type, or DBnull is compared with this and we have a Value.
		}

		public bool Equals(NullableSByte x)
		{
			return Equals(this, x);
		}

		public static bool Equals(NullableSByte x, NullableSByte y)
		{
			if (x.HasValue != y.HasValue) //one is null
				return false;
			else if (x.HasValue) //therefor y also HasValue
				return x.Value == y.Value;
			else //both are null
				return true;
		}

		public static bool operator ==(NullableSByte x, NullableSByte y)
		{
			return x.Equals(y);
		}

		public static bool operator ==(NullableSByte x, object y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(NullableSByte x, NullableSByte y)
		{
			return !x.Equals(y);
		}

		public static bool operator !=(NullableSByte x, object y)
		{
			return !x.Equals(y);
		}

		public static NullableInt32 operator +(NullableSByte x, NullableSByte y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableInt32.Default;

			return new NullableInt32(x.Value + y.Value);
		}

		public static NullableInt32 operator -(NullableSByte x, NullableSByte y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableInt32.Default;

			return new NullableInt32(x.Value - y.Value);
		}

		public static NullableInt32 operator *(NullableSByte x, NullableSByte y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableInt32.Default;

			return new NullableInt32(x.Value * y.Value);
		}

		public static NullableInt32 operator /(NullableSByte x, NullableSByte y)
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
			if (obj is NullableSByte) //chack and unbox
			{
				NullableSByte value = (NullableSByte) obj;

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
				SByte value = (SByte) obj;

				if (HasValue) //not null, so compare the real values.
					return Value.CompareTo(value);
				else
					return -1; //this is null, so less that the real value;
			}

			throw new ArgumentException("NullableSByte can only compare to another NullableSByte or a System.SByte");
		}

		#endregion

		#region Parse Members

		public static NullableSByte Parse(string s)
		{
			if ((s == null) || (s.Trim().Length == 0))
			{
				return new NullableSByte();
			}
			else
			{
				try
				{
					return new NullableSByte(SByte.Parse(s));
				}
				catch (Exception ex)
				{
					throw new FormatException("Error parsing '" + s + "' to NullableSByte.", ex);
				}
			}
		}

		// TODO: implement the rest of the Parse overloads found in SByte

		#endregion
	}
}