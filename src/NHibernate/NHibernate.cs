using System;

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
		/// NHibernate long type
		/// </summary>
		public static readonly NullableType Long = new LongType();
		
		/// <summary>
		/// NHibernate short type
		/// </summary>
		public static readonly NullableType Short = new ShortType();
		
		/// <summary>
		/// NHibernate integer type
		/// </summary>
		public static readonly NullableType Integer = new IntegerType();
		
		/// <summary>
		/// NHibernate byte type
		/// </summary>
		public static readonly NullableType Byte = new ByteType();
		
		/// <summary>
		/// NHibernate float type
		/// </summary>
		public static readonly NullableType Float = new FloatType();
		
		/// <summary>
		/// NHibernate double type
		/// </summary>
		public static readonly NullableType Double = new DoubleType();
		
		/// <summary>
		/// NHibernate character type
		/// </summary>
		public static readonly NullableType Character = new CharacterType();
		
		/// <summary>
		/// NHibernate string type
		/// </summary>
		public static readonly NullableType String = new StringType();
		
		/// <summary>
		/// NHibernate time type
		/// </summary>
		//public static readonly NullableType Time = new TimeType();
		
		/// <summary>
		/// NHibernate date type
		/// </summary>
		public static readonly NullableType Date = new DateType();
		
		/// <summary>
		/// NHibernate timestamp type
		/// </summary>
		public static readonly NullableType Timestamp = new TimestampType();
		
		/// <summary>
		/// NHibernate boolean type
		/// </summary>
		public static readonly NullableType Boolean = new BooleanType();
		
		/// <summary>
		/// NHibernate true_false type
		/// </summary>
		public static readonly NullableType TrueFalse = new TrueFalseType();
		
		/// <summary>
		/// NHibernate yes_no type
		/// </summary>
		public static readonly NullableType YesNo = new YesNoType();

		/// <summary>
		/// NHibernate decimal type
		/// </summary>
		public static readonly NullableType Decimal = new DecimalType();
		
		/// <summary>
		/// NHibernate big_decimal type
		/// </summary>
		//public static readonly NullableType BigDecimal = new BigDecimalType();
	
		/// <summary>
		/// NHibernate binary type
		/// </summary>
		public static readonly NullableType Binary = new BinaryType();
		
		/// <summary>
		/// NHibernate blob type
		/// </summary>
		//public static readonly NullableType Blob = new BlobType();
		
		/// <summary>
		/// NHibernate clob type
		/// </summary>
		//public static readonly NullableType Clob = new ClobType();
		
		/// <summary>
		/// NHibernate calendar type
		/// </summary>
		//public static readonly NullableType Calendar = new CalendarType();
		
		/// <summary>
		/// NHibernate calendar_date type
		/// </summary>
		//public static readonly NullableType CalendarDate = new CalendarDateType();
		
		/// <summary>
		/// NHibernate CultureInfo type
		/// </summary>
		public static readonly NullableType CultureInfo = new CultureInfoType();
		
		/// <summary>
		/// NHibernate currency type
		/// </summary>
		//public static readonly NullableType Currency = new CurrencyType();
		
		/// <summary>
		/// NHibernate timezone type
		/// </summary>
		//public static readonly NullableType Timezone = new TimeZoneType();
		
		/// <summary>
		/// NHibernate class type
		/// </summary>
		public static readonly NullableType Class = new ClassType();
		
		/// <summary>
		/// NHibernate object type
		/// </summary>
		public static readonly IType Object = new ObjectType();
		
		
		/// <summary>
		/// NHibernate serializable type
		/// </summary>
		public static readonly NullableType Serializable = new SerializableType(typeof(object));

		
		/// <summary>
		/// Cannot be instantiated.
		/// </summary>
		private NHibernate() {																	  throw new NotSupportedException();											}
		
		
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
			return new CustomType(userTypeClass);
		}
				
		/* Needs HybernateProxyHelper and PersistentCollection implementation
		 *
		/// <summary>
		/// Force initialization of a proxy or persistent collection.
		/// </summary>
		/// <param name="proxy">a persistable object, proxy, persistent collection or null</param>
		/// <exception cref="HibernateException">if we can't initialize the proxy at this time, eg. the Session was closed</exception>
		public static void Initialize(object proxy) {
			if (proxy==null) {
				return;
			}
			else if ( proxy is IHibernateProxy ) {
				HibernateProxyHelper.GetLazyInitializer( (IHibernateProxy) proxy ).Initialize();
			}
			else if ( proxy is PersistentCollection ) {
				( (PersistentCollection) proxy ).ForceLoad();
			}
		}
		*/
	
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
