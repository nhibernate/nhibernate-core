using System;
using System.Reflection;

namespace NHibernate.Property
{
	/// <summary>
	/// Access the Property through the <c>get</c> to get the value 
	/// and go directly to the Field to set the value.
	/// </summary>
	/// <remarks>
	/// This is most useful because Classes can provider a get for the Property
	/// that is the &lt;id&gt; but tell NHibernate there is no setter for the Property
	/// so the value should be written directly to the field.
	/// </remarks>
	public class NoSetterAccessor : IPropertyAccessor
	{
		IFieldNamingStrategy namingStrategy;

		public NoSetterAccessor(IFieldNamingStrategy namingStrategy) 
		{
			this.namingStrategy = namingStrategy;
		}
		
		#region IPropertyAccessor Members

		public IGetter GetGetter(System.Type theClass, string propertyName)
		{
			BasicGetter result = BasicPropertyAccessor.GetGetterOrNull(theClass, propertyName);
			if (result == null) throw new PropertyNotFoundException( "Could not find a setter for property " + propertyName + " in class " + theClass.FullName );
			return result;
		}

		public ISetter GetSetter(System.Type theClass, string propertyName)
		{
			string fieldName = namingStrategy.GetFieldName(propertyName);
			return new FieldSetter( FieldAccessor.GetField( theClass, fieldName ), theClass, fieldName );
		}

		#endregion
	}
}
