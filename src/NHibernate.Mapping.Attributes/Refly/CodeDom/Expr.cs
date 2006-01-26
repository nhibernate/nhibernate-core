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
using System.CodeDom;

namespace Refly.CodeDom
{
	using Refly.CodeDom.Expressions;
	using Refly.CodeDom.Statements;

	/// <summary>
	/// Helper class containing static methods to create <see cref="Expression"/>
	/// instances.
	/// </summary>
	public sealed class Expr
	{
		#region Constructors
		private Expr(){}
		#endregion

        #region Keywords
        /// <summary>
        /// Create a <c>this</c> reference expression
		/// </summary>
		/// <remarks>
		/// Generated code:
		/// <code>
		/// [C#]
		/// this
		/// </code>
		/// </remarks>
		public static ThisReferenceExpression This
		{
			get
			{
				return new ThisReferenceExpression();
			}
		}

		/// <summary>
		/// Create a <c>base</c> reference expression
		/// </summary>
		/// <remarks>
		/// Generated code:
		/// <code>
		/// [C#]
		/// base
		/// </code>
		/// </remarks>
		public static BaseReferenceExpression Base
		{
			get
			{
				return new BaseReferenceExpression();
			}
        }

        /// <summary>
        /// Create a <c>value</c> reference expression of a
		/// <c>set</c> section inside a property
		/// </summary>
		/// <remarks>
		/// Generated code:
		/// <code>
		/// [C#]
		/// value
		/// </code>
		/// </remarks>
		public static PropertySetValueReferenceExpression Value
		{
			get
			{
				return new PropertySetValueReferenceExpression();
			}
		}

		/// <summary>
		/// Create a <c>null</c> expression
		/// </summary>
		/// <remarks>
		/// Generated code:
		/// <code>
		/// [C#]
		/// null
		/// </code>
		/// </remarks>
		public static NativeExpression Null
		{
			get
			{
				return new NativeExpression(
						new CodePrimitiveExpression(null)
					);
			}
		}

		/// <summary>
		/// Create a <c>true</c> expression
		/// </summary>
		/// <remarks>
		/// Generated code:
		/// <code>
		/// [C#]
		/// true
		/// </code>
		/// </remarks>
		public static PrimitiveExpression True
		{
			get
			{
				return Prim(true);
			}
		}

		/// <summary>
		/// Create a <c>false</c> expression
		/// </summary>
		/// <remarks>
		/// Generated code:
		/// <code>
		/// [C#]
		/// false
		/// </code>
		/// </remarks>
		public static PrimitiveExpression False
		{
			get
			{
				return Prim(false);
			}
        }
        #endregion

        #region New
        /// <summary>
        /// Creates a <c>new type(...)</c> expression.
		/// </summary>
		/// <param name="type">
		/// Target <see cref="Type"/> name.
		/// </param>
		/// <param name="parameters">
		/// Parameters of the construcotr.
		/// </param>
		/// <include file='Refly.CodeDom.Doc.xml' path='doc/remarkss/remark[@name="Expr.New"]'/>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> is a null reference (Noting in Visual Basic)
		/// </exception>
		public static ObjectCreationExpression New(
			String type,
			params Expression[] parameters)
		{
			if (type==null)
				throw new ArgumentNullException("type");
			return New(new StringTypeDeclaration(type),parameters);
		}

		/// <summary>
		/// Creates a <c>new type(...)</c> expression.
		/// </summary>
		/// <param name="type">
		/// Target <see cref="Type"/>.
		/// </param>
		/// <param name="parameters">
		/// Parameters of the construcotr.
		/// </param>
		/// <include file='Refly.CodeDom.Doc.xml' path='doc/remarkss/remark[@name="Expr.New"]'/>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> is a null reference (Noting in Visual Basic)
		/// </exception>
		public static ObjectCreationExpression New(
			Type type,
			params Expression[] parameters)
		{
			if (type==null)
				throw new ArgumentNullException("type");
			return New(new TypeTypeDeclaration(type),parameters);
		}

