//$Id$
using System;
using System.Collections;
using NHibernate;
using NHibernate.Engine;

namespace NHibernate.Hql
{
	
	public class FilterTranslator : QueryTranslator
	{
		
		/// <summary> Compile a filter. This method may be called multiple
		/// times. Subsequent invocations are no-ops.
		/// </summary>
		public void Compile(string collectionRole, ISessionFactoryImplementor factory, string queryString, IDictionary replacements, bool scalar)
		{
			lock(this)
			{
				
				if (!compiled)
				{
					this.factory = factory; // yick!
					AddFromCollection("this", collectionRole);
					base.Compile(factory, queryString, replacements, scalar);
				}
			}
		}
	}
}