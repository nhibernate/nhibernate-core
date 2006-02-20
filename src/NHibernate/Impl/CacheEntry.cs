using System;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary>
	/// A cached instance of a persistent class
	/// </summary>
	[Serializable]
	internal class CacheEntry
	{
		private object[ ] state;
		private System.Type subclass;

		/// <summary></summary>
		public System.Type Subclass
		{
			get { return subclass; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="persister"></param>
		/// <param name="session"></param>
		public CacheEntry( object obj, IClassPersister persister, ISessionImplementor session )
		{
			state = Disassemble( obj, persister, session );
			subclass = obj.GetType();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="persister"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		private static object[ ] Disassemble( object obj, IClassPersister persister, ISessionImplementor session )
		{
			object[ ] values = persister.GetPropertyValues( obj );
			IType[ ] propertyTypes = persister.PropertyTypes;
			for( int i = 0; i < values.Length; i++ )
			{
				values[ i ] = propertyTypes[ i ].Disassemble( values[ i ], session );
			}
			return values;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="id"></param>
		/// <param name="persister"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public object[ ] Assemble( object instance, object id, IClassPersister persister, IInterceptor interceptor, ISessionImplementor session )
		{
			if( subclass != persister.MappedClass )
			{
				throw new AssertionFailure( "Tried to assemble a different subclass instance" );
			}

			return Assemble( state, instance, id, persister, interceptor, session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="values"></param>
		/// <param name="result"></param>
		/// <param name="id"></param>
		/// <param name="persister"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		private static object[ ] Assemble( object[ ] values, object result, object id, IClassPersister persister, IInterceptor interceptor, ISessionImplementor session )
		{
			IType[ ] propertyTypes = persister.PropertyTypes;
			object[ ] assembledProps = new object[propertyTypes.Length];
			for( int i = 0; i < values.Length; i++ )
			{
				assembledProps[ i ] = propertyTypes[ i ].Assemble( values[ i ], session, result );
			}

			interceptor.OnLoad( result, id, assembledProps, persister.PropertyNames, propertyTypes );

			persister.SetPropertyValues( result, assembledProps );

			if( persister.ImplementsLifecycle )
			{
				( ( ILifecycle ) result ).OnLoad( session, id );
			}

			return assembledProps;
		}
	}
}
