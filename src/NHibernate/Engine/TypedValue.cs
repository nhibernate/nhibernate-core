using System;
using NHibernate.Type;

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
		public TypedValue( IType type, object value )
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
				result = 37 * result + ( value == null ? 0 : value.GetHashCode() );
				return result;
			}
		}

		public override bool Equals(object obj)
		{
			TypedValue that = obj as TypedValue;
			if( that == null )
			{
				return false;
			}

			return that.type.Equals( type )
				&& object.Equals( that.value, value );
		}

		public override string ToString()
		{
			return value.ToString();
		}
	}
}