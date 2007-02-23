using System;
using System.Collections;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Engine
{
	/// <summary></summary>
	[Serializable]
	public sealed class TypedValue
	{
		private IType type;
		private object value;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="value"></param>
		public TypedValue(IType type, object value)
		{
			this.type = type;
			this.value = value;
		}

		/// <summary></summary>
		public object Value
		{
			get { return value; }
			set { this.value = value; }
		}

		/// <summary></summary>
		public IType Type
		{
			get { return type; }
			set { type = value; }
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = 17;
				result = 37 * result + type.GetHashCode();
				result = 37 * result + (value == null ? 0 : value.GetHashCode());
				return result;
			}
		}

		public override bool Equals(object obj)
		{
			TypedValue that = obj as TypedValue;
			if (that == null)
			{
				return false;
			}

			if (!that.type.Equals(type))
			{
				return false;
			}

			if (value is ICollection && that.value is ICollection)
			{
				return CollectionHelper.CollectionEquals((ICollection) value, (ICollection) that.value);
			}
			else
			{
				return Equals(that.value, value);
			}
		}

		public override string ToString()
		{
			if (value == null)
			{
				return "null";
			}
			return value.ToString();
		}
	}
}