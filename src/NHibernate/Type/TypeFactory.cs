using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Type 
{

	/// <summary>
	/// Used internally to obtain instances of IType.
	/// </summary>
	/// <remarks>
	/// Applications should use static methods and constants on NHibernate.NHibernate if the default
	/// IType is good enough.  For example, the TypeFactory should only be used when the String needs
	/// to have a length of 300 instead of 255.  At this point NHibernate.String does not get you the 
	/// correct IType.  Instead use TypeFactory.GetString(300) and keep a local variable that holds
	/// a reference to the IType.
	/// </remarks>

	public class TypeFactory 
	{

		private enum TypeClassification 
		{
			Plain,
			Length,
			PrecisionScale
		}

		private static char[] precisionScaleSplit = new char[] {'(', ')', ','};
		private static char[] lengthSplit = new char[]{'(', ')'};

		/*
		 * Maps the string representation of the type to the IType.  The string 
		 * representation is how the type will appear in the mapping file and in
		 * name of the IType.Name.  The list below provides a few examples of how
		 * the Key/Value pair will be.
		 * 
		 * key -> value
		 * "DateTime" -> instance of DateTimeType
		 * "NHibernate.Type.DateTimeType" -> instance of DateTimeType
		 * "Decimal" -> instance of DecimalType with default p,s
		 * "NHibernate.Type.DecimalType" -> instance of DecimalType with default p,s
		 * "Decimal(p, s)" -> instance of DecimalType with specified p,s
		 * "NHibernate.Type.DecimalType(p, s)" -> instance of DecimalType with specified p,s
		 * "String" -> instance of StringType with default l
		 * "NHibernate.Type.StringType" -> instance of StringType with default l
		 * "String(l)" -> instance of StringType with specified l
		 * "NHibernate.Type.StringType(l)" -> instance of StringType with specified l
		 */ 
		private static Hashtable typeByTypeOfName = Hashtable.Synchronized(new Hashtable(79));
		
		private static Hashtable getTypeDelegatesWithLength = Hashtable.Synchronized(new Hashtable(7));
		private static Hashtable getTypeDelegatesWithPrecision = Hashtable.Synchronized(new Hashtable(3));

		private delegate NullableType GetNullableType();
		private delegate NullableType GetNullableTypeWithLength(int length);
		private delegate NullableType GetNullableTypeWithPrecision(byte precision, byte scale);


		static TypeFactory() 
		{
			
			
			//basicTypes.Add(NHibernate.Blob.Name, NHibernate.Blob);
			//basicTypes.Add(NHibernate.Clob.Name, NHibernate.Clob);
			
			//basicTypes.Add(NHibernate.Currency.Name, NHibernate.Currency);
			
			// don't need an ObjectType in the basicMap because object has 
			// been depreciated in favor of the <any> element in the mapping.  The
			// <any> element refers to basic types of custom types.
			//basicTypes.Add(typeof(object).Name, NHibernate.Object);
			//AddToBasicTypes(NHibernate.Object);

			// the Timezone class .NET is not even close to the java.util.Timezone class - in
			// .NET all you can do is get the local Timezone - there is no "factory" method to
			// get a Timezone by name...
			//basicTypes.Add(NHibernate.Timezone.Name, NHibernate.Timezone);
			
			TypeFactory.GetBinaryType(); 
			TypeFactory.GetBooleanType();
			TypeFactory.GetByteType();
			TypeFactory.GetCharacterType();
			TypeFactory.GetCultureInfoType();
			TypeFactory.GetDateTimeType(); 
			TypeFactory.GetDateType(); 
			TypeFactory.GetDecimalType();  
			TypeFactory.GetDoubleType(); 
			TypeFactory.GetInt16Type();
			TypeFactory.GetInt32Type();
			TypeFactory.GetInt64Type();
			TypeFactory.GetSerializableType(); 
			TypeFactory.GetSingleType(); 
			TypeFactory.GetStringType();  
			TypeFactory.GetTimestampType(); 
			TypeFactory.GetTimeType();
			TypeFactory.GetTrueFalseType(); 
			TypeFactory.GetTypeType(); 
			TypeFactory.GetYesNoType(); 
			TypeFactory.GetTicksType();

			getTypeDelegatesWithLength.Add(TypeFactory.GetBinaryType().Name, new GetNullableTypeWithLength(GetBinaryType));
			getTypeDelegatesWithLength.Add(TypeFactory.GetDoubleType().Name, new GetNullableTypeWithLength(GetDoubleType));
			getTypeDelegatesWithLength.Add(TypeFactory.GetSerializableType().Name, new GetNullableTypeWithLength(GetSerializableType));
			getTypeDelegatesWithLength.Add(TypeFactory.GetSingleType().Name, new GetNullableTypeWithLength(GetSingleType));
			getTypeDelegatesWithLength.Add(TypeFactory.GetStringType().Name, new GetNullableTypeWithLength(GetStringType));
			getTypeDelegatesWithLength.Add(TypeFactory.GetTypeType().Name, new GetNullableTypeWithLength(GetTypeType));
			
			getTypeDelegatesWithPrecision.Add(TypeFactory.GetDecimalType().Name, new GetNullableTypeWithPrecision(GetDecimalType));
			
		}
	

		private TypeFactory() { throw new NotSupportedException(); }
		
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
				indexOfComma = typeName.IndexOf(",", indexOfOpenParen);

			if(indexOfOpenParen >= 0) 
			{
				if(indexOfComma >= 0) return TypeClassification.PrecisionScale;
				else return TypeClassification.Length;
			}
			else 
				return TypeClassification.Plain;

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
			string typeName = String.Empty;

			// Use the basic name (such as String or String(255)) to get the
			// instance of the IType object.
			IType returnType = null;
			returnType = (IType)typeByTypeOfName[name];
			if(returnType!=null) 
				return returnType;

			// if we get to here then the basic type with the length or precision/scale
			// combination doesn't exists - so lets figure out which one we have and 
			// invoke the appropriate delegate
			TypeClassification typeClassification = GetTypeClassification(name);

			if(typeClassification==TypeClassification.PrecisionScale) 
			{

				//precision/scale based
				GetNullableTypeWithPrecision precisionDelegate;
				byte precision;
				byte scale;
		
				string[] parsedName =  name.Split(precisionScaleSplit);
				if(parsedName.Length < 4) 
					throw new ApplicationException("The name " + name + " is not a valid Precision/Scale name");

				typeName = parsedName[0].Trim();
				precision = Byte.Parse(parsedName[1].Trim());
				scale = Byte.Parse(parsedName[2].Trim());

				if(getTypeDelegatesWithPrecision.ContainsKey(typeName)==false)
					return null;
				
				precisionDelegate = (GetNullableTypeWithPrecision)getTypeDelegatesWithPrecision[typeName];
				return precisionDelegate(precision, scale);

			}
			else if (typeClassification==TypeClassification.Length) 
			{
				//length based
				GetNullableTypeWithLength lengthDelegate;
				int length;
					
				string[] parsedName =  name.Split(lengthSplit);
				if(parsedName.Length < 3) 
					throw new ApplicationException("The name " + name + " is not a valid Length name");

				typeName = parsedName[0].Trim();
				length = Int32.Parse(parsedName[1].Trim());

				if(getTypeDelegatesWithLength.ContainsKey(typeName)==false)
					// we were not able to find a delegate to get the Type
					return null;
				
				lengthDelegate = (GetNullableTypeWithLength)getTypeDelegatesWithLength[typeName];
				return lengthDelegate(length);
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

		private static NullableType AddToTypeOfName(string key, NullableType type) 
		{
			typeByTypeOfName.Add(key, type);
			typeByTypeOfName.Add(type.Name, type);
			return type;
		}

		private static NullableType AddToTypeOfNameWithLength(string key, NullableType type) 
		{
			typeByTypeOfName.Add(key, type);
			typeByTypeOfName.Add(GetKeyForLengthBased(type), type);
			return type;
		}

		private static NullableType AddToTypeOfNameWithPrecision(string key, NullableType type) 
		{
			typeByTypeOfName.Add(key, type);
			typeByTypeOfName.Add(GetKeyForPrecisionScaleBased(type), type);
			return type;
		}


		private static string GetKeyForLengthBased(IType type) 
		{
			NullableType nullableType = type as NullableType;
			if(nullableType!=null) 
			{
				return GetKeyForLengthBased(type.Name, nullableType.SqlType.Length);
			}
			else
			{
				throw new ApplicationException("Can't get a Key for a Length Based item of IType = " + type.Name);
			}
		}

		private static string GetKeyForLengthBased(string name, int length)
		{
			return name + "(" + length + ")";
		}

		private static string GetKeyForPrecisionScaleBased(IType type) 
		{
			NullableType nullableType = type as NullableType;
			if(nullableType!=null) 
				return GetKeyForPrecisionScaleBased(nullableType.Name, nullableType.SqlType.Precision, nullableType.SqlType.Scale);
			else
				throw new ApplicationException("Can't get a Key for a Precision Scale Based item of IType = " + type.Name);
		}

		private static string GetKeyForPrecisionScaleBased(string name, byte precision, byte scale) 
		{
			return name + "(" + precision + ", " + scale + ")";
		}



		/// <summary>
		/// Uses hueristics to deduce a NHibernate type given a string naming the 
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
		public static IType HueristicType(string typeName) 
		{
			IType type = TypeFactory.Basic(typeName);
			
			if (type==null) {

				string[] parsedTypeName;
				TypeClassification typeClassification = GetTypeClassification(typeName);
				if(typeClassification==TypeClassification.Length)
					parsedTypeName = typeName.Split(lengthSplit);
				else if (typeClassification == TypeClassification.PrecisionScale)
					parsedTypeName = typeName.Split(precisionScaleSplit);
				else
					parsedTypeName = new string[] {typeName};


				System.Type typeClass;
				try 
				{
					typeClass = ReflectHelper.ClassForName(parsedTypeName[0]); //typeName);
				}
				catch (Exception) 
				{
					typeClass = null;
				}

				if (typeClass!=null) 
				{
					if ( typeof(IType).IsAssignableFrom(typeClass) ) 
					{
						try 
						{
							type = (IType) Activator.CreateInstance(typeClass);
						}
						catch (Exception e) 
						{
							throw new MappingException("Could not instantiate IType " + typeClass.Name + ": " + e);
						}
					}
					else if ( typeof(ICompositeUserType).IsAssignableFrom(typeClass) ) 
					{
						type = new CompositeCustomType(typeClass);
					}
					else if ( typeof(IUserType).IsAssignableFrom(typeClass) ) 
					{
						type = new CustomType(typeClass);
					}
					else if ( typeof(ILifecycle).IsAssignableFrom(typeClass) ) 
					{
						type = NHibernate.Association(typeClass);
					}
					else if ( typeof(IPersistentEnum).IsAssignableFrom(typeClass) ) 
					{
						type = NHibernate.Enum(typeClass);
					}
					else if ( typeClass.IsSerializable ) 
					{
						if(typeClassification==TypeClassification.Length)
							type = GetSerializableType(typeClass, Int32.Parse(parsedTypeName[1]));
						else
							type = GetSerializableType(typeClass);
					}
				}
			}
			return type;
		}

		/// <summary>
		/// Gets the BinaryType with the default size.
		/// </summary>
		/// <returns>A BinaryType</returns>
		/// <remarks>
		/// <para>
		/// In addition to returning the BinaryType it will also ensure that it has
		/// been added to the basicNameMap with the keys <c>Byte[]</c> and 
		/// <c>NHibernate.Type.BinaryType</c>.
		/// </para>
		/// <para>
		/// Since this method calls the <see cref="GetBinaryType(Int32)">GetBinaryType(Int32)</see>
		/// with the default length those keys will also be added.
		/// </para>
		/// </remarks>
		public static NullableType GetBinaryType() 
		{
			string key = typeof(BinaryType).FullName;
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = GetBinaryType(255);
                AddToTypeOfName(key, returnType);
			}

			return returnType;

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
			string key = GetKeyForLengthBased(typeof(BinaryType).FullName, length);
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new BinaryType(SqlTypeFactory.GetBinary(length));
				AddToTypeOfNameWithLength(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetBooleanType() 
		{
			string key = typeof(BooleanType).FullName;
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new BooleanType(SqlTypeFactory.GetBoolean());
				AddToTypeOfName(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetByteType() 
		{
			string key = typeof(ByteType).FullName;
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new ByteType(SqlTypeFactory.GetByte());
				AddToTypeOfName(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetCharacterType() 
		{
			string key = GetKeyForLengthBased(typeof(CharacterType).FullName, 1);
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new CharacterType(SqlTypeFactory.GetStringFixedLength(1));
				AddToTypeOfName(key, returnType);
			}

			return returnType;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetCultureInfoType() 
		{
			string key = typeof(CultureInfoType).FullName;
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				//TODO: I believe 10 is still set way to high.  If I read the doco correctly
				// then at most it should be 5...
				returnType = new CultureInfoType(SqlTypeFactory.GetString(10));
				AddToTypeOfName(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetDateTimeType() 
		{
			string key = typeof(DateTimeType).FullName;
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new DateTimeType(SqlTypeFactory.GetDateTime());
				AddToTypeOfName(key, returnType);
			}

			return returnType;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetDateType() 
		{
			string key = typeof(DateType).FullName;
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new DateType(SqlTypeFactory.GetDate());
				AddToTypeOfName(key, returnType);
			}

			return returnType;
		}

		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetDecimalType() 
		{
			string key = typeof(DecimalType).FullName;
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = GetDecimalType(19,0);
				AddToTypeOfName(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="precision"></param>
		/// <param name="scale"></param>
		/// <returns></returns>
		public static NullableType GetDecimalType(byte precision, byte scale) 
		{			
			string key = GetKeyForPrecisionScaleBased(typeof(DecimalType).FullName, precision, scale);
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new DecimalType(SqlTypeFactory.GetDecimal(precision, scale));
				AddToTypeOfNameWithPrecision(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetDoubleType() 
		{
			string key = typeof(DoubleType).FullName;
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = GetDoubleType(53);
				AddToTypeOfName(key, returnType);
			}

			return returnType;

			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static NullableType GetDoubleType(int length) 
		{
			string key = GetKeyForLengthBased(typeof(DoubleType).FullName, length);
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new DoubleType(SqlTypeFactory.GetDouble(length));
				AddToTypeOfNameWithLength(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetInt16Type() 
		{
			string key = typeof(Int16Type).FullName;
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new Int16Type(SqlTypeFactory.GetInt16());
				AddToTypeOfName(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetInt32Type() 
		{

			string key = typeof(Int32Type).FullName;
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new Int32Type(SqlTypeFactory.GetInt32());
				AddToTypeOfName(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetInt64Type() 
		{
			string key = typeof(Int64Type).FullName;
			
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) {
				returnType = new Int64Type(SqlTypeFactory.GetInt64());
				AddToTypeOfName(key, returnType);
			}

			return returnType;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetSingleType() 
		{
			string key = typeof(SingleType).FullName;
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = GetSingleType(24);
				AddToTypeOfName(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static NullableType GetSingleType(int length) 
		{
			string key = GetKeyForLengthBased(typeof(SingleType).FullName, length);
			
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new SingleType(SqlTypeFactory.GetSingle(length));
				AddToTypeOfNameWithLength(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetSerializableType() 
		{
			string key = typeof(SerializableType).FullName;
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = GetSerializableType(255);
				AddToTypeOfName(key, returnType);
			}

			return returnType;
			
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
		public static NullableType GetSerializableType(System.Type serializableType) 
		{
			string key = serializableType.AssemblyQualifiedName;
			
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = GetSerializableType(serializableType, 255);
				AddToTypeOfName(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serializableType"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static NullableType GetSerializableType(System.Type serializableType, int length) 
		{
			string key = GetKeyForLengthBased(serializableType.AssemblyQualifiedName, length);
			
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new SerializableType(serializableType, SqlTypeFactory.GetBinary(length));
				AddToTypeOfNameWithLength(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static NullableType GetSerializableType(int length) 
		{
			string key = GetKeyForLengthBased(typeof(SerializableType).FullName, length);
			
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new SerializableType(typeof(object), SqlTypeFactory.GetBinary(length));
				AddToTypeOfNameWithLength(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetStringType() 
		{
			string key = typeof(StringType).FullName;
			
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = GetStringType(255);
				AddToTypeOfName(key, returnType);
			}

			return returnType;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static NullableType GetStringType(int length) 
		{
			string key = GetKeyForLengthBased(typeof(StringType).FullName, length);
			
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new StringType(SqlTypeFactory.GetString(length));
				AddToTypeOfNameWithLength(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetTicksType() 
		{
			
			string key = typeof(TicksType).FullName;
			
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new TicksType(SqlTypeFactory.GetInt64());
				AddToTypeOfName(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetTimestampType() 
		{			
			string key = typeof(TimestampType).FullName;
			
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new TimestampType(SqlTypeFactory.GetDateTime());
				AddToTypeOfName(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetTimeType()
		{
			string key = typeof(TimeType).FullName;
			
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new TimeType(SqlTypeFactory.GetTime());
				AddToTypeOfName(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetTrueFalseType()
		{
			string key = typeof(TrueFalseType).FullName;
			
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new TrueFalseType(SqlTypeFactory.GetAnsiStringFixedLength(1));
				AddToTypeOfName(key, returnType);
			}

			return returnType;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetTypeType() 
		{
			// this will add the default basic name
			// for the TypeType it will add this TypeType as Type and Type(255)
			string key = typeof(TypeType).FullName;
			
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = GetTypeType(255);
				AddToTypeOfName(key, returnType);
			}

			return returnType;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static NullableType GetTypeType(int length) 
		{
			string key = GetKeyForLengthBased(typeof(TypeType).FullName, length);
			
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new TypeType(SqlTypeFactory.GetString(length));
				AddToTypeOfNameWithLength(key, returnType);
			}

			return returnType;
			
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetYesNoType()
		{
			string key = typeof(YesNoType).FullName;
			
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new YesNoType(SqlTypeFactory.GetAnsiStringFixedLength(1));
				AddToTypeOfName(key, returnType);
			}

			return returnType;
			
		}

		
		// Association Types

		/// <summary>
		/// A one-to-one association type for the given class and cascade style.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="foreignKeyType"></param>
		/// <returns></returns>
		public static IType OneToOne(System.Type persistentClass, ForeignKeyType foreignKeyType) 
		{
			return new OneToOneType(persistentClass, foreignKeyType);
		}

		/// <summary>
		/// A many-to-one association type for the given class and cascade style.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <returns></returns>
		public static IType ManyToOne(System.Type persistentClass) 
		{
			return new ManyToOneType(persistentClass);
		}

		// Collection Types:
	
		
		public static PersistentCollectionType Array(string role, System.Type elementClass) 
		{
			return new ArrayType(role, elementClass);
		}
		
		public static PersistentCollectionType List(string role) 
		{
			return new ListType(role);
		}
		
		/*public static PersistentCollectionType Bag(string role) 
		 * {
			return new BagType(role);
		}*/
		
		public static PersistentCollectionType Map(string role) 
		{
			return new MapType(role);
		}
		
		public static PersistentCollectionType Set(string role) 
		{
			return new SetType(role);
		}
		/*
		public static PersistentCollectionType SortedMap(string role, IComparer comparer) 
		{
			return new SortedMapType(role, comparer);
		}*/

		public static PersistentCollectionType SortedSet(string role, IComparer comparer) 
		{
			return new SortedSetType(role, comparer);
		}
		

		/// <summary>
		/// Deep copy values in the first array into the second
		/// </summary>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <param name="copy"></param>
		/// <param name="target"></param>
		public static void DeepCopy(object[] values, IType[] types, bool[] copy, object[] target) 
		{
			for ( int i=0; i<types.Length; i++ ) 
			{
				if ( copy[i] ) target[i] = types[i].DeepCopy( values[i] );
			}
		}
	
		/// <summary>
		/// Determine if any of the given field values are dirty,
		/// returning an array containing indexes of
		/// the dirty fields or null if no fields are dirty.
		/// </summary>
		/// <param name="types"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="check"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public static int[] FindDirty(IType[] types, object[] x, object[] y, bool[] check, ISessionImplementor session) 
		{
			int[] results = null;
			int count = 0;
			for (int i=0; i<types.Length; i++) {
				if ( check[i] && types[i].IsDirty( x[i], y[i], session ) ) 
				{
					if (results==null) results = new int[ types.Length ];
					results[count++]=i;
				}
			}
			if (count==0) 
			{
				return null;
			}
			else 
			{
				int[] trimmed = new int[count];
				System.Array.Copy(results, 0, trimmed, 0, count);
				return trimmed;
			}
		}
	
		/*
		/// <summary>
		/// Return <tt>-1</tt> if non-dirty, or the index of the first dirty value otherwise
		/// </summary>
		/// <param name="types"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="owner"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public static int FindDirty(IType[] types, object[] x, object[] y, object owner, ISessionFactoryImplementor factory) {
			for (int i=0; i<types.Length; i++) {
				if ( types[i].IsDirty( x[i], y[i], owner, factory ) ) {
					return i;
				}
			}
			return -1;
		}
		*/
	}
}