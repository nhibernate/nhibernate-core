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

namespace Refly.CodeDom.Expressions
{
	/// <summary>
	/// Summary description for Expression.
	/// </summary>
	public abstract class Expression
	{
		public abstract CodeExpression ToCodeDom();

		#region Members
		public FieldReferenceExpression Field(FieldDeclaration field)
		{
			return new FieldReferenceExpression(this,field);
		}

		public NativeExpression Field(string name)
		{
			return new NativeExpression(
				new CodeFieldReferenceExpression(this.ToCodeDom(),name)
				);
		}

		public PropertyReferenceExpression Prop(PropertyDeclaration prop)
		{
			return new PropertyReferenceExpression(this,prop);
		}

		public NativePropertyReferenceExpression Prop(string prop)
		{
			return new NativePropertyReferenceExpression(this,prop);
		}

		public MethodReferenceExpression Method(MethodDeclaration method)
		{
			return new MethodReferenceExpression(this,method);
		}

		public NativeMethodReferenceExpression Method(string method)
		{
			return new NativeMethodReferenceExpression(this,method);
		}

		public NativeExpression Event(string name)
		{
			return new NativeExpression(
				new CodeEventReferenceExpression(this.ToCodeDom(),name)
				);
		}

		public EventReferenceExpression Event(EventDeclaration e)
		{
			return new EventReferenceExpression(this,e);
		}
		#endregion

		#region Cast
		public CastExpression Cast(String t)
		{
			return Cast(new StringTypeDeclaration(t));
		}		
		
		public CastExpression Cast(Type t)
		{
			return Cast(new TypeTypeDeclaration(t));
		}

		public CastExpression Cast(ITypeDeclaration t)
		{
			return new CastExpression(t,this);
		}
		#endregion

		#region Arithmetic Operators
		public static BinaryOpOperatorExpression operator + (Expression left, Expression right)
		{
			return new BinaryOpOperatorExpression(
				left,
				right,
				CodeBinaryOperatorType.Add
				);
		}

		public static BinaryOpOperatorExpression operator - (Expression left, Expression right)
		{
			return new BinaryOpOperatorExpression(
				left,
				right,
				CodeBinaryOperatorType.Subtract
				);
		}

		public static BinaryOpOperatorExpression operator / (Expression left, Expression right)
		{
			return new BinaryOpOperatorExpression(
				left,
				right,
				CodeBinaryOperatorType.Divide
				);
		}

		public static BinaryOpOperatorExpression operator * (Expression left, Expression right)
		{
			return new BinaryOpOperatorExpression(
				left,
				right,
				CodeBinaryOperatorType.Multiply
				);
		}

		public static BinaryOpOperatorExpression operator % (Expression left, Expression right)
		{
			return new BinaryOpOperatorExpression(
				left,
				right,
				CodeBinaryOperatorType.Modulus
				);
		}
		#endregion

		#region Comparaison Operators
		public static BinaryOpOperatorExpression operator < (Expression left, Expression right)
		{
			return new BinaryOpOperatorExpression(
				left,
				right,
				CodeBinaryOperatorType.LessThan
				);
		}

		public static BinaryOpOperatorExpression operator <= (Expression left, Expression right)
		{
			return new BinaryOpOperatorExpression(
				left,
				right,
				CodeBinaryOperatorType.LessThanOrEqual
				);
		}

		public static BinaryOpOperatorExpression operator > (Expression left, Expression right)
		{
			return new BinaryOpOperatorExpression(
				left,
				right,
				CodeBinaryOperatorType.GreaterThan
				);
		}

		public static BinaryOpOperatorExpression operator >= (Expression left, Expression right)
		{
			return new BinaryOpOperatorExpression(
				left,
				right,
				CodeBinaryOperatorType.GreaterThanOrEqual
				);
		}

		public BinaryOpOperatorExpression Identity(Expression right)
		{
			return new BinaryOpOperatorExpression(
				this,
				right,
				CodeBinaryOperatorType.IdentityEquality
				);
		}

		public BinaryOpOperatorExpression NotIdentity(Expression right)
		{
			return new BinaryOpOperatorExpression(
				this,
				right,
				CodeBinaryOperatorType.IdentityInequality
				);
		}
		#endregion

		#region Indexer
        public IndexerExpression Item(int index)
        {
            return this.Item(Expr.Prim(index));
        }
        public IndexerExpression Item(params Expression[] indices)
		{
			return new IndexerExpression(this,indices);
		}
        public ArrayIndexerExpression ArrayItem(int index)
        {
            return this.ArrayItem(Expr.Prim(index));
        }
        public ArrayIndexerExpression ArrayItem(params Expression[] indices)
        {
            return new ArrayIndexerExpression(this, indices);
        }
		#endregion
	}
}
