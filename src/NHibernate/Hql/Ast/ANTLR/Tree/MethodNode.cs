using System;
using Antlr.Runtime;

using NHibernate.Dialect.Function;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents a method call
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class MethodNode : AbstractSelectExpression 
	{
		private static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof(MethodNode));

		private string[] _selectColumns;
		private string _methodName;
		private bool _inSelect;
		private FromElement _fromElement;
		private ISQLFunction _function;

		public MethodNode(IToken token) : base(token)
		{
		}
		
		public virtual void Resolve(bool inSelect)
		{
			// Get the function name node.
			IASTNode name = GetChild(0);
			InitializeMethodNode( name, inSelect );
			IASTNode exprList = name.NextSibling;

			// If the expression list has exactly one expression, and the type of the expression is a collection
			// then this might be a collection function, such as index(c) or size(c).
			if ( (exprList != null && exprList.ChildCount == 1) && IsCollectionPropertyMethod ) 
			{
				CollectionProperty( exprList.GetChild(0), name );
			}
			else 
			{
				DialectFunction( exprList );
			}
		}

		public override void SetScalarColumnText(int i)
		{
			if ( _selectColumns == null ) 
			{ 	// Dialect function
				ColumnHelper.GenerateSingleScalarColumn(Walker.ASTFactory, this, i );
			}
			else 
			{	// Collection 'property function'
				ColumnHelper.GenerateScalarColumns(Walker.ASTFactory, this, _selectColumns, i);
			}
		}

		public void InitializeMethodNode(IASTNode name, bool inSelect)
		{
			name.Type = HqlSqlWalker.METHOD_NAME;
			_methodName = name.Text.ToLowerInvariant();	// Use the lower case function name.
			_inSelect = inSelect;			// Remember whether we're in a SELECT clause or not.
		}

		public bool IsCollectionPropertyMethod
		{
			get { return CollectionProperties.IsAnyCollectionProperty(_methodName); }
		}

		public ISQLFunction SQLFunction
		{
			get { return _function; }
		}

		public override FromElement FromElement
		{
			get { return _fromElement; }
			set { base.FromElement = value; }
		}

		public override bool IsScalar
		{
			get { return true; }
		}

		public void ResolveCollectionProperty(IASTNode expr)
		{
			var propertyName = CollectionProperties.GetNormalizedPropertyName( _methodName );

			var collectionNode = expr as FromReferenceNode;
			if (collectionNode == null)
				throw new SemanticException(string.Format("Unexpected expression {0} found for collection function {1}", expr, propertyName));

			// If this is 'elements' then create a new FROM element.
			if (CollectionPropertyNames.Elements == propertyName)
			{
				HandleElements(collectionNode, propertyName);
			}
			else
			{
				// Not elements(x)
				_fromElement = collectionNode.FromElement;
				DataType = _fromElement.GetPropertyType(propertyName, propertyName);
				_selectColumns = _fromElement.ToColumns(_fromElement.TableAlias, propertyName, _inSelect);
			}
			var dotNode = collectionNode as DotNode;
			if (dotNode != null)
			{
				PrepareAnyImplicitJoins(dotNode);
			}
			if (!_inSelect)
			{
				_fromElement.Text = "";
				_fromElement.UseWhereFragment = false;
			}

			PrepareSelectColumns(_selectColumns);
			Text = _selectColumns[0];
			Type = HqlSqlWalker.SQL_TOKEN;
		}

		public String GetDisplayText()
		{
			return "{" +
					"method=" + _methodName +
					",selectColumns=" + _selectColumns +
					",fromElement=" + _fromElement.TableAlias +
					"}";
		}

		protected virtual void PrepareSelectColumns(string[] columns)
		{
		}

		private void CollectionProperty(IASTNode path, IASTNode name) 
		{
			if ( path == null ) 
			{
				throw new SemanticException( "Collection function " + name.Text + " has no path!" );
			}

			var expr = ( SqlNode ) path;
			IType type = expr.DataType;

			if ( Log.IsDebugEnabled ) 
			{
				Log.Debug( "collectionProperty() :  name=" + name + " type=" + type );
			}

			ResolveCollectionProperty( expr );
		}

		private static void PrepareAnyImplicitJoins(DotNode dotNode)
		{
			var lhs = dotNode.GetLhs() as DotNode;
			if ( lhs != null )
			{
				FromElement lhsOrigin = lhs.FromElement;
				if ( lhsOrigin != null && "" == lhsOrigin.Text )
				{
					String lhsOriginText = lhsOrigin.Queryable.TableName +
							" " + lhsOrigin.TableAlias;
					lhsOrigin.Text = lhsOriginText;
				}
				PrepareAnyImplicitJoins( lhs );
			}
		}

		private void HandleElements(FromReferenceNode collectionNode, String propertyName)
		{
			FromElement collectionFromElement = collectionNode.FromElement;
			IQueryableCollection queryableCollection = collectionFromElement.QueryableCollection;

			String path = collectionNode.Path + "[]." + propertyName;
			Log.Debug("Creating elements for " + path);

			_fromElement = collectionFromElement;
			if (!collectionFromElement.IsCollectionOfValuesOrComponents)
			{
				Walker.AddQuerySpaces(queryableCollection.ElementPersister.QuerySpaces);
			}

			DataType = queryableCollection.ElementType;
			_selectColumns = collectionFromElement.ToColumns(_fromElement.TableAlias, propertyName, _inSelect);
		}

		private void DialectFunction(IASTNode exprList)
		{
			_function = SessionFactoryHelper.FindSQLFunction(_methodName);

			if (_function != null)
			{
				IASTNode child = null;

				if (exprList != null)
				{
					child = _methodName == "iif" ? exprList.GetChild(1) : exprList.GetChild(0);
				}

				DataType = SessionFactoryHelper.FindFunctionReturnType(_methodName, child);
			}
			//TODO:
			/*else {
				methodName = (String) getWalker().getTokenReplacements().get( methodName );
			}*/
		}
	}
}
