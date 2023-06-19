using System;
using System.Collections.Generic;
using System.Reflection;
using Antlr.Runtime;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public class ConstructorNode : SelectExpressionList, ISelectExpressionExtension
	{
		private IType[] _constructorArgumentTypes;
		private ConstructorInfo _constructor;
		private bool _isMap;
		private bool _isList;
		private int _scalarColumnIndex = -1;

		public ConstructorNode(IToken token) : base(token)
		{
		}

		public IList<IType> ConstructorArgumentTypeList
		{
			get { return new List<IType>(_constructorArgumentTypes); }
		}

		public string[] GetAliases()
		{
			var selectExpressions = GetSelectExpressions();
			string[] aliases = new string[selectExpressions.Count];
			for (int i = 0; i < selectExpressions.Count; i++)
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

		public int ScalarColumnIndex
		{
			get { return _scalarColumnIndex; }
		}

		// Since v5.4
		[Obsolete("Use overload with aliasCreator parameter instead.")]
		public void SetScalarColumn(int i)
		{
			ISelectExpression[] selectExpressions = CollectSelectExpressions();
			// Invoke setScalarColumnText on each constructor argument.
			for (int j = 0; j < selectExpressions.Length; j++)
			{
				ISelectExpression selectExpression = selectExpressions[j];
				selectExpression.SetScalarColumn(j);
			}
		}

		/// <inheritdoc />
		public string[] SetScalarColumn(int i, Func<int, int, string> aliasCreator)
		{
			var selectExpressions = GetSelectExpressions();
			var aliases = new List<string>();
			// Invoke setScalarColumnText on each constructor argument.
			for (var j = 0; j < selectExpressions.Count; j++)
			{
				var selectExpression = selectExpressions[j];
				aliases.AddRange(selectExpression.SetScalarColumn(j, aliasCreator));
			}

			return aliases.ToArray();
		}

		// Since v5.4
		[Obsolete("This method has no more usage in NHibernate and will be removed in a future version.")]
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

			if (string.Equals(path, "map", StringComparison.OrdinalIgnoreCase))
			{
				_isMap = true;
			}
			else if (string.Equals(path, "list", StringComparison.OrdinalIgnoreCase)) 
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
			var argumentExpressions = GetSelectExpressions();
			var types = new IType[argumentExpressions.Count];
			for (var x = 0; x < argumentExpressions.Count; x++)
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
