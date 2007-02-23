using System;
using System.ComponentModel;

using Nullables.TypeConverters;

namespace Nullables
{
	/// <summary>
	/// An <see cref="INullableType"/> that wraps a <see cref="Char"/> value.
	/// </summary>
	[TypeConverter(typeof(NullableCharConverter)), Serializable()]
	public struct NullableChar : INullableType, IComparable
	{
		public static readonly NullableChar Default = new NullableChar();

		private Char _value;
		private bool hasValue;

		#region Constructors

		public NullableChar(Char value)
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

		public Char Value
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

		public static explicit operator Char(NullableChar nullable)
		{
			if (!nullable.HasValue)
				throw new NullReferenceException();

			return nullable.Value;
		}

		public static implicit operator NullableChar(Char value)
		{
			return new NullableChar(value);
		}

		public static implicit operator NullableChar(DBNull value)
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
			else if (obj is NullableChar)
				return Equals((NullableChar) obj);
			else
				return false;
					//if this is reached, it is either some other type, or DBnull is compared with this and we have a Value.
		}

		public bool Equals(NullableChar x)
		{
			return Equals(this, x);
		}

		public static bool Equals(NullableChar x, NullableChar y)
		{
			if (x.HasValue != y.HasValue) //one is null
				return false;
			else if (x.HasValue) //therefor y also HasValue
				return x.Value == y.Value;
			else //both are null
				return true;
		}

		public static bool operator ==(NullableChar x, NullableChar y)
		{
			return x.Equals(y);
		}

		public static bool operator ==(NullableChar x, object y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(NullableChar x, NullableChar y)
		{
			return !x.Equals(y);
		}

		public static bool operator !=(NullableChar x, object y)
		{
			return !x.Equals(y);
		}

		public static NullableInt32 operator +(NullableChar x, NullableChar y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableInt32.Default;

			return new NullableInt32(x.Value + y.Value);
		}

		public static NullableInt32 operator -(NullableChar x, NullableChar y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableInt32.Default;

			return new NullableInt32(x.Value - y.Value);
		}

		public static NullableInt32 operator *(NullableChar x, NullableChar y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableInt32.Default;

			return new NullableInt32(x.Value * y.Value);
		}

		public static NullableInt32 operator /(NullableChar x, NullableChar y)
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

		#region IComparable Members

		public int CompareTo(object obj)
		{
			if (obj is NullableChar) //chack and unbox
			{
				NullableChar value = (NullableChar) obj;

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
			else if (obj is Char)
			{
				Char value = (Char) obj;

				if (HasValue) //not null, so compare the real values.
					return Value.CompareTo(value);
				else
					return -1; //this is null, so less that the real value;
			}

			throw new ArgumentException("NullableChar can only compare to another NullableChar or a System.Char");
		}

		#endregion

		#region Parse Members

		public static NullableChar Parse(string s)
		{
			if ((s == null) || (s.Trim().Length == 0))
			{
				return new NullableChar();
			}
			else
			{
				try
				{
					return new NullableChar(Char.Parse(s));
				}
				catch (Exception ex)
				{
					throw new FormatException("Error parsing '" + s + "' to NullableChar.", ex);
				}
			}
		}

		#endregion
	}
}