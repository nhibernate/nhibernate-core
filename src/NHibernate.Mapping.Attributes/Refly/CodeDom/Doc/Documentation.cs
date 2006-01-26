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
using System.IO;
using System.Xml;
using System.CodeDom;

namespace Refly.CodeDom.Doc
{
	using Refly.CodeDom.Collections;

	public class Documentation
	{
		private XmlMarkup doc = new XmlMarkup("doc");
		private XmlMarkup summary=null;
		private XmlMarkup remarks=null;

		public Documentation()
		{}

        public bool HasSummary
        {
            get { return this.summary != null; }
        }
        public XmlMarkup Summary
		{
			get
			{
                if (this.summary==null)
                    this.summary = this.doc.Add("summary");
                return this.summary;
			}
		}

        public bool HasRemarks
        {
            get { return this.remarks != null; }
        }
        public XmlMarkup Remarks
        {
			get
			{
                if (this.remarks == null)
                    this.remarks = this.doc.Add("remarks");
                return this.remarks;
			}
		}

		public void AddException(Type t, string description)
		{
			XmlMarkup  m =this.doc.Add("exception",TypeFinder.CrossRef(description));
			m.AddAttribute("cref",t.FullName);
		}

		public void AddException(ThrowedExceptionDeclaration ex)
		{
			AddException(ex.ExceptionType,ex.Description);
		}

		public XmlMarkup AddParam(ParameterDeclaration para)
		{
			XmlMarkup m = this.doc.Add("param");
			m.AddAttribute("name",para.Name);
			return m;
		}

		public void AddInclude(string file, string path)
		{
			XmlMarkup m = this.doc.Add("include");
			m.AddAttribute("file",file);
			m.AddAttribute("path",path);
		}

		public void ToCodeDom(CodeCommentStatementCollection comments)
		{
			// comments
			foreach(XmlElement el in this.doc.Root.ChildNodes)
			{
				StringWriter sw = new StringWriter();
				XmlTextWriter w = new XmlTextWriter(sw);
				w.Formatting=Formatting.Indented;
				el.WriteTo(w);
				w.Flush();
				foreach(String s in sw.ToString().Split('\r'))
				{
					if (s.Length<=1)
						continue;
					if (s[0]=='\n')
						comments.Add(new CodeCommentStatement(
							s.Substring(1,s.Length-1),true)
							);
					else
						comments.Add(new CodeCommentStatement(
							s,true)
							);
				}
			}
		}
	}
}
