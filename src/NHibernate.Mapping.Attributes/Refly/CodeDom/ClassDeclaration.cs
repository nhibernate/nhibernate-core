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
using System.Collections.Specialized;

namespace Refly.CodeDom
{
	using Refly.CodeDom.Collections;
	using Refly.CodeDom.Expressions;
	using Refly.CodeDom.Statements;

	/// <summary>
	/// A class declaration
	/// </summary>
	public class ClassDeclaration : Declaration
	{
		private NamespaceDeclaration ns;
		private ITypeDeclaration parent = null;
		private ClassOutputType outputType = ClassOutputType.Class;
#if WHIDBEY
        private TypeParameterDeclarationCollection typeParameters = new TypeParameterDeclarationCollection();
#endif
        private TypeDeclarationCollection interfaces = new TypeDeclarationCollection();
		private StringConstantDeclaration constants = new StringConstantDeclaration();
		private StringFieldDeclarationDictionary fields = new StringFieldDeclarationDictionary();
		private EventDeclarationCollection events = new EventDeclarationCollection();
		private PropertyDeclarationCollection properties = new PropertyDeclarationCollection();
		private MethodDeclarationCollection methods = new MethodDeclarationCollection();
		private DelegateDeclarationCollection delegates = new DelegateDeclarationCollection();
		private ConstructorDeclarationCollection constructors = new ConstructorDeclarationCollection();
		private IndexerDeclarationCollection indexers = new IndexerDeclarationCollection();
		private StringClassDeclarationDictionary nestedClasses = new StringClassDeclarationDictionary();
		private StringSet imports = new StringSet();
        private TypeAttributes attributes = TypeAttributes.Public;

        internal ClassDeclaration(string name, NamespaceDeclaration ns)
			:base(name,ns.Conformer)
		{
			this.ns = ns;
		}

		/// <summary>
		/// Gets or sets the output type.
		/// </summary>
		/// <value>
		/// A <see cref="ClassOutputType"/> instance.
		/// </value>
		public ClassOutputType OutputType
		{
			get
			{
				return this.outputType;
			}
			set
			{
				this.outputType = value;
			}
		}

		public NamespaceDeclaration Namespace
		{
			get
			{
				return this.ns;
			}
		}

#if WHIDBEY
        public TypeParameterDeclarationCollection TypeParameters
        {
            get { return this.typeParameters; }
        }
#endif

		public override string FullName
		{
			get
			{
				return String.Format("{0}.{1}",this.Namespace.FullName,this.Name);
			}
		}

		public ITypeDeclaration Parent
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

        public new TypeAttributes Attributes
        {
            get
            {
                return this.attributes;
            }
            set
            {
                this.attributes=value;
            }
        }

        public TypeDeclarationCollection Interfaces
		{
			get
			{
				return this.interfaces;
			}
		}

		public StringConstantDeclaration Constants
		{
			get
			{
				return this.constants;
			}
		}

		public StringFieldDeclarationDictionary Fields
		{
			get
			{
				return this.fields;
			}
		}

		public EventDeclarationCollection Events
		{
			get
			{
				return this.events;
			}
		}

		public PropertyDeclarationCollection Properties
		{
			get
			{
				return this.properties;
			}
		}

		public MethodDeclarationCollection Methods
		{
			get
			{
				return this.methods;
			}
		}

		public DelegateDeclarationCollection Delegates {
			get {
				return this.delegates;
			}
		}

		public StringClassDeclarationDictionary NestedClasses
		{
			get
			{
				return this.nestedClasses;
			}
		}

		public IndexerDeclarationCollection Indexers
		{
			get
			{
				return this.indexers;
			}
		}

		public StringSet Imports
		{
			get
			{
				return this.imports;
			}
		}

		public string InterfaceName
		{
			get
			{
				return String.Format("I{0}",Name);
			}
		}

		public ConstantDeclaration AddConstant(Type type, string name,SnippetExpression expression)
		{
			if (type==null)
				throw new ArgumentNullException("type");
			if (name==null)
				throw new ArgumentNullException("name");
			if (expression==null)
				throw new ArgumentNullException("expression");
			if (this.constants.Contains(name))
				throw new ArgumentException("field already existing in class");

			ConstantDeclaration c = new ConstantDeclaration(this.Conformer.ToCamel(name),this,type,expression);
			this.constants.Add(c);
			return c;
		}

