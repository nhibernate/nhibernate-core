using System;
using System.Reflection;

namespace NHibernate.Property
{
	/// <summary>
	/// An <see cref="IGetter"/> that uses a Field instead of the Property <c>set</c>.
	/// </summary>
	public sealed class FieldSetter : ISetter
	{
		private readonly FieldInfo field;
		private readonly System.Type clazz;
		private readonly string name;

		/// <summary>
		/// Initializes a new instance of <see cref="FieldSetter"/>.
		/// </summary>
		/// <param name="clazz">The <see cref="System.Type"/> that contains the Field to use for the Property <c>set</c>.</param>
		/// <param name="field">The <see cref="FieldInfo"/> for reflection.</param>
		/// <param name="name">The name of the Field.</param>
		public FieldSetter( FieldInfo field, System.Type clazz, string name )
		{
			this.field = field;
			this.clazz = clazz;
			this.name = name;
		}

		#region ISetter Members

		/// <summary>
		/// Sets the value of the Field on the object.
		/// </summary>
		/// <param name="target">The object to set the Field value in.</param>
		/// <param name="value">The value to set the Field to.</param>
		/// <exception cref="PropertyAccessException">
		/// Thrown when there is a problem setting the value in the target.
		/// </exception>
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

		/// <summary>
		/// Gets the name of the Property.
		/// </summary>
		/// <value><c>null</c> since this is a Field - not a Property.</value>
		public string PropertyName
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the <see cref="PropertyInfo"/> for the Property.
		/// </summary>
		/// <value><c>null</c> since this is a Field - not a Property.</value>
		public PropertyInfo Property
		{
			get { return null; }
		}

		#endregion
	}

}