using System;
using System.Collections;
using System.Globalization;

using NHibernate;
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
		 * "System.DateTime" -> instance of DateTimeType
		 * "System.DateTime, fully assembly name" -> instance of DateTimeType
		 * "DateTime" -> instance of DateTimeType
		 * "System.Decimal" -> instance of DecimaType with default p,s
		 * "Decimal" -> instance of DecimalType with default p,s
		 * "Decimal(p, s)" -> instance of DecimalType with specified p,s
		 * "String" -> instance of StringType with default l
		 * "System.String" -> instance of StringType with default l
		 * "NHibernate.Type.StringType" -> instance of StringType with default l
		 * "String(l)" -> instance of StringType with specified l
		 * "System.String(l)" -> instance of StringType with specified l
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
			
			// the Timezone class .NET is not even close to the java.util.Timezone class - in
			// .NET all you can do is get the local Timezone - there is no "factory" method to
			// get a Timezone by name...
			//basicTypes.Add(NHibernate.Timezone.Name, NHibernate.Timezone);
			
			// set up the mappings of .NET Classes/Structs to their NHibernate types.
			typeByTypeOfName[ typeof(System.Byte[]).Name ] = NHibernate.Binary ;
			typeByTypeOfName[ typeof(System.Byte[]).AssemblyQualifiedName ] = NHibernate.Binary ;
			typeByTypeOfName[ typeof(System.Boolean).FullName ] = NHibernate.Boolean;
			typeByTypeOfName[ typeof(System.Boolean).AssemblyQualifiedName ] = NHibernate.Boolean;
			typeByTypeOfName[ typeof(System.Byte).FullName ] = NHibernate.Byte;
			typeByTypeOfName[ typeof(System.Byte).AssemblyQualifiedName ] = NHibernate.Byte;
			typeByTypeOfName[ typeof(System.Char).FullName ] = NHibernate.Character;
			typeByTypeOfName[ typeof(System.Char).AssemblyQualifiedName ] = NHibernate.Character;
			typeByTypeOfName[ typeof(System.Globalization.CultureInfo).FullName ] = NHibernate.CultureInfo;
			typeByTypeOfName[ typeof(System.Globalization.CultureInfo).AssemblyQualifiedName ] = NHibernate.CultureInfo;
			typeByTypeOfName[ typeof(System.DateTime).FullName ] = NHibernate.DateTime;
			typeByTypeOfName[ typeof(System.DateTime).AssemblyQualifiedName ] = NHibernate.DateTime;
			typeByTypeOfName[ typeof(System.Decimal).FullName ] = NHibernate.Decimal;
			typeByTypeOfName[ typeof(System.Decimal).AssemblyQualifiedName ] = NHibernate.Decimal;
			typeByTypeOfName[ typeof(System.Double).FullName ] = NHibernate.Double;
			typeByTypeOfName[ typeof(System.Double).AssemblyQualifiedName ] = NHibernate.Double;
			typeByTypeOfName[ typeof(System.Guid).FullName ] = NHibernate.Guid;
			typeByTypeOfName[ typeof(System.Guid).AssemblyQualifiedName ] = NHibernate.Guid;
			typeByTypeOfName[ typeof(System.Int16).FullName ] = NHibernate.Int16;
			typeByTypeOfName[ typeof(System.Int16).AssemblyQualifiedName ] = NHibernate.Int16;
			typeByTypeOfName[ typeof(System.Int32).FullName ] = NHibernate.Int32;
			typeByTypeOfName[ typeof(System.Int32).AssemblyQualifiedName ] = NHibernate.Int32;
			typeByTypeOfName[ typeof(System.Int64).FullName ] = NHibernate.Int64;
			typeByTypeOfName[ typeof(System.Int64).AssemblyQualifiedName ] = NHibernate.Int64;
			typeByTypeOfName[ typeof(System.Single).FullName ] = NHibernate.Single;
			typeByTypeOfName[ typeof(System.Single).AssemblyQualifiedName ] = NHibernate.Single;
			typeByTypeOfName[ typeof(System.String).FullName ] = NHibernate.String;
			typeByTypeOfName[ typeof(System.String).AssemblyQualifiedName ] = NHibernate.String;
			typeByTypeOfName[ typeof(System.TimeSpan).FullName ] = NHibernate.TimeSpan;
			typeByTypeOfName[ typeof(System.TimeSpan).AssemblyQualifiedName ] = NHibernate.TimeSpan;
			
			// add the mappings of the NHibernate specific names that are used in type=""
			typeByTypeOfName[ NHibernate.AnsiString.Name ] = NHibernate.AnsiString ;
			typeByTypeOfName[ NHibernate.Binary.Name ] = NHibernate.Binary ;
			typeByTypeOfName[ NHibernate.BinaryBlob.Name ] = NHibernate.BinaryBlob;
			typeByTypeOfName[ NHibernate.Boolean.Name ] = NHibernate.Boolean;
			typeByTypeOfName[ NHibernate.Byte.Name ] = NHibernate.Byte;
			typeByTypeOfName[ NHibernate.Character.Name ] = NHibernate.Character;
			typeByTypeOfName[ NHibernate.StringClob.Name ] = NHibernate.StringClob;
			typeByTypeOfName[ NHibernate.CultureInfo.Name ] = NHibernate.CultureInfo;
			typeByTypeOfName[ NHibernate.DateTime.Name ] = NHibernate.DateTime;
			typeByTypeOfName[ NHibernate.Date.Name ] = NHibernate.Date;
			typeByTypeOfName[ NHibernate.Decimal.Name ] = NHibernate.Decimal;
			typeByTypeOfName[ NHibernate.Double.Name ] = NHibernate.Double;
			typeByTypeOfName[ NHibernate.Guid.Name ] = NHibernate.Guid;
			typeByTypeOfName[ NHibernate.Int16.Name ] = NHibernate.Int16;
			typeByTypeOfName[ NHibernate.Int32.Name ] = NHibernate.Int32;
			typeByTypeOfName[ NHibernate.Int64.Name ] = NHibernate.Int64;
			typeByTypeOfName[ NHibernate.String.Name ] = NHibernate.String;
			typeByTypeOfName[ NHibernate.Single.Name ] = NHibernate.Single;
			typeByTypeOfName[ NHibernate.Timestamp.Name ] = NHibernate.Timestamp;
			typeByTypeOfName[ NHibernate.Time.Name ] = NHibernate.Time;
			typeByTypeOfName[ NHibernate.TrueFalse.Name ] = NHibernate.TrueFalse;
			typeByTypeOfName[ NHibernate.YesNo.Name ] = NHibernate.YesNo;
			typeByTypeOfName[ NHibernate.Ticks.Name ] = NHibernate.Ticks;
			typeByTypeOfName[ NHibernate.TimeSpan.Name ] = NHibernate.TimeSpan;
			
			typeByTypeOfName[ NHibernate.Class.Name ] = NHibernate.Class;
			typeByTypeOfName[ typeof(System.Type).FullName ] = NHibernate.Class;
			typeByTypeOfName[ typeof(System.Type).AssemblyQualifiedName ] = NHibernate.Class;

			// need to do add the key "Serializable" because the hbm files will have a 
			// type="Serializable", but the SerializableType returns the Name as 
			// "serializable - System.Object for the default SerializableType.
			typeByTypeOfName[ "Serializable" ] = NHibernate.Serializable;
			typeByTypeOfName[ NHibernate.Serializable.Name ] = NHibernate.Serializable;
			
			// object needs to have both class and serializable setup before it can
			// be created.
			typeByTypeOfName[ typeof(System.Object).FullName ] = NHibernate.Object;
			typeByTypeOfName[ typeof(System.Object).AssemblyQualifiedName ] = NHibernate.Object;
			typeByTypeOfName[ NHibernate.Object.Name ] = NHibernate.Object;
			
			// These are in here for Hibernate mapping compatibility
			typeByTypeOfName[ "binary" ] = NHibernate.Binary ;
			typeByTypeOfName[ "boolean" ] = NHibernate.Boolean;
			typeByTypeOfName[ "byte" ] = NHibernate.Byte;
			typeByTypeOfName[ "character" ] = NHibernate.Character;
			typeByTypeOfName[ "class" ] = NHibernate.Class;
			typeByTypeOfName[ "locale" ] = NHibernate.CultureInfo;
			typeByTypeOfName[ "date" ] = NHibernate.DateTime;
			typeByTypeOfName[ "big_decimal" ] = NHibernate.Decimal;
			typeByTypeOfName[ "double" ] = NHibernate.Double;
			typeByTypeOfName[ "short" ] = NHibernate.Int16;
			typeByTypeOfName[ "integer" ] = NHibernate.Int32;
			typeByTypeOfName[ "long" ] = NHibernate.Int64;
			typeByTypeOfName[ "float" ] = NHibernate.Single;
			typeByTypeOfName[ "serializable" ] = NHibernate.Serializable;
			typeByTypeOfName[ "string" ] = NHibernate.String;
			typeByTypeOfName[ "timestamp" ] = NHibernate.Timestamp;
			typeByTypeOfName[ "time" ] = NHibernate.Time;
			typeByTypeOfName[ "true_false" ] = NHibernate.TrueFalse;
			typeByTypeOfName[ "yes_no" ] = NHibernate.YesNo;
			typeByTypeOfName[ "object" ] = NHibernate.Object;
			
			
			getTypeDelegatesWithLength.Add(NHibernate.AnsiString.Name, new GetNullableTypeWithLength(GetAnsiStringType));
			getTypeDelegatesWithLength.Add(NHibernate.Binary.Name, new GetNullableTypeWithLength(GetBinaryType));
			getTypeDelegatesWithLength.Add(NHibernate.Serializable.Name, new GetNullableTypeWithLength(GetSerializableType));
			getTypeDelegatesWithLength.Add(NHibernate.String.Name, new GetNullableTypeWithLength(GetStringType));
			getTypeDelegatesWithLength.Add(NHibernate.Class.Name, new GetNullableTypeWithLength(GetTypeType));
			
			getTypeDelegatesWithPrecision.Add(NHibernate.Decimal.Name, new GetNullableTypeWithPrecision(GetDecimalType));
			
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

		private static IType AddToTypeOfName(string key, IType type) 
		{
			typeByTypeOfName.Add(key, type);
			typeByTypeOfName.Add(type.Name, type);
			return type;
		}

		private static IType AddToTypeOfNameWithLength(string key, IType type) 
		{
			typeByTypeOfName.Add(key, type);
			return type;
		}

		private static IType AddToTypeOfNameWithPrecision(string key, IType type) 
		{
			typeByTypeOfName.Add(key, type);
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
						type = NHibernate.Entity(typeClass);
					}
					else if ( typeClass.IsEnum ) 
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

		public static NullableType GetAnsiStringType() 
		{
			return NHibernate.AnsiString;
		}

		public static NullableType GetAnsiStringType(int length) 
		{
			string key = GetKeyForLengthBased(NHibernate.AnsiString.Name, length);
			
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new AnsiStringType( SqlTypeFactory.GetAnsiString(length) );
				AddToTypeOfNameWithLength(key, returnType);
			}
			return returnType;		
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
			return NHibernate.Binary;
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
			if(length==0) return NHibernate.Binary;

			string key = GetKeyForLengthBased(NHibernate.Binary.Name, length);
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
			return NHibernate.Boolean;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetByteType() 
		{
			return NHibernate.Byte;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetCharType() 
		{
			return NHibernate.Character;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetCultureInfoType() 
		{
			return NHibernate.CultureInfo;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetDateTimeType() 
		{
			return NHibernate.DateTime;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetDateType() 
		{
			return NHibernate.Date;
		}

		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetDecimalType() 
		{
			return NHibernate.Decimal;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="precision"></param>
		/// <param name="scale"></param>
		/// <returns></returns>
		public static NullableType GetDecimalType(byte precision, byte scale) 
		{			
			string key = GetKeyForPrecisionScaleBased(NHibernate.Decimal.Name, precision, scale);
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new DecimalType( SqlTypeFactory.GetDecimal(precision, scale) );
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
			return NHibernate.Double;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetGuidType() 
		{
			return NHibernate.Guid;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetInt16Type() 
		{
			return NHibernate.Int16;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetInt32Type() 
		{
			return NHibernate.Int32;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetInt64Type() 
		{
			return NHibernate.Int64;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static IType GetObjectType() 
		{
			return NHibernate.Object;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetSingleType() 
		{
			return NHibernate.Single;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetSerializableType() 
		{
			return NHibernate.Serializable;
			
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
				returnType = new SerializableType(serializableType); 
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
			string key = GetKeyForLengthBased(NHibernate.Serializable.Name, length);
			
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
			return NHibernate.String;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static NullableType GetStringType(int length) 
		{
			string key = GetKeyForLengthBased(NHibernate.String.Name, length);
			
			NullableType returnType = (NullableType)typeByTypeOfName[key];
			if(returnType==null) 
			{
				returnType = new StringType( SqlTypeFactory.GetString(length) );
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
			return NHibernate.Ticks;			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetTimeSpanType() 
		{
			return NHibernate.TimeSpan;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetTimestampType() 
		{	
			return NHibernate.Timestamp;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetTimeType()
		{
			return NHibernate.Time;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetTrueFalseType()
		{
			return NHibernate.TrueFalse;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetTypeType() 
		{
			return NHibernate.Class;
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
				returnType = new TypeType( SqlTypeFactory.GetString(length) );
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
			return NHibernate.YesNo;
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
		
		public static PersistentCollectionType Bag(string role) 
		{
			return new BagType(role);
		}
		
		public static PersistentCollectionType IdBag(string role) 
		{
			return new IdentifierBagType(role);
		}

		public static PersistentCollectionType Map(string role) 
		{
			return new MapType(role);
		}
		
		public static PersistentCollectionType Set(string role) 
		{
			return new SetType(role);
		}
		
		public static PersistentCollectionType SortedMap(string role, IComparer comparer) 
		{
			return new SortedMapType(role, comparer);
		}

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