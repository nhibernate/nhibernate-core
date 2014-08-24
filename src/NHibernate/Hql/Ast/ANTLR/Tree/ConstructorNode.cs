using System;
using System.Collections.Generic;
using System.Reflection;
using Antlr.Runtime;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public class ConstructorNode : SelectExpressionList, ISelectExpression 
	{
		private IType[] _constructorArgumentTypes;
		private ConstructorInfo _constructor;
		private bool _isMap;
		private bool _isList;

		public ConstructorNode(IToken token) : base(token)
		{
		}

		public IList<IType> ConstructorArgumentTypeList
		{
			get { return new List<IType>(_constructorArgumentTypes); }
		}

		public string[] GetAliases()
		{
			ISelectExpression[] selectExpressions = CollectSelectExpressions();
			string[] aliases = new string[selectExpressions.Length];
			for (int i = 0; i < selectExpressions.Length; i++)
			{
				string alias = selectExpressions[i].Alias;
				aliases[i] = alias ?? i.ToString();
			}
			return aliases;
		}

		protected internal override IASTNode GetFirstSelectExpression()
		{
			// Collect the select expressions, skip the first child because it is the class name.
			return GetChild(1);
		}

		public void SetScalarColumnText(int i)
		{
			ISelectExpression[] selectExpressions = CollectSelectExpressions();
			// Invoke setScalarColumnText on each constructor argument.
			for (int j = 0; j < selectExpressions.Length; j++)
			{
				ISelectExpression selectExpression = selectExpressions[j];
				selectExpression.SetScalarColumnText(j);
			}
		}

		public FromElement FromElement
		{
			get { return null; }
		}

		public bool IsConstructor
		{
			get { return true; }
		}

		public bool IsReturnableEntity
		{
			get { return false; }
		}

		public bool IsScalar
		{
			get { return true; }
		}

		public string Alias
		{
			get { throw new InvalidOperationException("constructor may not be aliased"); }
			set { throw new InvalidOperationException("constructor may not be aliased"); }
		}

		public ConstructorInfo Constructor
		{
			get { return _constructor; }
		}

		public bool IsMap
		{
			get { return _isMap; }
		}

		public bool IsList
		{
			get { return _isList; }
		}

		public void Prepare()
		{
			_constructorArgumentTypes = ResolveConstructorArgumentTypes();
			string path = ( ( IPathNode ) GetChild(0) ).Path;

			if (path.ToLowerInvariant() == "map")
			{
				_isMap = true;
			}
			else if (path.ToLowerInvariant() == "list") 
			{
				_isList = true;
			}
			else 
			{
                _constructor = ResolveConstructor(path);
			}
		}

		private IType[] ResolveConstructorArgumentTypes()
		{
			ISelectExpression[] argumentExpressions = CollectSelectExpressions();
			
			if ( argumentExpressions == null ) 
			{
				// return an empty Type array
				return new IType[]{};
			}

			IType[] types = new IType[argumentExpressions.Length];
			for ( int x = 0; x < argumentExpressions.Length; x++ ) 
			{
				types[x] = argumentExpressions[x].DataType;
			}

			return types;
		}

		private ConstructorInfo ResolveConstructor(String path)
		{
			string importedClassName = SessionFactoryHelper.GetImportedClassName( path );
			string className = StringHelper.IsEmpty( importedClassName ) ? path : importedClassName;

			if ( className == null ) 
			{
				throw new SemanticException( "Unable to locate class [" + path + "]" );
			}
			try 
			{
				System.Type holderClass = ReflectHelper.ClassForName( className );
				return ReflectHelper.GetConstructor( holderClass, _constructorArgumentTypes );
			}
			catch ( TypeLoadException e ) 
			{
				throw new QueryException( "Unable to locate class [" + className + "]", e );
			}
			catch (InstantiationException e) 
			{
				// this is the exception returned by ReflectHelper.getConstructor() if it cannot
				// locate an appropriate constructor
				throw new QueryException( "Unable to locate appropriate constructor on class [" + className + "]", e );
			}
		}
	}
}
