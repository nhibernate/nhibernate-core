using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using NHibernate.Bytecode;
using NHibernate.Classic;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NHibernate.Util;
using System.Runtime.CompilerServices;

namespace NHibernate.Type
{
	/// <summary>
	/// Used internally to obtain instances of IType.
	/// </summary>
	/// <remarks>
	/// Applications should use static methods and constants on NHibernate.NHibernateUtil if the default
	/// IType is good enough.  For example, the TypeFactory should only be used when the String needs
	/// to have a length of 300 instead of 255.  At this point NHibernateUtil.String does not get you the
	/// correct IType.  Instead use TypeFactory.GetString(300) and keep a local variable that holds
	/// a reference to the IType.
	/// </remarks>
	public sealed class TypeFactory
	{
		private enum TypeClassification
		{
			Plain,
			Length,
			PrecisionScale
		}
		
		private static readonly string[] EmptyAliases= new string[0];
		private static readonly char[] PrecisionScaleSplit = new[] { '(', ')', ',' };
		private static readonly char[] LengthSplit = new[] { '(', ')' };
		private static readonly TypeFactory Instance;
		private static readonly System.Type[] GenericCollectionSimpleSignature = new[] { typeof(string), typeof(string), typeof(bool) };

		/*
		 * Maps the string representation of the type to the IType.  The string
		 * representation is how the type will appear in the mapping file and in
		 * name of the IType.Name.  The list below provides a few examples of how
		 * the Key/Value pair will be.
		 *
		 * key -> value
		 * "System.DateTime" -> instance of DateTimeType
		 * "System.DateTime, fully assembly name" -> instance of DateTimeType
		 * "DateTime" -> instance of DateTimeType
		 * "System.Decimal" -> instance of DecimalType with default p,s
		 * "Decimal" -> instance of DecimalType with default p,s
		 * "Decimal(p, s)" -> instance of DecimalType with specified p,s
		 * "String" -> instance of StringType with default l
		 * "System.String" -> instance of StringType with default l
		 * "NHibernate.Type.StringType" -> instance of StringType with default l
		 * "String(l)" -> instance of StringType with specified l
		 * "System.String(l)" -> instance of StringType with specified l
		 */

		private static readonly IDictionary<string, IType> typeByTypeOfName =
			new ThreadSafeDictionary<string, IType>(new Dictionary<string, IType>());

		private static readonly IDictionary<string, GetNullableTypeWithLength> getTypeDelegatesWithLength =
			new ThreadSafeDictionary<string, GetNullableTypeWithLength>(new Dictionary<string, GetNullableTypeWithLength>());

		private static readonly IDictionary<string, GetNullableTypeWithPrecision> getTypeDelegatesWithPrecision =
			new ThreadSafeDictionary<string, GetNullableTypeWithPrecision>(new Dictionary<string, GetNullableTypeWithPrecision>());

		private delegate NullableType GetNullableTypeWithLength(int length); // Func<int, NullableType>

		private delegate NullableType GetNullableTypeWithPrecision(byte precision, byte scale);

		private delegate NullableType NullableTypeCreatorDelegate(SqlType sqlType);

		private static void RegisterType(System.Type systemType, IType nhibernateType, IEnumerable<string> aliases)
		{
			var typeAliases = new List<string>(aliases);
			typeAliases.AddRange(GetClrTypeAliases(systemType));

			RegisterType(nhibernateType, typeAliases);
		}

		private static void RegisterType(System.Type systemType, IType nhibernateType,
			IEnumerable<string> aliases, GetNullableTypeWithLength ctorLength)
		{
			var typeAliases = new List<string>(aliases);
			typeAliases.AddRange(GetClrTypeAliases(systemType));

			RegisterType(nhibernateType, typeAliases, ctorLength);
		}

		private static void RegisterType(System.Type systemType, IType nhibernateType,
			IEnumerable<string> aliases, GetNullableTypeWithPrecision ctorPrecision)
		{
			var typeAliases = new List<string>(aliases);
			typeAliases.AddRange(GetClrTypeAliases(systemType));

			RegisterType(nhibernateType, typeAliases, ctorPrecision);
		}

