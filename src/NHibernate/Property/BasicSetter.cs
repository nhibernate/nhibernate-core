using System;
using System.Reflection;

namespace NHibernate.Property
{
	/// <summary>
	/// An <see cref="ISetter"/> for a Property <c>set</c>.
	/// </summary>
	public sealed class BasicSetter : ISetter
	{
		private System.Type clazz;
		private PropertyInfo property;
		private string propertyName;

		/// <summary>
		/// Initializes a new instance of <see cref="BasicSetter"/>.
		/// </summary>
		/// <param name="clazz">The <see cref="System.Type"/> that contains the Property <c>set</c>.</param>
		/// <param name="property">The <see cref="PropertyInfo"/> for reflection.</param>
		/// <param name="propertyName">The name of the mapped Property.</param>
		public BasicSetter( System.Type clazz, PropertyInfo property, string propertyName )
		{
			this.clazz = clazz;
			this.property = property;
			this.propertyName = propertyName;
		}

		#region ISetter Members

		/// <summary>
		/// Sets the value of the Property on the object.
		/// </summary>
		/// <param name="target">The object to set the Property value in.</param>
		/// <param name="value">The value to set the Property to.</param>
		/// <exception cref="PropertyAccessException">
		/// Thrown when there is a problem setting the value in the target.
		/// </exception>
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

		/// <summary>
		/// Gets the name of the mapped Property.
		/// </summary>
		/// <value>The name of the mapped Property or <c>null</c>.</value>
		public string PropertyName
		{
			get { return property.Name; }
		}

		/// <summary>
		/// Gets the <see cref="PropertyInfo"/> for the mapped Property.
		/// </summary>
		/// <value>The <see cref="PropertyInfo"/> for the mapped Property.</value>
		public PropertyInfo Property
		{
			get { return property; }
		}

		#endregion
	}
}