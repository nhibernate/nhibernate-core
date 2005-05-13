using System.Reflection;

namespace NHibernate.Property
{
	/// <summary>
	/// Access the mapped property by using a Field to <c>get</c> and <c>set</c> the value.
	/// </summary>
	/// <remarks>
	/// The <see cref="FieldAccessor"/> is useful when you expose <c>getter</c> and <c>setters</c>
	/// for a Property, but they have extra code in them that shouldn't be executed when NHibernate
	/// is setting or getting the values for loads or saves.
	/// </remarks>
	public class FieldAccessor : IPropertyAccessor
	{
		private IFieldNamingStrategy namingStrategy;

		/// <summary>
		/// Initializes a new instance of <see cref="FieldAccessor"/>.
		/// </summary>
		public FieldAccessor()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="FieldAccessor"/>.
		/// </summary>
		/// <param name="namingStrategy">The <see cref="IFieldNamingStrategy"/> to use.</param>
		public FieldAccessor( IFieldNamingStrategy namingStrategy )
		{
			this.namingStrategy = namingStrategy;
		}

		/// <summary>
		/// Gets the <see cref="IFieldNamingStrategy"/> used to convert the name of the
		/// mapped Property in the hbm.xml file to the name of the field in the class.
		/// </summary>
		/// <value>The <see cref="IFieldNamingStrategy"/> or <c>null</c>.</value>
		public IFieldNamingStrategy NamingStrategy
		{
			get { return namingStrategy; }
		}

		#region IPropertyAccessor Members

		/// <summary>
		/// Create a <see cref="FieldGetter"/> to <c>get</c> the value of the mapped Property
		/// through a <c>Field</c>.
		/// </summary>
		/// <param name="theClass">The <see cref="System.Type"/> to find the Property in.</param>
		/// <param name="propertyName">The name of the mapped Property to get.</param>
		/// <returns>
		/// The <see cref="FieldGetter"/> to use to get the value of the Property from an
		/// instance of the <see cref="System.Type"/>.</returns>
		/// <exception cref="PropertyNotFoundException" >
		/// Thrown when a Field specified by the <c>propertyName</c> could not
		/// be found in the <see cref="System.Type"/>.
		/// </exception>
		public IGetter GetGetter( System.Type theClass, string propertyName )
		{
			string fieldName = GetFieldName( propertyName );
			return new FieldGetter( GetField( theClass, fieldName ), theClass, fieldName );
		}

		/// <summary>
		/// Create a <see cref="FieldSetter"/> to <c>set</c> the value of the mapped Property
		/// through a <c>Field</c>.
		/// </summary>
		/// <param name="theClass">The <see cref="System.Type"/> to find the mapped Property in.</param>
		/// <param name="propertyName">The name of the mapped Property to set.</param>
		/// <returns>
		/// The <see cref="FieldSetter"/> to use to set the value of the Property on an
		/// instance of the <see cref="System.Type"/>.
		/// </returns>
		/// <exception cref="PropertyNotFoundException" >
		/// Thrown when a Field for the Property specified by the <c>propertyName</c> using the
		/// <see cref="IFieldNamingStrategy"/> could not be found in the <see cref="System.Type"/>.
		/// </exception>
		public ISetter GetSetter( System.Type theClass, string propertyName )
		{
			string fieldName = GetFieldName( propertyName );
			return new FieldSetter( GetField( theClass, fieldName ), theClass, fieldName );
		}

		#endregion

		/// <summary>
		/// Helper method to find the Field.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to find the Field in.</param>
		/// <param name="fieldName">The name of the Field to find.</param>
		/// <returns>
		/// The <see cref="FieldInfo"/> for the field.
		/// </returns>
		/// <exception cref="PropertyNotFoundException">
		/// Thrown when a field could not be found.
		/// </exception>
		internal static FieldInfo GetField( System.Type type, string fieldName )
		{
			if( type == null || type == typeof( object ) )
			{
				// the full inheritance chain has been walked and we could
				// not find the Field
				throw new PropertyNotFoundException( type, fieldName );
			}

			FieldInfo field = type.GetField( fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly );
			if( field == null )
			{
				// recursively call this method for the base Type
				field = GetField( type.BaseType, fieldName );
			}

			// h2.0.3 has a check to see if the field is public - is there a worry about
			// that in .net??

			return field;
		}

		/// <summary>
		/// Converts the mapped property's name into a Field using 
		/// the <see cref="IFieldNamingStrategy"/> if one exists.
		/// </summary>
		/// <param name="propertyName">The name of the Property.</param>
		/// <returns>The name of the Field.</returns>
		private string GetFieldName( string propertyName )
		{
			if( namingStrategy == null )
			{
				return propertyName;
			}
			else
			{
				return namingStrategy.GetFieldName( propertyName );
			}
		}
	}
}