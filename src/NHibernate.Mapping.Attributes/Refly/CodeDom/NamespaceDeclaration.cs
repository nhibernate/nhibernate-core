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
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.Collections;

namespace Refly.CodeDom
{
	using Refly.CodeDom.Collections;

	/// <summary>
	/// A namespace declaration
	/// </summary>
	public class NamespaceDeclaration : Declaration
	{
		private NamespaceDeclaration parent = null;
		private StringSet imports = new StringSet();
		private StringNamespaceDeclarationDictionary namespaces = new StringNamespaceDeclarationDictionary();
		private StringClassDeclarationDictionary classes = new StringClassDeclarationDictionary();
		private StringEnumDeclarationDictionary enums = new StringEnumDeclarationDictionary();

		public NamespaceDeclaration(string name)
			:base(name, new NameConformer())
		{
			this.imports.Add("System");
		}

		public NamespaceDeclaration(string name, NameConformer conformer)
			:base(name,conformer)
		{
			this.imports.Add("System");
		}

		public NamespaceDeclaration Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		public StringNamespaceDeclarationDictionary Namespaces
		{
			get
			{
				return this.namespaces;
			}
		}

		public StringClassDeclarationDictionary Classes
		{
			get
			{
				return this.classes;
			}
		}

		public StringEnumDeclarationDictionary Enums
		{
			get
			{
				return this.enums;
			}
		}

		public override String FullName
		{
			get
			{
				if (parent!=null)
					return String.Format("{0}.{1}",
						parent.FullName,
						this.Name
						);
				else
					return this.Name;
			}
		}
	
		public virtual StringSet Imports
		{
			get
			{
				return this.imports;
			}
		}

		public NamespaceDeclaration AddNamespace(string name)
		{
			if (name==null)
				throw new ArgumentNullException("name");
			if (this.namespaces.Contains(name))
				throw new ApplicationException("namespace already created");

			NamespaceDeclaration ns = new NamespaceDeclaration(
				this.Conformer.ToCapitalized(name),
				this.Conformer
				);
			ns.parent = this;
			this.namespaces.Add(name,ns);
			return ns;
		}

		public EnumDeclaration AddEnum(string name, bool flags)
		{
			if (name==null)
				throw new ArgumentNullException("name");
			if (this.enums.Contains(name))
				throw new ArgumentException("enum already present");
			EnumDeclaration e = new EnumDeclaration(name,this,flags);
			this.enums.Add(e);
			return e;
		}

		public ClassDeclaration AddClass(string name)
		{
			if (name==null)
				throw new ArgumentNullException("name");
			if (this.Classes.Contains(name))
				throw new ArgumentException("class already existing in namespace");

			ClassDeclaration c = new ClassDeclaration(name,this);
			this.classes.Add(c);
			return c;
		}

		public ClassDeclaration AddClass(string name, TypeAttributes attributes)
		{
			if (name==null)
				throw new ArgumentNullException("name");
			if (this.Classes.Contains(name))
				throw new ArgumentException("class already existing in namespace");

			ClassDeclaration c = new ClassDeclaration(name,this);
			c.Attributes = attributes;
			this.Classes.Add(c);
			return c;
		}

		public IDictionary ToCodeDom()
		{
			Hashtable codeNs = new Hashtable();

			// namespaces
			foreach(NamespaceDeclaration ns in this.namespaces.Values)
			{
				foreach(DictionaryEntry de in ns.ToCodeDom())
				{
					codeNs.Add(de.Key,de.Value);
				}
			}

			// classes
			foreach(ClassDeclaration c in this.classes.Values)
			{
				CodeNamespace ns = new CodeNamespace(this.Name);

				c.ToCodeDom(ns.Types);

				StringCollection usings = new StringCollection();
				foreach(String s in this.Imports)
					usings.Add(s);
				foreach(String import in c.Imports)
				{
					if (!usings.Contains(import))
						usings.Add(import);
				}
				// imports
				foreach(String import in usings)
				{
					ns.Imports.Add( new CodeNamespaceImport(import));
				}

				codeNs.Add(
					new FileName(this.FullName,c.Name),
					ns);
			}

			// enums
			foreach(EnumDeclaration e in this.enums.Values)
			{
				CodeNamespace ns = new CodeNamespace(this.Name);

				ns.Types.Add((CodeTypeDeclaration)e.ToCodeDom());

				StringCollection usings = new StringCollection();
				foreach(String s in this.imports)
					usings.Add(s);
				foreach(String import in usings)
				{
					ns.Imports.Add( new CodeNamespaceImport(import));
				}

				codeNs.Add(
					new FileName(this.FullName,e.Name),
					ns);			
			}


			return codeNs;
		}
	}

	public struct FileName
	{
		public FileName(string _namespace, string name)
		{
			this.Namespace =_namespace;
			this.Name=name;
		}
		public string Namespace;
		public string Name;

		public string FullName
		{
			get
			{
				return String.Format("{0}\\{1}",this.Namespace, this.Name);
			}
		}
	}
}
