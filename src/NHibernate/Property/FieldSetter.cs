using System;
using System.Reflection;

namespace NHibernate.Property
{
	/// <summary></summary>
	public sealed class FieldSetter : ISetter
	{
		private readonly FieldInfo field;
		private readonly System.Type clazz;
		private readonly string name;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="field"></param>
		/// <param name="clazz"></param>
		/// <param name="name"></param>
		public FieldSetter( FieldInfo field, System.Type clazz, string name )
		{
			this.field = field;
			this.clazz = clazz;
			this.name = name;
		}

		#region ISetter Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="value"></param>
		public void Set( object target, object value )
		{
			try
			{
				field.SetValue( target, value );
			}
			catch( Exception e )
			{
				throw new PropertyAccessException( e, "could not set a field value by reflection", true, clazz, name );
			}
		}

		/// <summary></summary>
		public string PropertyName
		{
			get { return null; }
		}

		/// <summary></summary>
		public PropertyInfo Property
		{
			get { return null; }
		}

		#endregion
	}

}