using System;

//using NHibernate.Engine;

namespace NHibernate.Type {

	/// <summary>
	/// Enables other Component-like types to hold collections
	/// </summary>
	public interface IAbstractComponentType : IType	{
		
		public IType[] Subtypes {get;}

		public string[] PropertyNames {get;}
		
		public object[] GetPropertyValues(object component);
		
		public void SetPropertyValues(object component, object[] values);
		
		public object GetPropertyValue(object component, int i);
		
		//public object Instantiate(object parent, ISessionImplementor session);
		
		//public Cascades.CascadeStyle Cascade(int i);
		
		public int EnableJoinedFetch(int i);
	}
}