		/// <summary>
		/// Creates a <c>new t(...)</c> expression.
		/// </summary>
		/// <param name="type">
		/// Target <see cref="Type"/>.
		/// </param>
		/// <param name="parameters">
		/// Parameters of the construcotr.
		/// </param>
		/// <include file='Refly.CodeDom.Doc.xml' path='doc/remarkss/remark[@name="Expr.New"]'/>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> is a null reference (Noting in Visual Basic)
		/// </exception>
		public static ObjectCreationExpression New(
			ITypeDeclaration type,
			params Expression[] parameters)
		{
			if (type==null)
				throw new ArgumentNullException("type");
			return new ObjectCreationExpression(type,parameters);
        }
        #endregion 

        #region NewArray (size)
        /// <summary>
        /// Creates a <c>new type[size]</c> expression
        /// </summary>
        /// <param name="type">Array item type</param>
        /// <param name="size">Array size</param>
        /// <returns>A <see cref="ArrayCreationWithSizeExpression"/> instance</returns>
        /// <include file='Refly.CodeDom.Doc.xml' path='doc/remarkss/remark[@name="Expr.NewArray"]'/>
        /// <exception cref="ArgumentNullExpression">
        /// <paramref name="type"/> is a null reference.
        /// </exception>
        public static ArrayCreationWithSizeExpression NewArray(
            Type type,
            int size
            )
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return new ArrayCreationWithSizeExpression(
                new TypeTypeDeclaration(type), 
                Expr.Prim(size));
        }