		private static IEnumerable<string> GetClrTypeAliases(System.Type systemType)
		{
			var typeAliases = new List<string>
								{
									systemType.FullName,
														systemType.AssemblyQualifiedName,
								};
			if (systemType.IsValueType)
			{
				// Also register Nullable<systemType> for ValueTypes
				var nullableType = typeof(Nullable<>).MakeGenericType(systemType);
				typeAliases.Add(nullableType.FullName);
				typeAliases.Add(nullableType.AssemblyQualifiedName);
			}
			return typeAliases;
		}

		private static void RegisterType(IType nhibernateType, IEnumerable<string> aliases)
		{
			var typeAliases = new List<string>(aliases) { nhibernateType.Name };
			foreach (var alias in typeAliases)
			{
				typeByTypeOfName[alias] = nhibernateType;
			}
		}

		private static void RegisterType(IType nhibernateType, IEnumerable<string> aliases, GetNullableTypeWithLength ctorLength)
		{
			var typeAliases = new List<string>(aliases) { nhibernateType.Name };
			foreach (var alias in typeAliases)
			{
				typeByTypeOfName[alias] = nhibernateType;
				getTypeDelegatesWithLength.Add(alias, ctorLength);
			}
		}

		private static void RegisterType(IType nhibernateType, IEnumerable<string> aliases, GetNullableTypeWithPrecision ctorPrecision)
		{
			var typeAliases = new List<string>(aliases) { nhibernateType.Name };
			foreach (var alias in typeAliases)
			{
				typeByTypeOfName[alias] = nhibernateType;
				getTypeDelegatesWithPrecision.Add(alias, ctorPrecision);
			}
		}

		/// <summary></summary>
		static TypeFactory()
		{
			Instance = new TypeFactory();
			
			// set up the mappings of .NET Classes/Structs to their NHibernate types.
			RegisterDefaultNetTypes();

			// add the mappings of the NHibernate specific names that are used in type=""
			RegisterBuiltInTypes();
		}

		/// <summary>
		/// Register other Default .NET type
		/// </summary>
		/// <remarks>
		/// These type will be used, as default, even when the "type" attribute was NOT specified in the mapping
		/// </remarks>
		private static void RegisterDefaultNetTypes()
		{
			// NOTE : each .NET type mut appear only one time
			RegisterType(typeof (Byte[]), NHibernateUtil.Binary, new[] {"binary"},
						 l => GetType(NHibernateUtil.Binary, l, len => new BinaryType(SqlTypeFactory.GetBinary(len))));

			RegisterType(typeof(Boolean), NHibernateUtil.Boolean, new[] { "boolean", "bool" });
			RegisterType(typeof (Byte), NHibernateUtil.Byte, new[]{ "byte"});
			RegisterType(typeof (Char), NHibernateUtil.Character, new[] {"character", "char"});
			RegisterType(typeof (CultureInfo), NHibernateUtil.CultureInfo, new[]{ "locale"});
			RegisterType(typeof (DateTime), NHibernateUtil.DateTime, new[]{ "datetime"} );
			RegisterType(typeof (DateTimeOffset), NHibernateUtil.DateTimeOffset, new[]{ "datetimeoffset"});

			RegisterType(typeof (Decimal), NHibernateUtil.Decimal, new[] {"big_decimal", "decimal"},
						 (p, s) => GetType(NHibernateUtil.Decimal, p, s, st => new DecimalType(st)));

			RegisterType(typeof (Double), NHibernateUtil.Double, new[] {"double"},
						 (p, s) => GetType(NHibernateUtil.Double, p, s, st => new DoubleType(st)));

			RegisterType(typeof (Guid), NHibernateUtil.Guid, new[]{ "guid"});
			RegisterType(typeof (Int16), NHibernateUtil.Int16, new[]{ "short"});
			RegisterType(typeof (Int32), NHibernateUtil.Int32, new[] {"integer", "int"});
			RegisterType(typeof (Int64), NHibernateUtil.Int64, new[]{ "long"});
			RegisterType(typeof(SByte), NHibernateUtil.SByte, EmptyAliases);

			RegisterType(typeof (Single), NHibernateUtil.Single, new[] {"float", "single"},
						 (p, s) => GetType(NHibernateUtil.Single, p, s, st => new SingleType(st)));

			RegisterType(typeof (String), NHibernateUtil.String, new[] {"string"},
						 l => GetType(NHibernateUtil.String, l, len => new StringType(SqlTypeFactory.GetString(len))));

			RegisterType(typeof (TimeSpan), NHibernateUtil.TimeSpan, new[] {"timespan"});

			RegisterType(typeof (System.Type), NHibernateUtil.Class, new[] {"class"},
						 l => GetType(NHibernateUtil.Class, l, len => new TypeType(SqlTypeFactory.GetString(len))));

			RegisterType(typeof (UInt16), NHibernateUtil.UInt16, new[] {"ushort"});
			RegisterType(typeof (UInt32), NHibernateUtil.UInt32, new[] {"uint"});
			RegisterType(typeof (UInt64), NHibernateUtil.UInt64, new[] {"ulong"});

			RegisterType(typeof (XmlDocument), NHibernateUtil.XmlDoc, new[] {"xmldoc", "xmldocument", "xml"});
			
			RegisterType(typeof (Uri), NHibernateUtil.Uri, new[] {"uri", "url"});

			RegisterType(typeof(XDocument), NHibernateUtil.XDoc, new[] { "xdoc", "xdocument" });

			// object needs to have both class and serializable setup before it can
			// be created.
			RegisterType(typeof (Object), NHibernateUtil.Object, new[] {"object"});
		}

