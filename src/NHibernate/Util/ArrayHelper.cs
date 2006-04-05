using System;
using System.Collections;
using System.Data;
using System.Text;

using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Util
{
	/// <summary>
	/// Helper class that contains common array functions and 
	/// data structures used through out NHibernate.
	/// </summary>
	public sealed class ArrayHelper
	{
		public static readonly object[ ] EmptyObjectArray = new object[0];
		public static readonly IType[ ] EmptyTypeArray = new IType[0];
		public static readonly int[ ] EmptyIntArray = new int[0];
		public static readonly bool[ ] EmptyBoolArray = new bool[0];

		private ArrayHelper()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static bool IsAllNegative( int[ ] array )
		{
			for( int i = 0; i < array.Length; i++ )
			{
				if( array[ i ] >= 0 )
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Compares two byte[] arrays for equality.
		/// </summary>
		/// <param name="lhs">The byte[] array on the Left Hand Side </param>
		/// <param name="rhs">The byte[] array on the Right Hand Side</param>
		/// <returns>true if they contain the same items</returns>
		public static bool Equals( byte[ ] lhs, byte[ ] rhs )
		{
			// just for luck, check for reference equality
			if( lhs == rhs )
			{
				return true;
			}

			// if they don't have the same reference and one of them
			// is null, then they are not Equal
			if( lhs == null || rhs == null )
			{
				return false;
			}

			// if they don't have the same length they are not equal
			if( lhs.Length != rhs.Length )
			{
				return false;
			}

			// move through every object in the array and hope that it 
			// implements the Equals method correctly
			for( int i = 0; i < lhs.Length; i++ )
			{
				if( lhs[ i ].Equals( rhs[ i ] ) == false )
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Compares two object arrays for equality.
		/// </summary>
		/// <param name="lhs">The object array on the Left Hand Side </param>
		/// <param name="rhs">The object array on the Right Hand Side</param>
		/// <returns>true if they contain the same items</returns>
		/// <remarks>
		/// This relies on the objects in the array implementing the method Equals(object)
		/// correctly.
		/// </remarks>
		public static bool Equals( Array lhs, Array rhs )
		{
			// just for luck, check for reference equality
			if( lhs == rhs )
			{
				return true;
			}

			// if they don't have the same reference and one of them
			// is null, then they are not Equal
			if( lhs == null || rhs == null )
			{
				return false;
			}

			// if they don't have the same length they are not equal
			if( lhs.Length != rhs.Length )
			{
				return false;
			}

			// move through every object in the array and hope that it 
			// implements the Equals method correctly
			for( int i = 0; i < lhs.Length; i++ )
			{
				if( lhs.GetValue( i ).Equals( rhs.GetValue( i ) ) == false )
				{
					return false;
				}
			}

			return true;
		}

		public static string[ ] ToStringArray( object[ ] objects )
		{
			int length = objects.Length;
			string[ ] result = new string[length];
			for( int i = 0; i < length; i++ )
			{
				result[ i ] = objects[ i ].ToString();
			}
			return result;
		}

		public static string[ ] FillArray( string str, int length )
		{
			string[ ] result = new string[length];
			for( int i = 0; i < length; i++ )
			{
				result[ i ] = str;
			}
			return result;
		}

		public static LockMode[ ] FillArray( LockMode lockMode, int length )
		{
			LockMode[ ] result = new LockMode[length];
			for( int i = 0; i < length; i++ )
			{
				result[ i ] = lockMode;
			}
			return result;
		}

		public static IType[ ] FillArray( IType type, int length )
		{
			IType[ ] result = new IType[length];
			for( int i = 0; i < length; i++ )
			{
				result[ i ] = type;
			}
			return result;
		}

		public static string[ ] ToStringArray( ICollection coll )
		{
			string[ ] result = new string[coll.Count];
			coll.CopyTo( result, 0 );
			return result;
		}

		public static int[ ] ToIntArray( ICollection coll )
		{
			int[ ] result = new int[coll.Count];
			coll.CopyTo( result, 0 );
			return result;
		}

		public static string[ ] Slice( string[ ] strings, int begin, int length )
		{
			string[ ] result = new string[length];
			Array.Copy( strings, begin, result, 0, length );
			return result;
		}

		public static object[ ] Slice( object[ ] objects, int begin, int length )
		{
			object[ ] result = new object[length];
			Array.Copy( objects, begin, result, 0, length );
			return result;
		}

		public static string[ ] Join( string[ ] x, string[ ] y )
		{
			string[ ] result = new string[x.Length + y.Length];
			Array.Copy( x, 0, result, 0, x.Length );
			Array.Copy( y, 0, result, x.Length, y.Length );
			return result;
		}

		public static DbType[ ] Join( DbType[ ] x, DbType[ ] y )
		{
			DbType[ ] result = new DbType[x.Length + y.Length];
			Array.Copy( x, 0, result, 0, x.Length );
			Array.Copy( y, 0, result, x.Length, y.Length );
			return result;
		}

		public static SqlType[ ] Join( SqlType[ ] x, SqlType[ ] y )
		{
			SqlType[ ] result = new SqlType[x.Length + y.Length];
			Array.Copy( x, 0, result, 0, x.Length );
			Array.Copy( y, 0, result, x.Length, y.Length );
			return result;
		}

		public static object[ ] Join( object[ ] x, object[ ] y )
		{
			object[ ] result = new object[ x.Length + y.Length ];
			Array.Copy( x, 0, result, 0, x.Length );
			Array.Copy( y, 0, result, x.Length, y.Length );
			return result;
		}

		public static bool IsAllFalse( bool[ ] array )
		{
			return Array.IndexOf( array, true ) < 0;
		}

		public static string[ ][ ] To2DStringArray( ICollection coll )
		{
			string[ ][ ] result = new string[coll.Count][ ];
			coll.CopyTo( result, 0 );
			return result;
		}

		public static string ToString( object[ ] array )
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( "[" );
			for( int i = 0; i < array.Length; i++ )
			{
				sb.Append( array[ i ] );
				if( i < array.Length - 1 )
				{
					sb.Append( "," );
				}
			}
			sb.Append( "]" );
			return sb.ToString();
		}

		// NH-specific
		public static void AddAll( IList to, IList from )
		{
			foreach( object obj in from )
			{
				to.Add( obj );
			}
		}

		public static int[ ] GetBatchSizes( int maxBatchSize )
		{
			int batchSize = maxBatchSize;
			int n = 1;

			while( batchSize > 1 )
			{
				batchSize = GetNextBatchSize( batchSize );
				n++;
			}

			int[ ] result = new int[n];
			batchSize = maxBatchSize;

			for( int i = 0; i < n; i++ )
			{
				result[ i ] = batchSize;
				batchSize = GetNextBatchSize( batchSize );
			}

			return result;
		}

		private static int GetNextBatchSize( int batchSize )
		{
			if( batchSize <= 10 )
			{
				return batchSize - 1; // allow 9,8,7,6,5,4,3,2,1
			}
			else if( batchSize/2 < 10 )
			{
				return 10;
			}
			else
			{
				return batchSize/2;
			}
		}
	}
}