using System;
using NHibernate.Collection;
using NHibernate.Intercept;
using NHibernate.Proxy;
using NHibernate.Type;
using NHibernate.UserTypes;
using NHibernate.Util;

namespace NHibernate
{
	/// <summary>
	/// Provides access to the full range of NHibernate built-in types.
	/// IType instances may be used to bind values to query parameters.
	/// <see cref="TypeFactory" /> if needing to specify type size,
	/// precision or scale.
	/// </summary>
	public static partial class NHibernateUtil
	{
		/// <summary>
		/// Guesses the IType of this object
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns></returns>
		public static IType GuessType(object obj)
		{
			System.Type type = (obj as System.Type) ?? obj.GetType();
			return GuessType(type);
		}

		/// <summary>
		/// Guesses the IType by the type
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static IType GuessType(System.Type type)
		{
			type = type.UnwrapIfNullable();

			var value = TypeFactory.GetDefaultTypeFor(type);
			if (value != null)
				return value;
			
			if (type.IsEnum)
				return (IType) Activator.CreateInstance(typeof (EnumType<>).MakeGenericType(type));
			
			if (typeof(IUserType).IsAssignableFrom(type) ||
				typeof(ICompositeUserType).IsAssignableFrom(type))
			{
				return Custom(type);
			}

			return Entity(type);
		}

		/// <summary>
		/// NHibernate Ansi String type
		/// </summary>
		public static readonly AnsiStringType AnsiString = new AnsiStringType();

		/// <summary>
		/// NHibernate binary type
		/// </summary>
		public static readonly BinaryType Binary = new BinaryType();

		/// <summary>
		/// NHibernate binary blob type
		/// </summary>
		public static readonly BinaryBlobType BinaryBlob = new BinaryBlobType();

		/// <summary>
		/// NHibernate boolean type
		/// </summary>
		public static readonly BooleanType Boolean = new BooleanType();

		/// <summary>
		/// NHibernate byte type
		/// </summary>
		public static readonly ByteType Byte = new ByteType();

		/// <summary>
		/// NHibernate character type
		/// </summary>
		public static readonly CharType Character = new CharType();

		/// <summary>
		/// NHibernate Culture Info type
		/// </summary>
		public static readonly CultureInfoType CultureInfo = new CultureInfoType();

		/// <summary>
		/// NHibernate date time type. Since v5.0, does no more cut fractional seconds.
		/// </summary>
		/// <remarks>Use <see cref="DateTimeNoMs" /> if needing cutting milliseconds.</remarks>
		public static readonly DateTimeType DateTime = new DateTimeType();

		/// <summary>
		/// NHibernate date time cutting milliseconds type
		/// </summary>
		public static readonly DateTimeNoMsType DateTimeNoMs = new DateTimeNoMsType();

		// Obsolete since v5.0
		/// <summary>
		/// NHibernate date time 2 type
		/// </summary>
		[Obsolete("Use DateTimeType instead, it uses DateTime2 with dialects supporting it.")]
		public static readonly DateTime2Type DateTime2 = new DateTime2Type();

		/// <summary>
		/// NHibernate local date time type
		/// </summary>
		public static readonly LocalDateTimeType LocalDateTime = new LocalDateTimeType();

		/// <summary>
		/// NHibernate utc date time type
		/// </summary>
		public static readonly UtcDateTimeType UtcDateTime = new UtcDateTimeType();

		/// <summary>
		/// NHibernate local date time cutting milliseconds type
		/// </summary>
		public static readonly LocalDateTimeNoMsType LocalDateTimeNoMs = new LocalDateTimeNoMsType();

		/// <summary>
		/// NHibernate utc date time cutting milliseconds type
		/// </summary>
		public static readonly UtcDateTimeNoMsType UtcDateTimeNoMs = new UtcDateTimeNoMsType();

		/// <summary>
		/// NHibernate date time with offset type
		/// </summary>
		public static readonly DateTimeOffsetType DateTimeOffset = new DateTimeOffsetType();

		/// <summary>
		/// NHibernate date type
		/// </summary>
		public static readonly DateType Date = new DateType();

		/// <summary>
		/// NHibernate decimal type
		/// </summary>
		public static readonly DecimalType Decimal = new DecimalType();

		/// <summary>
		/// NHibernate double type
		/// </summary>
		public static readonly DoubleType Double = new DoubleType();

