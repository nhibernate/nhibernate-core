using System.Reflection;
using NHibernate.Engine;

namespace NHibernate.Type
{
	/// <summary>
	/// Enables other Component-like types to hold collections and have cascades, etc.
	/// </summary>
	public interface IAbstractComponentType : IType
	{
		/// <summary>Get the types of the component properties</summary>
		IType[] Subtypes { get; }

		/// <summary>Get the names of the component properties</summary>
		string[] PropertyNames { get; }

		/// <summary>
		/// Optional operation
		/// </summary>
		/// <value>nullability of component properties</value>
		bool[] PropertyNullability { get; }

		/// <summary>
		/// Get the values of the component properties of 
		/// a component instance
		/// </summary>
		object[] GetPropertyValues(object component, ISessionImplementor session);

		/// <summary>
		/// Optional Operation
		/// </summary>
		object[] GetPropertyValues(object component, EntityMode entityMode);

		/// <summary>
		/// Optional operation
		/// </summary>
		void SetPropertyValues(object component, object[] values, EntityMode entityMode);

		object GetPropertyValue(object component, int i, ISessionImplementor session);

		CascadeStyle GetCascadeStyle(int i);

		FetchMode GetFetchMode(int i);

		bool IsEmbedded { get;}

		bool IsMethodOf(MethodBase method);
	}
}