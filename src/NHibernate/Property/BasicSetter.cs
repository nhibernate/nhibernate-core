using System;
using System.Reflection;

namespace NHibernate.Property
{
	/// <summary></summary>
	public sealed class BasicSetter : ISetter
	{
		private System.Type clazz;
		private PropertyInfo property;
		private string propertyName;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="property"></param>
		/// <param name="propertyName"></param>
		public BasicSetter( System.Type clazz, PropertyInfo property, string propertyName )
		{
			this.clazz = clazz;
			this.property = property;
			this.propertyName = propertyName;
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
				property.SetValue( target, value, new object[0] );
			}
			catch( Exception e )
			{
				throw new PropertyAccessException( e, "Exception occurred", true, clazz, propertyName );
			}
		}

		/// <summary></summary>
		public string PropertyName
		{
			get { return property.Name; }
		}

		/// <summary></summary>
		public PropertyInfo Property
		{
			get { return property; }
		}

		#endregion
	}
}