using System;

using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Proxy;
using NHibernate.Collection;

namespace NHibernate {

	/// <summary>
	/// Provides access to the full range of NHibernate built-in types.
	/// IType instances may be used to bind values to query parameters.
	/// Also a factory for new Blobs and Clobs.
	/// </summary>
	public class NHibernate	{

		/// <summary>
		/// NHibernate binary type
		/// </summary>
		public static readonly NullableType Binary = TypeFactory.GetBinaryType(); 
		
		/// <summary>
		/// NHibernate boolean type
		/// </summary>
		public static readonly NullableType Boolean = TypeFactory.GetBooleanType(); 
		
		/// <summary>
		/// NHibernate byte type
		/// </summary>
		public static readonly NullableType Byte = TypeFactory.GetByteType() ; 
		
		/// <summary>
		/// NHibernate character type
		/// </summary>
		public static readonly NullableType Character = TypeFactory.GetCharType(); 
		
		/// <summary>
		/// NHibernate class type
		/// </summary>
		public static readonly NullableType Class = TypeFactory.GetTypeType(); 

		/// <summary>
		/// NHibernate CultureInfo type
		/// </summary>
		public static readonly NullableType CultureInfo = TypeFactory.GetCultureInfoType(); 
		
		/// <summary>
		/// NHibernate date type
		/// </summary>
		public static readonly NullableType DateTime = TypeFactory.GetDateTimeType(); 
		
		/// <summary>
		/// NHibernate date type
		/// </summary>
		public static readonly NullableType Date = TypeFactory.GetDateType(); 

		/// <summary>
		/// NHibernate decimal type
		/// </summary>
		public static readonly NullableType Decimal = TypeFactory.GetDecimalType(); 
		
		/// <summary>
		/// NHibernate double type
		/// </summary>
		public static readonly NullableType Double = TypeFactory.GetDoubleType(); 
		
		/// <summary>
		/// NHibernate Guid type.
		/// </summary>
		public static readonly NullableType Guid = TypeFactory.GetGuidType();
		
		/// <summary>
		/// NHibernate System.Int16 (short in C#) type
		/// </summary>
		public static readonly NullableType Int16 = TypeFactory.GetInt16Type(); 
		
		/// <summary>
		/// NHibernate System.Int32 (int in C#) type
		/// </summary>
		public static readonly NullableType Int32 = TypeFactory.GetInt32Type(); 
		
		/// <summary>
		/// NHibernate System.Int64 (long in C#) type
		/// </summary>
		public static readonly NullableType Int64 = TypeFactory.GetInt64Type();

		/// <summary>
		/// NHibernate serializable type
		/// </summary>
		public static readonly NullableType Serializable = TypeFactory.GetSerializableType(); 

		/// <summary>
		/// NHIbernate System.Single (float in C#) Type
		/// </summary>
		public static readonly NullableType Single = TypeFactory.GetSingleType(); 

		/// <summary>
		/// NHibernate string type
		/// </summary>
		public static readonly NullableType String = TypeFactory.GetStringType(); 
		
		/// <summary>
		/// NHibernate time type
		/// </summary>
		public static readonly NullableType Time = TypeFactory.GetTimeType();
		
		/// <summary>
		/// NHibernate Ticks type
		/// </summary>
		public static readonly NullableType Ticks = TypeFactory.GetTicksType();
		
		/// <summary>
		/// NHibernate timestamp type
		/// </summary>
		public static readonly NullableType Timestamp = TypeFactory.GetTimestampType(); //new TimestampType();
		
		/// <summary>
		/// NHibernate true_false type
		/// </summary>
		public static readonly NullableType TrueFalse = TypeFactory.GetTrueFalseType(); 
		
		/// <summary>
		/// NHibernate yes_no type
		/// </summary>
		public static readonly NullableType YesNo = TypeFactory.GetYesNoType(); 

		/// <summary>
		/// NHibernate blob type
		/// </summary>
		//public static readonly NullableType Blob = new BlobType();
		
		/// <summary>
		/// NHibernate clob type
		/// </summary>
		//public static readonly NullableType Clob = new ClobType();
		
		/// <summary>
		/// Cannot be instantiated.
		/// </summary>
		private NHibernate() {																	  
			throw new NotSupportedException();											
		}
		
		
		/// <summary>
		/// A NHibernate persistent enum type
		/// </summary>
		/// <param name="enumClass"></param>
		/// <returns></returns>
		public static IType Enum(System.Type enumClass) {
			return new PersistentEnumType(enumClass);
		}
		
		
		/// <summary>
		/// A NHibernate serializable type
		/// </summary>
		/// <param name="serializableClass"></param>
		/// <returns></returns>
		public static IType GetSerializable(System.Type serializableClass) {
			return new SerializableType(serializableClass);
		}

