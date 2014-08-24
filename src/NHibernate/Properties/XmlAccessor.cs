using System;
using System.Collections;
using System.Reflection;
using System.Xml;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Properties
{
	/// <summary> 
	/// Responsible for accessing property values represented as a XmlElement
	/// or XmlAttribute. 
	/// </summary>
	[Serializable]
	public class XmlAccessor : IPropertyAccessor
	{
		private readonly ISessionFactoryImplementor factory;
		private readonly string nodeName;
		private readonly IType propertyType;

		public XmlAccessor(string nodeName, IType propertyType, ISessionFactoryImplementor factory)
		{
			this.factory = factory;
			this.nodeName = nodeName;
			this.propertyType = propertyType;
		}

		#region Implementation of IPropertyAccessor

		public IGetter GetGetter(System.Type theClass, string propertyName)
		{
			if (nodeName == null)
			{
				throw new MappingException("no node name for property: " + propertyName);
			}
			if (".".Equals(nodeName))
			{
				return new TextGetter(propertyType, factory);
			}
			else if (nodeName.IndexOf('/') > -1)
			{
				return new ElementAttributeGetter(nodeName, propertyType, factory);
			}
			else if (nodeName.IndexOf('@') > -1)
			{
				return new AttributeGetter(nodeName, propertyType, factory);
			}
			else
			{
				return new ElementGetter(nodeName, propertyType, factory);
			}
		}

		public ISetter GetSetter(System.Type theClass, string propertyName)
		{
			if (nodeName == null)
			{
				throw new MappingException("no node name for property: " + propertyName);
			}
			if (".".Equals(nodeName))
			{
				return new TextSetter(propertyType);
			}
			else if (nodeName.IndexOf('/') > -1)
			{
				return new ElementAttributeSetter(nodeName, propertyType);
			}
			else if (nodeName.IndexOf('@') > -1)
			{
				return new AttributeSetter(nodeName, propertyType);
			}
			else
			{
				return new ElementSetter(nodeName, propertyType);
			}
		}

		public bool CanAccessThroughReflectionOptimizer
		{
			get { return true; }
		}

		#endregion

		#region Nested type: AttributeGetter

		/// <summary> For nodes like <tt>"@bar"</tt></summary>
		[Serializable]
		public class AttributeGetter : XmlGetter
		{
			private readonly string attributeName;

			public AttributeGetter(string name, IType propertyType, ISessionFactoryImplementor factory)
				: base(propertyType, factory)
			{
				attributeName = name.Substring(1);
			}

			public override object Get(object owner)
			{
				var ownerElement = (XmlNode) owner;
				XmlNode attribute = ownerElement.Attributes[attributeName];
				return attribute == null ? null : propertyType.FromXMLNode(attribute, factory);
			}
		}

		#endregion

		#region Nested type: AttributeSetter

		/// <summary> For nodes like <tt>"@bar"</tt></summary>
		[Serializable]
		public class AttributeSetter : XmlSetter
		{
			private readonly string attributeName;

			public AttributeSetter(string name, IType propertyType) : base(propertyType)
			{
				attributeName = name.Substring(1);
			}

			public override void Set(object target, object value)
			{
				var owner = (XmlElement) target;
				XmlAttribute attribute = owner.Attributes[attributeName];
				if (value == null)
				{
					if (attribute != null)
					{
						owner.Attributes.Remove(attribute);
					}
				}
				else
				{
					if (attribute == null)
					{
						owner.SetAttribute(attributeName, "null");
						attribute = owner.Attributes[attributeName];
					}
					propertyType.SetToXMLNode(attribute, value, null);
				}
			}
		}

		#endregion

		#region Nested type: ElementAttributeGetter

		/// <summary> For nodes like <tt>"foo/@bar"</tt></summary>
		[Serializable]
		public class ElementAttributeGetter : XmlGetter
		{
			private readonly string attributeName;
			private readonly string elementName;

			public ElementAttributeGetter(string name, IType propertyType, ISessionFactoryImplementor factory)
				: base(propertyType, factory)
			{
				elementName = name.Substring(0, (name.IndexOf('/')));
				attributeName = name.Substring(name.IndexOf('/') + 2);
			}

			public override object Get(object owner)
			{
				var ownerElement = (XmlNode) owner;

				XmlNode element = ownerElement[elementName];

				if (element == null)
				{
					return null;
				}
				else
				{
					XmlAttribute attribute = element.Attributes[attributeName];
					if (attribute == null)
					{
						return null;
					}
					else
					{
						return propertyType.FromXMLNode(attribute, factory);
					}
				}
			}
		}

		#endregion

		#region Nested type: ElementAttributeSetter

		/// <summary> For nodes like <tt>"foo/@bar"</tt></summary>
		[Serializable]
		public class ElementAttributeSetter : XmlSetter
		{
			private readonly string attributeName;
			private readonly string elementName;

			public ElementAttributeSetter(string name, IType propertyType) : base(propertyType)
			{
				elementName = name.Substring(0, name.IndexOf('/'));
				attributeName = name.Substring(name.IndexOf('/') + 2);
			}

			public override void Set(object target, object value)
			{
				var owner = (XmlElement) target;
				XmlElement element = owner[elementName];
				if (value == null)
				{
					if (element != null)
					{
						owner.RemoveChild(element);
					}
				}
				else
				{
					XmlAttribute attribute;
					if (element == null)
					{
						element = owner.OwnerDocument.CreateElement(elementName);
						owner.AppendChild(element);
						attribute = null;
					}
					else
					{
						attribute = element.Attributes[attributeName];
					}

					if (attribute == null)
					{
						element.SetAttribute(attributeName, "null");
						attribute = element.Attributes[attributeName];
					}
					propertyType.SetToXMLNode(attribute, value, null);
				}
			}
		}

		#endregion

		#region Nested type: ElementGetter

		/// <summary> For nodes like <tt>"foo"</tt></summary>
		[Serializable]
		public class ElementGetter : XmlGetter
		{
			private readonly string elementName;

			public ElementGetter(string name, IType propertyType, ISessionFactoryImplementor factory)
				: base(propertyType, factory)
			{
				elementName = name;
			}

			public override object Get(object owner)
			{
				var ownerElement = (XmlNode) owner;
				XmlNode element = ownerElement[elementName];
				return element == null ? null : propertyType.FromXMLNode(element, factory);
			}
		}

		#endregion

		#region Nested type: ElementSetter

		/// <summary> For nodes like <tt>"foo"</tt></summary>
		[Serializable]
		public class ElementSetter : XmlSetter
		{
			private readonly string elementName;

			public ElementSetter(string name, IType propertyType) : base(propertyType)
			{
				elementName = name;
			}

			public override void Set(object target, object value)
			{
				if (value != CollectionType.UnfetchedCollection)
				{
					var owner = (XmlElement) target;
					XmlElement existing = owner[elementName];
					if (existing != null)
					{
						owner.RemoveChild(existing);
					}
					if (value != null)
					{
						XmlElement element = owner.OwnerDocument.CreateElement(elementName);
						owner.AppendChild(element);
						propertyType.SetToXMLNode(element, value, null);
					}
				}
			}
		}

		#endregion

		#region Nested type: TextGetter

		/// <summary> For nodes like <tt>"."</tt></summary>
		[Serializable]
		public class TextGetter : XmlGetter
		{
			public TextGetter(IType propertyType, ISessionFactoryImplementor factory) : base(propertyType, factory) {}

			public override object Get(object owner)
			{
				var ownerElement = (XmlNode) owner;
				return propertyType.FromXMLNode(ownerElement, factory);
			}
		}

		#endregion

		#region Nested type: TextSetter

		/// <summary> For nodes like <tt>"."</tt></summary>
		[Serializable]
		public class TextSetter : XmlSetter
		{
			public TextSetter(IType propertyType) : base(propertyType) {}

			public override void Set(object target, object value)
			{
				var owner = (XmlNode) target;
				if (!propertyType.IsXMLElement)
				{
					//kinda ugly, but needed for collections with a "." node mapping
					if (value == null)
					{
						owner.InnerText = ""; //is this ok?
					}
					else
					{
						propertyType.SetToXMLNode(owner, value, null);
					}
				}
			}
		}

		#endregion

		#region Nested type: XmlGetter

		/// <summary> Defines the strategy for getting property values out of a dom4j Node.</summary>
		[Serializable]
		public abstract class XmlGetter : IGetter
		{
			protected ISessionFactoryImplementor factory;
			protected IType propertyType;

			internal XmlGetter(IType propertyType, ISessionFactoryImplementor factory)
			{
				this.propertyType = propertyType;
				this.factory = factory;
			}

			#region IGetter Members

			/// <summary> Get the declared type</summary>
			public virtual System.Type ReturnType
			{
				get { return typeof (object); }
			}

			/// <summary> Optional operation (return null)</summary>
			public virtual string PropertyName
			{
				get { return null; }
			}

			/// <summary> Optional operation (return null)</summary>
			public virtual MethodInfo Method
			{
				get { return null; }
			}

			public virtual object GetForInsert(object owner, IDictionary mergeMap, ISessionImplementor session)
			{
				return Get(owner);
			}

			public abstract object Get(object target);

			#endregion
		}

		#endregion

		#region Nested type: XmlSetter

		[Serializable]
		public abstract class XmlSetter : ISetter
		{
			protected internal IType propertyType;

			internal XmlSetter(IType propertyType)
			{
				this.propertyType = propertyType;
			}

			#region ISetter Members

			/// <summary> Optional operation (return null)</summary>
			public virtual string PropertyName
			{
				get { return null; }
			}

			/// <summary> Optional operation (return null)</summary>
			public virtual MethodInfo Method
			{
				get { return null; }
			}

			public abstract void Set(object target, object value);

			#endregion
		}

		#endregion
	}
}