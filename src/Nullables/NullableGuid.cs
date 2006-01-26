using System;
using System.Runtime.Serialization;

namespace Nullables
{
	/// <summary>
	/// An <see cref="INullableType"/> that wraps a <see cref="Guid"/> value.
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(Nullables.TypeConverters.NullableGuidConverter)), Serializable()]
	public struct NullableGuid : INullableType, IFormattable, IComparable
	{
		public static readonly NullableGuid Default = new NullableGuid();

		Guid _value;
		bool hasValue;

		#region Constructors

		public NullableGuid(Guid value)
		{
			_value = value;
			hasValue = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NullableGuid"/> class using the 
		/// value represented by the specified string.
		/// </summary>
		/// <param name="g">
		/// A <see cref="string"/> that contains a GUID as described
		/// in the <see cref="Guid(String)"/> constructor.
		/// </param>
		/// <remarks>
		/// If the <c>guid</c> string is <c>null</c> or <c>Empty</c> then the <c>HasValue</c> 
		/// property will be <c>false</c>.		
		/// </remarks>
		/// <exception cref="FormatException">
		/// The format of <c>g</c> is invalid.
		/// </exception>
		public NullableGuid(string g)
		{
			if( g==null || g.Trim().Length==0 )
			{
				hasValue = false;
				_value = Guid.Empty;
			}
			else
			{
				try 
				{
					_value = new Guid( g );
					hasValue = true;
				}
				catch (System.Exception ex) 
				{ 
					throw new FormatException("Error parsing '" + g + "' to NullableGuid." , ex);
				}
			}
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

		public Guid Value
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

		public static explicit operator Guid(NullableGuid nullable)
		{
			if (!nullable.HasValue)
				throw new NullReferenceException();

			return nullable.Value;
		}

		public static implicit operator NullableGuid(Guid value)
		{
			return new NullableGuid(value);
		}

		public static implicit operator NullableGuid(DBNull value)
		{
			return NullableGuid.Default;
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
			else if (obj is NullableGuid)
				return Equals((NullableGuid)obj);
			else
				return false; //if this is reached, it is either some other type, or DBnull is compared with this and we have a Value.
		}

		public bool Equals(NullableGuid x)
		{
			return Equals(this, x);
		}

		public static bool Equals(NullableGuid x, NullableGuid y)
		{
			if (x.HasValue != y.HasValue) //one is null
				return false;
			else if (x.HasValue) //therefor y also HasValue
				return x.Value == y.Value;
			else //both are null
				return true;
		}

		public static bool operator ==(NullableGuid x, NullableGuid y)
		{
			return x.Equals(y);
		}

		public static bool operator ==(NullableGuid x, object y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(NullableGuid x, NullableGuid y)
		{
			return !x.Equals(y);
		}

		public static bool operator !=(NullableGuid x, object y)
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
			if (obj is NullableGuid) //chack and unbox
			{
				NullableGuid value = (NullableGuid)obj;

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
			else if (obj is Guid)
			{
				Guid value = (Guid)obj;

				if (HasValue) //not null, so compare the real values.
					return Value.CompareTo(value);
				else
					return -1; //this is null, so less that the real value;
			}

			throw new ArgumentException("NullableGuid can only compare to another NullableGuid or a System.Guid");
		}

		#endregion
	}
}
