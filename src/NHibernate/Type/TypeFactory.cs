using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Classic;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

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
	public static class TypeFactory
	{
		private enum TypeClassification
		{
			Plain,
			LengthOrScale,
			PrecisionScale
		}

		public static readonly string[] EmptyAliases = System.Array.Empty<string>();

		private static readonly INHibernateLogger _log = NHibernateLogger.For(typeof(TypeFactory));
		private static readonly char[] PrecisionScaleSplit = { '(', ')', ',' };
		private static readonly char[] LengthSplit = { '(', ')' };

		private static readonly MethodInfo BagDefinition = ReflectHelper.GetMethodDefinition<ICollectionTypeFactory>(
			f => f.Bag<object>(null, null));
		private static readonly MethodInfo IdBagDefinition = ReflectHelper.GetMethodDefinition<ICollectionTypeFactory>(
			f => f.IdBag<object>(null, null));
		private static readonly MethodInfo ListDefinition = ReflectHelper.GetMethodDefinition<ICollectionTypeFactory>(
			f => f.List<object>(null, null));
		private static readonly MethodInfo MapDefinition = ReflectHelper.GetMethodDefinition<ICollectionTypeFactory>(
			f => f.Map<object, object>(null, null));
		private static readonly MethodInfo SortedListDefinition = ReflectHelper.GetMethodDefinition<ICollectionTypeFactory>(
			f => f.SortedList<object, object>(null, null, null));
		private static readonly MethodInfo SortedDictionaryDefinition = ReflectHelper.GetMethodDefinition<ICollectionTypeFactory>(
			f => f.SortedDictionary<object, object>(null, null, null));
		private static readonly MethodInfo SetDefinition = ReflectHelper.GetMethodDefinition<ICollectionTypeFactory>(
			f => f.Set<object>(null, null));
		private static readonly MethodInfo SortedSetDefinition = ReflectHelper.GetMethodDefinition<ICollectionTypeFactory>(
			f => f.SortedSet<object>(null, null, null));
		private static readonly MethodInfo OrderedSetDefinition = ReflectHelper.GetMethodDefinition<ICollectionTypeFactory>(
			f => f.OrderedSet<object>(null, null));

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

		private static readonly ConcurrentDictionary<string, IType> typeByTypeOfName =
			new ConcurrentDictionary<string, IType>();

		private static readonly ConcurrentDictionary<string, string> _obsoleteMessageByAlias =
			new ConcurrentDictionary<string, string>();

		private static readonly ConcurrentDictionary<string, GetNullableTypeWithLengthOrScale> _getTypeDelegatesWithLengthOrScale =
			new ConcurrentDictionary<string, GetNullableTypeWithLengthOrScale>();

		private static readonly ConcurrentDictionary<string, GetNullableTypeWithPrecision> getTypeDelegatesWithPrecision =
			new ConcurrentDictionary<string, GetNullableTypeWithPrecision>();

		private delegate NullableType GetNullableTypeWithLengthOrScale(int lengthOrScale); // Func<int, NullableType>

		private delegate NullableType GetNullableTypeWithPrecision(byte precision, byte scale);

		private delegate NullableType NullableTypeCreatorDelegate(SqlType sqlType);

		/// <summary>
		/// <para>Defines which NHibernate type should be chosen by default for handling a given .Net type.</para>
		/// <para>This must be done before any operation on NHibernate, including building its
		/// <see cref="Configuration" /> and building session factory. Otherwise the behavior will be undefined.</para>
		/// </summary>
		/// <param name="systemType">The .Net type.</param>
		/// <param name="nhibernateType">The NHibernate type.</param>
		/// <param name="aliases">The additional aliases to map to the type. Use <see cref="EmptyAliases"/> if none.</param>
		public static void RegisterType(System.Type systemType, IType nhibernateType, IEnumerable<string> aliases)
		{
			var typeAliases = new List<string>(aliases);
			typeAliases.AddRange(GetClrTypeAliases(systemType));

			RegisterType(nhibernateType, typeAliases);
		}

		private static void RegisterType(System.Type systemType, IType nhibernateType,
			IEnumerable<string> aliases, GetNullableTypeWithLengthOrScale ctorLengthOrScale)
		{
			var typeAliases = new List<string>(aliases);
			typeAliases.AddRange(GetClrTypeAliases(systemType));

			RegisterType(nhibernateType, typeAliases, ctorLengthOrScale);
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
			var typeAliases =
				new List<string>
				{
					systemType.FullName,
					systemType.AssemblyQualifiedName
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
				RegisterTypeAlias(nhibernateType, alias);
			}
		}

		private static void RegisterType(IType nhibernateType, IEnumerable<string> aliases, GetNullableTypeWithLengthOrScale ctorLengthOrScale)
		{
			var typeAliases = new List<string>(aliases) { nhibernateType.Name };
			foreach (var alias in typeAliases)
			{
				RegisterTypeAlias(nhibernateType, alias);
				if (!_getTypeDelegatesWithLengthOrScale.TryAdd(alias, ctorLengthOrScale))
				{
					throw new HibernateException("An item with the same key has already been added to getTypeDelegatesWithLength.");
				}
			}
		}

		private static void RegisterType(IType nhibernateType, IEnumerable<string> aliases, GetNullableTypeWithPrecision ctorPrecision)
		{
			var typeAliases = new List<string>(aliases) { nhibernateType.Name };
			foreach (var alias in typeAliases)
			{
				RegisterTypeAlias(nhibernateType, alias);
				if (!getTypeDelegatesWithPrecision.TryAdd(alias, ctorPrecision))
				{
					throw new HibernateException("An item with the same key has already been added to getTypeDelegatesWithPrecision.");
				}
			}
		}

		private static void RegisterTypeAlias(IType nhibernateType, string alias)
		{
			typeByTypeOfName[alias] = nhibernateType;
			// Ignore obsolete search for aliases which are to be remapped to other types.
			switch (alias)
			{
				case "timestamp":
				case "Timestamp":
					return;
			}
			var obsolete = nhibernateType.GetType().GetCustomAttribute<ObsoleteAttribute>(false);
			if (obsolete != null)
			{
				_obsoleteMessageByAlias[alias] = obsolete.Message;
			}
		}

		/// <summary></summary>
		static TypeFactory()
		{
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
			// NOTE: each .NET type should appear only one time
			RegisterType(typeof (Byte[]), NHibernateUtil.Binary, new[] {"binary"},
						 l => GetType(NHibernateUtil.Binary, l, len => new BinaryType(SqlTypeFactory.GetBinary(len))));

			RegisterType(typeof(Boolean), NHibernateUtil.Boolean, new[] { "boolean", "bool" });
			RegisterType(typeof (Byte), NHibernateUtil.Byte, new[]{ "byte"});
			RegisterType(typeof (Char), NHibernateUtil.Character, new[] {"character", "char"});
			RegisterType(typeof (CultureInfo), NHibernateUtil.CultureInfo, new[]{ "locale"});
			RegisterType(typeof(DateTime), NHibernateUtil.DateTime, new[] { "datetime" },
				s => GetType(NHibernateUtil.DateTime, s, scale => new DateTimeType(SqlTypeFactory.GetDateTime((byte)scale))));
			RegisterType(typeof (DateTimeOffset), NHibernateUtil.DateTimeOffset, new[]{ "datetimeoffset"},
				s => GetType(NHibernateUtil.DateTimeOffset, s, scale => new DateTimeOffsetType(SqlTypeFactory.GetDateTimeOffset((byte)scale))));

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

			RegisterType(NHibernateUtil.DateTimeNoMs, new[] { "datetimenoms" });
			RegisterType(NHibernateUtil.Date, new[] { "date" });
#pragma warning disable 618 // Timestamp is obsolete
			RegisterType(NHibernateUtil.Timestamp, new[] { "timestamp" });
#pragma warning restore 618
			RegisterType(NHibernateUtil.DbTimestamp, new[] { "dbtimestamp" });
			RegisterType(NHibernateUtil.Time, new[] { "time" },
				s => GetType(NHibernateUtil.Time, s, scale => new TimeType(SqlTypeFactory.GetTime((byte)scale))));
			RegisterType(NHibernateUtil.TrueFalse, new[] { "true_false" });
			RegisterType(NHibernateUtil.YesNo, new[] { "yes_no" });
			RegisterType(NHibernateUtil.Ticks, new[] { "ticks" });
			RegisterType(NHibernateUtil.UtcTicks, new[] { "utcticks" });
			RegisterType(NHibernateUtil.TimeAsTimeSpan, new[] { "timeastimespan" },
				s => GetType(NHibernateUtil.TimeAsTimeSpan, s, scale => new TimeAsTimeSpanType(SqlTypeFactory.GetTime((byte)scale))));
			RegisterType(NHibernateUtil.LocalDateTime, new[] { "localdatetime" },
				s => GetType(NHibernateUtil.LocalDateTime, s, scale => new LocalDateTimeType(SqlTypeFactory.GetDateTime((byte)scale))));
			RegisterType(NHibernateUtil.UtcDateTime, new[] { "utcdatetime" },
				s => GetType(NHibernateUtil.UtcDateTime, s, scale => new UtcDateTimeType(SqlTypeFactory.GetDateTime((byte)scale))));
			RegisterType(NHibernateUtil.LocalDateTimeNoMs, new[] { "localdatetimenoms" });
			RegisterType(NHibernateUtil.UtcDateTimeNoMs, new[] { "utcdatetimenoms" });

			RegisterType(NHibernateUtil.Currency, new[] { "currency" },
				(p, s) => GetType(NHibernateUtil.Currency, p, s, st => new CurrencyType(st)));

#pragma warning disable 618 // DateTime2 is obsolete
			RegisterType(NHibernateUtil.DateTime2, new[] { "datetime2" },
				s => GetType(NHibernateUtil.DateTime2, s, scale => new DateTime2Type(SqlTypeFactory.GetDateTime2((byte)scale))));
#pragma warning restore 618
			RegisterType(NHibernateUtil.Serializable, new[] {"Serializable", "serializable"},
						 l =>
						 GetType(NHibernateUtil.Serializable, l,
								 len => new SerializableType(typeof (object), SqlTypeFactory.GetBinary(len))));
		}

		private static ICollectionTypeFactory CollectionTypeFactory =>
			Environment.BytecodeProvider.CollectionTypeFactory;

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
			var indexOfOpenParen = typeName.IndexOf("(", StringComparison.Ordinal);
			var indexOfComma = 0;
			if (indexOfOpenParen >= 0)
			{
				indexOfComma = typeName.IndexOf(",", indexOfOpenParen, StringComparison.Ordinal);
			}

			if (indexOfOpenParen >= 0)
			{
				if (indexOfComma >= 0)
				{
					return TypeClassification.PrecisionScale;
				}
				else
				{
					return TypeClassification.LengthOrScale;
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
			return Basic(name, null);
		}

		/// <summary>
		/// Given the name of a Hibernate type such as Decimal, Decimal(19,0),
		/// Int32, or even NHibernate.Type.DecimalType, NHibernate.Type.DecimalType(19,0),
		/// NHibernate.Type.Int32Type, then return an instance of NHibernate.Type.IType
		/// </summary>
		/// <param name="name">The name of the type.</param>
		/// <param name="parameters">The parameters for the type, if any.</param>
		/// <returns>The instance of the IType that the string represents.</returns>
		/// <remarks>
		/// This method will return null if the name is not found in the basicNameMap.
		/// </remarks>
		public static IType Basic(string name, IDictionary<string, string> parameters)
		{
			string typeName;

			// Use the basic name (such as String or String(255)) to get the
			// instance of the IType object.
			IType returnType;
			if (typeByTypeOfName.TryGetValue(name, out returnType))
			{
				if (_obsoleteMessageByAlias.TryGetValue(name, out string obsoleteMessage))
					_log.Warn("{0} is obsolete. {1}", name, obsoleteMessage);

				if (parameters?.Count > 0 && returnType is IParameterizedType)
				{
					// The type is parameterized, must apply the parameters to a new instance of the type.
					// Some built-in types have internal default constructor like StringType, so we need to
					// allow non-public constructors.
					returnType = (IType) Activator.CreateInstance(returnType.GetType(), true);
					InjectParameters(returnType, parameters);
				}
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
					throw new ArgumentOutOfRangeException(
						"TypeClassification.PrecisionScale", name, "It is not a valid Precision/Scale name");
				}

				typeName = parsedName[0].Trim();
				byte precision = Byte.Parse(parsedName[1].Trim());
				byte scale = Byte.Parse(parsedName[2].Trim());

				returnType = BuiltInType(typeName, precision, scale);
			}
			else if (typeClassification == TypeClassification.LengthOrScale)
			{
				//length or scale based

				string[] parsedName = name.Split(LengthSplit);
				if (parsedName.Length < 3)
				{
					throw new ArgumentOutOfRangeException(
						"TypeClassification.LengthOrScale", name, "It is not a valid Length or Scale name");
				}

				typeName = parsedName[0].Trim();
				int length = Int32.Parse(parsedName[1].Trim());

				returnType = BuiltInType(typeName, length);
			}

			else
			{
				// it is not in the basicNameMap and typeByTypeOfName
				// nor is it a Length or Precision/Scale based type
				// so it must be a user defined type or something else that NHibernate
				// doesn't have built into it.
				return null;
			}

			InjectParameters(returnType, parameters);
			return returnType;
		}

		internal static IType BuiltInType(string typeName, int lengthOrScale)
		{
			GetNullableTypeWithLengthOrScale lengthOrScaleDelegate;

			return !_getTypeDelegatesWithLengthOrScale.TryGetValue(typeName, out lengthOrScaleDelegate) ? null : lengthOrScaleDelegate(lengthOrScale);
		}

		internal static IType BuiltInType(string typeName, byte precision, byte scale)
		{
			GetNullableTypeWithPrecision precisionDelegate;
			return !getTypeDelegatesWithPrecision.TryGetValue(typeName, out precisionDelegate)
					? null
					: precisionDelegate(precision, scale);
		}

		private static string GetKeyForLengthOrScaleBased(string name, int lengthOrScale)
		{
			return name + "(" + lengthOrScale + ")";
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
			IType type = Basic(typeName, parameters);

			if (type != null)
				return type;
			
			string[] parsedTypeName;
			TypeClassification typeClassification = GetTypeClassification(typeName);
			if (typeClassification == TypeClassification.LengthOrScale)
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
					type = (IType) Environment.ObjectsFactory.CreateInstance(typeClass);
				}
				catch (Exception e)
				{
					throw new MappingException("Could not instantiate IType " + typeClass.Name + ": " + e, e);
				}
				InjectParameters(type, parameters);

				var obsolete = typeClass.GetCustomAttribute<ObsoleteAttribute>(false);
				if (obsolete != null)
				{
					_log.Warn("{0} is obsolete. {1}", typeName, obsolete.Message);
				}
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
				return (IType) Activator.CreateInstance(typeof (EnumType<>).MakeGenericType(unwrapped));
			}

			if (!typeClass.IsSerializable)
				return null;

			if (typeClassification == TypeClassification.LengthOrScale)
				return GetSerializableType(typeClass, Int32.Parse(parsedTypeName[1]));
			
			if (length.HasValue)
				return GetSerializableType(typeClass, length.Value);

			return GetSerializableType(typeClass);
		}

		/// <summary>
		/// Get the current default NHibernate type for a .Net type.
		/// </summary>
		/// <param name="type">The .Net type for which to get the corresponding default NHibernate type.</param>
		/// <returns>The current default NHibernate type for a .Net type if any, otherwise <see langword="null" />.</returns>
		public static IType GetDefaultTypeFor(System.Type type)
		{
			return typeByTypeOfName.TryGetValue(type.FullName, out var nhType) ? nhType : null;
		}

		public static NullableType GetAnsiStringType(int length)
		{
			var key = GetKeyForLengthOrScaleBased(NHibernateUtil.AnsiString.Name, length);
			return (NullableType)typeByTypeOfName.GetOrAdd(key, k => new AnsiStringType(SqlTypeFactory.GetAnsiString(length)));
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
		public static NullableType GetBinaryType(int length)
		{
			//HACK: don't understand why SerializableType calls this with length=0
			if (length == 0)
			{
				return NHibernateUtil.Binary;
			}

			var key = GetKeyForLengthOrScaleBased(NHibernateUtil.Binary.Name, length);
			return (NullableType)typeByTypeOfName.GetOrAdd(key, k => new BinaryType(SqlTypeFactory.GetBinary(length)));
		}

		private static NullableType GetType(NullableType defaultUnqualifiedType, int lengthOrScale, GetNullableTypeWithLengthOrScale ctorDelegate)
		{
			var key = GetKeyForLengthOrScaleBased(defaultUnqualifiedType.Name, lengthOrScale);
			return (NullableType)typeByTypeOfName.GetOrAdd(key, k => ctorDelegate(lengthOrScale));
		}

		private static NullableType GetType(NullableType defaultUnqualifiedType, byte precision, byte scale, NullableTypeCreatorDelegate ctor)
		{
			var key = GetKeyForPrecisionScaleBased(defaultUnqualifiedType.Name, precision, scale);
			return (NullableType)typeByTypeOfName.GetOrAdd(key, k => ctor(SqlTypeFactory.GetSqlType(defaultUnqualifiedType.SqlType.DbType, precision, scale)));
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
		/// </remarks>
		public static NullableType GetSerializableType(System.Type serializableType)
		{
			var key = serializableType.AssemblyQualifiedName;

			// The value factory may be run concurrently, but only one resulting value will be yielded to all threads.
			// So we should add the type with its other key in a later operation in order to ensure we cache the same
			// instance for both keys.
			var added = false;
			var type = (NullableType)typeByTypeOfName.GetOrAdd(
				key,
				k =>
				{
					var returnType = new SerializableType(serializableType);
					added = true;
					return returnType;
				});
			if (added && typeByTypeOfName.GetOrAdd(type.Name, type) != type)
			{
				throw new HibernateException($"Another item with the key {type.Name} has already been added to typeByTypeOfName.");
			}

			return type;
		}

		public static NullableType GetSerializableType(System.Type serializableType, int length)
		{
			var key = GetKeyForLengthOrScaleBased(serializableType.AssemblyQualifiedName, length);
			return (NullableType)typeByTypeOfName.GetOrAdd(key, k => new SerializableType(serializableType, SqlTypeFactory.GetBinary(length)));
		}

		public static NullableType GetSerializableType(int length)
		{
			var key = GetKeyForLengthOrScaleBased(NHibernateUtil.Serializable.Name, length);
			return (NullableType)typeByTypeOfName.GetOrAdd(key, k => new SerializableType(typeof(object), SqlTypeFactory.GetBinary(length)));
		}

		public static NullableType GetStringType(int length)
		{
			var key = GetKeyForLengthOrScaleBased(NHibernateUtil.String.Name, length);
			return (NullableType)typeByTypeOfName.GetOrAdd(key, k => new StringType(SqlTypeFactory.GetString(length)));
		}

		public static NullableType GetTypeType(int length)
		{
			var key = GetKeyForLengthOrScaleBased(typeof(TypeType).FullName, length);
			return (NullableType)typeByTypeOfName.GetOrAdd(key, k => new TypeType(SqlTypeFactory.GetString(length)));
		}

		/// <summary>
		/// Gets a <see cref="DateTimeType" /> with desired fractional seconds precision.
		/// </summary>
		/// <param name="fractionalSecondsPrecision">The fractional seconds precision.</param>
		/// <returns>The NHibernate type.</returns>
		public static NullableType GetDateTimeType(byte fractionalSecondsPrecision)
		{
			var key = GetKeyForLengthOrScaleBased(NHibernateUtil.DateTime.Name, fractionalSecondsPrecision);
			return (NullableType)typeByTypeOfName.GetOrAdd(key, k => new DateTimeType(SqlTypeFactory.GetDateTime(fractionalSecondsPrecision)));
		}

		/// <summary>
		/// Gets a <see cref="DateTime2Type" /> with desired fractional seconds precision.
		/// </summary>
		/// <param name="fractionalSecondsPrecision">The fractional seconds precision.</param>
		/// <returns>The NHibernate type.</returns>
		// Since v5.0
		[Obsolete("Use GetDateTimeType instead, it uses DateTime2 with dialects supporting it.")]
		public static NullableType GetDateTime2Type(byte fractionalSecondsPrecision)
		{
			var key = GetKeyForLengthOrScaleBased(NHibernateUtil.DateTime2.Name, fractionalSecondsPrecision);
			return (NullableType)typeByTypeOfName.GetOrAdd(key, k => new DateTime2Type(SqlTypeFactory.GetDateTime2(fractionalSecondsPrecision)));
		}

		/// <summary>
		/// Gets a <see cref="LocalDateTimeType" /> with desired fractional seconds precision.
		/// </summary>
		/// <param name="fractionalSecondsPrecision">The fractional seconds precision.</param>
		/// <returns>The NHibernate type.</returns>
		public static NullableType GetLocalDateTimeType(byte fractionalSecondsPrecision)
		{
			var key = GetKeyForLengthOrScaleBased(NHibernateUtil.LocalDateTime.Name, fractionalSecondsPrecision);
			return (NullableType)typeByTypeOfName.GetOrAdd(key, k => new LocalDateTimeType(SqlTypeFactory.GetDateTime(fractionalSecondsPrecision)));
		}

		/// <summary>
		/// Gets a <see cref="UtcDateTimeType" /> with desired fractional seconds precision.
		/// </summary>
		/// <param name="fractionalSecondsPrecision">The fractional seconds precision.</param>
		/// <returns>The NHibernate type.</returns>
		public static NullableType GetUtcDateTimeType(byte fractionalSecondsPrecision)
		{
			var key = GetKeyForLengthOrScaleBased(NHibernateUtil.UtcDateTime.Name, fractionalSecondsPrecision);
			return (NullableType)typeByTypeOfName.GetOrAdd(key, k => new UtcDateTimeType(SqlTypeFactory.GetDateTime(fractionalSecondsPrecision)));
		}

		/// <summary>
		/// Gets a <see cref="DateTimeOffsetType" /> with desired fractional seconds precision.
		/// </summary>
		/// <param name="fractionalSecondsPrecision">The fractional seconds precision.</param>
		/// <returns>The NHibernate type.</returns>
		public static NullableType GetDateTimeOffsetType(byte fractionalSecondsPrecision)
		{
			var key = GetKeyForLengthOrScaleBased(NHibernateUtil.DateTimeOffset.Name, fractionalSecondsPrecision);
			return (NullableType)typeByTypeOfName.GetOrAdd(key, k => new DateTimeOffsetType(SqlTypeFactory.GetDateTimeOffset(fractionalSecondsPrecision)));
		}

		/// <summary>
		/// Gets a <see cref="TimeAsTimeSpanType" /> with desired fractional seconds precision.
		/// </summary>
		/// <param name="fractionalSecondsPrecision">The fractional seconds precision.</param>
		/// <returns>The NHibernate type.</returns>
		public static NullableType GetTimeAsTimeSpanType(byte fractionalSecondsPrecision)
		{
			var key = GetKeyForLengthOrScaleBased(NHibernateUtil.TimeAsTimeSpan.Name, fractionalSecondsPrecision);
			return (NullableType)typeByTypeOfName.GetOrAdd(key, k => new TimeAsTimeSpanType(SqlTypeFactory.GetTime(fractionalSecondsPrecision)));
		}

		/// <summary>
		/// Gets a <see cref="TimeType" /> with desired fractional seconds precision.
		/// </summary>
		/// <param name="fractionalSecondsPrecision">The fractional seconds precision.</param>
		/// <returns>The NHibernate type.</returns>
		public static NullableType GetTimeType(byte fractionalSecondsPrecision)
		{
			var key = GetKeyForLengthOrScaleBased(NHibernateUtil.Time.Name, fractionalSecondsPrecision);
			return (NullableType)typeByTypeOfName.GetOrAdd(key, k => new TimeType(SqlTypeFactory.GetTime(fractionalSecondsPrecision)));
		}

		// Association Types

		/// <summary>
		/// A one-to-one association type for the given class and cascade style.
		/// </summary>
		public static EntityType OneToOne(string persistentClass, ForeignKeyDirection foreignKeyType, string uniqueKeyPropertyName,
			bool lazy, bool unwrapProxy, string entityName, string propertyName)
		{
			return
				new OneToOneType(
					persistentClass, foreignKeyType, uniqueKeyPropertyName, lazy, unwrapProxy, entityName, propertyName);
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
		public static EntityType ManyToOne(string persistentClass, string uniqueKeyPropertyName, bool lazy, bool unwrapProxy, bool ignoreNotFound, bool isLogicalOneToOne)
		{
			return new ManyToOneType(persistentClass, uniqueKeyPropertyName, lazy, unwrapProxy, ignoreNotFound, isLogicalOneToOne);
		}

		public static CollectionType Array(string role, string propertyRef, System.Type elementClass)
		{
			return CollectionTypeFactory.Array(role, propertyRef, elementClass);
		}


		public static CollectionType GenericBag(string role, string propertyRef, System.Type elementClass)
		{
			MethodInfo mi = BagDefinition.MakeGenericMethod(elementClass);

			return (CollectionType)mi.Invoke(CollectionTypeFactory, new object[] { role, propertyRef });
		}

		public static CollectionType GenericIdBag(string role, string propertyRef, System.Type elementClass)
		{
			MethodInfo mi = IdBagDefinition.MakeGenericMethod(elementClass);

			return (CollectionType)mi.Invoke(CollectionTypeFactory, new object[] { role, propertyRef });
		}

		public static CollectionType GenericList(string role, string propertyRef, System.Type elementClass)
		{
			MethodInfo mi = ListDefinition.MakeGenericMethod(elementClass);

			return (CollectionType)mi.Invoke(CollectionTypeFactory, new object[] { role, propertyRef });
		}

		public static CollectionType GenericMap(string role, string propertyRef, System.Type indexClass, System.Type elementClass)
		{
			MethodInfo mi = MapDefinition.MakeGenericMethod(indexClass, elementClass);

			return (CollectionType)mi.Invoke(CollectionTypeFactory, new object[] { role, propertyRef });
		}

		public static CollectionType GenericSortedList(string role, string propertyRef, object comparer, System.Type indexClass, System.Type elementClass)
		{
			MethodInfo mi = SortedListDefinition.MakeGenericMethod(indexClass, elementClass);

			return (CollectionType)mi.Invoke(CollectionTypeFactory, new[] { role, propertyRef, comparer });
		}

		public static CollectionType GenericSortedDictionary(string role, string propertyRef, object comparer, System.Type indexClass, System.Type elementClass)
		{
			MethodInfo mi = SortedDictionaryDefinition.MakeGenericMethod(indexClass, elementClass);

			return (CollectionType)mi.Invoke(CollectionTypeFactory, new[] { role, propertyRef, comparer });
		}

		public static CollectionType GenericSet(string role, string propertyRef, System.Type elementClass)
		{
			MethodInfo mi = SetDefinition.MakeGenericMethod(elementClass);

			return (CollectionType)mi.Invoke(CollectionTypeFactory, new object[] { role, propertyRef });
		}

		public static CollectionType GenericSortedSet(string role, string propertyRef, object comparer, System.Type elementClass)
		{
			MethodInfo mi = SortedSetDefinition.MakeGenericMethod(elementClass);

			return (CollectionType)mi.Invoke(CollectionTypeFactory, new[] { role, propertyRef, comparer });
		}

		public static CollectionType GenericOrderedSet(string role, string propertyRef, System.Type elementClass)
		{
			MethodInfo mi = OrderedSetDefinition.MakeGenericMethod(elementClass);

			return (CollectionType)mi.Invoke(CollectionTypeFactory, new object[] { role, propertyRef });
		}

		public static CollectionType CustomCollection(string typeName, IDictionary<string, string> typeParameters, string role, string propertyRef)
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
			CustomCollectionType result = new CustomCollectionType(typeClass, role, propertyRef);
			if (typeParameters != null)
			{
				InjectParameters(result.UserType, typeParameters);
			}
			return result;
		}

		public static void InjectParameters(Object type, IDictionary<string, string> parameters)
		{
			if (type is IParameterizedType parameterizedType)
			{
				parameterizedType.SetParameterValues(parameters);
			}
			else if (parameters != null && parameters.Count != 0)
			{
				throw new MappingException("type is not parameterized: " + type.GetType().Name);
			}
		}
	}
}
