/*
* Created on 28-09-2003
* 
* To change the template for this generated file go to Window - Preferences -
* Java - Code Generation - Code and Comments
*/
using System;
using MultiMap = System.Collections.Hashtable;
using Element = System.Xml.XmlElement;

namespace NHibernate.tool.hbm2net
{
	/// <author>  MAX
	/// 
	/// To change the template for this generated type comment go to Window -
	/// Preferences - Java - Code Generation - Code and Comments
	/// </author>
	public class MappingElement
	{
		virtual public MappingElement ParentElement
		{
			get
			{
				return parentElement;
			}
			
		}
		virtual public Element XMLElement
		{
			get
			{
				return element;
			}
			
		}
		virtual public Element Element
		{
			set
			{
				this.element = value;
			}
			
		}
		virtual protected internal MultiMap MetaAttribs
		{
			get
			{
				return metaattribs;
			}
			
			set
			{
				this.metaattribs = value;
			}
			
		}
		
		private Element element;
		private MappingElement parentElement;
		private MultiMap metaattribs;
		
		public MappingElement(Element element, MappingElement parentElement)
		{
			this.element = element;
			this.parentElement = parentElement; // merge with parent meta map
			/*
			* MultiMap inherited = null; if (parentModel != null) { inherited =
			* parentModel.getMetaMap(); }
			*/
		}
		
		/// <summary>Returns true if this element has the meta attribute </summary>
		public virtual bool hasMeta(System.String attribute)
		{
			//UPGRADE_ISSUE: Method 'java.util.Map.containsKey' was not converted. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1000"'
			return metaattribs.ContainsKey(attribute);
		}
		
		/* Given a key, return the list of metaattribs. Can return null! */
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		public virtual SupportClass.ListCollectionSupport getMeta(System.String attribute)
		{
			//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			//UPGRADE_TODO: Method 'java.util.Map.get' was converted to 'System.Collections.IDictionary.Item' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilMapget_javalangObject"'
			return (SupportClass.ListCollectionSupport) metaattribs[attribute];
		}
		
		/// <summary> Returns all meta items as one large string.
		/// 
		/// </summary>
		/// <param name="">string
		/// </param>
		/// <returns> String
		/// </returns>
		public virtual System.String getMetaAsString(System.String attribute)
		{
			//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			SupportClass.ListCollectionSupport c = getMeta(attribute);
			
			return MetaAttributeHelper.getMetaAsString(c);
		}
		
		public virtual System.String getMetaAsString(System.String attribute, System.String seperator)
		{
			return MetaAttributeHelper.getMetaAsString(getMeta(attribute), seperator);
		}
		
		public virtual bool getMetaAsBool(System.String attribute)
		{
			return getMetaAsBool(attribute, false);
		}
		
		public virtual bool getMetaAsBool(System.String attribute, bool defaultValue)
		{
			//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			SupportClass.ListCollectionSupport c = getMeta(attribute);
			
			return MetaAttributeHelper.getMetaAsBool(c, defaultValue);
		}
	}
}