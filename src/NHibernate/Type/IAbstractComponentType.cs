using System;

using NHibernate.Loader;
using NHibernate.Engine;

namespace NHibernate.Type {

	/// <summary>
	/// Enables other Component-like types to hold collections and have cascades, etc.
	/// </summary>
	public interface IAbstractComponentType : IType	{
		
		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		IType[] Subtypes {get;}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		string[] PropertyNames {get;}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		object[] GetPropertyValues(object component, ISessionImplementor session);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="values"></param>
		void SetPropertyValues(object component, object[] values);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="i"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		object GetPropertyValue(object component, int i, ISessionImplementor session);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		Cascades.CascadeStyle Cascade(int i);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		OuterJoinLoaderType EnableJoinedFetch(int i);
	}
}