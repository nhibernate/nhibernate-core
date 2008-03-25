using NHibernate.Engine;

namespace NHibernate.Tuple.Component
{
	/// <summary> 
	/// Defines further responsibilities reagarding tuplization based on
	/// a mapped components.
	/// </summary>
	/// <remarks>
	/// ComponentTuplizer implementations should have the following constructor signature:
	/// (org.hibernate.mapping.Component)
	/// </remarks>
	public interface IComponentTuplizer : ITuplizer
	{
		/// <summary> Retreive the current value of the parent property. </summary>
		/// <param name="component">
		/// The component instance from which to extract the parent property value. 
		/// </param>
		/// <returns> The current value of the parent property. </returns>
		object GetParent(object component);

		/// <summary> Set the value of the parent property. </summary>
		/// <param name="component">The component instance on which to set the parent. </param>
		/// <param name="parent">The parent to be set on the comonent. </param>
		/// <param name="factory">The current session factory. </param>
		void SetParent(object component, object parent, ISessionFactoryImplementor factory);

		/// <summary> Does the component managed by this tuuplizer contain a parent property? </summary>
		/// <returns> True if the component does contain a parent property; false otherwise. </returns>
		bool HasParentProperty { get;}
	}
}