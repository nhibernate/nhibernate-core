using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Id
{
	/// <summary>
	/// An <c>IIdentifierGenerator</c> that uses the value of the id property of an associated object
	/// 
	/// One mapping parameter is required: property.
	/// </summary>
	public class ForeignGenerator : IIdentifierGenerator, IConfigurable
	{
		private string propertyName;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object Generate( ISessionImplementor session, object obj )
		{
			object associatedObject = session.Factory
				.GetClassMetadata( obj.GetType() )
				.GetPropertyValue( obj, propertyName );
			//return session.getEntityIdentifierIfNotUnsaved(associatedObject);
			object id = session.Save( associatedObject );
			if( session.Contains( obj ) )
			{
				//abort the save (the object is already saved by a circular cascade)
				return IdentifierGeneratorFactory.ShortCircuitIndicator;
				//throw new IdentifierGenerationException("save associated object first, or disable cascade for inverse association");
			}
			return id;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="parms"></param>
		/// <param name="d"></param>
		public void Configure( IType type, IDictionary parms, Dialect.Dialect d )
		{
			propertyName = ( string ) parms[ "property" ];
			if( propertyName == null || propertyName.Length == 0 )
			{
				throw new MappingException( "param named \"property\" is required for foreign id generation strategy" );
			}
		}
	}
}