		/// <summary>
		/// Register other NO Default .NET type
		/// </summary>
		/// <remarks>
		/// These type will be used only when the "type" attribute was is specified in the mapping.
		/// These are in here because needed to NO override default CLR types and be available in mappings
		/// </remarks>
		private static void RegisterBuiltInTypes()
		{
			RegisterType(NHibernateUtil.AnsiString, EmptyAliases,
						 l => GetType(NHibernateUtil.AnsiString, l, len => new AnsiStringType(SqlTypeFactory.GetAnsiString(len))));

			RegisterType(NHibernateUtil.AnsiChar, EmptyAliases);
			
			RegisterType(NHibernateUtil.BinaryBlob, EmptyAliases,
						 l => GetType(NHibernateUtil.BinaryBlob, l, len => new BinaryBlobType(SqlTypeFactory.GetBinaryBlob(len))));
			
			RegisterType(NHibernateUtil.StringClob, EmptyAliases,
						 l => GetType(NHibernateUtil.StringClob, l, len => new StringClobType(SqlTypeFactory.GetStringClob(len))));
			
			RegisterType(NHibernateUtil.Date, new[] { "date" });
			RegisterType(NHibernateUtil.Timestamp, new[] { "timestamp" });
			RegisterType(NHibernateUtil.DbTimestamp, new[] { "dbtimestamp" });
			RegisterType(NHibernateUtil.Time, new[] { "time" });
			RegisterType(NHibernateUtil.TrueFalse, new[] { "true_false" });
			RegisterType(NHibernateUtil.YesNo, new[] { "yes_no" });
			RegisterType(NHibernateUtil.Ticks, new[] { "ticks" });
			RegisterType(NHibernateUtil.TimeAsTimeSpan, EmptyAliases);
			RegisterType(NHibernateUtil.LocalDateTime, new[] { "localdatetime" });
			RegisterType(NHibernateUtil.UtcDateTime, new[] { "utcdatetime" });
			
			RegisterType(NHibernateUtil.Currency, new[] { "currency" },
				(p, s) => GetType(NHibernateUtil.Currency, p, s, st => new CurrencyType(st)));
			
			RegisterType(NHibernateUtil.DateTime2, new[] { "datetime2" });
			RegisterType(NHibernateUtil.Serializable, new[] {"Serializable", "serializable"},
						 l =>
						 GetType(NHibernateUtil.Serializable, l,
								 len => new SerializableType(typeof (object), SqlTypeFactory.GetBinary(len))));
		}

		public ICollectionTypeFactory CollectionTypeFactory
		{
			get { return Cfg.Environment.BytecodeProvider.CollectionTypeFactory; }
		}

		private TypeFactory()
		{
		}

