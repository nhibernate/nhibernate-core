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
using System.Collections;
using System.CodeDom;
using System.ComponentModel;
using System.Data;

namespace Refly.Templates
{
	using Refly.CodeDom;
	using Refly.CodeDom.Expressions;
	using Refly.CodeDom.Statements;
	using Refly.CodeDom.Collections;

	/// <summary>
	/// Summary description for DataReaderTemplate.
	/// </summary>
	public class DataReaderTemplate : Template
	{
		[Category("Data")]
		public bool Enumerator = true;		
		[Category("Data")]
		public DataTemplate Data = new DataTemplate();

		public DataReaderTemplate()
			:base("Strongly Typed Data Reader","{0}DataReader")
		{}

		public string DataReaderName
		{
			get
			{
				return String.Format(this.NameFormat,this.Data.DataName);
			}
		}

		#region ITemplate Members


		public override void Generate()
		{
			// generate data
			this.Data.NamespaceDeclaration = this.NamespaceDeclaration;
			this.Data.Generate();

			// generate the rest
			this.NamespaceDeclaration.Imports.Add("System.Data");

			// create class
			ClassDeclaration c = this.NamespaceDeclaration.AddClass(this.DataReaderName);

			// IDisposable
			c.Interfaces.Add(typeof(IDisposable));

			// add datareader field
			FieldDeclaration dr = c.AddField(typeof(IDataReader),"dr");

			// add data field
			FieldDeclaration data = c.AddField(
				this.Data.DataName
				,"data");
			data.InitExpression = 
				Expr.New(data.Type);
			PropertyDeclaration datap = c.AddProperty(data,true,false,false);

			// foreach field values, add get property
			foreach(DictionaryEntry de in this.Data.Properties)
			{
				DictionaryEntry dde = (DictionaryEntry)de.Key;
				PropertyDeclaration pd = (PropertyDeclaration)de.Value;

				PropertyDeclaration pcd = c.AddProperty(pd.Type,pd.Name);
				pcd.Get.Return(
					Expr.This.Field(data).Prop(pd)
					);
			}


			// add constructor
			ConstructorDeclaration cs = c.AddConstructor();
			ParameterDeclaration drp = cs.Signature.Parameters.Add(dr.Type,"dr",false);
			cs.Body.Add( Stm.ThrowIfNull(drp) );
			cs.Body.Add(
				Stm.Assign(
					Expr.This.Field(dr),
					Expr.Arg(drp)
					)
				);

			// add close method
			MethodDeclaration close = c.AddMethod("Close");
			// if dr ==null return;
			close.Body.Add(
				Stm.IfNull(Expr.This.Field(dr),Stm.Return())
				);
			// dr.Close();
			close.Body.Add(
				Expr.This.Field(dr).Method("Close").Invoke()
				);
			// dr = null;
			close.Body.AddAssign( Expr.This.Field(dr),Expr.Null );
			// data = null
			close.Body.AddAssign( Expr.This.Field(data), Expr.Null );

			// add read method
			MethodDeclaration read = c.AddMethod("Read");
			read.Signature.ReturnType = new TypeTypeDeclaration(typeof(bool));

			// if (!dr.Read()){close and return)
			ConditionStatement ifnotread = Stm.IfIdentity(
				Expr.This.Field(dr).Method("Read").Invoke(),
				Expr.False,
				Stm.ToStm(Expr.This.Method(close).Invoke()),
				Stm.Return(Expr.False)
				);
			read.Body.Add(ifnotread);


			// foreach field values
			foreach(DictionaryEntry de in this.Data.Properties)
			{
				DictionaryEntry dde = (DictionaryEntry)de.Key;
				PropertyDeclaration pd = (PropertyDeclaration)de.Value;
				read.Body.AddAssign(
					Expr.This.Field(data).Prop(pd),
					(
						Expr.This.Field(dr).Item(Expr.Prim(dde.Key.ToString()))
					).Cast(dde.Value.ToString())
					);
			}
			// return true
			read.Body.Return(Expr.True);

			// add dispose method
			MethodDeclaration dispose  = c.AddMethod("Dispose");
			dispose.ImplementationTypes.Add( typeof(IDisposable) );

			// Close();
			dispose.Body.Add(
				Expr.This.Method(close).Invoke()
				);

			if (this.Enumerator)
				AddEnumerator(c,data,close);
		}

		private void AddEnumerator(ClassDeclaration c, FieldDeclaration data, MethodDeclaration close)
		{
			c.Interfaces.Add(typeof(IEnumerable));
			// create subclass
			ClassDeclaration en = c.AddClass("Enumerator");
			// add wrapped field
			FieldDeclaration wrapped = en.AddField(
				c,"wrapped"
				);

			ITypeDeclaration enumeratorType = new TypeTypeDeclaration(typeof(IEnumerator));
			ITypeDeclaration disposableType = new TypeTypeDeclaration(typeof(IDisposable));

			// add IEnumerator
			en.Interfaces.Add(enumeratorType);
			en.Interfaces.Add(disposableType);

			// add constructor
			ConstructorDeclaration cs = en.AddConstructor();
			ParameterDeclaration collection = cs.Signature.Parameters.Add(c,"collection",true);
			cs.Body.AddAssign(Expr.This.Field(wrapped),Expr.Arg(collection));

			// add current 
			PropertyDeclaration current = en.AddProperty(data.Type,"Current");
			current.Get.Return(
				Expr.This.Field(wrapped).Prop("Data")
				);

			// add explicit interface implementation
			PropertyDeclaration currentEn = en.AddProperty(typeof(Object),"Current");
			currentEn.Get.Return(Expr.This.Prop(current));
			currentEn.PrivateImplementationType = enumeratorType;

			// add reset
			MethodDeclaration reset = en.AddMethod("Reset");
			reset.ImplementationTypes.Add( wrapped.Type );
			reset.Body.Add( Stm.Throw(typeof(InvalidOperationException),Expr.Prim("Not supported")));

			// add movenext
			MethodDeclaration movenext = en.AddMethod("MoveNext");
			movenext.ImplementationTypes.Add(wrapped.Type );
			movenext.Signature.ReturnType = new TypeTypeDeclaration(typeof(bool));
			movenext.Body.Return( Expr.This.Field(wrapped).Method("Read").Invoke());

			// add dispose
			MethodDeclaration disposeEn  = en.AddMethod("Dispose");
			disposeEn.ImplementationTypes.Add( disposableType );
			disposeEn.Body.Add(
				Expr.This.Field(wrapped).Method(close).Invoke()
				);
			disposeEn.Body.AddAssign( Expr.This.Field(wrapped),Expr.Null );


			// add get enuemrator
			MethodDeclaration geten = c.AddMethod("GetEnumerator");
			geten.Signature.ReturnType = en;
			geten.Body.Return(Expr.New(en,Expr.This));

			MethodDeclaration igeten = c.AddMethod("GetEnumerator");
			igeten.PrivateImplementationType = new TypeTypeDeclaration(typeof(IEnumerable));
			igeten.Signature.ReturnType = new TypeTypeDeclaration(typeof(IEnumerator));
			igeten.Body.Return(Expr.This.Method("GetEnumerator").Invoke());

		}

		#endregion
	}
}
