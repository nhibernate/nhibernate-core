using System;
using System.Collections;
using System.Xml;

namespace NHibernate.Util
{
	//Much of this code is taken from Maverick.NET
	/// <summary></summary>
	public class PropertiesHelper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="property"></param>
		/// <param name="properties"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static bool GetBoolean( string property, IDictionary properties, bool defaultValue )
		{
			return properties[ property ] == null ?
				defaultValue :
				bool.Parse( properties[ property ] as string );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="property"></param>
		/// <param name="properties"></param>
		/// <returns></returns>
		public static bool GetBoolean( string property, IDictionary properties )
		{
			return properties[ property ] == null ?
				false :
				bool.Parse( properties[ property ] as string );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="property"></param>
		/// <param name="properties"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static int GetInt32( string property, IDictionary properties, int defaultValue )
		{
			string propValue = properties[ property ] as string;
			return ( propValue == null ) ? defaultValue : int.Parse( propValue );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="property"></param>
		/// <param name="properties"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static string GetString( string property, IDictionary properties, string defaultValue )
		{
			string propValue = properties[ property ] as string;
			return ( propValue == null ) ? defaultValue : propValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="property"></param>
		/// <param name="delim"></param>
		/// <param name="properties"></param>
		/// <returns></returns>
		public static IDictionary ToDictionary( string property, string delim, IDictionary properties )
		{
			IDictionary map = new Hashtable();
			string propValue = ( string ) properties[ property ];
			if( propValue != null )
			{
				StringTokenizer tokens = new StringTokenizer( propValue, delim, false );
				IEnumerator en = tokens.GetEnumerator();
				while( en.MoveNext() )
				{
					string key = ( string ) en.Current;

					string value = en.MoveNext() ? ( string ) en.Current : String.Empty;
					map[ key ] = value;
				}
			}
			return map;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="property"></param>
		/// <param name="delim"></param>
		/// <param name="properties"></param>
		/// <returns></returns>
		public static string[ ] ToStringArray( string property, string delim, IDictionary properties )
		{
			return ToStringArray( ( string ) properties[ property ], delim );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propValue"></param>
		/// <param name="delim"></param>
		/// <returns></returns>
		public static string[ ] ToStringArray( string propValue, string delim )
		{
			if( propValue != null )
			{
				return StringHelper.Split( delim, propValue );
			}
			else
			{
				return new string[0];
			}
		}


		/// <summary></summary>
		public const string TagParam = "param";
		/// <summary></summary>
		public const string AttrValue = "value";
		/// <summary></summary>
		public const string AttrName = "name";

		/// <summary>
		/// Extracts a set of param child nodes from the specified node
		/// &lt;param name="theName" value="theValue"/&gt;
		/// </summary>
		/// <param name="node">Parent element.</param>
		/// <returns>null if no parameters are found</returns>
		public static IDictionary GetParams( XmlElement node )
		{
			IDictionary result = new Hashtable();

			foreach( XmlElement paramNode in node.GetElementsByTagName( TagParam ) )
			{
				string name = GetAttribute( paramNode, AttrName );
				string val = GetAttribute( paramNode, AttrValue );
				if( val == null )
					if( paramNode.HasChildNodes )
					{
						val = paramNode.InnerText; //TODO: allow for multiple values?
					}
					else
					{
						val = paramNode.InnerText;
					}

				result.Add( name, val );
			}
			return result;
		}

		private static string GetAttribute( XmlElement node, string attr )
		{
			string result = node.GetAttribute( attr );
			if( result != null && result.Trim().Length == 0 )
				result = null;

			return result;
		}
	}
}