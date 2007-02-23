using System;
using System.Collections;
using System.Text;

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
			internal string value_Renamed;
			internal bool inheritable = true;

			internal MetaAttribute(string value_Renamed, bool inherit)
			{
				this.value_Renamed = value_Renamed;
				this.inheritable = inherit;
			}

			public override string ToString()
			{
				return value_Renamed;
			}
		}

		/// <summary> Load meta attributes from jdom element into a MultiMap.
		/// 
		/// </summary>
		/// <returns> MultiMap
		/// </returns>
		protected internal static MultiMap loadMetaMap(Element element)
		{
			MultiMap result = new MultiHashMap();
			SupportClass.ListCollectionSupport metaAttributeList = new SupportClass.ListCollectionSupport();
			metaAttributeList.AddAll(element.SelectNodes("urn:meta", CodeGenerator.nsmgr));

			for (IEnumerator iter = metaAttributeList.GetEnumerator(); iter.MoveNext();)
			{
				Element metaAttrib = (Element) iter.Current;
				// does not use getTextNormalize() or getTextTrim() as that would remove the formatting in new lines in items like description for javadocs.
				string attribute = (metaAttrib.Attributes["attribute"] == null
				                    	? string.Empty : metaAttrib.Attributes["attribute"].Value);
				string value_Renamed = metaAttrib.InnerText;
				string inheritStr = (metaAttrib.Attributes["inherit"] == null ? null : metaAttrib.Attributes["inherit"].Value);
				bool inherit = true;
				if ((Object) inheritStr != null)
				{
					try
					{
						inherit = Boolean.Parse(inheritStr);
					}
					catch
					{
					}
				}

				MetaAttribute ma = new MetaAttribute(value_Renamed, inherit);
				if (result[attribute] == null)
					result[attribute] = new SupportClass.ListCollectionSupport();

				((SupportClass.ListCollectionSupport) result[attribute]).Add(ma);
			}
			return result;
		}

		/// <summary> Merges a Multimap with inherited maps.
		/// Values specified always overrules/replaces the inherited values.
		/// 
		/// </summary>
		/// <returns> a MultiMap with all values from local and extra values
		/// from inherited
		/// </returns>
		public static MultiMap mergeMetaMaps(MultiMap local, MultiMap inherited)
		{
			MultiHashMap result = new MultiHashMap();
			SupportClass.PutAll(result, local);

			if (inherited != null)
			{
				for (IEnumerator iter = new SupportClass.SetSupport(inherited.Keys).GetEnumerator(); iter.MoveNext();)
				{
					string key = (String) iter.Current;

					if (!local.ContainsKey(key))
					{
						// inheriting a meta attribute only if it is inheritable
						ArrayList ml = (ArrayList) inherited[key];
						for (IEnumerator iterator = ml.GetEnumerator(); iterator.MoveNext();)
						{
							MetaAttribute element = (MetaAttribute) iterator.Current;
							if (element.inheritable)
							{
								if (result[key] == null)
									result[key] = new SupportClass.ListCollectionSupport();

								((SupportClass.ListCollectionSupport) result[key]).Add(element);
							}
						}
					}
				}
			}

			return result;
		}

		/// <summary> Method loadAndMergeMetaMap.</summary>
		/// <returns> MultiMap
		/// </returns>
		public static MultiMap loadAndMergeMetaMap(Element classElement, MultiMap inheritedMeta)
		{
			return mergeMetaMaps(loadMetaMap(classElement), inheritedMeta);
		}

		public static string getMetaAsString(SupportClass.ListCollectionSupport meta, string seperator)
		{
			StringBuilder buf = new StringBuilder();
			bool first = true;
			for (IEnumerator iter = meta.GetEnumerator(); iter.MoveNext();)
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
				catch
				{
				}
				return defaultValue;
			}
		}

		internal static string getMetaAsString(SupportClass.ListCollectionSupport c)
		{
			if (c == null || c.IsEmpty())
			{
				return string.Empty;
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				for (IEnumerator iter = c.GetEnumerator(); iter.MoveNext();)
				{
					Object element = iter.Current;
					sb.Append(element.ToString());
				}
				return sb.ToString();
			}
		}
	}
}