		public FieldDeclaration AddField(ITypeDeclaration type, string name)
		{
			if (type==null)
				throw new ArgumentNullException("type");
			if (name==null)
				throw new ArgumentNullException("name");
			if (this.fields.Contains(name))
				throw new ArgumentException("field already existing in class");

			FieldDeclaration f = new FieldDeclaration(this.Conformer.ToCamel(name),this,type);
			this.fields.Add(f);
			return f;
		}

		public FieldDeclaration AddField(Type type, string name)
		{
			return AddField(new TypeTypeDeclaration(type),name);
		}

		public FieldDeclaration AddField(String type, string name)
		{
			return AddField(new StringTypeDeclaration(type),name);
		}

		public EventDeclaration AddEvent(ITypeDeclaration type, string name)
		{
			if (type==null)
				throw new ArgumentNullException("type");
			if (name==null)
				throw new ArgumentNullException("name");

			EventDeclaration e = new EventDeclaration(this.Conformer.ToCapitalized(name),this,type);
			this.events.Add(e);
			return e;
		}

		public EventDeclaration AddEvent(Type type, string name)
		{
			return AddEvent(new TypeTypeDeclaration(type),name);
		}

		public EventDeclaration AddEvent(string type, string name)
		{
			return AddEvent(new StringTypeDeclaration(type),name);
		}

		public ClassDeclaration AddClass(string name)
		{
			ClassDeclaration c = new ClassDeclaration(this.Conformer.ToCapitalized(name),this.Namespace);
			this.nestedClasses.Add(c);
			return c;
		}

		public IndexerDeclaration AddIndexer(ITypeDeclaration type)
		{
			if (type==null)
				throw new ArgumentNullException("type");
			IndexerDeclaration index = new IndexerDeclaration(
				this,
				type
				);
			this.indexers.Add(index);
			return index;
		}

		public IndexerDeclaration AddIndexer(Type type)
		{
			return AddIndexer(new TypeTypeDeclaration(type));
		}

		public IndexerDeclaration AddIndexer(string type)
		{
			return AddIndexer(new StringTypeDeclaration(type));
		}

		public PropertyDeclaration AddProperty(ITypeDeclaration type, string name)
		{
			if (type==null)
				throw new ArgumentNullException("type");
			if (name==null)
				throw new ArgumentNullException("name");

			PropertyDeclaration p = new PropertyDeclaration(this.Conformer.ToCapitalized(name),this,type);
			this.properties.Add(p);
			return p;
		}

		public PropertyDeclaration AddProperty(Type type, string name)
		{
			return AddProperty(new TypeTypeDeclaration(type),name);
		}

		public PropertyDeclaration AddProperty(String type, string name)
		{
			return AddProperty(new StringTypeDeclaration(type),name);
		}

		public PropertyDeclaration AddProperty(
			FieldDeclaration f,
			string name,
			bool hasGet, 
			bool hasSet, 
			bool checkNonNull
			) 
		{
			PropertyDeclaration p = this.AddProperty(
				f.Type,
				name);
			if (hasGet) 
			{
				p.Get.Return(Expr.This.Field(f));
			}
			if (hasSet) 
			{
				if (checkNonNull) 
				{
					ConditionStatement ifnull = Stm.If(Expr.Value.Identity(Expr.Null));
					p.Set.Add(ifnull);
					ifnull.TrueStatements.Add(
						Stm.Throw(typeof(ArgumentNullException))
						);
					p.SetExceptions.Add(new ThrowedExceptionDeclaration(
						typeof(ArgumentNullException),
						"value is a null reference"
						));
				}
				p.Set.Add(
					Stm.Assign(
					Expr.This.Field(f),
					Expr.Value
					)
					);
			}

			return p;
		}

		public PropertyDeclaration AddProperty(
			FieldDeclaration f, 
			bool hasGet, 
			bool hasSet, 
			bool checkNonNull
			) {
			return this.AddProperty(f, f.Name, hasGet, hasSet, checkNonNull);
		}

		public ConstructorDeclaration AddConstructor()
		{
			ConstructorDeclaration ctr = new ConstructorDeclaration(this);
			this.constructors.Add(ctr);
			return ctr;
		}

		public MethodDeclaration AddMethod(string name)
		{
			if (name==null)
				throw new ArgumentNullException("name");

			MethodDeclaration m = new MethodDeclaration(name,this);
			this.methods.Add(m);
			return m;
		}

