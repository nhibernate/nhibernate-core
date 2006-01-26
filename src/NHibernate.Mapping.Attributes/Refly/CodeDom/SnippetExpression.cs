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
using System.CodeDom;
using System.CodeDom.Compiler;

namespace Refly.CodeDom
{
	/// <summary>
	/// Summary description for SnippetExpression.
	/// </summary>
	public class SnippetExpression
	{
		private StringWriter sw;
		private IndentedTextWriter writer;

		public SnippetExpression(string expr)
		{
			this.sw = new StringWriter();
			this.writer = new IndentedTextWriter(sw,"    ");
			this.writer.Write(expr);
		}

		public SnippetExpression()
		{
			this.sw = new StringWriter();
			this.writer = new IndentedTextWriter(sw,"    ");
		}

		public IndentedTextWriter Out
		{
			get
			{
				return this.writer;
			}
		}

		public void Write(string format, params object[] args)
		{
			this.writer.Write(format,args);
		}

		public CodeExpression ToCodeDom()
		{
			return new CodeSnippetExpression(this.sw.ToString());
		}
	}
}
