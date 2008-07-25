namespace NHibernate.Properties
{
	/// <summary>
	/// Abstracts the notion of a "property". Defines a strategy for accessing the
	/// value of a mapped property.
	/// </summary>
	public interface IPropertyAccessor
	{
		/// <summary>
		/// When implemented by a class, create a "getter" for the mapped property.
		/// </summary>
		/// <param name="theClass">The <see cref="System.Type"/> to find the Property in.</param>
		/// <param name="propertyName">The name of the mapped Property to get.</param>
		/// <returns>
		/// The <see cref="IGetter"/> to use to get the value of the Property from an
		/// instance of the <see cref="System.Type"/>.</returns>
		/// <exception cref="PropertyNotFoundException" >
		/// Thrown when a Property specified by the <c>propertyName</c> could not
		/// be found in the <see cref="System.Type"/>.
		/// </exception>
		IGetter GetGetter(System.Type theClass, string propertyName);

		/// <summary>
		/// When implemented by a class, create a "setter" for the mapped property.
		/// </summary>
		/// <param name="theClass">The <see cref="System.Type"/> to find the Property in.</param>
		/// <param name="propertyName">The name of the mapped Property to set.</param>
		/// <returns>
		/// The <see cref="ISetter"/> to use to set the value of the Property on an
		/// instance of the <see cref="System.Type"/>.
		/// </returns>
		/// <exception cref="PropertyNotFoundException" >
		/// Thrown when a Property specified by the <c>propertyName</c> could not
		/// be found in the <see cref="System.Type"/>.
		/// </exception>
		ISetter GetSetter(System.Type theClass, string propertyName);

		#region NH specific
		/// <summary>
		/// Allow embedded and custom accessors to define if the ReflectionOptimizer can be used.
		/// </summary>
		bool CanAccessTroughReflectionOptimizer { get;}
		#endregion
	}
}