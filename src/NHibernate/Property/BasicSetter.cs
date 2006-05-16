using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NHibernate.Property
{
	/// <summary>
	/// An <see cref="ISetter"/> for a Property <c>set</c>.
	/// </summary>
	public sealed class BasicSetter : ISetter, IOptimizableSetter
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

		public PropertyInfo Property
		{
			get { return property; }
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
			catch( ArgumentException ae )
			{
				// if I'm reading the msdn docs correctly this is the only reason the ArgumentException
				// would be thrown, but it doesn't hurt to make sure.
				if( property.PropertyType.IsAssignableFrom( value.GetType() )==false )
				{
					// add some details to the error message - there have been a few forum posts an they are 
					// all related to an ISet and IDictionary mixups.
					string msg = String.Format( "The type {0} can not be assigned to a property of type {1}", value.GetType().ToString(), property.PropertyType.ToString() );
					throw new PropertyAccessException( ae, msg, true, clazz, propertyName );
				}
				else
				{
					throw new PropertyAccessException( ae, "ArgumentException while setting the property value by reflection", true, clazz, propertyName );
				}
			}
			catch( Exception e )
			{
				throw new PropertyAccessException( e, "could not set a property value by reflection", true, clazz, propertyName );
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
		public MethodInfo Method
		{
			get { return property.GetSetMethod( true ); }
		}

		#endregion

		public void Emit( ILGenerator il )
		{
			il.EmitCall( OpCodes.Callvirt, Method, null ); 
		}
	}
}