		/// <summary>
		/// Gets the classification of the Type based on the string.
		/// </summary>
		/// <param name="typeName">The name of the Type to get the classification for.</param>
		/// <returns>The Type of Classification</returns>
		/// <remarks>
		/// This parses through the string and makes the assumption that no class
		/// name and no assembly name will contain the <c>"("</c>.
		/// <para>
		/// If it finds
		/// the <c>"("</c> and then finds a <c>","</c> afterwards then it is a
		/// <c>TypeClassification.PrecisionScale</c>.
		/// </para>
		/// <para>
		/// If it finds the <c>"("</c>
		/// and doesn't find a <c>","</c> afterwards, then it is a
		/// <c>TypeClassification.Length</c>.
		/// </para>
		/// <para>
		/// If it doesn't find the <c>"("</c> then it assumes that it is a
		/// <c>TypeClassification.Plain</c>.
		/// </para>
		/// </remarks>
		private static TypeClassification GetTypeClassification(string typeName)
		{
			int indexOfOpenParen = typeName.IndexOf("(");
			int indexOfComma = 0;
			if (indexOfOpenParen >= 0)
			{
				indexOfComma = typeName.IndexOf(",", indexOfOpenParen);
			}

			if (indexOfOpenParen >= 0)
			{
				if (indexOfComma >= 0)
				{
					return TypeClassification.PrecisionScale;
				}
				else
				{
					return TypeClassification.Length;
				}
			}
			else
			{
				return TypeClassification.Plain;
			}
		}

		/// <summary>
		/// Given the name of a Hibernate type such as Decimal, Decimal(19,0)
		/// , Int32, or even NHibernate.Type.DecimalType, NHibernate.Type.DecimalType(19,0),
		/// NHibernate.Type.Int32Type, then return an instance of NHibernate.Type.IType
		/// </summary>
		/// <param name="name">The name of the type.</param>
		/// <returns>The instance of the IType that the string represents.</returns>
		/// <remarks>
		/// This method will return null if the name is not found in the basicNameMap.
		/// </remarks>
		public static IType Basic(string name)
		{
			string typeName;

			// Use the basic name (such as String or String(255)) to get the
			// instance of the IType object.
			IType returnType;
			if (typeByTypeOfName.TryGetValue(name, out returnType))
			{
				return returnType;
			}

			// if we get to here then the basic type with the length or precision/scale
			// combination doesn't exists - so lets figure out which one we have and
			// invoke the appropriate delegate
			TypeClassification typeClassification = GetTypeClassification(name);

			if (typeClassification == TypeClassification.PrecisionScale)
			{
				//precision/scale based

				string[] parsedName = name.Split(PrecisionScaleSplit);
				if (parsedName.Length < 4)
				{
					throw new ArgumentOutOfRangeException("TypeClassification.PrecisionScale", name,
																								"It is not a valid Precision/Scale name");
				}

				typeName = parsedName[0].Trim();
				byte precision = Byte.Parse(parsedName[1].Trim());
				byte scale = Byte.Parse(parsedName[2].Trim());

				return BuiltInType(typeName, precision, scale);
			}
			else if (typeClassification == TypeClassification.Length)
			{
				//length based

				string[] parsedName = name.Split(LengthSplit);
				if (parsedName.Length < 3)
				{
					throw new ArgumentOutOfRangeException("TypeClassification.Length", name, "It is not a valid Length name");
				}

				typeName = parsedName[0].Trim();
				int length = Int32.Parse(parsedName[1].Trim());

				return BuiltInType(typeName, length);
			}

			else
			{
				// it is not in the basicNameMap and typeByTypeOfName
				// nor is it a Length or Precision/Scale based type
				// so it must be a user defined type or something else that NHibernate
				// doesn't have built into it.
				return null;
			}
		}

		internal static IType BuiltInType(string typeName, int length)
		{
			GetNullableTypeWithLength lengthDelegate;

			return !getTypeDelegatesWithLength.TryGetValue(typeName, out lengthDelegate) ? null : lengthDelegate(length);
		}

		internal static IType BuiltInType(string typeName, byte precision, byte scale)
		{
			GetNullableTypeWithPrecision precisionDelegate;
			return !getTypeDelegatesWithPrecision.TryGetValue(typeName, out precisionDelegate)
					? null
					: precisionDelegate(precision, scale);
		}

		private static void AddToTypeOfName(string key, IType type)
		{
			typeByTypeOfName.Add(key, type);
			typeByTypeOfName.Add(type.Name, type);
		}

