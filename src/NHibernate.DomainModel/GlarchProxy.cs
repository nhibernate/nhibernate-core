using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	public interface GlarchProxy 
	{
		int version
		{
			get;
			set;
		}
		int derivedVersion
		{
			get;
		}
	
		string name
		{
			get;
			set;
		}
	
		GlarchProxy next
		{
			get;
			set;
		}
	
		short order
		{
			get;
			set;
		}
	
		IList strings
		{
			get;
			set;
		}	
		object dynaBean
		{
			get;
			set;
		}	
		IDictionary stringSets
		{
			get;
			set;
		}	
		IList fooComponents
		{
			get;
			set;
		}	
		GlarchProxy[] proxyArray
		{
			get;
			set;
		}	
		IList proxySet
		{
			get;
			set;
		}	
		Multiplicity multiple
		{
			get;
			set;
		}	
		object any
		{
			get;
			set;
		}	
	}
}