using System;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;

namespace NHibernate.Util {
	
	//Much of this code is taken from Maverick.NET
	public class PropertiesHelper {

		public static bool GetBoolean(string property, IDictionary properties) {
			return properties[property] == null ?
				false :
				bool.Parse(properties[property] as string);
		}
		public static int GetInt(string property, IDictionary properties, int defaultValue) {
			string propValue = properties[property] as string;
			return (propValue==null) ? defaultValue : int.Parse(propValue);
		}
		public static string GetString(string property, IDictionary properties, string defaultValue) {
			string propValue = properties[property] as string;
			return (propValue==null) ? defaultValue : propValue;
		}
		
		public static IDictionary ToDictionary(string property, string delim, IDictionary properties) {
			IDictionary map = new Hashtable();
			string propValue = (string) properties[ property ];
			if (propValue!=null) {
				StringTokenizer tokens = new StringTokenizer(propValue, delim);
				IEnumerator en = tokens.GetEnumerator();
				while ( en.MoveNext() ) {
					string key = (string) en.Current;
					
					string value = en.MoveNext() ? (string) en.Current : StringHelper.EmptyString;
					map.Add( key, value );
				}
			}
			return map;
		}

		public static string[] ToStringArray(string property, string delim, IDictionary properties) {
			return ToStringArray( (string) properties[ property ], delim );
		}

		public static string[] ToStringArray(string propValue, string delim) {
			if (propValue!=null) {
				return StringHelper.Split(delim, propValue);
			} else {
				return new string[0];
			}
		}


		public const string TagParam = "param";
		public const string AttrValue = "value";
		public const string AttrName = "name";

		/// <summary>
		/// Extracts a set of param child nodes from the specified node
		/// &lt;param name="theName" value="theValue"/&gt;
		/// </summary>
		/// <param name="node">Parent element.</param>
		/// <returns>null if no parameters are found</returns>
		public static IDictionary GetParams(XmlElement node) {
			IDictionary result = new Hashtable();
			
			foreach( XmlElement paramNode in node.GetElementsByTagName(TagParam) ) {
				string name = GetAttribute(paramNode, AttrName);
				string val = GetAttribute(paramNode, AttrValue);
				if (val == null)
					if (paramNode.HasChildNodes) {
						val = paramNode.InnerText; //TODO: allow for multiple values?
					} else {
						val = paramNode.InnerText;
					}

				result.Add(name, val);
			}
			return result;
		}

		private static string GetAttribute(XmlElement node, string attr) {
			string result = node.GetAttribute(attr);
			if (result!=null && result.Trim().Length == 0)
				result = null;

			return result;
		}
	}
}
