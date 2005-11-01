using NHibernate.Engine;
using NHibernate.Loader;

namespace NHibernate.Type
{
	/// <summary>
	/// Enables other Component-like types to hold collections and have cascades, etc.
	/// </summary>
	public interface IAbstractComponentType : IType
	{
		/// <summary></summary>
		IType[ ] Subtypes { get; }

		/// <summary></summary>
		string[ ] PropertyNames { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		object[ ] GetPropertyValues( object component, ISessionImplementor session );

		/// <summary>
		/// Optional Operation
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		object[] GetPropertyValues(object component);

		/// <summary>
		/// Optional operation
		/// </summary>
		/// <param name="component"></param>
		/// <param name="values"></param>
		void SetPropertyValues( object component, object[ ] values );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="i"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		object GetPropertyValue( object component, int i, ISessionImplementor session );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		Cascades.CascadeStyle Cascade( int i );

		FetchMode GetFetchMode( int i );
	}
}