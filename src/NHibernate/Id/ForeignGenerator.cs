using System;
using System.Collections;

using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Id
{
	/// <summary>
	/// An <c>IIdentifierGenerator</c> that uses the value of the id property of an associated object
	/// 
	/// One mapping parameter id supported: property.
	/// </summary>
	public class ForeignGenerator : IIdentifierGenerator, IConfigurable
	{
		private string propertyName;

		public object Generate(ISessionImplementor session, object obj) {
			object associatedObject = session.Factory
				.GetClassMetadata( obj.GetType() )
				.GetPropertyValue(obj,  propertyName);		
			//return session.getEntityIdentifierIfNotUnsaved(associatedObject);
			object id = session.Save(associatedObject);
			if ( session.Contains(obj) ) {
				//abort the save (the object is already saved by a circular cascade)
				return IdentifierGeneratorFactory.ShortCircuitIndicator; 
				//throw new IdentifierGenerationException("save associated object first, or disable cascade for inverse association");
			}
			return id;
		}

		public void Configure(IType type, IDictionary parms, Dialect.Dialect d) {
			propertyName = (string)parms["property"];
		}
	}
}
