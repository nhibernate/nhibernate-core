using System;

using Element = System.Xml.XmlElement;
using MultiMap = System.Collections.Hashtable;

namespace NHibernate.Tool.hbm2net
{
	/// <summary>
	/// Place holder until we can get the superclass
	/// </summary>
	public class SubclassMapping
	{
		private string classPackage;
		private MappingElement mappingElement;
		private string superClass;
		private Element clazz;
		private MultiMap multiMap;
		private bool orphaned;

		public SubclassMapping(string classPackage, MappingElement mappingElement, string superClass, Element clazz,
		                       MultiMap multiMap)
		{
			this.classPackage = classPackage;
			this.mappingElement = mappingElement;
			this.superClass = superClass;
			this.clazz = clazz;
			this.multiMap = multiMap;
			this.orphaned = true;
		}

		/// <summary>
		/// Property ClassPackage (string)
		/// </summary>
		public string ClassPackage
		{
			get { return this.classPackage; }
		}

		public string Name
		{
			get { return clazz.Attributes["name"].Value; }
		}

		/// <summary>
		/// Property MappingElement (MappingElement)
		/// </summary>
		public MappingElement MappingElement
		{
			get { return this.mappingElement; }
		}

		/// <summary>
		/// Property SuperClass (string)
		/// </summary>
		public string SuperClass
		{
			get { return this.superClass; }
		}

		/// <summary>
		/// Property Clazz (Element)
		/// </summary>
		public Element Clazz
		{
			get { return this.clazz; }
		}

		/// <summary>
		/// Property MultiMap (MultiMap)
		/// </summary>
		public MultiMap MultiMap
		{
			get { return this.multiMap; }
		}

		/// <summary>
		/// Property Orphaned (bool)
		/// </summary>
		public bool Orphaned
		{
			get { return this.orphaned; }
			set { this.orphaned = value; }
		}
	}
}