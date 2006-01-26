using System;

namespace Nullables
{
	/// <summary>
	/// An <see cref="INullableType"/> that wraps a <see cref="Byte"/> value.
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(Nullables.TypeConverters.NullableByteConverter)), Serializable()]
	public struct NullableByte : INullableType, IFormattable, IComparable
	{
		public static readonly NullableByte Default = new NullableByte();

		Byte _value;
		bool hasValue;

		#region Constructors

		public NullableByte(Byte value)
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

		public Byte Value
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

		public static explicit operator Byte(NullableByte nullable)
		{
			if (!nullable.HasValue)
				throw new NullReferenceException();

			return nullable.Value;
		}

		public static implicit operator NullableByte(Byte value)
		{
			return new NullableByte(value);
		}

		public static implicit operator NullableByte(DBNull value)
		{
			return NullableByte.Default;
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
			else if (obj is NullableByte)
				return Equals((NullableByte)obj);
			else
				return false; //if this is reached, it is either some other type, or DBnull is compared with this and we have a Value.
		}

		public bool Equals(NullableByte x)
		{
			return Equals(this, x);
		}

		public static bool Equals(NullableByte x, NullableByte y)
		{
			if (x.HasValue != y.HasValue) //one is null
				return false;
			else if (x.HasValue) //therefor y also HasValue
				return x.Value == y.Value;
			else //both are null
				return true;
		}

		public static bool operator ==(NullableByte x, NullableByte y)
		{
			return x.Equals(y);
		}

		public static bool operator ==(NullableByte x, object y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(NullableByte x, NullableByte y)
		{
			return !x.Equals(y);
		}

		public static bool operator !=(NullableByte x, object y)
		{
			return !x.Equals(y);
		}

		public static NullableInt32 operator +(NullableByte x, NullableByte y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableInt32.Default;

			return new NullableInt32(x.Value + y.Value);
		}

		public static NullableInt32 operator -(NullableByte x, NullableByte y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableInt32.Default;

			return new NullableInt32(x.Value - y.Value);
		}

		public static NullableInt32 operator *(NullableByte x, NullableByte y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableInt32.Default;

			return new NullableInt32(x.Value * y.Value);
		}

		public static NullableInt32 operator /(NullableByte x, NullableByte y)
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

		string System.IFormattable.ToString(string format, IFormatProvider formatProvider)
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
			if (obj is NullableByte) //chack and unbox
			{
				NullableByte value = (NullableByte)obj;

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
			else if (obj is Byte)
			{
				Byte value = (Byte)obj;

				if (HasValue) //not null, so compare the real values.
					return Value.CompareTo(value);
				else
					return -1; //this is null, so less that the real value;
			}

			throw new ArgumentException("NullableByte can only compare to another NullableByte or a System.Byte");
		}

		#endregion

		#region Parse Members

		public static NullableByte Parse(string s)
		{
			if ((s == null) || (s.Trim().Length==0)) 
			{		
				return new NullableByte();
			}
			else
			{
				try 
				{
					return new NullableByte(Byte.Parse(s));
				}
				catch (System.Exception ex) 
				{ 
					throw new FormatException("Error parsing '" + s + "' to NullableByte." , ex);
				}
			}
		}

		// TODO: implement the rest of the Parse overloads found in Byte
		#endregion
	}
}
