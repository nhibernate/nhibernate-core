using System;
using System.Reflection;

namespace NHibernate.Properties
{
	/// <summary>
	/// Access the mapped property through a Property <c>get</c> to get the value 
	/// and do nothing to set the value.
	/// </summary>
	/// <remarks>
	/// This is useful to allow calculated properties in the domain that will never
	/// be recovered from the DB but can be used for querying.
	/// </remarks>
	[Serializable]
	public class ReadOnlyAccessor : IPropertyAccessor
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ReadOnlyAccessor"/>.
		/// </summary>
		public ReadOnlyAccessor()
		{
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
		/// Create a <see cref="NoopAccessor.NoopSetter"/> to do nothing when trying to
		/// se the value of the mapped Property
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to find the mapped Property in.</param>
		/// <param name="propertyName">The name of the mapped Property to set.</param>
		/// <returns>
		/// An instance of <see cref="NoopAccessor.NoopSetter"/>.
		/// </returns>
		public ISetter GetSetter(System.Type type, string propertyName)
		{
			return new NoopSetter();
		}

		public bool CanAccessThroughReflectionOptimizer
		{
			get { return true; }
		}

		#endregion

		[Serializable]
		private class NoopSetter : ISetter
		{
			public void Set(object target, object value) {}

			public string PropertyName { get { return null; } }

			public MethodInfo Method { get { return null; } }
		}
	}
}
