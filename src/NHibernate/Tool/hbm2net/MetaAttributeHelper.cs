using System;
using StringHelper = NHibernate.Util.StringHelper;
using MultiHashMap = System.Collections.Hashtable;
using MultiMap = System.Collections.Hashtable;
using Element = System.Xml.XmlElement;

namespace NHibernate.tool.hbm2net
{
	
	/// <summary> Helper for loading, merging  and accessing <meta> tags.
	/// 
	/// </summary>
	/// <author>  max
	/// 
	/// 
	/// </author>
	public class MetaAttributeHelper
	{
		
		internal class MetaAttribute
		{
			internal System.String value_Renamed;
			internal bool inheritable = true;
			
			internal MetaAttribute(System.String value_Renamed, bool inherit)
			{
				this.value_Renamed = value_Renamed;
				this.inheritable = inherit;
			}
			
			public override System.String ToString()
			{
				return value_Renamed;
			}
		}
		
		/// <summary> Load meta attributes from jdom element into a MultiMap.
		/// 
		/// </summary>
		/// <param name="">element
		/// </param>
		/// <returns> MultiMap
		/// </returns>
		static protected internal MultiMap loadMetaMap(Element element)
		{
			MultiMap result = new MultiHashMap();
			//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			SupportClass.ListCollectionSupport metaAttributeList = new SupportClass.ListCollectionSupport();
			metaAttributeList.AddAll(element.GetElementsByTagName("meta"));
			
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			for (System.Collections.IEnumerator iter = metaAttributeList.GetEnumerator(); iter.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				Element metaAttrib = (Element) iter.Current;
				// does not use getTextNormalize() or getTextTrim() as that would remove the formatting in new lines in items like description for javadocs.
				System.String attribute = metaAttrib.Attributes["attribute"].Value;
				System.String value_Renamed = metaAttrib.InnerText;
				System.String inheritStr = metaAttrib.Attributes["inherit"].Value;
				bool inherit = true;
				if ((System.Object) inheritStr != null)
				{
					//UPGRADE_NOTE: Exceptions thrown by the equivalent in .NET of method 'java.lang.Boolean.valueOf' may be different. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1099"'
					inherit = System.Boolean.Parse(inheritStr);
				}
				
				MetaAttribute ma = new MetaAttribute(value_Renamed, inherit);
				object tempObject;
				tempObject = ma;
				result[attribute] = tempObject;
				System.Object generatedAux2 = tempObject;
			}
			return result;
		}
		
		/// <summary> Merges a Multimap with inherited maps.
		/// Values specified always overrules/replaces the inherited values.
		/// 
		/// </summary>
		/// <param name="">local
		/// </param>
		/// <param name="">inherited
		/// </param>
		/// <returns> a MultiMap with all values from local and extra values
		/// from inherited
		/// </returns>
		static public MultiMap mergeMetaMaps(MultiMap local, MultiMap inherited)
		{
			MultiHashMap result = new MultiHashMap();
			SupportClass.PutAll(result, local);
			
			if (inherited != null)
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
				for (System.Collections.IEnumerator iter = new SupportClass.SetSupport(inherited.Keys).GetEnumerator(); iter.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
					System.String key = (System.String) iter.Current;
					
					//UPGRADE_ISSUE: Method 'java.util.Map.containsKey' was not converted. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1000"'
					if (!local.ContainsKey(key))
					{
						// inheriting a meta attribute only if it is inheritable
						//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
						//UPGRADE_TODO: Method 'java.util.Map.get' was converted to 'System.Collections.IDictionary.Item' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilMapget_javalangObject"'
						System.Collections.ArrayList ml = (System.Collections.ArrayList) inherited[key];
						//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
						for (System.Collections.IEnumerator iterator = ml.GetEnumerator(); iterator.MoveNext(); )
						{
							//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
							MetaAttribute element = (MetaAttribute) iterator.Current;
							if (element.inheritable)
							{
								object tempObject;
								tempObject = element;
								result[key] = tempObject;
								System.Object generatedAux = tempObject;
							}
						}
					}
				}
			}
			
			return result;
		}
		/// <summary> Method loadAndMergeMetaMap.</summary>
		/// <param name="">classElement
		/// </param>
		/// <param name="">inheritedMeta
		/// </param>
		/// <returns> MultiMap
		/// </returns>
		public static MultiMap loadAndMergeMetaMap(Element classElement, MultiMap inheritedMeta)
		{
			return mergeMetaMaps(loadMetaMap(classElement), inheritedMeta);
		}
		
		/// <param name="">collection
		/// </param>
		/// <param name="">string
		/// </param>
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.Collection'' and ''SupportClass.CollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		public static System.String getMetaAsString(SupportClass.ListCollectionSupport meta, System.String seperator)
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			for (System.Collections.IEnumerator iter = meta.GetEnumerator(); iter.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				buf.Append(iter.Current);
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
				if (iter.MoveNext())
					buf.Append(seperator);
			}
			return buf.ToString();
		}
		
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.Collection'' and ''SupportClass.CollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		internal static bool getMetaAsBool(SupportClass.ListCollectionSupport c, bool defaultValue)
		{
			if (c == null || c.IsEmpty())
			{
				return defaultValue;
			}
			else
			{
				//UPGRADE_NOTE: Exceptions thrown by the equivalent in .NET of method 'java.lang.Boolean.valueOf' may be different. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1099"'
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				return System.Boolean.Parse(c.GetEnumerator().Current.ToString());
			}
		}
		
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.Collection'' and ''SupportClass.CollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		internal static System.String getMetaAsString(SupportClass.ListCollectionSupport c)
		{
			if (c == null || c.IsEmpty())
			{
				return string.Empty;
			}
			else
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
				for (System.Collections.IEnumerator iter = c.GetEnumerator(); iter.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
					System.Object element = iter.Current;
					//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
					sb.Append(element.ToString());
				}
				return sb.ToString();
			}
		}
	}
}