using System;

//using NHibernate.Engine;

namespace NHibernate.Type {

	/// <summary>
	/// Enables other Component-like types to hold collections
	/// </summary>
	public interface IAbstractComponentType : IType	{
		
		IType[] Subtypes {get;}

		string[] PropertyNames {get;}
		
		object[] GetPropertyValues(object component);
		
		void SetPropertyValues(object component, object[] values);
		
		object GetPropertyValue(object component, int i);
		
		//public object Instantiate(object parent, ISessionImplementor session);
		
		//public Cascades.CascadeStyle Cascade(int i);
		
		int EnableJoinedFetch(int i);
	}
}