using System;
using System.Collections;

using log4net;
using NHibernate.Engine;
using NHibernate.Metadata;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl
{
	public sealed class Printer
	{
		private ISessionFactoryImplementor _factory;
		private static readonly ILog log = LogManager.GetLogger( typeof( Printer ) );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity">an actual entity object, not a proxy!</param>
		/// <returns></returns>
		public string ToString( object entity )
		{
			IClassMetadata cm = _factory.GetClassMetadata( entity.GetType() );
			if( cm == null ) return entity.GetType().FullName;

			IDictionary result = new Hashtable();
		
			if ( cm.HasIdentifierProperty ) 
			{
				result[ cm.IdentifierPropertyName ] =
					cm.IdentifierType.ToString( cm.GetIdentifier( entity ), _factory );
			}
		
			IType[] types = cm.PropertyTypes;
			string[] names = cm.PropertyNames;
			object[] values = cm.GetPropertyValues( entity );
			
			for( int i = 0; i < types.Length; i++ )
			{
				result[ names[i] ] = types[i].ToString( values[i], _factory );
			}
			
			return cm.MappedClass.FullName + CollectionPrinter.ToString( result );
		}

		public string ToString( IType[ ] types, object[ ] values )
		{
			IList list = new ArrayList( types.Length );
			for ( int i = 0; i < types.Length; i++ ) 
			{
				if( types[i] != null )
				{
					list.Add( types[ i ].ToString( values[ i ], _factory ) );
				}
			}
			return CollectionPrinter.ToString( list );
		}

		public Printer( ISessionFactoryImplementor factory )
		{
			_factory = factory;
		}
	}
}
