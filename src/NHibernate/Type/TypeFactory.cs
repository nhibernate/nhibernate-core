using System;
using System.Collections;
using System.Globalization;
using NHibernate.Engine;
using NHibernate.SqlTypes;
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
	public sealed class TypeFactory
	{
		private enum TypeClassification
		{
			Plain,
			Length,
			PrecisionScale
		}

		private static char[ ] precisionScaleSplit = new char[ ] {'(', ')', ','};
		private static char[ ] lengthSplit = new char[ ] {'(', ')'};

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
		private static Hashtable typeByTypeOfName = Hashtable.Synchronized( new Hashtable( 79 ) );

		private static Hashtable getTypeDelegatesWithLength = Hashtable.Synchronized( new Hashtable( 7 ) );
		private static Hashtable getTypeDelegatesWithPrecision = Hashtable.Synchronized( new Hashtable( 3 ) );

//		private delegate NullableType GetNullableType();

		private delegate NullableType GetNullableTypeWithLength( int length );

		private delegate NullableType GetNullableTypeWithPrecision( byte precision, byte scale );

		/// <summary></summary>
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
			typeByTypeOfName[ typeof( Byte[ ] ).Name ] = NHibernateUtil.Binary;
			typeByTypeOfName[ typeof( Byte[ ] ).AssemblyQualifiedName ] = NHibernateUtil.Binary;
			typeByTypeOfName[ typeof( Boolean ).FullName ] = NHibernateUtil.Boolean;
			typeByTypeOfName[ typeof( Boolean ).AssemblyQualifiedName ] = NHibernateUtil.Boolean;
			typeByTypeOfName[ typeof( Byte ).FullName ] = NHibernateUtil.Byte;
			typeByTypeOfName[ typeof( Byte ).AssemblyQualifiedName ] = NHibernateUtil.Byte;
			typeByTypeOfName[ typeof( Char ).FullName ] = NHibernateUtil.Character;
			typeByTypeOfName[ typeof( Char ).AssemblyQualifiedName ] = NHibernateUtil.Character;
			typeByTypeOfName[ typeof( CultureInfo ).FullName ] = NHibernateUtil.CultureInfo;
			typeByTypeOfName[ typeof( CultureInfo ).AssemblyQualifiedName ] = NHibernateUtil.CultureInfo;
			typeByTypeOfName[ typeof( DateTime ).FullName ] = NHibernateUtil.DateTime;
			typeByTypeOfName[ typeof( DateTime ).AssemblyQualifiedName ] = NHibernateUtil.DateTime;
			typeByTypeOfName[ typeof( Decimal ).FullName ] = NHibernateUtil.Decimal;
			typeByTypeOfName[ typeof( Decimal ).AssemblyQualifiedName ] = NHibernateUtil.Decimal;
			typeByTypeOfName[ typeof( Double ).FullName ] = NHibernateUtil.Double;
			typeByTypeOfName[ typeof( Double ).AssemblyQualifiedName ] = NHibernateUtil.Double;
			typeByTypeOfName[ typeof( Guid ).FullName ] = NHibernateUtil.Guid;
			typeByTypeOfName[ typeof( Guid ).AssemblyQualifiedName ] = NHibernateUtil.Guid;
			typeByTypeOfName[ typeof( Int16 ).FullName ] = NHibernateUtil.Int16;
			typeByTypeOfName[ typeof( Int16 ).AssemblyQualifiedName ] = NHibernateUtil.Int16;
			typeByTypeOfName[ typeof( Int32 ).FullName ] = NHibernateUtil.Int32;
			typeByTypeOfName[ typeof( Int32 ).AssemblyQualifiedName ] = NHibernateUtil.Int32;
			typeByTypeOfName[ typeof( Int64 ).FullName ] = NHibernateUtil.Int64;
			typeByTypeOfName[ typeof( Int64 ).AssemblyQualifiedName ] = NHibernateUtil.Int64;
			typeByTypeOfName[ typeof( SByte ).FullName ] = NHibernateUtil.SByte;
			typeByTypeOfName[ typeof( SByte ).AssemblyQualifiedName ] = NHibernateUtil.SByte;
			typeByTypeOfName[ typeof( Single ).FullName ] = NHibernateUtil.Single;
			typeByTypeOfName[ typeof( Single ).AssemblyQualifiedName ] = NHibernateUtil.Single;
			typeByTypeOfName[ typeof( String ).FullName ] = NHibernateUtil.String;
			typeByTypeOfName[ typeof( String ).AssemblyQualifiedName ] = NHibernateUtil.String;
			typeByTypeOfName[ typeof( TimeSpan ).FullName ] = NHibernateUtil.TimeSpan;
			typeByTypeOfName[ typeof( TimeSpan ).AssemblyQualifiedName ] = NHibernateUtil.TimeSpan;

			// add the mappings of the NHibernate specific names that are used in type=""
			typeByTypeOfName[ NHibernateUtil.AnsiString.Name ] = NHibernateUtil.AnsiString;
			typeByTypeOfName[ NHibernateUtil.Binary.Name ] = NHibernateUtil.Binary;
			typeByTypeOfName[ NHibernateUtil.BinaryBlob.Name ] = NHibernateUtil.BinaryBlob;
			typeByTypeOfName[ NHibernateUtil.Boolean.Name ] = NHibernateUtil.Boolean;
			typeByTypeOfName[ NHibernateUtil.Byte.Name ] = NHibernateUtil.Byte;
			typeByTypeOfName[ NHibernateUtil.SByte.Name ] = NHibernateUtil.SByte;
			typeByTypeOfName[ NHibernateUtil.Character.Name ] = NHibernateUtil.Character;
			typeByTypeOfName[ NHibernateUtil.StringClob.Name ] = NHibernateUtil.StringClob;
			typeByTypeOfName[ NHibernateUtil.CultureInfo.Name ] = NHibernateUtil.CultureInfo;
			typeByTypeOfName[ NHibernateUtil.DateTime.Name ] = NHibernateUtil.DateTime;
			typeByTypeOfName[ NHibernateUtil.Date.Name ] = NHibernateUtil.Date;
			typeByTypeOfName[ NHibernateUtil.Decimal.Name ] = NHibernateUtil.Decimal;
			typeByTypeOfName[ NHibernateUtil.Double.Name ] = NHibernateUtil.Double;
			typeByTypeOfName[ NHibernateUtil.Guid.Name ] = NHibernateUtil.Guid;
			typeByTypeOfName[ NHibernateUtil.Int16.Name ] = NHibernateUtil.Int16;
			typeByTypeOfName[ NHibernateUtil.Int32.Name ] = NHibernateUtil.Int32;
			typeByTypeOfName[ NHibernateUtil.Int64.Name ] = NHibernateUtil.Int64;
			typeByTypeOfName[ NHibernateUtil.SByte.Name ] = NHibernateUtil.SByte;
			typeByTypeOfName[ NHibernateUtil.String.Name ] = NHibernateUtil.String;
			typeByTypeOfName[ NHibernateUtil.Single.Name ] = NHibernateUtil.Single;
			typeByTypeOfName[ NHibernateUtil.Timestamp.Name ] = NHibernateUtil.Timestamp;
			typeByTypeOfName[ NHibernateUtil.Time.Name ] = NHibernateUtil.Time;
			typeByTypeOfName[ NHibernateUtil.TrueFalse.Name ] = NHibernateUtil.TrueFalse;
			typeByTypeOfName[ NHibernateUtil.YesNo.Name ] = NHibernateUtil.YesNo;
			typeByTypeOfName[ NHibernateUtil.Ticks.Name ] = NHibernateUtil.Ticks;
			typeByTypeOfName[ NHibernateUtil.TimeSpan.Name ] = NHibernateUtil.TimeSpan;

			typeByTypeOfName[ NHibernateUtil.Class.Name ] = NHibernateUtil.Class;
			typeByTypeOfName[ typeof( System.Type ).FullName ] = NHibernateUtil.Class;
			typeByTypeOfName[ typeof( System.Type ).AssemblyQualifiedName ] = NHibernateUtil.Class;

			// need to do add the key "Serializable" because the hbm files will have a 
			// type="Serializable", but the SerializableType returns the Name as 
			// "serializable - System.Object for the default SerializableType.
			typeByTypeOfName[ "Serializable" ] = NHibernateUtil.Serializable;
			typeByTypeOfName[ NHibernateUtil.Serializable.Name ] = NHibernateUtil.Serializable;

			// object needs to have both class and serializable setup before it can
			// be created.
			typeByTypeOfName[ typeof( Object ).FullName ] = NHibernateUtil.Object;
			typeByTypeOfName[ typeof( Object ).AssemblyQualifiedName ] = NHibernateUtil.Object;
			typeByTypeOfName[ NHibernateUtil.Object.Name ] = NHibernateUtil.Object;

			// These are in here for Hibernate mapping compatibility
			typeByTypeOfName[ "binary" ] = NHibernateUtil.Binary;
			typeByTypeOfName[ "boolean" ] = NHibernateUtil.Boolean;
			typeByTypeOfName[ "byte" ] = NHibernateUtil.Byte;
			typeByTypeOfName[ "character" ] = NHibernateUtil.Character;
			typeByTypeOfName[ "class" ] = NHibernateUtil.Class;
			typeByTypeOfName[ "locale" ] = NHibernateUtil.CultureInfo;
			typeByTypeOfName[ "date" ] = NHibernateUtil.DateTime;
			typeByTypeOfName[ "big_decimal" ] = NHibernateUtil.Decimal;
			typeByTypeOfName[ "double" ] = NHibernateUtil.Double;
			typeByTypeOfName[ "short" ] = NHibernateUtil.Int16;
			typeByTypeOfName[ "integer" ] = NHibernateUtil.Int32;
			typeByTypeOfName[ "long" ] = NHibernateUtil.Int64;
			typeByTypeOfName[ "float" ] = NHibernateUtil.Single;
			typeByTypeOfName[ "serializable" ] = NHibernateUtil.Serializable;
			typeByTypeOfName[ "string" ] = NHibernateUtil.String;
			typeByTypeOfName[ "timestamp" ] = NHibernateUtil.Timestamp;
			typeByTypeOfName[ "time" ] = NHibernateUtil.Time;
			typeByTypeOfName[ "true_false" ] = NHibernateUtil.TrueFalse;
			typeByTypeOfName[ "yes_no" ] = NHibernateUtil.YesNo;
			typeByTypeOfName[ "object" ] = NHibernateUtil.Object;


			getTypeDelegatesWithLength.Add( NHibernateUtil.AnsiString.Name, new GetNullableTypeWithLength( GetAnsiStringType ) );
			getTypeDelegatesWithLength.Add( NHibernateUtil.Binary.Name, new GetNullableTypeWithLength( GetBinaryType ) );
			getTypeDelegatesWithLength.Add( NHibernateUtil.Serializable.Name, new GetNullableTypeWithLength( GetSerializableType ) );
			getTypeDelegatesWithLength.Add( NHibernateUtil.String.Name, new GetNullableTypeWithLength( GetStringType ) );
			getTypeDelegatesWithLength.Add( NHibernateUtil.Class.Name, new GetNullableTypeWithLength( GetTypeType ) );

			getTypeDelegatesWithPrecision.Add( NHibernateUtil.Decimal.Name, new GetNullableTypeWithPrecision( GetDecimalType ) );

		}


		private TypeFactory()
		{
			throw new NotSupportedException();
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
		private static TypeClassification GetTypeClassification( string typeName )
		{
			int indexOfOpenParen = typeName.IndexOf( "(" );
			int indexOfComma = 0;
			if( indexOfOpenParen >= 0 )
			{
				indexOfComma = typeName.IndexOf( ",", indexOfOpenParen );
			}

			if( indexOfOpenParen >= 0 )
			{
				if( indexOfComma >= 0 )
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
		public static IType Basic( string name )
		{
			string typeName = String.Empty;

			// Use the basic name (such as String or String(255)) to get the
			// instance of the IType object.
			IType returnType = null;
			returnType = ( IType ) typeByTypeOfName[ name ];
			if( returnType != null )
			{
				return returnType;
			}

			// if we get to here then the basic type with the length or precision/scale
			// combination doesn't exists - so lets figure out which one we have and 
			// invoke the appropriate delegate
			TypeClassification typeClassification = GetTypeClassification( name );

			if( typeClassification == TypeClassification.PrecisionScale )
			{
				//precision/scale based
				GetNullableTypeWithPrecision precisionDelegate;
				byte precision;
				byte scale;

				string[ ] parsedName = name.Split( precisionScaleSplit );
				if( parsedName.Length < 4 )
				{
					throw new ApplicationException( "The name " + name + " is not a valid Precision/Scale name" );
				}

				typeName = parsedName[ 0 ].Trim();
				precision = Byte.Parse( parsedName[ 1 ].Trim() );
				scale = Byte.Parse( parsedName[ 2 ].Trim() );

				if( getTypeDelegatesWithPrecision.ContainsKey( typeName ) == false )
				{
					return null;
				}

				precisionDelegate = ( GetNullableTypeWithPrecision ) getTypeDelegatesWithPrecision[ typeName ];
				return precisionDelegate( precision, scale );

			}
			else if( typeClassification == TypeClassification.Length )
			{
				//length based
				GetNullableTypeWithLength lengthDelegate;
				int length;

				string[ ] parsedName = name.Split( lengthSplit );
				if( parsedName.Length < 3 )
				{
					throw new ApplicationException( "The name " + name + " is not a valid Length name" );
				}

				typeName = parsedName[ 0 ].Trim();
				length = Int32.Parse( parsedName[ 1 ].Trim() );

				if( getTypeDelegatesWithLength.ContainsKey( typeName ) == false )
					// we were not able to find a delegate to get the Type
				{
					return null;
				}

				lengthDelegate = ( GetNullableTypeWithLength ) getTypeDelegatesWithLength[ typeName ];
				return lengthDelegate( length );
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

		private static IType AddToTypeOfName( string key, IType type )
		{
			typeByTypeOfName.Add( key, type );
			typeByTypeOfName.Add( type.Name, type );
			return type;
		}

		private static IType AddToTypeOfNameWithLength( string key, IType type )
		{
			typeByTypeOfName.Add( key, type );
			return type;
		}

		private static IType AddToTypeOfNameWithPrecision( string key, IType type )
		{
			typeByTypeOfName.Add( key, type );
			return type;
		}

		// not used !?!
//		private static string GetKeyForLengthBased( IType type )
//		{
//			NullableType nullableType = type as NullableType;
//			if( nullableType != null )
//			{
//				return GetKeyForLengthBased( type.Name, nullableType.SqlType.Length );
//			}
//			else
//			{
//				throw new ApplicationException( "Can't get a Key for a Length Based item of IType = " + type.Name );
//			}
//		}

		private static string GetKeyForLengthBased( string name, int length )
		{
			return name + "(" + length + ")";
		}

		// not used !?!
//		private static string GetKeyForPrecisionScaleBased( IType type )
//		{
//			NullableType nullableType = type as NullableType;
//			if( nullableType != null )
//			{
//				return GetKeyForPrecisionScaleBased( nullableType.Name, nullableType.SqlType.Precision, nullableType.SqlType.Scale );
//			}
//			else
//			{
//				throw new ApplicationException( "Can't get a Key for a Precision Scale Based item of IType = " + type.Name );
//			}
//		}

		private static string GetKeyForPrecisionScaleBased( string name, byte precision, byte scale )
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
		public static IType HueristicType( string typeName )
		{
			IType type = TypeFactory.Basic( typeName );

			if( type == null )
			{
				string[ ] parsedTypeName;
				TypeClassification typeClassification = GetTypeClassification( typeName );
				if( typeClassification == TypeClassification.Length )
				{
					parsedTypeName = typeName.Split( lengthSplit );
				}
				else if( typeClassification == TypeClassification.PrecisionScale )
				{
					parsedTypeName = typeName.Split( precisionScaleSplit );
				}
				else
				{
					parsedTypeName = new string[ ] {typeName};
				}


				System.Type typeClass;
				try
				{
					typeClass = ReflectHelper.ClassForName( parsedTypeName[ 0 ] ); //typeName);
				}
				catch( Exception )
				{
					typeClass = null;
				}

				if( typeClass != null )
				{
					if( typeof( IType ).IsAssignableFrom( typeClass ) )
					{
						try
						{
							type = ( IType ) Activator.CreateInstance( typeClass );
						}
						catch( Exception e )
						{
							throw new MappingException( "Could not instantiate IType " + typeClass.Name + ": " + e );
						}
					}
					else if( typeof( ICompositeUserType ).IsAssignableFrom( typeClass ) )
					{
						type = new CompositeCustomType( typeClass );
					}
					else if( typeof( IUserType ).IsAssignableFrom( typeClass ) )
					{
						type = new CustomType( typeClass );
					}
					else if( typeof( ILifecycle ).IsAssignableFrom( typeClass ) )
					{
						type = NHibernateUtil.Entity( typeClass );
					}
					else if( typeClass.IsEnum )
					{
						type = NHibernateUtil.Enum( typeClass );
					}
					else if( typeClass.IsSerializable )
					{
						if( typeClassification == TypeClassification.Length )
						{
							type = GetSerializableType( typeClass, Int32.Parse( parsedTypeName[ 1 ] ) );
						}
						else
						{
							type = GetSerializableType( typeClass );
						}
					}
				}
			}
			return type;
		}

		/// <summary></summary>
		public static NullableType GetAnsiStringType()
		{
			return NHibernateUtil.AnsiString;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static NullableType GetAnsiStringType( int length )
		{
			string key = GetKeyForLengthBased( NHibernateUtil.AnsiString.Name, length );

			NullableType returnType = ( NullableType ) typeByTypeOfName[ key ];
			if( returnType == null )
			{
				returnType = new AnsiStringType( SqlTypeFactory.GetAnsiString( length ) );
				AddToTypeOfNameWithLength( key, returnType );
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
			return NHibernateUtil.Binary;
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
		public static NullableType GetBinaryType( int length )
		{
			//HACK: don't understand why SerializableType calls this with length=0
			if( length == 0 )
			{
				return NHibernateUtil.Binary;
			}

			string key = GetKeyForLengthBased( NHibernateUtil.Binary.Name, length );
			NullableType returnType = ( NullableType ) typeByTypeOfName[ key ];
			if( returnType == null )
			{
				returnType = new BinaryType( SqlTypeFactory.GetBinary( length ) );
				AddToTypeOfNameWithLength( key, returnType );
			}

			return returnType;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetBooleanType()
		{
			return NHibernateUtil.Boolean;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetByteType()
		{
			return NHibernateUtil.Byte;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetSByteType() 
		{
			return NHibernateUtil.SByte;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetCharType() 
		{
			return NHibernateUtil.Character;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetCultureInfoType()
		{
			return NHibernateUtil.CultureInfo;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetDateTimeType()
		{
			return NHibernateUtil.DateTime;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetDateType()
		{
			return NHibernateUtil.Date;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetDecimalType()
		{
			return NHibernateUtil.Decimal;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="precision"></param>
		/// <param name="scale"></param>
		/// <returns></returns>
		public static NullableType GetDecimalType( byte precision, byte scale )
		{
			string key = GetKeyForPrecisionScaleBased( NHibernateUtil.Decimal.Name, precision, scale );
			NullableType returnType = ( NullableType ) typeByTypeOfName[ key ];
			if( returnType == null )
			{
				returnType = new DecimalType( SqlTypeFactory.GetDecimal( precision, scale ) );
				AddToTypeOfNameWithPrecision( key, returnType );
			}

			return returnType;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetDoubleType()
		{
			return NHibernateUtil.Double;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetGuidType()
		{
			return NHibernateUtil.Guid;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetInt16Type()
		{
			return NHibernateUtil.Int16;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetInt32Type()
		{
			return NHibernateUtil.Int32;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetInt64Type()
		{
			return NHibernateUtil.Int64;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static IType GetObjectType()
		{
			return NHibernateUtil.Object;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetSingleType()
		{
			return NHibernateUtil.Single;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetSerializableType()
		{
			return NHibernateUtil.Serializable;

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
		public static NullableType GetSerializableType( System.Type serializableType )
		{
			string key = serializableType.AssemblyQualifiedName;

			NullableType returnType = ( NullableType ) typeByTypeOfName[ key ];
			if( returnType == null )
			{
				returnType = new SerializableType( serializableType );
				AddToTypeOfName( key, returnType );
			}

			return returnType;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serializableType"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static NullableType GetSerializableType( System.Type serializableType, int length )
		{
			string key = GetKeyForLengthBased( serializableType.AssemblyQualifiedName, length );

			NullableType returnType = ( NullableType ) typeByTypeOfName[ key ];
			if( returnType == null )
			{
				returnType = new SerializableType( serializableType, SqlTypeFactory.GetBinary( length ) );
				AddToTypeOfNameWithLength( key, returnType );
			}

			return returnType;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static NullableType GetSerializableType( int length )
		{
			string key = GetKeyForLengthBased( NHibernateUtil.Serializable.Name, length );

			NullableType returnType = ( NullableType ) typeByTypeOfName[ key ];
			if( returnType == null )
			{
				returnType = new SerializableType( typeof( object ), SqlTypeFactory.GetBinary( length ) );
				AddToTypeOfNameWithLength( key, returnType );
			}

			return returnType;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetStringType()
		{
			return NHibernateUtil.String;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static NullableType GetStringType( int length )
		{
			string key = GetKeyForLengthBased( NHibernateUtil.String.Name, length );

			NullableType returnType = ( NullableType ) typeByTypeOfName[ key ];
			if( returnType == null )
			{
				returnType = new StringType( SqlTypeFactory.GetString( length ) );
				AddToTypeOfNameWithLength( key, returnType );
			}

			return returnType;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetTicksType()
		{
			return NHibernateUtil.Ticks;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetTimeSpanType()
		{
			return NHibernateUtil.TimeSpan;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetTimestampType()
		{
			return NHibernateUtil.Timestamp;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetTimeType()
		{
			return NHibernateUtil.Time;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetTrueFalseType()
		{
			return NHibernateUtil.TrueFalse;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetTypeType()
		{
			return NHibernateUtil.Class;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static NullableType GetTypeType( int length )
		{
			string key = GetKeyForLengthBased( typeof( TypeType ).FullName, length );

			NullableType returnType = ( NullableType ) typeByTypeOfName[ key ];
			if( returnType == null )
			{
				returnType = new TypeType( SqlTypeFactory.GetString( length ) );
				AddToTypeOfNameWithLength( key, returnType );
			}

			return returnType;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static NullableType GetYesNoType()
		{
			return NHibernateUtil.YesNo;
		}


		// Association Types

		/// <summary>
		/// A one-to-one association type for the given class and cascade style.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="foreignKeyType"></param>
		/// <returns></returns>
		public static IType OneToOne( System.Type persistentClass, ForeignKeyType foreignKeyType )
		{
			return OneToOne( persistentClass, foreignKeyType, null );
		}

		/// <summary>
		/// A one-to-one association type for the given class and cascade style.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="foreignKeyType"></param>
		/// <param name="uniqueKeyPropertyName"></param>
		/// <returns></returns>
		public static IType OneToOne( System.Type persistentClass, ForeignKeyType foreignKeyType, string uniqueKeyPropertyName )
		{
			return new OneToOneType( persistentClass, foreignKeyType, uniqueKeyPropertyName );
		}

		/// <summary>
		/// A many-to-one association type for the given class and cascade style.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <returns></returns>
		public static IType ManyToOne( System.Type persistentClass )
		{
			return ManyToOne( persistentClass, null );
		}

		/// <summary>
		/// A many-to-one association type for the given class and cascade style.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="uniqueKeyPropertyName"></param>
		/// <returns></returns>
		public static IType ManyToOne( System.Type persistentClass, string uniqueKeyPropertyName )
		{
			return new ManyToOneType( persistentClass, uniqueKeyPropertyName );
		}

		// Collection Types:


		/// <summary>
		/// 
		/// </summary>
		/// <param name="role"></param>
		/// <param name="elementClass"></param>
		/// <returns></returns>
		public static PersistentCollectionType Array( string role, System.Type elementClass )
		{
			return new ArrayType( role, elementClass );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public static PersistentCollectionType List( string role )
		{
			return new ListType( role );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public static PersistentCollectionType Bag( string role )
		{
			return new BagType( role );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public static PersistentCollectionType IdBag( string role )
		{
			return new IdentifierBagType( role );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public static PersistentCollectionType Map( string role )
		{
			return new MapType( role );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public static PersistentCollectionType Set( string role )
		{
			return new SetType( role );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="role"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static PersistentCollectionType SortedMap( string role, IComparer comparer )
		{
			return new SortedMapType( role, comparer );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="role"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static PersistentCollectionType SortedSet( string role, IComparer comparer )
		{
			return new SortedSetType( role, comparer );
		}


		/// <summary>
		/// Deep copy values in the first array into the second
		/// </summary>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <param name="copy"></param>
		/// <param name="target"></param>
		public static void DeepCopy( object[ ] values, IType[ ] types, bool[ ] copy, object[ ] target )
		{
			for( int i = 0; i < types.Length; i++ )
			{
				if( copy[ i ] )
				{
					target[ i ] = types[ i ].DeepCopy( values[ i ] );
				}
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
		public static int[ ] FindDirty( IType[ ] types, object[ ] x, object[ ] y, bool[ ] check, ISessionImplementor session )
		{
			int[ ] results = null;
			int count = 0;
			for( int i = 0; i < types.Length; i++ )
			{
				if( check[ i ] && types[ i ].IsDirty( x[ i ], y[ i ], session ) )
				{
					if( results == null )
					{
						results = new int[types.Length];
					}
					results[ count++ ] = i;
				}
			}
			if( count == 0 )
			{
				return null;
			}
			else
			{
				int[ ] trimmed = new int[count];
				System.Array.Copy( results, 0, trimmed, 0, count );
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