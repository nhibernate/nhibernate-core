using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary></summary>
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
	}
}