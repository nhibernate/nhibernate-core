using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// An <see cref="INullableType"/> that wraps an <see cref="Int32"/> value.
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(NullableInt32Converter)), Serializable()]
	public struct NullableInt32 : IFormattable, IComparable
	{
		public static readonly NullableInt32 Default = new NullableInt32();

		Int32 _value;
		bool hasValue;

		#region Constructors

		public NullableInt32(Int32 value)
		{
			_value = value;
			hasValue = true;
		}

		#endregion

		#region INullable Members

		public bool HasValue
		{
			get { return hasValue; }
		}

		#endregion

		public Int32 Value
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

		public static explicit operator Int32(NullableInt32 nullable)
		{
			if (!nullable.HasValue)
				throw new NullReferenceException();

			return nullable.Value;
		}

		public static implicit operator NullableInt32(Int32 value)
		{
			return new NullableInt32(value);
		}

		public static implicit operator NullableInt32(DBNull value)
		{
			return NullableInt32.Default;
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
			else if (obj is NullableInt32)
				return Equals((NullableInt32)obj);
			else
				return false; //if this is reached, it is either some other type, or DBnull is compared with this and we have a Value.
		}

		public bool Equals(NullableInt32 x)
		{
			return Equals(this, x);
		}

		public static bool Equals(NullableInt32 x, NullableInt32 y)
		{
			if (x.HasValue != y.HasValue) //one is null
				return false;
			else if (x.HasValue) //therefor y also HasValue
				return x.Value == y.Value;
			else //both are null
				return true;
		}

		public static bool operator ==(NullableInt32 x, NullableInt32 y)
		{
			return x.Equals(y);
		}

		public static bool operator ==(NullableInt32 x, object y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(NullableInt32 x, NullableInt32 y)
		{
			return !x.Equals(y);
		}

		public static bool operator !=(NullableInt32 x, object y)
		{
			return !x.Equals(y);
		}

		public static NullableInt32 operator +(NullableInt32 x, NullableInt32 y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableInt32.Default;

			return new NullableInt32(x.Value + y.Value);
		}

		public static NullableInt32 operator -(NullableInt32 x, NullableInt32 y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableInt32.Default;

			return new NullableInt32(x.Value - y.Value);
		}

		public static NullableInt32 operator *(NullableInt32 x, NullableInt32 y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableInt32.Default;

			return new NullableInt32(x.Value * y.Value);
		}

		public static NullableInt32 operator /(NullableInt32 x, NullableInt32 y)
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

		public string ToString(string format, IFormatProvider formatProvider)
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
			if (obj is NullableInt32) //chack and unbox
			{
				NullableInt32 value = (NullableInt32)obj;

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
				Int32 value = (Int32)obj;

				if (HasValue) //not null, so compare the real values.
					return Value.CompareTo(value);
				else
					return -1; //this is null, so less that the real value;
			}

			throw new ArgumentException("NullableInt32 can only compare to another NullableInt32 or a System.Int32");
		}

		#endregion

		#region Parse Members

		public static NullableInt32 Parse(string s)
		{
			if ((s == null) || (s.Trim().Length==0) )
			{
				return new NullableInt32();
			}
			else
			{
				try 
				{
					return new NullableInt32(Int32.Parse(s));
				}
				catch (System.Exception ex) 
				{ 
					throw new FormatException("Error parsing '" + s + "' to NullableInt32." , ex);
				}
			}
		}
		// TODO: implement the rest of the Parse overloads found in Int32

		#endregion
	}
}
