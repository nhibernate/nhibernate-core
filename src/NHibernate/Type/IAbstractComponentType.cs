using System;

using NHibernate.Loader;
using NHibernate.Engine;

namespace NHibernate.Type {

	/// <summary>
	/// Enables other Component-like types to hold collections and have cascades, etc.
	/// </summary>
	public interface IAbstractComponentType : IType	{
		
		IType[] Subtypes {get;}

		string[] PropertyNames {get;}
		
		object[] GetPropertyValues(object component, ISessionImplementor session);
		
		void SetPropertyValues(object component, object[] values);
		
		object GetPropertyValue(object component, int i, ISessionImplementor session);
		
		Cascades.CascadeStyle Cascade(int i);
		
		OuterJoinLoaderType EnableJoinedFetch(int i);
	}
}