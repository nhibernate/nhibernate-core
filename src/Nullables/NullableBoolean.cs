using System;

namespace Nullables
{
	/// <summary>
	/// An <see cref="INullableType"/> that wraps a <see cref="Boolean"/> value.
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(Nullables.TypeConverters.NullableBooleanConverter)), Serializable()]
	public struct NullableBoolean : INullableType, IComparable
	{
		public static readonly NullableBoolean Default = new NullableBoolean();

		Boolean _value;
		bool hasValue;

		#region Constructors

		public NullableBoolean(Boolean value)
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

		public Boolean Value
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

		public static explicit operator Boolean(NullableBoolean nullable)
		{
			if (!nullable.HasValue)
				throw new NullReferenceException();

			return nullable.Value;
		}

		public static implicit operator NullableBoolean(Boolean value)
		{
			return new NullableBoolean(value);
		}

		public static implicit operator NullableBoolean(DBNull value)
		{
			return NullableBoolean.Default;
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
			else if (obj is NullableBoolean)
				return Equals((NullableBoolean)obj);
			else
				return false; //if this is reached, it is either some other type, or DBnull is compared with this and we have a Value.
		}

		public bool Equals(NullableBoolean x)
		{
			return Equals(this, x);
		}

		public static bool Equals(NullableBoolean x, NullableBoolean y)
		{
			if (x.HasValue != y.HasValue) //one is null
				return false;
			else if (x.HasValue) //therefor y also HasValue
				return x.Value == y.Value;
			else //both are null
				return true;
		}

		public static bool operator ==(NullableBoolean x, NullableBoolean y)
		{
			return x.Equals(y);
		}

		public static bool operator ==(NullableBoolean x, object y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(NullableBoolean x, NullableBoolean y)
		{
			return !x.Equals(y);
		}

		public static bool operator !=(NullableBoolean x, object y)
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

		//TODO: Operators for bool (or, and, xor, etc)

		#region IComparable Members

		public int CompareTo(object obj)
		{
			if (obj is NullableBoolean) //chack and unbox
			{
				NullableBoolean value = (NullableBoolean)obj;

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
			else if (obj is Boolean)
			{
				Boolean value = (Boolean)obj;

				if (HasValue) //not null, so compare the real values.
					return Value.CompareTo(value);
				else
					return -1; //this is null, so less that the real value;
			}

			throw new ArgumentException("NullableBoolean can only compare to another NullableBoolean or a System.Boolean");
		}

		#endregion

		#region Parse Members

		public static NullableBoolean Parse(string s)
		{
			if ((s == null) || (s.Trim().Length==0)) 
			{		
				return new NullableBoolean();
			}
			else
			{
				try 
				{
					return new NullableBoolean(Boolean.Parse(s));
				}
				catch (System.Exception ex) 
				{ 
					throw new FormatException("Error parsing '" + s + "' to NullableBoolean." , ex);
				}
			}
		}

		#endregion
	}
}
