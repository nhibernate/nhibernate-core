using System;
using System.Reflection;

namespace NHibernate.Property
{
	/// <summary>
	/// Access fields directly.
	/// </summary>
	/// <remarks>
	/// This accesses fields with the following naming convention unless a 
	/// <see cref="IFieldNamingStrategy"/> is supplied.
	/// Property Name = "Id"
	/// Field Name = "Id"
	/// </remarks>
	public class FieldAccessor : IPropertyAccessor
	{
		private IFieldNamingStrategy namingStrategy;

		public FieldAccessor() 
		{
		}

		public FieldAccessor(IFieldNamingStrategy namingStrategy) 
		{
			this.namingStrategy = namingStrategy;
		}

		/// <summary>
		/// Gets the <see cref="IFieldNamingStrategy"/> used to convert the name of the
		/// Property in the hbm.xml file to the name of the field in the class.
		/// </summary>
		/// <value>The <see cref="IFieldNamingStrategy"/> or <c>null</c>.</value>
		public IFieldNamingStrategy NamingStrategy 
		{
			get { return namingStrategy; }
		}

		#region IPropertyAccessor Members

		public IGetter GetGetter(System.Type theClass, string propertyName)
		{
			string fieldName = GetFieldName(propertyName);
			return new FieldGetter( GetField( theClass, fieldName ), theClass, fieldName );
		}

		public ISetter GetSetter(System.Type theClass, string propertyName)
		{
			string fieldName = GetFieldName(propertyName);
			return new FieldSetter( GetField( theClass, fieldName ), theClass, fieldName );
		}

		#endregion
	
		internal static FieldInfo GetField(System.Type clazz, string fieldName) 
		{
			if( clazz==null || clazz==typeof(object) )
			{
				throw new PropertyNotFoundException("field not found: " + fieldName);
			}

			FieldInfo field = clazz.GetField( fieldName, BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly );
			if(field==null) 
			{
				field = GetField( clazz.BaseType, fieldName );
			}

			// h2.0.3 has a check to see if the field is public - is there a worry about
			// that in .net??

			return field;
		}

		/// <summary>
		/// Converts the Property's name into a Field using camel style casing.
		/// </summary>
		/// <param name="propertyName">The name of the Property.</param>
		/// <returns>The name of the Field.</returns>
		/// <remarks>
		/// This uses the convention that a Property named <c>Id</c> will have a field <c>id</c>
		/// </remarks>
		private string GetFieldName(string propertyName) 
		{
			if( namingStrategy==null ) 
			{
				return propertyName;
			}
			else 
			{
				return namingStrategy.GetFieldName(propertyName);
			}
		}
	}
}
