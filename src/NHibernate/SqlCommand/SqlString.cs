using System;
using System.Collections;
using System.Text;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// This is a non-modifiable Sql statement that is ready to be prepared 
	/// and sent to the Database for execution.
	/// 
	/// If you need to modify this object pass it to a <c>SqlStringBuilder</c> and
	/// get a new object back from it.
	/// </summary>
	[Serializable]
	public class SqlString : ICloneable
	{
		private readonly object[ ] sqlParts;
		private int[ ] parameterIndexes;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlPart"></param>
		public SqlString( string sqlPart ) : this( new object[ ] {sqlPart} )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlParts"></param>
		public SqlString( object[ ] sqlParts )
		{
			this.sqlParts = sqlParts;
		}

		/// <summary></summary>
		/// <remarks>
		/// TODO: modify this from object[] to an ICollection so that we can
		/// safely say that SqlString truly is immutable.  Right now someone
		/// can change the contents of the array - hence changing the SqlString
		/// and throwing any place this is used as a key in a hashtable into
		/// an unfindable instance.
		/// </remarks>
		public object[ ] SqlParts
		{
			get { return sqlParts; }
		}

		/// <summary>
		/// Appends the SqlString parameter to the end of the current SqlString to create a 
		/// new SqlString object.
		/// </summary>
		/// <param name="rhs">The SqlString to append.</param>
		/// <returns>A new SqlString object.</returns>
		/// <remarks>
		/// A SqlString object is immutable so this returns a new SqlString.  If multiple Appends 
		/// are called it is better to use the SqlStringBuilder.
		/// </remarks>
		public SqlString Append( SqlString rhs )
		{
			object[ ] temp = new object[rhs.SqlParts.Length + sqlParts.Length];
			Array.Copy( sqlParts, 0, temp, 0, sqlParts.Length );
			Array.Copy( rhs.SqlParts, 0, temp, sqlParts.Length, rhs.SqlParts.Length );

			return new SqlString( temp );
		}

		/// <summary>
		/// Appends the string parameter to the end of the current SqlString to create a 
		/// new SqlString object.
		/// </summary>
		/// <param name="rhs">The string to append.</param>
		/// <returns>A new SqlString object.</returns>
		/// <remarks>
		/// A SqlString object is immutable so this returns a new SqlString.  If multiple Appends 
		/// are called it is better to use the SqlStringBuilder.
		/// </remarks>
		public SqlString Append( string rhs )
		{
			object[ ] temp = new object[sqlParts.Length + 1];
			Array.Copy( sqlParts, 0, temp, 0, sqlParts.Length );
			temp[ sqlParts.Length ] = rhs;

			return new SqlString( temp );
		}

		/// <summary>
		/// Compacts the SqlString into the fewest parts possible.
		/// </summary>
		/// <returns>A new SqlString.</returns>
		/// <remarks>
		/// Combines all SqlParts that are strings and next to each other into
		/// one SqlPart.
		/// </remarks>
		public SqlString Compact()
		{
			StringBuilder builder = new StringBuilder();
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();
			string builderString = String.Empty;

			foreach( object part in SqlParts )
			{
				string stringPart = part as string;

				if( stringPart != null )
				{
					builder.Append( stringPart );
				}
				else
				{
					builderString = builder.ToString();

					// don't add an empty string into the new compacted SqlString
					if( builderString.Length != 0 )
					{
						sqlBuilder.Add( builderString );
					}

					builder = new StringBuilder();

					sqlBuilder.Add( ( Parameter ) part );
				}

			}

			// make sure the contents of the builder have been added to the sqlBuilder
			builderString = builder.ToString();

			if( builderString.Length > 0 )
			{
				sqlBuilder.Add( builderString );
			}

			return sqlBuilder.ToSqlString();
		}

		/// <summary>
		/// Gets a bool that indicates if there is a Parameter that has a null SqlType.
		/// </summary>
		/// <value>true if there is a Parameter with a null SqlType.</value>
		public bool ContainsUntypedParameter
		{
			get
			{
				for( int i = 0; i < sqlParts.Length; i++ )
				{
					Parameter paramPart = sqlParts[ i ] as Parameter;
					if( paramPart != null )
					{
						if( paramPart.SqlType == null )
						{
							// only need to find one null SqlType
							return true;
						}
					}
				}

				return false;
			}
		}

		/// <summary>
		/// Gets the number of SqlParts contained in this SqlString.
		/// </summary>
		/// <value>The number of SqlParts contained in this SqlString.</value>
		/// <remarks>
		/// If a SqlPart contains a SqlString then this recursively looks at each SqlPart
		/// for the Count.
		/// </remarks>
		public int Count
		{
			get
			{
				int count = 0;
				for( int i = 0; i < sqlParts.Length; i++ )
				{
					SqlString sqlString = sqlParts[ i ] as SqlString;
					if( sqlString != null )
					{
						count += sqlString.Count;
					}
					else
					{
						count++;
					}
				}

				return count;
			}
		}

		/// <summary>
		/// Determines whether the end of this instance matches the specified String.
		/// </summary>
		/// <param name="value">A string to seek at the end.</param>
		/// <returns><c>true</c> if the end of this instance matches value; otherwise, <c>false</c></returns>
		public bool EndsWith( string value )
		{
			SqlString tempSql = Compact();

			int endIndex = tempSql.SqlParts.Length - 1;

			if( tempSql.SqlParts.Length == 0 )
			{
				return false;
			}


			string lastPart = tempSql.SqlParts[ endIndex ] as string;
			if( lastPart != null )
			{
				return lastPart.EndsWith( value );
			}

			return false;
		}

		/// <summary>
		/// Gets the indexes of the Parameters in the SqlParts array.
		/// </summary>
		/// <value>
		/// An Int32 array that contains the indexes of the Parameters in the SqlPart array.
		/// </value>
		public int[ ] ParameterIndexes
		{
			get
			{
				// only calculate this one time because this object is immutable.
				if( parameterIndexes == null )
				{
					ArrayList paramList = new ArrayList();
					for( int i = 0; i < sqlParts.Length; i++ )
					{
						if( sqlParts[ i ] is Parameter )
						{
							paramList.Add( i );
						}
					}

					parameterIndexes = ( int[ ] ) paramList.ToArray( typeof( int ) );
				}

				return parameterIndexes;
			}

		}

		/// <summary>
		/// Replaces all occurrences of a specified <see cref="String"/> in this instance, 
		/// with another specified <see cref="String"/> .
		/// </summary>
		/// <param name="oldValue">A String to be replaced.</param>
		/// <param name="newValue">A String to replace all occurrences of oldValue. </param>
		/// <returns>
		/// A new SqlString with oldValue replaced by the newValue.  The new SqlString is 
		/// in the Compacted form.
		/// </returns>
		public SqlString Replace( string oldValue, string newValue )
		{
			// compacting returns a new SqlString object, so we are free to modify
			// any of the parts because it has not been put in a hashtable so we can
			// consider it mutable - there is no danger yet in changing the value that
			// GetHashCode would return.
			SqlString compacted = this.Compact();

			for( int i = 0; i < compacted.SqlParts.Length; i++ )
			{
				string sqlPart = compacted.SqlParts[ i ] as string;
				if( sqlPart != null )
				{
					compacted.SqlParts[ i ] = sqlPart.Replace( oldValue, newValue );
				}
			}

			return compacted;
		}

		/// <summary>
		/// Determines whether the beginning of this SqlString matches the specified System.String
		/// </summary>
		/// <param name="value">The System.String to seek</param>
		/// <returns>true if the SqlString starts with the value.</returns>
		public bool StartsWith( string value )
		{
			SqlString tempSql = this.Compact();

			foreach( object sqlPart in tempSql.SqlParts )
			{
				string partText = sqlPart as string;

				// if this part is not a string then we know we did not start with the string 
				// value
				if( partText == null )
				{
					return false;
				}

				// if for some reason we had an empty string in here then just 
				// move on to the next SqlPart, otherwise lets make sure that 
				// it does in fact start with the value
				if( partText.Length > 0 )
				{
					return partText.StartsWith( value );
				}
			}

			// if we get down to here that means there were no sql parts in the SqlString
			// so obviously it doesn't start with the value
			return false;
		}

		/// <summary>
		/// Retrieves a substring from this instance. The substring starts at a specified character position. 
		/// </summary>
		/// <param name="startIndex">The starting character position of a substring in this instance.</param>
		/// <returns>
		/// A new SqlString to the substring that begins at startIndex in this instance. 
		/// </returns>
		/// <remarks>
		/// If the first SqlPart is a Parameter then no action is taken and a copy of the SqlString is
		/// returned.
		/// 
		/// If the startIndex is greater than the length of the strings before the first SqlPart that
		/// is a Parameter then all of the strings will be removed and the first SqlPart returned
		/// will be the Parameter. 
		/// </remarks>
		public SqlString Substring( int startIndex )
		{
			SqlStringBuilder builder = new SqlStringBuilder( Compact() );

			string part = builder[ 0 ] as string;

			// if the first part is null then it is not a string so just
			// return them the compacted version
			if( part != null )
			{
				if( part.Length < startIndex )
				{
					builder.RemoveAt( 0 );
				}
				else
				{
					builder[ 0 ] = part.Substring( startIndex );
				}
			}

			return builder.ToSqlString();
		}

		/// <summary>
		/// Removes all occurrences of white space characters from the beginning and end of this instance.
		/// </summary>
		/// <returns>
		/// A new SqlString equivalent to this instance after white space characters 
		/// are removed from the beginning and end.
		/// </returns>
		public SqlString Trim()
		{
			SqlStringBuilder builder = new SqlStringBuilder( Compact() );

			// there is nothing in the builder to Trim 
			if( builder.Count == 0 )
			{
				return builder.ToSqlString();
			}

			string begin = builder[ 0 ] as string;
			int endIndex = builder.Count - 1;
			string end = builder[ endIndex ] as string;

			if( endIndex == 0 && begin != null )
			{
				builder[ 0 ] = begin.Trim();
			}
			else
			{
				if( begin != null )
				{
					builder[ 0 ] = begin.TrimStart();
				}

				if( end != null )
				{
					builder[ builder.Count - 1 ] = end.TrimEnd();
				}
			}

			return builder.ToSqlString();
		}

		#region System.Object Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals( object obj )
		{
			SqlString rhs;

			// Step1: Perform an equals test
			if( obj == this )
			{
				return true;
			}

			// Step	2: Instance of check
			rhs = obj as SqlString;
			if( rhs == null )
			{
				return false;
			}

			//Step 3: Check each important field

			// if they don't contain the same number of parts then we
			// can exit early because they are different
			if( sqlParts.Length!=rhs.SqlParts.Length )
			{
				return false;
			}

			// they have the same number of parts - so compare each
			// part for equallity.
			for( int i = 0; i < sqlParts.Length; i++ )
			{
				if( this.sqlParts[ i ].Equals( rhs.SqlParts[ i ] ) == false )
				{
					return false;
				}
			}

			// nothing has been found that is different - so they are equal.
			return true;
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			int hashCode = 0;

			unchecked
			{
				for( int i = 0; i < sqlParts.Length; i++ )
				{
					hashCode += sqlParts[ i ].GetHashCode();
				}
			}

			return hashCode;
		}

		/// <summary>
		/// Returns the SqlString in a string where it looks like
		/// SELECT col1, col2 FROM table WHERE col1 = :param1
		/// 
		/// The ":" is used as the indicator of a parameter because at this point
		/// we are not using the specific Provider so we don't know how that provider
		/// wants our parameters formatted.
		/// </summary>
		/// <returns>A Provider nuetral version of the CommandText</returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder( sqlParts.Length*15 );

			for( int i = 0; i < sqlParts.Length; i++ )
			{
				builder.Append( sqlParts[ i ].ToString() );
			}

			return builder.ToString();
		}

		#endregion

		#region ICloneable Members

		/// <summary></summary>
		public SqlString Clone()
		{
			object[ ] clonedParts = new object[sqlParts.Length];
			Parameter param;

			for( int i = 0; i < sqlParts.Length; i++ )
			{
				param = sqlParts[ i ] as Parameter;
				if( param != null )
				{
					clonedParts[ i ] = param.Clone();
				}
				else
				{
					clonedParts[ i ] = ( string ) sqlParts[ i ];
				}
			}

			return new SqlString( clonedParts );
		}

		/// <summary></summary>
		object ICloneable.Clone()
		{
			return Clone();
		}

		#endregion
	}
}