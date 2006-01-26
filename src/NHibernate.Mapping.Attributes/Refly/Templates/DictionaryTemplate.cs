/// Refly License
/// 
/// Copyright (c) 2004 Jonathan de Halleux, http://www.dotnetwiki.org
///
/// This software is provided 'as-is', without any express or implied warranty. In no event will the authors be held liable for any damages arising from the use of this software.
/// 
/// Permission is granted to anyone to use this software for any purpose, including commercial applications, and to alter it and redistribute it freely, subject to the following restrictions:
///
/// 1. The origin of this software must not be misrepresented; you must not claim that you wrote the original software. If you use this software in a product, an acknowledgment in the product documentation would be appreciated but is not required.
/// 
/// 2. Altered source versions must be plainly marked as such, and must not be misrepresented as being the original software.
///
///3. This notice may not be removed or altered from any source distribution.

using System;
using System.ComponentModel;
using System.Collections;
using System.Xml.Serialization;

namespace Refly.Templates
{
	using Refly.CodeDom;
	using Refly.CodeDom.Statements;

	public class DictionaryTemplate : Template
	{
		[Category("Data")]
		public String Key = null;
		[Category("Data")]
		public String Value = null;
		[Category("Data")]
		public bool ItemGet = true;
		[Category("Data")]
		public bool ItemSet = true;
		[Category("Data")]
		public bool Add = true;
		[Category("Data")]
		public bool Contains = true;
		[Category("Data")]
		public bool Remove = true;

		public DictionaryTemplate()
			:base("Strongly Typed Dictionary", "{0}{1}Dictionary")
		{}

		protected ITypeDeclaration KeyType
		{
			get
			{
				if (this.Key==null)
					throw new InvalidOperationException("Key type is undifned");
				return new StringTypeDeclaration(this.Key);
			}
		}

		protected ITypeDeclaration ValueType
		{
			get
			{
				if (this.Value==null)
					throw new InvalidOperationException("Value type is undifned");
				return new StringTypeDeclaration(this.Value);
			}
		}

		public ClassDeclaration AddClass(NamespaceDeclaration ns)
		{
			ClassDeclaration col = ns.AddClass(this.DictionaryName);

			// set base class as CollectionBase
			col.Parent = new TypeTypeDeclaration(typeof(DictionaryBase));

			// default constructor
			col.AddConstructor();

			// add indexer
			if (this.ItemGet || this.ItemSet)
			{
				IndexerDeclaration index = col.AddIndexer(
					this.ValueType
					);
				ParameterDeclaration pindex = index.Signature.Parameters.Add(KeyType,"key",false);

				// get body
				if (this.ItemGet)
				{
					index.Get.Return(
						(Expr.This.Prop("Dictionary").Item(Expr.Arg(pindex)).Cast(this.ValueType)
						)
						);
				}
				// set body
				if (this.ItemSet)
				{
					index.Set.Add(
						Stm.Assign(
						Expr.This.Prop("Dictionary").Item(Expr.Arg(pindex)),
						Expr.Value
						)
						);
				}

			}
			
			// add method
			if (this.Add)
			{
				MethodDeclaration add = col.AddMethod("Add");
				ParameterDeclaration pKey = add.Signature.Parameters.Add(this.KeyType,"key",true);
				ParameterDeclaration pValue = add.Signature.Parameters.Add(this.ValueType,"value",true);
				add.Body.Add(
					Expr.This.Prop("Dictionary").Method("Add").Invoke(pKey,pValue)
					);
			}

			// contains method
			if (this.Contains)
			{
				MethodDeclaration contains = col.AddMethod("Contains");
				contains.Signature.ReturnType = new TypeTypeDeclaration(typeof(bool));
				ParameterDeclaration pKey = contains.Signature.Parameters.Add(this.KeyType,"key",true);
				contains.Body.Return(
					Expr.This.Prop("Dictionary").Method("Contains").Invoke(pKey)
					);
			}

			// remove method
			if (this.Remove)
			{
				MethodDeclaration remove = col.AddMethod("Remove");
				ParameterDeclaration pKey = remove.Signature.Parameters.Add(this.KeyType,"key",true);

				remove.Body.Add(
					Expr.This.Prop("Dictionary").Method("Remove").Invoke(pKey)
					);
			}

			return col;
		}

		protected string DictionaryName
		{
			get
			{
				return String.Format(this.NameFormat,this.KeyType.Name,this.ValueType.Name);
			}
		}

		#region ITemplate Members

		public override void Generate()
		{
            this.Prepare();
            AddClass(this.NamespaceDeclaration);
            this.Compile();
        }

		#endregion
	}
}
