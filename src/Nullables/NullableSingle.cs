using System;

namespace Nullables
{
	/// <summary>
	/// An <see cref="INullableType"/> that wraps a <see cref="Single"/> value.
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(Nullables.TypeConverters.NullableSingleConverter)), Serializable()]
	public struct NullableSingle : INullableType, IFormattable, IComparable
	{
		public static readonly NullableSingle Default = new NullableSingle();

		Single _value;
		bool hasValue;

		#region Constructors

		public NullableSingle(Single value)
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

		public Single Value
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

		public static explicit operator Single(NullableSingle nullable)
		{
			if (!nullable.HasValue)
				throw new NullReferenceException();

			return nullable.Value;
		}

		public static implicit operator NullableSingle(Single value)
		{
			return new NullableSingle(value);
		}

		public static implicit operator NullableSingle(DBNull value)
		{
			return NullableSingle.Default;
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
			else if (obj is NullableSingle)
				return Equals((NullableSingle)obj);
			else
				return false; //if this is reached, it is either some other type, or DBnull is compared with this and we have a Value.
		}

		public bool Equals(NullableSingle x)
		{
			return Equals(this, x);
		}

		public static bool Equals(NullableSingle x, NullableSingle y)
		{
			if (x.HasValue != y.HasValue) //one is null
				return false;
			else if (x.HasValue) //therefor y also HasValue
				return x.Value == y.Value;
			else //both are null
				return true;
		}

		public static bool operator ==(NullableSingle x, NullableSingle y)
		{
			return x.Equals(y);
		}

		public static bool operator ==(NullableSingle x, object y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(NullableSingle x, NullableSingle y)
		{
			return !x.Equals(y);
		}

		public static bool operator !=(NullableSingle x, object y)
		{
			return !x.Equals(y);
		}

		public static NullableSingle operator +(NullableSingle x, NullableSingle y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableSingle.Default;

			return new NullableSingle(x.Value + y.Value);
		}

		public static NullableSingle operator -(NullableSingle x, NullableSingle y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableSingle.Default;

			return new NullableSingle(x.Value - y.Value);
		}

		public static NullableSingle operator *(NullableSingle x, NullableSingle y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableSingle.Default;

			return new NullableSingle(x.Value * y.Value);
		}

		public static NullableSingle operator /(NullableSingle x, NullableSingle y)
		{
			if (!x.HasValue || !y.HasValue) //one or both are null
				return NullableSingle.Default;

			return new NullableSingle(x.Value / y.Value);
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
			if (obj is NullableSingle) //chack and unbox
			{
				NullableSingle value = (NullableSingle)obj;

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
			else if (obj is Single)
			{
				Single value = (Single)obj;

				if (HasValue) //not null, so compare the real values.
					return Value.CompareTo(value);
				else
					return -1; //this is null, so less that the real value;
			}

			throw new ArgumentException("NullableSingle can only compare to another NullableSingle or a System.Single");
		}

		#endregion

		#region Parse Members

		public static NullableSingle Parse(string s)
		{
			if ((s == null) || (s.Trim().Length==0))
			{		
				return new NullableSingle();
			}
			else
			{
				try 
				{
					return new NullableSingle(Single.Parse(s));
				}
				catch (System.Exception ex) 
				{ 
					throw new FormatException("Error parsing '" + s + "' to NullableSingle." , ex);
				}
			}
		}
		// TODO: implement the rest of the Parse overloads found in Single

		#endregion
	}
}