		/// <summary>
		/// NHibernate Currency type (System.Decimal - DbType.Currency)
		/// </summary>
		public static readonly CurrencyType Currency = new CurrencyType();

		/// <summary>
		/// NHibernate Guid type.
		/// </summary>
		public static readonly GuidType Guid = new GuidType();

		/// <summary>
		/// NHibernate System.Int16 (short in C#) type
		/// </summary>
		public static readonly Int16Type Int16 = new Int16Type();

		/// <summary>
		/// NHibernate System.Int32 (int in C#) type
		/// </summary>
		public static readonly Int32Type Int32 = new Int32Type();

		/// <summary>
		/// NHibernate System.Int64 (long in C#) type
		/// </summary>
		public static readonly Int64Type Int64 = new Int64Type();

		/// <summary>
		/// NHibernate System.SByte type
		/// </summary>
		public static readonly SByteType SByte = new SByteType();

		/// <summary>
		/// NHibernate System.UInt16 (ushort in C#) type
		/// </summary>
		public static readonly UInt16Type UInt16 = new UInt16Type();

		/// <summary>
		/// NHibernate System.UInt32 (uint in C#) type
		/// </summary>
		public static readonly UInt32Type UInt32 = new UInt32Type();

		/// <summary>
		/// NHibernate System.UInt64 (ulong in C#) type
		/// </summary>
		public static readonly UInt64Type UInt64 = new UInt64Type();

		/// <summary>
		/// NHibernate System.Single (float in C#) Type
		/// </summary>
		public static readonly SingleType Single = new SingleType();

		/// <summary>
		/// NHibernate String type
		/// </summary>
		public static readonly StringType String = new StringType();

		/// <summary>
		/// NHibernate string clob type
		/// </summary>
		public static readonly StringClobType StringClob = new StringClobType();

		/// <summary>
		/// NHibernate Time type
		/// </summary>
		public static readonly TimeType Time = new TimeType();

		/// <summary>
		/// NHibernate Ticks type
		/// </summary>
		public static readonly TicksType Ticks = new TicksType();

		/// <summary>
		/// NHibernate UTC Ticks type
		/// </summary>
		public static readonly UtcTicksType UtcTicks = new UtcTicksType();

		/// <summary>
		/// NHibernate TimeAsTimeSpan type
		/// </summary>
		public static readonly TimeAsTimeSpanType TimeAsTimeSpan = new TimeAsTimeSpanType();

		/// <summary>
		/// NHibernate TimeSpan type
		/// </summary>
		public static readonly TimeSpanType TimeSpan = new TimeSpanType();

		// Obsolete since v5.0
		/// <summary>
		/// NHibernate Timestamp type
		/// </summary>
		[Obsolete("Use DateTime instead.")]
		public static readonly TimestampType Timestamp = new TimestampType();

		/// <summary>
		/// NHibernate Timestamp type, seeded db side.
		/// </summary>
		public static readonly DbTimestampType DbTimestamp = new DbTimestampType();

		/// <summary>
		/// NHibernate Timestamp type, seeded db side, in UTC.
		/// </summary>
		public static readonly UtcDbTimestampType UtcDbTimestamp = new UtcDbTimestampType();

		/// <summary>
		/// NHibernate TrueFalse type
		/// </summary>
		public static readonly TrueFalseType TrueFalse = new TrueFalseType();

		/// <summary>
		/// NHibernate YesNo type
		/// </summary>
		public static readonly YesNoType YesNo = new YesNoType();

		/// <summary>
		/// NHibernate class type
		/// </summary>
		public static readonly TypeType Class = new TypeType();

		/// <summary>
		/// NHibernate class meta type for association of kind <code>any</code>.
		/// </summary>
		/// <seealso cref="AnyType"/>
		[Obsolete("Use MetaType without meta-values instead.")]
		public static readonly ClassMetaType ClassMetaType = new ClassMetaType();

		/// <summary>
		/// NHibernate meta type for association of kind <code>any</code> without meta-values.
		/// </summary>
		/// <seealso cref="AnyType"/>
		public static readonly MetaType MetaType = new MetaType(null, String);

		/// <summary>
		/// NHibernate serializable type
		/// </summary>
		public static readonly SerializableType Serializable = new SerializableType();

		/// <summary>
		/// NHibernate System.Object type
		/// </summary>
		public static readonly AnyType Object = new AnyType();