		/// <summary>
		/// A NHibernate serializable type
		/// </summary>
		/// <param name="serializableClass"></param>
		/// <returns></returns>
		public static IType Any(IType metaType, IType identifierType) {
			return new ObjectType(metaType, identifierType);
		}
		
		/// <summary>
		/// A NHibernate persistent object (entity) type
		/// </summary>
		/// <param name="persistentClass">a mapped entity class</param>
		/// <returns></returns>
		public static IType Association(System.Type persistentClass) {
			// not really a many-to-one association *necessarily*
			return new ManyToOneType(persistentClass);
		}
		
		/// <summary>
		/// A NHibernate custom type
		/// </summary>
		/// <param name="userTypeClass">a class that implements UserType</param>
		/// <returns></returns>
		public static IType Custom(System.Type userTypeClass) {
			if( typeof(ICompositeUserType).IsAssignableFrom( userTypeClass )) {
				return new CompositeCustomType( userTypeClass );
			}
			else {
				return new CustomType(userTypeClass);
			}
		}
				
		
		/// <summary>
		/// Force initialization of a proxy or persistent collection.
		/// </summary>
		/// <param name="proxy">a persistable object, proxy, persistent collection or null</param>
		/// <exception cref="HibernateException">if we can't initialize the proxy at this time, eg. the Session was closed</exception>
		public static void Initialize(object proxy) {
			if (proxy==null) {
				return;
			}
			else if ( proxy is HibernateProxy ) {
				((HibernateProxy) proxy).Initialize();
			}
			else if ( proxy is PersistentCollection ) {
				( (PersistentCollection) proxy ).ForceLoad();
			}
		}

		/// <summary>
		/// Is the proxy or persistent collection initialized?
		/// </summary>
		/// <param name="proxy">a persistable object, proxy, persistent collection or null</param>
		/// <returns>true if the argument is already initialized, or is not a proxy or collection</returns>
		public static bool IsInitialized(object proxy) {
			if ( proxy is HibernateProxy ) {
				return !HibernateProxyHelper.GetLazyInitializer( (HibernateProxy) proxy ).IsUninitialized;
			} else if ( proxy is PersistentCollection ) {
				return ( (PersistentCollection) proxy).WasInitialized;
			} else {
				return true;
			}
		}

		public System.Type GetClass(object proxy) {
			if(proxy is HibernateProxy) {
				return HibernateProxyHelper.GetLazyInitializer( (HibernateProxy) proxy ).GetImplementation().GetType();
			}
			else {
				return proxy.GetType();
			}
		}
	
		/*
		/// <summary>
		/// Create a new Blob. The returned object will be initially immutable.
		/// </summary>
		/// <param name="bytes">a byte array</param>
		/// <returns></returns>
		public static Blob CreateBlob(byte[] bytes) {
			return new BlobImpl(bytes);
		}
	
		/// <summary>
		/// Create a new Blob. The returned object will be initially immutable.
		/// </summary>
		/// <param name="stream">a binary stream</param>
		/// <param name="length">the number of bytes in the stream</param>
		/// <returns></returns>
		public static Blob CreateBlob(StreamReader stream, int length) {
			return new BlobImpl(stream, length);
		}
		
		/// <summary>
		/// Create a new Blob. The returned object will be initially immutable.
		/// </summary>
		/// <param name="stream">a binary stream</param>
		/// <returns></returns>
		public static Blob CreateBlob(StreamReader stream) {
			return new BlobImpl( stream, stream.available() );
		}
		
		/// <summary>
		/// Create a new Clob. The returned object will be
		/// initially immutable.
		/// </summary>
		/// <param name="str">a String</param>
		/// <returns></returns>
		public static Clob CreateClob(String str) {
			return new ClobImpl(str);
		}

		/// <summary>
		/// Create a new Clob. The returned object will be initially immutable.
		/// </summary>
		/// <param name="reader">a character stream</param>
		/// <param name="length">the number of characters in the stream</param>
		/// <returns></returns>
		public static Clob CreateClob(StreamReader reader, int length) {
			return new ClobImpl(reader, length);
		}
		*/
	}
}
