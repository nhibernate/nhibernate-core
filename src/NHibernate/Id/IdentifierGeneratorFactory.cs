using System;
using System.Collections;
using System.Data;
using NHibernate.Type;

namespace NHibernate.Id
{
	/// <summary>
	/// Factory methods for <c>IdentifierGenerator</c> framework.
	/// </summary>
	public sealed class IdentifierGeneratorFactory
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="clazz"></param>
		/// <returns></returns>
		public static object Get( IDataReader rs, System.Type clazz )
		{
			// here is an interesting one: 
			// - MsSql's @@identity returns a numeric - which translates to a C# decimal type.  
			// - MySql LAST_IDENITY() returns an Int64 			
			try
			{
				object identityValue = rs[ 0 ];
				return Convert.ChangeType( identityValue, clazz );
			}
			catch( Exception e )
			{
				throw new IdentifierGenerationException( "this id generator generates Int64, Int32, Int16", e );
			}
		}

		private static readonly Hashtable idgenerators = new Hashtable();

		/// <summary></summary>
		public static readonly string ShortCircuitIndicator = String.Empty;

		/// <summary></summary>
		static IdentifierGeneratorFactory()
		{
			idgenerators.Add( "uuid.hex", typeof( UUIDHexGenerator ) );
			idgenerators.Add( "uuid.string", typeof( UUIDStringGenerator ) );
			idgenerators.Add( "hilo", typeof( TableHiLoGenerator ) );
			idgenerators.Add( "assigned", typeof( Assigned ) );
			idgenerators.Add( "identity", typeof( IdentityGenerator ) );
			idgenerators.Add( "sequence", typeof( SequenceGenerator ) );
			idgenerators.Add( "seqhilo", typeof( SequenceHiLoGenerator ) );
			idgenerators.Add( "vm", typeof( CounterGenerator ) );
			idgenerators.Add( "foreign", typeof( ForeignGenerator ) );
			idgenerators.Add( "guid", typeof( GuidGenerator ) );
			idgenerators.Add( "guid.comb", typeof( GuidCombGenerator ) );
		}

		private IdentifierGeneratorFactory()
		{
		} //cannot be instantiated

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strategy"></param>
		/// <param name="type"></param>
		/// <param name="parms"></param>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public static IIdentifierGenerator Create( string strategy, IType type, IDictionary parms, Dialect.Dialect dialect )
		{
			try
			{
				System.Type clazz = ( System.Type ) idgenerators[ strategy ];
				if( "native".Equals( strategy ) )
				{
					if( dialect.SupportsIdentityColumns )
					{
						clazz = typeof( IdentityGenerator );
					}
					else if( dialect.SupportsSequences )
					{
						clazz = typeof( SequenceGenerator );
					}
					else
					{
						clazz = typeof( TableHiLoGenerator );
					}
				}
				if( clazz == null )
				{
					clazz = System.Type.GetType( strategy );
				}
				IIdentifierGenerator idgen = ( IIdentifierGenerator ) Activator.CreateInstance( clazz );
				if( idgen is IConfigurable )
				{
					( ( IConfigurable ) idgen ).Configure( type, parms, dialect );
				}
				return idgen;
			}
			catch( Exception e )
			{
				throw new MappingException( "could not instantiate id generator", e );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static object CreateNumber( long value, System.Type type )
		{
			if( type == typeof( long ) )
			{
				return value;
			}
			else if( type == typeof( int ) )
			{
				return ( int ) value;
			}
			else if( type == typeof( short ) )
			{
				return ( short ) value;
			}
			else
			{
				throw new IdentifierGenerationException( "this id generator generates Int64, Int32, Int16" );
			}
		}
	}
}