using System;

namespace NHibernate.Properties
{
	/// <summary>
	/// Access the mapped property through a Property <c>get</c> to get the value 
	/// and go directly to the Field to set the value.
	/// </summary>
	/// <remarks>
	/// This is most useful because Classes can provider a get for the Property
	/// that is the <c>&lt;id&gt;</c> but tell NHibernate there is no setter for the Property
	/// so the value should be written directly to the field.
	/// </remarks>
	[Serializable]
	public class NoSetterAccessor : IPropertyAccessor
	{
		private readonly IFieldNamingStrategy namingStrategy;

		/// <summary>
		/// Initializes a new instance of <see cref="NoSetterAccessor"/>.
		/// </summary>
		/// <param name="namingStrategy">The <see cref="IFieldNamingStrategy"/> to use.</param>
		public NoSetterAccessor(IFieldNamingStrategy namingStrategy)
		{
			this.namingStrategy = namingStrategy;
		}

		#region IPropertyAccessor Members

		/// <summary>
		/// Creates an <see cref="BasicPropertyAccessor.BasicGetter"/> to <c>get</c> the value from the Property.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to find the Property in.</param>
		/// <param name="propertyName">The name of the mapped Property to get.</param>
		/// <returns>
		/// The <see cref="BasicPropertyAccessor.BasicGetter"/> to use to get the value of the Property from an
		/// instance of the <see cref="System.Type"/>.</returns>
		/// <exception cref="PropertyNotFoundException" >
		/// Thrown when a Property specified by the <c>propertyName</c> could not
		/// be found in the <see cref="System.Type"/>.
		/// </exception>
		public IGetter GetGetter(System.Type type, string propertyName)
		{
			BasicPropertyAccessor.BasicGetter result = BasicPropertyAccessor.GetGetterOrNull(type, propertyName);
			if (result == null)
			{
				throw new PropertyNotFoundException(type, propertyName, "getter");
			}
			return result;
		}

		/// <summary>
		/// Create a <see cref="FieldAccessor.FieldSetter"/> to <c>set</c> the value of the mapped Property
		/// through a <c>Field</c>.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to find the mapped Property in.</param>
		/// <param name="propertyName">The name of the mapped Property to set.</param>
		/// <returns>
		/// The <see cref="FieldAccessor.FieldSetter"/> to use to set the value of the Property on an
		/// instance of the <see cref="System.Type"/>.
		/// </returns>
		/// <exception cref="PropertyNotFoundException" >
		/// Thrown when a Field for the Property specified by the <c>propertyName</c> using the
		/// <see cref="IFieldNamingStrategy"/> could not be found in the <see cref="System.Type"/>.
		/// </exception>
		public ISetter GetSetter(System.Type type, string propertyName)
		{
			string fieldName = namingStrategy.GetFieldName(propertyName);
			return new FieldAccessor.FieldSetter(FieldAccessor.GetField(type, fieldName), type, fieldName);
		}

		public bool CanAccessThroughReflectionOptimizer
		{
			get { return true; }
		}

		#endregion
	}
}
