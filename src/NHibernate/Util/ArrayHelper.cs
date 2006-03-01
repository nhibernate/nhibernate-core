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
		public static readonly object[] EmptyObjectArray = new object[0];
		public static readonly IType[] EmptyTypeArray = new IType[0];
		public static readonly int[] EmptyIntArray = new int[0];
		public static readonly bool[] EmptyBoolArray = new bool[0];

		private ArrayHelper()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static bool IsAllNegative( int[] array )
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
		public static bool Equals( byte[] lhs, byte[] rhs )
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objects"></param>
		/// <returns></returns>
		public static string[] ToStringArray( object[] objects )
		{
			int length = objects.Length;
			string[] result = new string[length];
			for( int i = 0; i < length; i++ )
			{
				result[ i ] = objects[ i ].ToString();
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string[] FillArray( string str, int length )
		{
			string[] result = new string[length];
			for( int i = 0; i < length; i++ )
			{
				result[ i ] = str;
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="coll"></param>
		/// <returns></returns>
		public static string[] ToStringArray( ICollection coll )
		{
			string[] result = new string[coll.Count];
			int i = 0;
			foreach( object obj in coll )
			{
				result[ i++ ] = obj.ToString();
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="coll"></param>
		/// <returns></returns>
		public static int[] ToIntArray( ICollection coll )
		{
			int[] result = new int[coll.Count];
			int i = 0;
			foreach( object obj in coll )
			{
				result[ i++ ] = int.Parse( obj.ToString() );
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strings"></param>
		/// <param name="begin"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string[] Slice( string[] strings, int begin, int length )
		{
			string[] result = new string[length];
			for( int i = 0; i < length; i++ )
			{
				result[ i ] = strings[ begin + i ];
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objects"></param>
		/// <param name="begin"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static object[] Slice( object[] objects, int begin, int length )
		{
			object[] result = new object[length];
			for( int i = 0; i < length; i++ )
			{
				result[ i ] = objects[ begin + i ];
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static string[] Join( string[] x, string[] y )
		{
			string[] result = new string[x.Length + y.Length];
			for( int i = 0; i < x.Length; i++ )
			{
				result[ i ] = x[ i ];
			}
			for( int i = 0; i < y.Length; i++ )
			{
				result[ i + x.Length ] = y[ i ];
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static DbType[] Join( DbType[] x, DbType[] y )
		{
			DbType[] result = new DbType[x.Length + y.Length];
			for( int i = 0; i < x.Length; i++ )
			{
				result[ i ] = x[ i ];
			}
			for( int i = 0; i < y.Length; i++ )
			{
				result[ i + x.Length ] = y[ i ];
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static SqlType[] Join( SqlType[] x, SqlType[] y )
		{
			SqlType[] result = new SqlType[x.Length + y.Length];
			for( int i = 0; i < x.Length; i++ )
			{
				result[ i ] = x[ i ];
			}
			for( int i = 0; i < y.Length; i++ )
			{
				result[ i + x.Length ] = y[ i ];
			}
			return result;
		}

		public static bool IsAllFalse( bool[] array )
		{
			foreach( bool value in array )
			{
				if( value )
				{
					return false;
				}
			}

			return true;
		}

		public static string[][] To2DStringArray( ICollection coll )
		{
			string[][] result = new string[coll.Count][];
			coll.CopyTo( result, 0 );
			return result;
		}

		public static string ToString( object[] array )
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
	}
}