		private static void AddToTypeOfNameWithLength(string key, IType type)
		{
			typeByTypeOfName.Add(key, type);
		}

		private static void AddToTypeOfNameWithPrecision(string key, IType type)
		{
			typeByTypeOfName.Add(key, type);
		}

		private static string GetKeyForLengthBased(string name, int length)
		{
			return name + "(" + length + ")";
		}

		private static string GetKeyForPrecisionScaleBased(string name, byte precision, byte scale)
		{
			return name + "(" + precision + ", " + scale + ")";
		}

		/// <summary>
		/// Uses heuristics to deduce a NHibernate type given a string naming the
		/// type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns>An instance of <c>NHibernate.Type.IType</c></returns>
		/// <remarks>
		/// When looking for the NHibernate type it will look in the cache of the Basic types first.
		/// If it doesn't find it in the cache then it uses the typeName to get a reference to the
		/// Class (Type in .NET).  Once we get the reference to the .NET class we check to see if it
		/// implements IType, ICompositeUserType, IUserType, ILifecycle (Association), or
		/// IPersistentEnum.  If none of those are implemented then we will serialize the Type to the
		/// database using NHibernate.Type.SerializableType(typeName)
		/// </remarks>
		public static IType HeuristicType(string typeName)
		{
			return HeuristicType(typeName, null);
		}

		/// <summary>
		/// Uses heuristics to deduce a NHibernate type given a string naming the type.
		/// </summary>
		/// <param name="typeName">the type name</param>
		/// <param name="parameters">parameters for the type</param>
		/// <returns>An instance of <c>NHibernate.Type.IType</c></returns>
		public static IType HeuristicType(string typeName, IDictionary<string, string> parameters)
		{
			return HeuristicType(typeName, parameters, null);
		}
		
		/// <summary>
		/// Uses heuristics to deduce a NHibernate type given a string naming the type.
		/// </summary>
		/// <param name="typeName">the type name</param>
		/// <param name="parameters">parameters for the type</param>
		/// <param name="length">optionally, the size of the type</param>
		/// <returns></returns>
		public static IType HeuristicType(string typeName, IDictionary<string, string> parameters, int? length)
		{
			IType type = Basic(typeName);

			if (type != null)
				return type;
			
			string[] parsedTypeName;
			TypeClassification typeClassification = GetTypeClassification(typeName);
			if (typeClassification == TypeClassification.Length)
				parsedTypeName = typeName.Split(LengthSplit);
			else
				parsedTypeName = typeClassification == TypeClassification.PrecisionScale ? typeName.Split(PrecisionScaleSplit) : new[] { typeName };


			System.Type typeClass;
			try
			{
				typeClass = ReflectHelper.ClassForName(parsedTypeName[0]); //typeName);
			}
			catch (Exception)
			{
				typeClass = null;
			}

			if (typeClass == null)
				return null;
				
			if (typeof(IType).IsAssignableFrom(typeClass))
			{
				try
				{
					type = (IType) Cfg.Environment.BytecodeProvider.ObjectsFactory.CreateInstance(typeClass);
				}
				catch (Exception e)
				{
					throw new MappingException("Could not instantiate IType " + typeClass.Name + ": " + e, e);
				}
				InjectParameters(type, parameters);
				return type;
			}
			if (typeof(ICompositeUserType).IsAssignableFrom(typeClass))
			{
				return new CompositeCustomType(typeClass, parameters);
			}
			if (typeof(IUserType).IsAssignableFrom(typeClass))
			{
				return new CustomType(typeClass, parameters);
			}
			if (typeof(ILifecycle).IsAssignableFrom(typeClass))
			{
				return NHibernateUtil.Entity(typeClass);
			}

			var unwrapped = typeClass.UnwrapIfNullable();
			if (unwrapped.IsEnum)
			{
				try
				{
					return (IType) Activator.CreateInstance(typeof (EnumType<>).MakeGenericType(unwrapped));
				}
				catch (Exception e)
				{
					throw new MappingException(string.Format("Can't instantiate enum {0}; The enum can't be empty", typeClass.FullName), e);
				}
			}

			if (!typeClass.IsSerializable)
				return null;

			if (typeClassification == TypeClassification.Length)
				return GetSerializableType(typeClass, Int32.Parse(parsedTypeName[1]));
			
			if (length.HasValue)
				return GetSerializableType(typeClass, length.Value);

			return GetSerializableType(typeClass);
		}


