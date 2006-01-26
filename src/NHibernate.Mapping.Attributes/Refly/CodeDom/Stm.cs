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
	using Refly.CodeDom.Collections;
	using Refly.CodeDom.Statements;

	/// <summary>
	/// Helper containing static methods for creating statements.
	/// </summary>
	public sealed class Stm
	{
		#region Constructors
		private Stm(){}
		#endregion

		/// <summary>
		/// Creates an assign statement: <c>left = right</c>
		/// </summary>
		/// <param name="left">
		/// Left <see cref="Expression"/> instance</param>
		/// <param name="right">
		/// Right <see cref="Expression"/> instance
		/// </param>
		/// <returns>
		/// A <see cref="AssignStatement"/> instance.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="left"/> or <paramref name="right"/>
		/// is a null reference (Nothing in Visual Basic)
		/// </exception>
		static public AssignStatement Assign(Expression left, Expression right)
		{
			if (left==null)
				throw new ArgumentNullException("left");
			if (right==null)
				throw new ArgumentNullException("right");
			return new AssignStatement(left,right);
		}

		static public ExpressionStatement ToStm(Expression expr)
		{
			if (expr==null)
				throw new ArgumentNullException("expr");
			return new ExpressionStatement(expr);
		}

		static public MethodReturnStatement Return(Expression expr)
		{
			if (expr==null)
				throw new ArgumentNullException("expr");
			return new MethodReturnStatement(expr);
		}

		static public MethodReturnStatement Return()
		{
			return new MethodReturnStatement();
		}

		static public NativeStatement Comment(string comment)
		{
			if (comment==null)
				throw new ArgumentNullException("comment");
			return new NativeStatement( 
				new CodeCommentStatement(comment,false)
				);
		}

		static public ConditionStatement If(Expression condition, params Statement[] trueStatements)
		{
			if (condition==null)
				throw new ArgumentNullException("condition");
			return new ConditionStatement(condition,trueStatements);
		}

		static public ConditionStatement IfIdentity(Expression left, Expression right, params Statement[] trueStatements)
		{
			if (left==null)
				throw new ArgumentNullException("left");
			if (right==null)
				throw new ArgumentNullException("right");
			return If(
				left.Identity(right),
				trueStatements
				);
		}

		static public ConditionStatement ThrowIfNull(Expression condition, Expression toThrow)
		{
			if (condition==null)
				throw new ArgumentNullException("condition");
			if (toThrow==null)
				throw new ArgumentNullException("toThrow");
			return IfNull(condition,
				Stm.Throw(toThrow)
				);
		}

		static public ConditionStatement ThrowIfNull(Expression condition, string message)
		{
			if (condition==null)
				throw new ArgumentNullException("condition");
			if (message==null)
				throw new ArgumentNullException("message");
			return ThrowIfNull(
				condition,
				Expr.New(typeof(NullReferenceException),Expr.Prim(message))
				);
		}

		static public ConditionStatement ThrowIfNull(ParameterDeclaration param)
		{
			if (param==null)
				throw new ArgumentNullException("param");
			return ThrowIfNull(
				Expr.Arg(param),
				Expr.New(typeof(ArgumentNullException),Expr.Prim(param.Name))
				);
		}

		static public ConditionStatement IfNotIdentity(Expression left, Expression right, params Statement[] trueStatements)
		{
			if (left==null)
				throw new ArgumentNullException("left");
			if (right==null)
				throw new ArgumentNullException("right");
			return If(
				left.NotIdentity(right),
				trueStatements
				);
		}

		static public ConditionStatement IfNull(Expression left,params Statement[] trueStatements)
		{
			if (left==null)
				throw new ArgumentNullException("left");
			return IfIdentity(
				left,Expr.Null,
				trueStatements
				);
		}

		static public ConditionStatement IfNotNull(Expression left,params Statement[] trueStatements)
		{
			if (left==null)
				throw new ArgumentNullException("left");
			return IfNotIdentity(
				left,Expr.Null,
				trueStatements
				);
		}

		static public NativeStatement Snippet(string snippet)
		{
			if (snippet==null)
				throw new ArgumentNullException("snippet");
			return new NativeStatement(
				new CodeSnippetStatement(snippet)
				);
		}

		static public ThrowExceptionStatement Throw(Expression toThrow)
		{
			if (toThrow==null)
				throw new ArgumentNullException("toThrow");
			return new ThrowExceptionStatement(toThrow);
		}

		static public ThrowExceptionStatement Throw(String t, params Expression[] args)
		{
			return Throw(new StringTypeDeclaration(t),args);
		}

		static public ThrowExceptionStatement Throw(Type t, params Expression[] args)
		{
			return Throw(new TypeTypeDeclaration(t),args);
		}

		static public ThrowExceptionStatement Throw(ITypeDeclaration t, params Expression[] args)
		{
			return new ThrowExceptionStatement(
					Expr.New(t,args)
				);
		}

		static public TryCatchFinallyStatement Try()
		{
			return new TryCatchFinallyStatement();
		}

		static public CatchClause Catch(ParameterDeclaration localParam)
		{
			return new CatchClause(localParam);
		}

		static public CatchClause Catch(String t,  string name)
		{
			return Catch(new StringTypeDeclaration(t),name);
		}

		static public CatchClause Catch(Type t,  string name)
		{
			return Catch(new TypeTypeDeclaration(t),name);
		}

		static public CatchClause Catch(ITypeDeclaration t,  string name)
		{
			return Catch(new ParameterDeclaration(t,name,false));
		}

		static public TryCatchFinallyStatement Guard(Type expectedExceptionType)
		{
			TryCatchFinallyStatement try_ = Try();
			try_.CatchClauses.Add( Catch(expectedExceptionType,"") );
			return try_;
		}

		static public VariableDeclarationStatement Var(String type, string name)
		{
			return Var(new StringTypeDeclaration(type),name);
		}

		static public VariableDeclarationStatement Var(Type type, string name)
		{
			return Var(new TypeTypeDeclaration(type),name);
		}

		static public VariableDeclarationStatement Var(ITypeDeclaration type, string name)
		{
			return new VariableDeclarationStatement(type,name);
		}

		static public VariableDeclarationStatement Var(String type, string name,Expression initExpression)
		{
			return Var(new StringTypeDeclaration(type),name,initExpression);
		}

		static public VariableDeclarationStatement Var(Type type, string name,Expression initExpression)
		{
			return Var(new TypeTypeDeclaration(type),name,initExpression);
		}

		static public VariableDeclarationStatement Var(ITypeDeclaration type, string name,Expression initExpression)
		{
			VariableDeclarationStatement var =  Var(type,name);
			var.InitExpression = initExpression;
			return var;
		}

		static public IterationStatement For(
			Statement initStatement, Expression testExpression, Statement incrementStatement
			)
		{
			return new IterationStatement(initStatement,testExpression,incrementStatement);
		}

		static public IterationStatement While(
			Expression testExpression
			)
		{
			return new IterationStatement(
				new SnippetStatement(""),
				testExpression,
				new SnippetStatement("")
				);
		}

		public static ForEachStatement ForEach( 
			String localType,
			string localName,
			Expression collection,
			bool enumeratorDisposable
			)
		{
			return ForEach(new StringTypeDeclaration(localType),localName,collection,enumeratorDisposable);
		}

		public static ForEachStatement ForEach( 
			Type localType,
			string localName,
			Expression collection,
			bool enumeratorDisposable
			)
		{
			return ForEach(new TypeTypeDeclaration(localType),localName,collection,enumeratorDisposable);
		}
		public static ForEachStatement ForEach( 
			ITypeDeclaration localType,
			string localName,
			Expression collection,
			bool enumeratorDisposable
			)
		{
			return new ForEachStatement(localType,localName,collection
				,enumeratorDisposable
				);
		}
		
		public static AttachRemoveEventStatement Attach(
			EventReferenceExpression eventRef,
			Expression listener)
		{
			return new AttachRemoveEventStatement(eventRef,listener,true);	
		}
		
		public static AttachRemoveEventStatement Remove(
			EventReferenceExpression eventRef,
			Expression listener)
		{
			return new AttachRemoveEventStatement(eventRef,listener,false);	
		}
		
	}
}