		//		/// <summary>
		//		/// NHibernate blob type
		//		/// </summary>
		//		public static readonly NullableType Blob = new BlobType();
		//		/// <summary>
		//		/// NHibernate clob type
		//		/// </summary>
		//		public static readonly NullableType Clob = new ClobType();

		/// <summary>
		/// NHibernate AnsiChar type
		/// </summary>
		public static readonly AnsiCharType AnsiChar = new AnsiCharType();

		/// <summary>
		/// NHibernate XmlDoc type
		/// </summary>
		public static readonly XmlDocType XmlDoc = new XmlDocType();

		/// <summary>
		/// NHibernate XDoc type
		/// </summary>
		public static readonly XDocType XDoc = new XDocType();

		/// <summary>
		/// NHibernate Uri type
		/// </summary>
		public static readonly UriType Uri = new UriType();

		/// <summary>
		/// A NHibernate persistent enum type
		/// </summary>
		/// <param name="enumClass"></param>
		/// <returns></returns>
		public static IType Enum(System.Type enumClass)
		{
			return new PersistentEnumType(enumClass);
		}

		/// <summary>
		/// A NHibernate serializable type
		/// </summary>
		/// <param name="serializableClass"></param>
		/// <returns></returns>
		public static IType GetSerializable(System.Type serializableClass)
		{
			return new SerializableType(serializableClass);
		}

		/// <summary>
		/// A NHibernate serializable type
		/// </summary>
		/// <param name="metaType">a type mapping <see cref="IType"/> to a single column</param>
		/// <param name="identifierType">the entity identifier type</param>
		/// <returns></returns>
		public static IType Any(IType metaType, IType identifierType)
		{
			return new AnyType(metaType, identifierType);
		}

		/// <summary>
		/// A NHibernate persistent object (entity) type
		/// </summary>
		/// <param name="persistentClass">a mapped entity class</param>
		/// <returns></returns>
		public static IType Entity(System.Type persistentClass)
		{
			// not really a many-to-one association *necessarily*
			return new ManyToOneType(persistentClass.FullName);
		}

		/// <summary> A Hibernate persistent object (entity) type. </summary>
		/// <param name="entityName">a mapped entity class </param>
		public static IType Entity(string entityName)
		{
			// not really a many-to-one association *necessarily*
			return new ManyToOneType(entityName);
		}
		/// <summary>
		/// A NHibernate custom type
		/// </summary>
		/// <param name="userTypeClass">a class that implements UserType</param>
		/// <returns></returns>
		public static IType Custom(System.Type userTypeClass)
		{
			if (typeof(ICompositeUserType).IsAssignableFrom(userTypeClass))
			{
				return new CompositeCustomType(userTypeClass, null);
			}
			else
			{
				return new CustomType(userTypeClass, null);
			}
		}

		/// <summary>
		/// Force initialization of a proxy or persistent collection.
		/// </summary>
		/// <param name="proxy">a persistable object, proxy, persistent collection or null</param>
		/// <exception cref="HibernateException">if we can't initialize the proxy at this time, eg. the Session was closed</exception>
		public static void Initialize(object proxy)
		{
			if (proxy == null)
			{
				return;
			}
			if (proxy.IsProxy())
			{
				((INHibernateProxy)proxy).HibernateLazyInitializer.Initialize();
			}
			else if (proxy is ILazyInitializedCollection coll)
			{
				coll.ForceInitialization();
			}
			// 6.0 TODO: remove once IPersistentCollection derives from ILazyInitializedCollection
			else if (proxy is IPersistentCollection persistent)
			{
				persistent.ForceInitialization();
			}
		}

