using System;
using StringHelper = NHibernate.Util.StringHelper;
using MultiHashMap = System.Collections.Hashtable;
using MultiMap = System.Collections.Hashtable;
using Element = System.Xml.XmlElement;

namespace NHibernate.Tool.hbm2net
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
			SupportClass.ListCollectionSupport metaAttributeList = new SupportClass.ListCollectionSupport();
			metaAttributeList.AddAll(element.SelectNodes("meta", CodeGenerator.nsmgr));
			metaAttributeList.AddAll(element.SelectNodes("urn:meta", CodeGenerator.nsmgr));
			
			for (System.Collections.IEnumerator iter = metaAttributeList.GetEnumerator(); iter.MoveNext(); )
			{
				Element metaAttrib = (Element) iter.Current;
				// does not use getTextNormalize() or getTextTrim() as that would remove the formatting in new lines in items like description for javadocs.
				System.String attribute = (metaAttrib.Attributes["attribute"] == null?string.Empty:metaAttrib.Attributes["attribute"].Value);
				System.String value_Renamed = metaAttrib.InnerText;
				System.String inheritStr = (metaAttrib.Attributes["inherit"] == null?null:metaAttrib.Attributes["inherit"].Value);
				bool inherit = true;
				if ((System.Object) inheritStr != null)
				{
					try
					{
						inherit = System.Boolean.Parse(inheritStr);
					}
					catch{}
				}
				
				MetaAttribute ma = new MetaAttribute(value_Renamed, inherit);
				if (result[attribute] == null)
					result[attribute] = new SupportClass.ListCollectionSupport();

				((SupportClass.ListCollectionSupport)result[attribute]).Add(ma);
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
				for (System.Collections.IEnumerator iter = new SupportClass.SetSupport(inherited.Keys).GetEnumerator(); iter.MoveNext(); )
				{
					System.String key = (System.String) iter.Current;
					
					if (!local.ContainsKey(key))
					{
						// inheriting a meta attribute only if it is inheritable
						System.Collections.ArrayList ml = (System.Collections.ArrayList) inherited[key];
						for (System.Collections.IEnumerator iterator = ml.GetEnumerator(); iterator.MoveNext(); )
						{
							MetaAttribute element = (MetaAttribute) iterator.Current;
							if (element.inheritable)
							{
								if (result[key] == null)
									result[key] = new SupportClass.ListCollectionSupport();

								((SupportClass.ListCollectionSupport)result[key]).Add(element);
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
		public static System.String getMetaAsString(SupportClass.ListCollectionSupport meta, System.String seperator)
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			bool first = true;
			for (System.Collections.IEnumerator iter = meta.GetEnumerator(); iter.MoveNext(); )
			{
				if (first)
					first = false;
				else
					buf.Append(seperator);

				buf.Append(iter.Current);
			}
			return buf.ToString();
		}
		
		internal static bool getMetaAsBool(SupportClass.ListCollectionSupport c, bool defaultValue)
		{
			if (c == null || c.IsEmpty())
			{
				return defaultValue;
			}
			else
			{
				try
				{
					//return System.Boolean.Parse(c.GetEnumerator().Current.ToString());
					return Convert.ToBoolean(c[0].ToString());
				}
				catch{}
				return defaultValue;
			}
		}
		
		internal static System.String getMetaAsString(SupportClass.ListCollectionSupport c)
		{
			if (c == null || c.IsEmpty())
			{
				return string.Empty;
			}
			else
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				for (System.Collections.IEnumerator iter = c.GetEnumerator(); iter.MoveNext(); )
				{
					System.Object element = iter.Current;
					sb.Append(element.ToString());
				}
				return sb.ToString();
			}
		}
	}
}