        /// <summary>
        /// Creates a <c>new type[size]</c> expression
        /// </summary>
        /// <param name="type">Array item type</param>
        /// <param name="size">Array size</param>
        /// <returns>A <see cref="ArrayCreationWithSizeExpression"/> instance</returns>
        /// <include file='Refly.CodeDom.Doc.xml' path='doc/remarkss/remark[@name="Expr.NewArray"]'/>
        /// <exception cref="ArgumentNullExpression">
        /// <paramref name="type"/> is a null reference.
        /// </exception>
        public static ArrayCreationWithSizeExpression NewArray(
            String type,
            int size
            )
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return new ArrayCreationWithSizeExpression(
                new StringTypeDeclaration(type),
                Expr.Prim(size));
        }

        /// <summary>
        /// Creates a <c>new type[size]</c> expression
        /// </summary>
        /// <param name="type">Array item type</param>
        /// <param name="size">Array size</param>
        /// <returns>A <see cref="ArrayCreationWithSizeExpression"/> instance</returns>
        /// <include file='Refly.CodeDom.Doc.xml' path='doc/remarkss/remark[@name="Expr.NewArray"]'/>
        /// <exception cref="ArgumentNullExpression">
        /// <paramref name="type"/> is a null reference.
        /// </exception>
        public static ArrayCreationWithSizeExpression NewArray(
            ITypeDeclaration type,
            int size
            )
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return new ArrayCreationWithSizeExpression(type,
                Expr.Prim(size));
        }

        /// <summary>
        /// Creates a <c>new type[expression]</c> expression
        /// </summary>
        /// <param name="type">Array item type</param>
        /// <param name="sizeExpression">Array size <see cref="Expression"/></param>
        /// <returns>A <see cref="ArrayCreationWithSizeExpression"/> instance</returns>
        /// <include file='Refly.CodeDom.Doc.xml' path='doc/remarkss/remark[@name="Expr.NewArray"]'/>
        /// <exception cref="ArgumentNullExpression">
        /// <paramref name="type"/> or <paramref name="sizeExpression"/>
        /// is a null reference.
        /// </exception>
        public static ArrayCreationWithSizeExpression NewArray(
            Type type,
            Expression sizeExpression
            )
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return new ArrayCreationWithSizeExpression(
                new TypeTypeDeclaration(type), sizeExpression);
        }

        /// <summary>
        /// Creates a <c>new type[expression]</c> expression
        /// </summary>
        /// <param name="type">Array item type</param>
        /// <param name="sizeExpression">Array size <see cref="Expression"/></param>
        /// <returns>A <see cref="ArrayCreationWithSizeExpression"/> instance</returns>
        /// <include file='Refly.CodeDom.Doc.xml' path='doc/remarkss/remark[@name="Expr.NewArray"]'/>
        /// <exception cref="ArgumentNullExpression">
        /// <paramref name="type"/> or <paramref name="sizeExpression"/>
        /// is a null reference.
        /// </exception>
        public static ArrayCreationWithSizeExpression NewArray(
            String type,
            Expression sizeExpression
            )
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return new ArrayCreationWithSizeExpression(
                new StringTypeDeclaration(type), sizeExpression);
        }

        /// <summary>
        /// Creates a <c>new type[expression]</c> expression
        /// </summary>
        /// <param name="type">Array item type</param>
        /// <param name="sizeExpression">Array size <see cref="Expression"/></param>
        /// <returns>A <see cref="ArrayCreationWithSizeExpression"/> instance</returns>
        /// <include file='Refly.CodeDom.Doc.xml' path='doc/remarkss/remark[@name="Expr.NewArray"]'/>
        /// <exception cref="ArgumentNullExpression">
        /// <paramref name="type"/> or <paramref name="sizeExpression"/>
        /// is a null reference.
        /// </exception>
        public static ArrayCreationWithSizeExpression NewArray(
            ITypeDeclaration type,
            Expression sizeExpression
            )
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (sizeExpression == null)
                throw new ArgumentNullException("sizeExpression");
            return new ArrayCreationWithSizeExpression(type, sizeExpression);
        }
        #endregion

        #region NewArray (initializers)
        /// <summary>
        /// Creates a <c>new type[] { initializers }</c> expression
        /// </summary>
        /// <param name="type">Array item type</param>
        /// <param name="initializers">Array items</param>
        /// <returns>A <see cref="ArrayCreationWithInitializersExpression"/> instance</returns>
        /// <include file='Refly.CodeDom.Doc.xml' path='doc/remarkss/remark[@name="Expr.NewArray"]'/>
        /// <exception cref="ArgumentNullExpression">
        /// <paramref name="type"/> is a null reference.
        /// </exception>
        public static ArrayCreationWithInitializersExpression NewArray(
            Type type,
            params Expression[] initialiers
            )
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return new ArrayCreationWithInitializersExpression(
                new TypeTypeDeclaration(type),
                initialiers);
        }

        /// <summary>
        /// Creates a <c>new type[] { initializers }</c> expression
        /// </summary>
        /// <param name="type">Array item type</param>
        /// <param name="initializers">Array items</param>
        /// <returns>A <see cref="ArrayCreationWithInitializersExpression"/> instance</returns>
        /// <include file='Refly.CodeDom.Doc.xml' path='doc/remarkss/remark[@name="Expr.NewArray"]'/>
        /// <exception cref="ArgumentNullExpression">
        /// <paramref name="type"/> is a null reference.
        /// </exception>
        public static ArrayCreationWithInitializersExpression NewArray(
            String type,
            params Expression[] initialiers
            )
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return new ArrayCreationWithInitializersExpression(
                new StringTypeDeclaration(type),
                initialiers);
        }

        /// <summary>
        /// Creates a <c>new type[] { initializers }</c> expression
        /// </summary>
        /// <param name="type">Array item type</param>
        /// <param name="initializers">Array items</param>
        /// <returns>A <see cref="ArrayCreationWithInitializersExpression"/> instance</returns>
        /// <include file='Refly.CodeDom.Doc.xml' path='doc/remarkss/remark[@name="Expr.NewArray"]'/>
        /// <exception cref="ArgumentNullExpression">
        /// <paramref name="type"/> is a null reference.
        /// </exception>
        public static ArrayCreationWithInitializersExpression NewArray(
            ITypeDeclaration type,
            params Expression[] initialiers
            )
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return new ArrayCreationWithInitializersExpression(
                type, 
                initialiers);
        }
        #endregion

        #region Arg, Var
        /// <summary>
        /// Creates a reference to a given argument
		/// </summary>
		/// <param name="p">
		/// The <see cref="ParameterDeclaration"/> instance to reference.
		/// </param>
		/// <returns>
		/// A <see cref="ArgumentReferenceExpression"/> instance referencing
		/// <paramref name="p"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="p"/> is a null reference (Noting in Visual Basic)
		/// </exception>
		public static ArgumentReferenceExpression Arg(
			ParameterDeclaration p)
		{
			if (p==null)
				throw new ArgumentNullException("p");
			return new ArgumentReferenceExpression(p);
		}

		/// <summary>
		/// Creates a reference to a given variable
		/// </summary>
		/// <param name="p">
		/// The <see cref="VariableDeclarationStatement"/> instance to reference.
		/// </param>
		/// <returns>
		/// A <see cref="VariableReferenceExpression"/> instance referencing
		/// <paramref name="v"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="v"/> is a null reference (Noting in Visual Basic)
		/// </exception>
		public static VariableReferenceExpression Var(
			VariableDeclarationStatement v)
		{
			if (v==null)
				throw new ArgumentNullException("v");
			return new VariableReferenceExpression(v);
        }
        #endregion

        #region Prim
        /// <summary>
        /// Creates a primitive <see cref="Boolean"/> value.
		/// </summary>
		/// <param name="value">
		/// <see cref="Boolean"/> value to generate.
		/// </param>
		/// <returns>
		/// A <see cref="PrimitiveExpression"/> instance that
		/// will generate the value.
		/// </returns>
		public static PrimitiveExpression Prim(bool value)
		{
			return new PrimitiveExpression(value);
		}

		/// <summary>
		/// Creates a primitive <see cref="String"/> value.
		/// </summary>
		/// <param name="value">
		/// <see cref="String"/> value to generate.
		/// </param>
		/// <returns>
		/// A <see cref="PrimitiveExpression"/> instance that
		/// will generate the value.
		/// </returns>
		public static PrimitiveExpression Prim(string value)
		{
			return new PrimitiveExpression(value);
		}

		/// <summary>
		/// Creates a primitive <see cref="int"/> value.
		/// </summary>
		/// <param name="value">
		/// <see cref="int"/> value to generate.
		/// </param>
		/// <returns>
		/// A <see cref="PrimitiveExpression"/> instance that
		/// will generate the value.
		/// </returns>
		public static PrimitiveExpression Prim(int value)
		{
			return new PrimitiveExpression(value);
		}

		/// <summary>
		/// Creates a primitive <see cref="long"/> value.
		/// </summary>
		/// <param name="value">
		/// <see cref="long"/> value to generate.
		/// </param>
		/// <returns>
		/// A <see cref="PrimitiveExpression"/> instance that
		/// will generate the value.
		/// </returns>
		public static PrimitiveExpression Prim(long value)
		{
			return new PrimitiveExpression(value);
		}

		/// <summary>
		/// Creates a primitive <see cref="decimal"/> value.
		/// </summary>
		/// <param name="value">
		/// <see cref="decimal"/> value to generate.
		/// </param>
		/// <returns>
		/// A <see cref="PrimitiveExpression"/> instance that
		/// will generate the value.
		/// </returns>
		public static PrimitiveExpression Prim(decimal value)
		{
			return new PrimitiveExpression(value);
		}

		/// <summary>
		/// Creates a primitive <see cref="double"/> value.
		/// </summary>
		/// <param name="value">
		/// <see cref="double"/> value to generate.
		/// </param>
		/// <returns>
		/// A <see cref="PrimitiveExpression"/> instance that
		/// will generate the value.
		/// </returns>
		public static PrimitiveExpression Prim(double value)
		{
			return new PrimitiveExpression(value);
		}

		/// <summary>
		/// Creates a primitive <see cref="short"/> value.
		/// </summary>
		/// <param name="value">
		/// <see cref="short"/> value to generate.
		/// </param>
		/// <returns>
		/// A <see cref="PrimitiveExpression"/> instance that
		/// will generate the value.
		/// </returns>
		public static PrimitiveExpression Prim(short value)
		{
			return new PrimitiveExpression(value);
		}

		/// <summary>
		/// Creates a primitive <see cref="byte"/> value.
		/// </summary>
		/// <param name="value">
		/// <see cref="byte"/> value to generate.
		/// </param>
		/// <returns>
		/// A <see cref="PrimitiveExpression"/> instance that
		/// will generate the value.
		/// </returns>
		public static PrimitiveExpression Prim(byte value)
		{
			return new PrimitiveExpression(value);
		}

		/// <summary>
		/// Creates a primitive <see cref="DateTime"/> value.
		/// </summary>
		/// <param name="value">
		/// <see cref="DateTime"/> value to generate.
		/// </param>
		/// <returns>
		/// A <see cref="PrimitiveExpression"/> instance that
		/// will generate the value.
		/// </returns>
		public static PrimitiveExpression Prim(DateTime value)
		{
			return new PrimitiveExpression(value);
        }
        #endregion

        #region Cast
        /// <summary>
        /// Creates a case of the <see cref="Expression"/> 
		/// <paramref name="e"/> to the <see cref="Type"/> 
		/// <paramref name="type"/>.
		/// </summary>
		/// <param name="type">Target <see cref="Type"/></param>
		/// <param name="e"><see cref="Expression"/> instance to case</param>
		/// <returns>
		/// A <see cref="CastExpressoin"/> that will generate the cast.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> is a null reference (Noting in Visual Basic)
		/// </exception>		
		public static CastExpression Cast(String type, Expression e)
		{
			if (type==null)
				throw new ArgumentNullException("type");
			return Cast(new StringTypeDeclaration(type),e);
		}

		/// <summary>
		/// Creates a case of the <see cref="Expression"/> 
		/// <paramref name="e"/> to the <see cref="Type"/> 
		/// <paramref name="type"/>.
		/// </summary>
		/// <param name="type">Target <see cref="Type"/></param>
		/// <param name="e"><see cref="Expression"/> instance to case</param>
		/// <returns>
		/// A <see cref="CastExpressoin"/> that will generate the cast.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> is a null reference (Noting in Visual Basic)
		/// </exception>		
		public static CastExpression Cast(Type type, Expression e)
		{
			if (type==null)
				throw new ArgumentNullException("type");
			return Cast(new TypeTypeDeclaration(type),e);
		}

		/// <summary>
		/// Creates a case of the <see cref="Expression"/> 
		/// <paramref name="e"/> to the <see cref="Type"/> 
		/// <paramref name="type"/>.
		/// </summary>
		/// <param name="type">Target <see cref="Type"/></param>
		/// <param name="e"><see cref="Expression"/> instance to case</param>
		/// <returns>
		/// A <see cref="CastExpressoin"/> that will generate the cast.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> is a null reference (Noting in Visual Basic)
		/// </exception>		
		public static CastExpression Cast(ITypeDeclaration type, Expression e)
		{
			return new CastExpression(type,e);
        }
        #endregion

        #region Snippet
        /// <summary>
        /// Creates a snippet of code that will be outputed as such.
		/// </summary>
		/// <param name="snippet">
		/// Snippet of code
		/// </param>
		/// <remarks>
		/// <para>
		/// Try not to use this type of generators because you will not be
		/// able to output different languages. This one is for the lazy users!
		/// </para>
		/// </remarks>
		/// <returns>
		/// A <see cref="NativeExpression"/> instance that will output the snippet.
		/// </returns>
		public static NativeExpression Snippet(string snippet)
		{
			return new NativeExpression(
				new CodeSnippetExpression(snippet)
				);
        }
        #endregion 

        #region TypeOf
        /// <summary>
        /// Creates a <c>typeof(type)</c> expression.
		/// </summary>
		/// <param name="type">
		/// Target <see cref="Type"/> name.
		/// </param>
		/// <returns>
		/// A <see cref="NativeExpression"/> that will generate the expression.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> is a null reference (Nothing in Visual Basic)
		/// </exception>
		public static NativeExpression TypeOf(String type)
		{
			if (type==null)
				throw new ArgumentNullException("type");
			return TypeOf(new StringTypeDeclaration(type));
		}

		/// <summary>
		/// Creates a <c>typeof(type)</c> expression.
		/// </summary>
		/// <param name="type">
		/// Target <see cref="Type"/>
		/// </param>
		/// <returns>
		/// A <see cref="NativeExpression"/> that will generate the expression.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> is a null reference (Nothing in Visual Basic)
		/// </exception>
		public static NativeExpression TypeOf(Type type)
		{
			if (type==null)
				throw new ArgumentNullException("type");
			return TypeOf(new TypeTypeDeclaration(type));
		}

		/// <summary>
		/// Creates a <c>typeof(type)</c> expression.
		/// </summary>
		/// <param name="type">
		/// Target <see cref="Type"/>
		/// </param>
		/// <returns>
		/// A <see cref="NativeExpression"/> that will generate the expression.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> is a null reference (Nothing in Visual Basic)
		/// </exception>
		public static NativeExpression TypeOf(ITypeDeclaration type)
		{
			if (type==null)
				throw new ArgumentNullException("type");
			return new NativeExpression(
				new CodeTypeOfExpression(type.TypeReference)
				);
        }
        #endregion

        #region Type
        /// <summary>
        /// Creates a reference expression to a given <see cref="Type"/>.
		/// </summary>
		/// <param name="type">
		/// Target <see cref="Type"/> name
		/// </param>
		/// <returns>
		/// A <see cref="NativeExpression"/> that will generate the expression.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> is a null reference (Nothing in Visual Basic)
		/// </exception>
		public static TypeReferenceExpression Type(String type)
		{
			if (type==null)
				throw new ArgumentNullException("type");
			return Type(new StringTypeDeclaration(type));			
		}
		
		/// <summary>
		/// Creates a reference expression to a given <see cref="Type"/>.
		/// </summary>
		/// <param name="type">
		/// Target <see cref="Type"/> name
		/// </param>
		/// <returns>
		/// A <see cref="NativeExpression"/> that will generate the expression.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> is a null reference (Nothing in Visual Basic)
		/// </exception>
		public static TypeReferenceExpression Type(Type type)
		{
			if (type==null)
				throw new ArgumentNullException("type");
			return Type(new TypeTypeDeclaration(type));
		}
		
		/// <summary>
		/// Creates a reference expression to a given <see cref="Type"/>.
		/// </summary>
		/// <param name="type">
		/// Target <see cref="Type"/> name
		/// </param>
		/// <returns>
		/// A <see cref="NativeExpression"/> that will generate the expression.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="type"/> is a null reference (Nothing in Visual Basic)
		/// </exception>
		public static TypeReferenceExpression Type(ITypeDeclaration type)
		{
			if (type==null)
				throw new ArgumentNullException("type");
			return new TypeReferenceExpression(type);
		}
        #endregion

        #region Delegate
        /// <summary>
        /// Creates a delegate constructr
		/// </summary>
		/// <param name="delegateType">
		/// The delegate type
		/// </param>
		/// <param name="method">
		/// The listener method
		/// </param>
		/// <returns>
		/// A <see cref="DelegateCreateExpression"/> representing the
		/// delegate creation.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="delegateType"/> or <paramref name="method"/>
		/// is a null reference (Nothing in Visual Basic)
		/// </exception>
		public static DelegateCreateExpression Delegate(
			ITypeDeclaration delegateType,
			MethodReferenceExpression method)
		{
			if (delegateType==null)
				throw new ArgumentNullException("delegateType");
			if (method==null)
				throw new ArgumentNullException("method");
			return new DelegateCreateExpression(delegateType,method);		
		}

		/// <summary>
		/// Creates a delegate constructr
		/// </summary>
		/// <param name="delegateType">
		/// The delegate type
		/// </param>
		/// <param name="method">
		/// The listener method
		/// </param>
		/// <returns>
		/// A <see cref="DelegateCreateExpression"/> representing the
		/// delegate creation.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="delegateType"/> or <paramref name="method"/>
		/// is a null reference (Nothing in Visual Basic)
		/// </exception>
		public static DelegateCreateExpression Delegate(
			string delegateType,
			MethodReferenceExpression method)
		{
			if (delegateType==null)
				throw new ArgumentNullException("delegateType");
			if (method==null)
				throw new ArgumentNullException("method");
			return new DelegateCreateExpression(new StringTypeDeclaration(delegateType),method);		
		}

		/// <summary>
		/// Creates a delegate constructr
		/// </summary>
		/// <param name="delegateType">
		/// The delegate type
		/// </param>
		/// <param name="method">
		/// The listener method
		/// </param>
		/// <returns>
		/// A <see cref="DelegateCreateExpression"/> representing the
		/// delegate creation.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="delegateType"/> or <paramref name="method"/>
		/// is a null reference (Nothing in Visual Basic)
		/// </exception>
		public static DelegateCreateExpression Delegate(
			Type delegateType,
			MethodReferenceExpression method)
		{
			if (delegateType==null)
				throw new ArgumentNullException("delegateType");
			if (method==null)
				throw new ArgumentNullException("method");
			return new DelegateCreateExpression(new TypeTypeDeclaration(delegateType),method);
        }
        #endregion

        #region DelegateInvoke
        /// <summary>
        /// Create a <c>firedEvent(parameters)</c> invocation.</c>
        /// </summary>
        /// <param name="firedEvent">Delegate to invoke</param>
        /// <param name="parameters">Delegate arguments</param>
        /// <returns></returns>
        public static DelegateInvokeExpression DelegateInvoke(
            EventReferenceExpression firedEvent, 
            params Expression[] parameters
            )
        {
            if (firedEvent == null)
                throw new ArgumentNullException("firedEvent");
            DelegateInvokeExpression expr = new DelegateInvokeExpression(firedEvent, parameters);
            return expr;
        }
        #endregion
    }
}