		[MethodImpl(MethodImplOptions.Synchronized)]
		public static NullableType GetAnsiStringType(int length)
		{
			string key = GetKeyForLengthBased(NHibernateUtil.AnsiString.Name, length);

			IType returnType;
			if (!typeByTypeOfName.TryGetValue(key, out returnType))
			{
				returnType = new AnsiStringType(SqlTypeFactory.GetAnsiString(length));
				AddToTypeOfNameWithLength(key, returnType);
			}
			return (NullableType)returnType;
		}

		/// <summary>
		/// Gets the BinaryType with the specified length.
		/// </summary>
		/// <param name="length">The length of the data to store in the database.</param>
		/// <returns>A BinaryType</returns>
		/// <remarks>
		/// In addition to returning the BinaryType it will also ensure that it has
		/// been added to the basicNameMap with the keys <c>Byte[](length)</c> and
		/// <c>NHibernate.Type.BinaryType(length)</c>.
		/// </remarks>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static NullableType GetBinaryType(int length)
		{
			//HACK: don't understand why SerializableType calls this with length=0
			if (length == 0)
			{
				return NHibernateUtil.Binary;
			}

			string key = GetKeyForLengthBased(NHibernateUtil.Binary.Name, length);
			IType returnType;
			if (!typeByTypeOfName.TryGetValue(key, out returnType))
			{
				returnType = new BinaryType(SqlTypeFactory.GetBinary(length));
				AddToTypeOfNameWithLength(key, returnType);
			}

			return (NullableType)returnType;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static NullableType GetType(NullableType defaultUnqualifiedType, int length, GetNullableTypeWithLength ctorDelegate)
		{
			string key = GetKeyForLengthBased(defaultUnqualifiedType.Name, length);
			IType returnType;
			if (!typeByTypeOfName.TryGetValue(key, out returnType))
			{
				returnType = ctorDelegate(length);
				AddToTypeOfNameWithLength(key, returnType);
			}

			return (NullableType)returnType;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static NullableType GetType(NullableType defaultUnqualifiedType, byte precision, byte scale, NullableTypeCreatorDelegate ctor)
		{
			string key = GetKeyForPrecisionScaleBased(defaultUnqualifiedType.Name, precision, scale);
			IType returnType;
			if (!typeByTypeOfName.TryGetValue(key, out returnType))
			{
				returnType = ctor(SqlTypeFactory.GetSqlType(defaultUnqualifiedType.SqlType.DbType, precision, scale));
				AddToTypeOfNameWithPrecision(key, returnType);
			}

			return (NullableType)returnType;
		}

		/// <summary>
		/// Gets the SerializableType for the specified Type
		/// </summary>
		/// <param name="serializableType">The Type that will be Serialized to the database.</param>
		/// <returns>A SerializableType</returns>
		/// <remarks>
		/// <para>
		/// In addition to returning the SerializableType it will also ensure that it has
		/// been added to the basicNameMap with the keys <c>Type.FullName</c> (the result
		/// of <c>IType.Name</c> and <c>Type.AssemblyQualifiedName</c>.  This is different
		/// from the other items put in the basicNameMap because it is uses the AQN and the
		/// FQN as opposed to the short name used in the maps and the FQN.
		/// </para>
		/// <para>
		/// Since this method calls the method
		/// <see cref="GetSerializableType(System.Type, Int32)">GetSerializableType(System.Type, Int32)</see>
		/// with the default length, those keys will also be added.
		/// </para>
		/// </remarks>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static NullableType GetSerializableType(System.Type serializableType)
		{
			string key = serializableType.AssemblyQualifiedName;

			IType returnType;
			if (!typeByTypeOfName.TryGetValue(key, out returnType))
			{
				returnType = new SerializableType(serializableType);
				AddToTypeOfName(key, returnType);
			}

			return (NullableType)returnType;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static NullableType GetSerializableType(System.Type serializableType, int length)
		{
			string key = GetKeyForLengthBased(serializableType.AssemblyQualifiedName, length);

			IType returnType;
			if (!typeByTypeOfName.TryGetValue(key, out returnType))
			{
				returnType = new SerializableType(serializableType, SqlTypeFactory.GetBinary(length));
				AddToTypeOfNameWithLength(key, returnType);
			}

			return (NullableType)returnType;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static NullableType GetSerializableType(int length)
		{
			string key = GetKeyForLengthBased(NHibernateUtil.Serializable.Name, length);

			IType returnType;
			if (!typeByTypeOfName.TryGetValue(key, out returnType))
			{
				returnType = new SerializableType(typeof(object), SqlTypeFactory.GetBinary(length));
				AddToTypeOfNameWithLength(key, returnType);
			}

			return (NullableType)returnType;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static NullableType GetStringType(int length)
		{
			string key = GetKeyForLengthBased(NHibernateUtil.String.Name, length);

			IType returnType;
			if (!typeByTypeOfName.TryGetValue(key, out returnType))
			{
				returnType = new StringType(SqlTypeFactory.GetString(length));
				AddToTypeOfNameWithLength(key, returnType);
			}

			return (NullableType)returnType;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static NullableType GetTypeType(int length)
		{
			string key = GetKeyForLengthBased(typeof(TypeType).FullName, length);

			IType returnType;
			if (!typeByTypeOfName.TryGetValue(key, out returnType))
			{
				returnType = new TypeType(SqlTypeFactory.GetString(length));
				AddToTypeOfNameWithLength(key, returnType);
			}

			return (NullableType)returnType;
		}

		// Association Types

		/// <summary>
		/// A one-to-one association type for the given class and cascade style.
		/// </summary>
		public static EntityType OneToOne(string persistentClass, ForeignKeyDirection foreignKeyType, string uniqueKeyPropertyName,
			bool lazy, bool unwrapProxy, bool isEmbeddedInXML, string entityName, string propertyName)
		{
			return
				new OneToOneType(persistentClass, foreignKeyType, uniqueKeyPropertyName, lazy, unwrapProxy, isEmbeddedInXML,
												 entityName, propertyName);
		}

		/// <summary>
		/// A many-to-one association type for the given class and cascade style.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <returns></returns>
		public static EntityType ManyToOne(string persistentClass)
		{
			return new ManyToOneType(persistentClass);
		}

		/// <summary>
		/// A many-to-one association type for the given class and cascade style.
		/// </summary>
		public static EntityType ManyToOne(string persistentClass, bool lazy)
		{
			return new ManyToOneType(persistentClass, lazy);
		}

		/// <summary>
		/// A many-to-one association type for the given class and cascade style.
		/// </summary>
		public static EntityType ManyToOne(string persistentClass, string uniqueKeyPropertyName, bool lazy, bool unwrapProxy,
			bool isEmbeddedInXML, bool ignoreNotFound)
		{
			return new ManyToOneType(persistentClass, uniqueKeyPropertyName, lazy, unwrapProxy, isEmbeddedInXML, ignoreNotFound);
		}

		public static CollectionType Array(string role, string propertyRef, bool embedded, System.Type elementClass)
		{
			return Instance.CollectionTypeFactory.Array(role, propertyRef, embedded, elementClass);
		}


		public static CollectionType GenericBag(string role, string propertyRef, System.Type elementClass)
		{
			MethodInfo mi = ReflectHelper.GetGenericMethodFrom<ICollectionTypeFactory>("Bag", new[] {elementClass},
																						 GenericCollectionSimpleSignature);

			return (CollectionType)mi.Invoke(Instance.CollectionTypeFactory, new object[] { role, propertyRef, false });
		}

		public static CollectionType GenericIdBag(string role, string propertyRef, System.Type elementClass)
		{
			MethodInfo mi = ReflectHelper.GetGenericMethodFrom<ICollectionTypeFactory>("IdBag", new[] { elementClass },
																																									 GenericCollectionSimpleSignature);

			return (CollectionType)mi.Invoke(Instance.CollectionTypeFactory, new object[] { role, propertyRef, false });
		}

		public static CollectionType GenericList(string role, string propertyRef, System.Type elementClass)
		{
			MethodInfo mi = ReflectHelper.GetGenericMethodFrom<ICollectionTypeFactory>("List", new[] { elementClass },
																																									 GenericCollectionSimpleSignature);

			return (CollectionType)mi.Invoke(Instance.CollectionTypeFactory, new object[] { role, propertyRef, false });
		}

		public static CollectionType GenericMap(string role, string propertyRef, System.Type indexClass,
																						System.Type elementClass)
		{
			MethodInfo mi = ReflectHelper.GetGenericMethodFrom<ICollectionTypeFactory>("Map", new[] {indexClass, elementClass },
																																									 GenericCollectionSimpleSignature);

			return (CollectionType)mi.Invoke(Instance.CollectionTypeFactory, new object[] { role, propertyRef, false });
		}

		public static CollectionType GenericSortedList(string role, string propertyRef, object comparer,
																									 System.Type indexClass, System.Type elementClass)
		{
			var signature = new[] { typeof(string), typeof(string), typeof(bool), typeof(IComparer<>).MakeGenericType(indexClass) };
			MethodInfo mi = ReflectHelper.GetGenericMethodFrom<ICollectionTypeFactory>("SortedList", new[] { indexClass, elementClass },
																																									 signature);

			return (CollectionType)mi.Invoke(Instance.CollectionTypeFactory, new object[] { role, propertyRef, false, comparer });
		}

		public static CollectionType GenericSortedDictionary(string role, string propertyRef, object comparer,
																												 System.Type indexClass, System.Type elementClass)
		{
			var signature = new[] { typeof(string), typeof(string), typeof(bool), typeof(IComparer<>).MakeGenericType(indexClass) };
			MethodInfo mi = ReflectHelper.GetGenericMethodFrom<ICollectionTypeFactory>("SortedDictionary", new[] { indexClass, elementClass },
																																						 signature);

			return (CollectionType)mi.Invoke(Instance.CollectionTypeFactory, new object[] { role, propertyRef, false, comparer });
		}

		public static CollectionType GenericSet(string role, string propertyRef, System.Type elementClass)
		{
			MethodInfo mi = ReflectHelper.GetGenericMethodFrom<ICollectionTypeFactory>("Set", new[] { elementClass },
																																									 GenericCollectionSimpleSignature);

			return (CollectionType)mi.Invoke(Instance.CollectionTypeFactory, new object[] { role, propertyRef, false });
		}

		public static CollectionType GenericSortedSet(string role, string propertyRef, object comparer,
																									System.Type elementClass)
		{
			var signature = new[] { typeof(string), typeof(string), typeof(bool), typeof(IComparer<>).MakeGenericType(elementClass) };
			MethodInfo mi = ReflectHelper.GetGenericMethodFrom<ICollectionTypeFactory>("SortedSet", new[] { elementClass },
																																									 signature);

			return (CollectionType)mi.Invoke(Instance.CollectionTypeFactory, new object[] { role, propertyRef, false, comparer });
		}

		public static CollectionType GenericOrderedSet(string role, string propertyRef,
																									System.Type elementClass)
		{
			MethodInfo mi = ReflectHelper.GetGenericMethodFrom<ICollectionTypeFactory>("OrderedSet", new[] { elementClass },
																																									 GenericCollectionSimpleSignature);

			return (CollectionType)mi.Invoke(Instance.CollectionTypeFactory, new object[] { role, propertyRef, false });
		}

		public static CollectionType CustomCollection(string typeName, IDictionary<string, string> typeParameters,
			string role, string propertyRef, bool embedded)
		{
			System.Type typeClass;
			try
			{
				typeClass = ReflectHelper.ClassForName(typeName);
			}
			catch (Exception cnfe)
			{
				throw new MappingException("user collection type class not found: " + typeName, cnfe);
			}
			CustomCollectionType result = new CustomCollectionType(typeClass, role, propertyRef, embedded);
			if (typeParameters != null)
			{
				InjectParameters(result.UserType, typeParameters);
			}
			return result;
		}

		public static void InjectParameters(Object type, IDictionary<string, string> parameters)
		{
			if (type is IParameterizedType)
			{
				((IParameterizedType) type).SetParameterValues(parameters);
			}
			else if (parameters != null && !(parameters.Count == 0))
			{
				throw new MappingException("type is not parameterized: " + type.GetType().Name);
			}
		}
	}
}
