using System;
using System.Collections.Generic;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.Persister.Collection;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents the [] operator and provides it's semantics.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class IndexNode : FromReferenceNode
	{
		private static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof(IndexNode));

		public IndexNode(IToken token) : base(token)
		{
		}

		public override void SetScalarColumnText(int i)
		{
			throw new InvalidOperationException("An IndexNode cannot generate column text!");
		}

		public override void ResolveIndex(IASTNode parent)
		{
			throw new InvalidOperationException();
		}

		public override void  Resolve(bool generateJoin, bool implicitJoin, string classAlias, IASTNode parent)
		{
			if (IsResolved) 
			{
				return;
			}

			FromReferenceNode collectionNode = ( FromReferenceNode ) GetChild(0);
			SessionFactoryHelperExtensions sessionFactoryHelper = SessionFactoryHelper;
			collectionNode.ResolveIndex( this );		// Fully resolve the map reference, create implicit joins.

			IType type = collectionNode.DataType;
			if ( !type.IsCollectionType ) 
			{
				throw new SemanticException( "The [] operator cannot be applied to type " + type);
			}

			string collectionRole = ( ( CollectionType ) type ).Role;
			IQueryableCollection queryableCollection = sessionFactoryHelper.RequireQueryableCollection( collectionRole );
			if ( !queryableCollection.HasIndex ) 
			{
				throw new QueryException( "unindexed fromElement before []: " + collectionNode.Path );
			}

			// Generate the inner join -- The elements need to be joined to the collection they are in.
			FromElement fromElement = collectionNode.FromElement;
			String elementTable = fromElement.TableAlias;
			FromClause fromClause = fromElement.FromClause;
			String path = collectionNode.Path;

			FromElement elem = fromClause.FindCollectionJoin( path );
			if ( elem == null ) 
			{
				FromElementFactory factory = new FromElementFactory( fromClause, fromElement, path );
				elem = factory.CreateCollectionElementsJoin( queryableCollection, elementTable );
				if ( Log.IsDebugEnabled )
				{
					Log.Debug( "No FROM element found for the elements of collection join path " + path
							+ ", created " + elem );
				}
			}
			else 
			{
				if ( Log.IsDebugEnabled ) 
				{
					Log.Debug( "FROM element found for collection join path " + path );
				}
			}

			// The 'from element' that represents the elements of the collection.
			FromElement = fromElement;

			// Add the condition to the join sequence that qualifies the indexed element.
			IASTNode selector = GetChild(1);
			if ( selector == null ) 
			{
				throw new QueryException( "No index value!" );
			}

			// Sometimes use the element table alias, sometimes use the... umm... collection table alias (many to many)
			String collectionTableAlias = elementTable;
			if ( elem.CollectionTableAlias != null ) 
			{
				collectionTableAlias = elem.CollectionTableAlias;
			}

			// TODO: get SQL rendering out of here, create an AST for the join expressions.
			// Use the SQL generator grammar to generate the SQL text for the index expression.
			JoinSequence joinSequence = fromElement.JoinSequence;
			string[] indexCols = queryableCollection.IndexColumnNames;
			if ( indexCols.Length != 1 ) 
			{
				throw new QueryException( "composite-index appears in []: " + collectionNode.Path );
			}

			SqlGenerator gen = new SqlGenerator(SessionFactoryHelper.Factory, new CommonTreeNodeStream(selector));

			try 
			{
				gen.simpleExpr(); //TODO: used to be exprNoParens! was this needed?
			}
			catch ( RecognitionException e ) 
			{
				throw new QueryException( e.Message, e );
			}

			string selectorExpression = gen.GetSQL().ToString();

			joinSequence.AddCondition(new SqlString(collectionTableAlias + '.' + indexCols[0] + " = " + selectorExpression ));
			//joinSequence.AddCondition(collectionTableAlias, new string[] { indexCols[0] }, selectorExpression, false);

			IList<IParameterSpecification> paramSpecs = gen.GetCollectedParameters();
			if ( paramSpecs != null ) 
			{
				switch ( paramSpecs.Count ) 
				{
					case 0 :
						// nothing to do
						break;
					case 1 :
						IParameterSpecification paramSpec = paramSpecs[0];
						paramSpec.ExpectedType = queryableCollection.IndexType;
						fromElement.SetIndexCollectionSelectorParamSpec( paramSpec );
						break;
					default:
						fromElement.SetIndexCollectionSelectorParamSpec(
								new AggregatedIndexCollectionSelectorParameterSpecifications( paramSpecs )
						);
						break;
				}
			}

			// Now, set the text for this node.  It should be the element columns.
			String[] elementColumns = queryableCollection.GetElementColumnNames( elementTable );
			Text = elementColumns[0];

		    IsResolved = true;
		}

    	public override void PrepareForDot(string propertyName) 
        {
    	    FromElement fromElement = FromElement;

		    if ( fromElement == null ) 
            {
			    throw new InvalidOperationException( "No FROM element for index operator!" );
		    }

		    IQueryableCollection queryableCollection = fromElement.QueryableCollection;

		    if ( queryableCollection != null && !queryableCollection.IsOneToMany) 
            {
			    FromReferenceNode collectionNode = ( FromReferenceNode ) GetChild(0);
			    String path = collectionNode.Path + "[]." + propertyName;

			    if (Log.IsDebugEnabled) 
                {
				    Log.Debug( "Creating join for many-to-many elements for " + path );
			    }

			    FromElementFactory factory = new FromElementFactory( fromElement.FromClause, fromElement, path );

			    // This will add the new from element to the origin.
			    FromElement elementJoin = factory.CreateElementJoin( queryableCollection );
			    FromElement = elementJoin;
		    }
	    }
    }
}
