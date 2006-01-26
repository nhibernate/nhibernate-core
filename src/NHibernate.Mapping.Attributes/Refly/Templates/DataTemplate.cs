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
	public class DataTemplate : Template
	{
		[Category("Data")]
		public string Name;
		[Category("Data")]
		public bool ReadOnly = false;
		[Category("Data")]
		public Hashtable Fields = new Hashtable();
		[Browsable(false)]
		public Hashtable Properties = null;

		public DataTemplate()
			:base("Data","{0}Data")
		{
		}

		public string DataName
		{
			get
			{
				return String.Format(this.NameFormat,this.Name);
			}
		}

		#region ITemplate Members

		public override void Generate()
		{
			if (this.Name==null)
				throw new InvalidOperationException("name not set");

			// create class
			ClassDeclaration c = this.NamespaceDeclaration.AddClass(this.DataName);

			this.Properties = new Hashtable();
			// add fields and properties
			foreach(DictionaryEntry de in this.Fields)
			{
				FieldDeclaration f = c.AddField(de.Value.ToString(), de.Key.ToString());

				PropertyDeclaration p = c.AddProperty(f,true,!ReadOnly,false);
				this.Properties.Add(de,p);
			}
            this.Compile();
        }

		#endregion
	}
}