		public DelegateDeclaration AddDelegate(string name) 
		{
			if (name==null)
				throw new ArgumentNullException("name");

			DelegateDeclaration d = new DelegateDeclaration(name,this);
			this.delegates.Add(d);
			return d;
		}

		public void ToCodeDom(CodeTypeDeclarationCollection types)
		{
			if(this.OutputType!=ClassOutputType.Interface)
			{
				CodeTypeDeclaration c = new CodeTypeDeclaration(this.Name);
				c.TypeAttributes = this.Attributes;
				types.Add(c);

				ToClassCodeDom(c);
			}
			if (this.OutputType==ClassOutputType.Interface || this.OutputType==ClassOutputType.ClassAndInterface)
			{
				CodeTypeDeclaration c = new CodeTypeDeclaration(InterfaceName);
				types.Add(c);
				ToInterfaceCodeDom(c);
			}
		}

		private void ToClassCodeDom(CodeTypeDeclaration c)
		{
			base.ToCodeDom(c);
            // set as class
			switch(this.OutputType)
			{
				case ClassOutputType.Class:
					c.IsClass = true;
					break;
				case ClassOutputType.ClassAndInterface:
					c.IsClass = true;
					break;
				case ClassOutputType.Struct:
					c.IsStruct = true;
					break;
			}
			// add parent
			if (this.Parent!=null)
				c.BaseTypes.Add(this.Parent.TypeReference);

#if WHIDBEY
            // adding generic parameters
            foreach (TypeParameterDeclaration typeParameter in this.TypeParameters)
                c.TypeParameters.Add(typeParameter.ToCodeDom());
#endif

			// add interfaces
			if (this.OutputType==ClassOutputType.ClassAndInterface)
				c.BaseTypes.Add(new CodeTypeReference(InterfaceName));
			foreach(ITypeDeclaration itf in this.interfaces)
			{
				c.BaseTypes.Add(itf.TypeReference);
			}

			// add methods
			foreach(DelegateDeclaration d in this.delegates) 
			{
				c.Members.Add( d.ToCodeDom() );
			}

			// add constants
			foreach(ConstantDeclaration ct in this.constants.Values)
			{
				c.Members.Add( ct.ToCodeDom() );
			}

			// add fields
			foreach(FieldDeclaration f in this.fields.Values)
			{
				c.Members.Add( f.ToCodeDom());
			}

			// add constructors
			foreach(ConstructorDeclaration ctr in this.constructors)
			{
				c.Members.Add( ctr.ToCodeDom() );
			}

			// add events
			foreach(EventDeclaration e in this.events)
			{
				c.Members.Add( e.ToCodeDom());
			}

			// add inderxers
			foreach(IndexerDeclaration index in this.indexers)
			{
				c.Members.Add( index.ToCodeDom());
			}

			// add properties
			foreach(PropertyDeclaration p in this.properties)
			{
				c.Members.Add( p.ToCodeDom());
			}

			// add methods
			foreach(MethodDeclaration m in this.methods)
			{
				c.Members.Add( m.ToCodeDom() );
			}

			// add nested classes
			foreach(ClassDeclaration nc in this.NestedClasses.Values)
			{
				CodeTypeDeclaration ctype = new CodeTypeDeclaration(nc.Name);
				nc.ToClassCodeDom(ctype);				
				c.Members.Add(ctype);
			}

		}

		private void ToInterfaceCodeDom(CodeTypeDeclaration c)
		{
			base.ToCodeDom(c);
			// set as class
			c.IsInterface = true;
#if WHIDBEY
            // adding generic parameters
            foreach (TypeParameterDeclaration typeParameter in this.TypeParameters)
                c.TypeParameters.Add(typeParameter.ToCodeDom());
#endif
			// add interfaces
			foreach(ITypeDeclaration itf in this.interfaces)
			{
				c.BaseTypes.Add(itf.TypeReference);
			}

			// add events
			foreach(EventDeclaration e in this.events)
			{
				c.Members.Add( e.ToCodeDom());
			}

			// add inderxers
			foreach(IndexerDeclaration index in this.indexers)
			{
				c.Members.Add( index.ToCodeDom());
			}

			// add properties
			foreach(PropertyDeclaration p in this.properties)
			{
				c.Members.Add( p.ToCodeDom());
			}

			// add methods
			foreach(MethodDeclaration m in this.methods)
			{
				c.Members.Add( m.ToCodeDom() );
			}
		}
	}
}