		/// <summary>
		/// Is the proxy or persistent collection initialized?
		/// </summary>
		/// <param name="proxy">a persistable object, proxy, persistent collection or null</param>
		/// <returns>true if the argument is already initialized, or is not a proxy or collection</returns>
		public static bool IsInitialized(object proxy)
		{
			if (proxy.IsProxy())
			{
				return !((INHibernateProxy)proxy).HibernateLazyInitializer.IsUninitialized;
			}
			else if (proxy is ILazyInitializedCollection coll)
			{
				return coll.WasInitialized;
			}
			// 6.0 TODO: remove once IPersistentCollection derives from ILazyInitializedCollection
			else if (proxy is IPersistentCollection persistent)
			{
				return persistent.WasInitialized;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Get the true, underlying class of a proxied persistent class. This operation
		/// will initialize a proxy by side-effect.
		/// </summary>
		/// <param name="proxy">a persistable object or proxy</param>
		/// <returns>the true class of the instance</returns>
		public static System.Type GetClass(object proxy)
		{
			if (proxy.IsProxy())
			{
				return ((INHibernateProxy)proxy).HibernateLazyInitializer.GetImplementation().GetType();
			}
			else
			{
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
		public static Blob CreateBlob(TextReader stream, int length) {
			return new BlobImpl(stream, length);
		}
		
		/// <summary>
		/// Create a new Blob. The returned object will be initially immutable.
		/// </summary>
		/// <param name="stream">a binary stream</param>
		/// <param name="length">the number of bytes in the stream</param>
		/// <returns></returns>
		public static Blob CreateBlob(BinaryReader stream, int length) 
		{
			return new BlobImpl(stream, length);
		}

		/// <summary>
		/// Create a new Blob. The returned object will be initially immutable.
		/// </summary>
		/// <param name="stream">a binary stream</param>
		/// <returns></returns>
		public static Blob CreateBlob(StreamReader stream) 
		{
			return new BlobImpl( stream, stream.available() );
		}
		
		/// <summary>
		/// Create a new Blob. The returned object will be initially immutable.
		/// </summary>
		/// <param name="stream">a binary stream</param>
		/// <returns></returns>
		public static Blob CreateBlob(BinaryReader stream) 
		{
			return new BlobImpl( stream, stream.available() );
		}

		/// <summary>
		/// Create a new Clob. The returned object will be
		/// initially immutable.
		/// </summary>
		/// <param name="str">a String</param>
		/// <returns></returns>
		public static Clob CreateClob(string str) 
		{
			return new ClobImpl(str);
		}

		/// <summary>
		/// Create a new Clob. The returned object will be initially immutable.
		/// </summary>
		/// <param name="reader">a character stream</param>
		/// <param name="length">the number of characters in the stream</param>
		/// <returns></returns>
		public static Clob CreateClob(TextReader reader, int length) {
			return new ClobImpl(reader, length);
		}
		*/

		/// <summary>
		/// Close an <see cref="IEnumerator" /> obtained from an <see cref="IEnumerable" />
		/// returned by NHibernate immediately, instead of waiting until the session is
		/// closed or disconnected.
		/// </summary>
		public static void Close(IEnumerator enumerator)
		{
			var hibernateEnumerator = enumerator as IDisposable;
			if (hibernateEnumerator == null)
			{
				throw new ArgumentException("Not a NHibernate enumerator", nameof(enumerator));
			}
			hibernateEnumerator.Dispose();
		}

		/// <summary>
		/// Close an <see cref="IEnumerable" /> returned by NHibernate immediately,
		/// instead of waiting until the session is closed or disconnected.
		/// </summary>
		public static void Close(IEnumerable enumerable)
		{
			var hibernateEnumerable = enumerable as IDisposable;
			if (hibernateEnumerable == null)
			{
				throw new ArgumentException("Not a NHibernate enumerable", nameof(enumerable));
			}
			hibernateEnumerable.Dispose();
		}

		/// <summary> 
		/// Check if the property is initialized. If the named property does not exist
		/// or is not persistent, this method always returns <tt>true</tt>. 
		/// </summary>
		/// <param name="proxy">The potential proxy </param>
		/// <param name="propertyName">the name of a persistent attribute of the object </param>
		/// <returns> 
		/// true if the named property of the object is not listed as uninitialized;
		/// false if the object is an uninitialized proxy, or the named property is uninitialized 
		/// </returns>
		public static bool IsPropertyInitialized(object proxy, string propertyName)
		{
			object entity;
			if (proxy.IsProxy())
			{
				ILazyInitializer li = ((INHibernateProxy)proxy).HibernateLazyInitializer;
				if (li.IsUninitialized)
				{
					return false;
				}
				else
				{
					entity = li.GetImplementation();
				}
			}
			else
			{
				entity = proxy;
			}

			if (entity is IFieldInterceptorAccessor interceptorAccessor)
			{
				var interceptor = interceptorAccessor.FieldInterceptor;
				return interceptor == null || interceptor.IsInitializedField(propertyName);
			}
			else
			{
				return true;
			}
		}
	}
}
