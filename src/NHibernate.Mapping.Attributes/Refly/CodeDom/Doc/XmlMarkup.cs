/// Refly License
/// 
/// Copyright (c) 2004 Jonathan de Halleux, http://www.dotnetwiki.org
///
/// This software is provided 'as-is', without any express or implied warranty. 
/// In no event will the authors be held liable for any damages arising from 
/// the use of this software.
/// 
/// Permission is granted to anyone to use this software for any purpose, 
/// including commercial applications, and to alter it and redistribute it 
/// freely, subject to the following restrictions:
///
/// 1. The origin of this software must not be misrepresented; 
/// you must not claim that you wrote the original software. 
/// If you use this software in a product, an acknowledgment in the product 
/// documentation would be appreciated but is not required.
/// 
/// 2. Altered source versions must be plainly marked as such, 
/// and must not be misrepresented as being the original software.
///
///3. This notice may not be removed or altered from any source distribution.

using System;
using System.Xml;

namespace Refly.CodeDom.Doc
{
	/// <summary>
	/// Summary description for XmlMarkup.
	/// </summary>
	public class XmlMarkup
	{
		private XmlDocument doc = new XmlDocument();
		private XmlElement root = null;
		private XmlElement current = null;

		public XmlMarkup(string root)
		{
			if (root == null)
				throw new ArgumentNullException("root");
			this.root =  this.doc.CreateElement(root);
			this.current = this.root;
		}

		public XmlMarkup(XmlDocument doc, XmlElement root)
		{
			this.doc = doc;
			this.current = this.root = root;
		}

		public XmlDocument Doc
		{
			get
			{
				return this.doc;
			}
		}

		public XmlElement Current
		{
			get
			{
				return this.current;
			}
		}

		public XmlElement Root
		{
			get
			{
				return this.root;
			}
		}

		public void ResetCursor()
		{
			this.current = this.root;
		}

		public void Into()
		{
			XmlElement child = this.current.FirstChild as XmlElement;
			if (child==null)
				throw new ArgumentException("reached tree boundary");
			this.current = child;
		}

		public void OutOf()
		{
			this.current = (XmlElement)this.current.ParentNode;
		}

		public XmlMarkup Add(string name)
		{
			XmlElement el = this.doc.CreateElement(name);
			this.current.AppendChild(el);

			return new XmlMarkup(this.doc,el);
		}

		public XmlMarkup Add(string name, string textFormat, params Object[] args)
		{
			XmlElement el = this.doc.CreateElement(name);
			this.current.AppendChild(el);

			XmlText t = this.doc.CreateTextNode(String.Format(textFormat,args));
			el.AppendChild(t);

			return new XmlMarkup(this.doc,el);
		}

		public void AddText(string textFormat, params Object[] args)
		{
			XmlText t = this.doc.CreateTextNode(String.Format(textFormat,args));
			this.current.AppendChild(t);
		}

		public XmlMarkup AddCDATA(string name, string textFormat, params Object[] args)
		{
			XmlElement el = this.doc.CreateElement(name);
			this.current.AppendChild(el);

			XmlCDataSection t = this.doc.CreateCDataSection(String.Format(textFormat,args));
			el.AppendChild(t);

			return new XmlMarkup(this.doc,el);
		}

		public XmlComment AddComment(string comment)
		{
			XmlComment com = this.doc.CreateComment(comment);
			this.current.AppendChild(com);
			return com;
		}

		public XmlAttribute AddAttribute(string name, string textFormat, params Object[] args)
		{
			XmlAttribute at = this.doc.CreateAttribute(name);
			at.Value = String.Format(textFormat,args);
			this.current.Attributes.Append(at);
			return at;
		}
	}
}
