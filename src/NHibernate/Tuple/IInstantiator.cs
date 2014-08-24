namespace NHibernate.Tuple
{
	/// <summary> Contract for implementors responsible for instantiating entity/component instances. </summary>
	public interface IInstantiator
	{
		/// <summary> Perform the requested entity instantiation. </summary>
		/// <param name="id">The id of the entity to be instantiated. </param>
		/// <returns> An appropriately instantiated entity. </returns>
		/// <remarks>This form is never called for component instantiation, only entity instantiation.</remarks>
		object Instantiate(object id);

		/// <summary> Perform the requested instantiation. </summary>
		/// <returns> The instantiated data structure.  </returns>
		object Instantiate();

		/// <summary> 
		/// Performs check to see if the given object is an instance of the entity
		/// or component which this Instantiator instantiates. 
		/// </summary>
		/// <param name="obj">The object to be checked. </param>
		/// <returns> True is the object does represent an instance of the underlying entity/component. </returns>
		bool IsInstance(object obj);
	}
}
