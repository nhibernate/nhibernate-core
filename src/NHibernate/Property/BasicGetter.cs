using System;
using System.Reflection;

namespace NHibernate.Property
{
	/// <summary></summary>
	public sealed class BasicGetter : IGetter
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
		public BasicGetter( System.Type clazz, PropertyInfo property, string propertyName )
		{
			this.clazz = clazz;
			this.property = property;
			this.propertyName = propertyName;
		}

		#region IGetter Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public object Get( object target )
		{
			try
			{
				return property.GetValue( target, new object[0] );
			}
			catch( Exception e )
			{
				throw new PropertyAccessException( e, "Exception occurred", false, clazz, propertyName );
			}
		}

		/// <summary></summary>
		public System.Type ReturnType
		{
			get { return property.PropertyType; }
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