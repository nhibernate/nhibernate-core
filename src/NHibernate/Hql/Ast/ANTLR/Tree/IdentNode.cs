using System;
using System.Collections.Generic;
using Antlr.Runtime;
using NHibernate.Dialect.Function;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public class IdentNode : FromReferenceNode, ISelectExpression
	{
		private const int Unknown = 0;
		private const int PropertyRef = 1;
		private const int ComponentRef = 2;
		private bool _nakedPropertyRef;

		public IdentNode(IToken token) : base(token)
		{
		}

		public override IType DataType
		{
			get
			{
					
				IType type = base.DataType;
				if ( type != null ) 
				{
					return type;
				}
				FromElement fe = FromElement;
				if ( fe != null ) 
				{
					return fe.DataType;
				}
				ISQLFunction sf = Walker.SessionFactoryHelper.FindSQLFunction(Text);
				return sf == null ? null : sf.ReturnType(null, null);
			}

			set
			{
				base.DataType = value;
			}
		}

		public override void SetScalarColumnText(int i)
		{
			if (_nakedPropertyRef) 
			{
				// do *not* over-write the column text, as that has already been
				// "rendered" during resolve
				ColumnHelper.GenerateSingleScalarColumn(Walker.ASTFactory, this, i);
			}
			else {
				FromElement fe = FromElement;
				if (fe != null) {
					Text = fe.RenderScalarIdentifierSelect(i);
				}
				else {
					ColumnHelper.GenerateSingleScalarColumn(Walker.ASTFactory, this, i);
				}
			}
		}

		public override void ResolveIndex(IASTNode parent)
		{
			// An ident node can represent an index expression if the ident
			// represents a naked property ref
			//      *Note: this makes the assumption (which is currently the case
			//      in the hql-sql grammar) that the ident is first resolved
			//      itself (addrExpr -> resolve()).  The other option, if that
			//      changes, is to call resolve from here; but it is
			//      currently un-needed overhead.
			if (!(IsResolved && _nakedPropertyRef)) 
			{
				throw new InvalidOperationException();
			}

			string propertyName = OriginalText;
			if (!DataType.IsCollectionType) 
			{
				throw new SemanticException("Collection expected; [" + propertyName + "] does not refer to a collection property");
			}

			// TODO : most of below was taken verbatim from DotNode; should either delegate this logic or super-type it
			CollectionType type = (CollectionType) DataType;
			string role = type.Role;
			IQueryableCollection queryableCollection = SessionFactoryHelper.RequireQueryableCollection(role);

			string columnTableAlias = FromElement.TableAlias;

			FromElementFactory factory = new FromElementFactory(
				  	Walker.CurrentFromClause,
					FromElement,
					propertyName,
					null,
					FromElement.ToColumns(columnTableAlias, propertyName, false),
					true
			);

			FromElement elem = factory.CreateCollection(queryableCollection, role, JoinType.InnerJoin, false, true);
			FromElement = elem;
			Walker.AddQuerySpaces(queryableCollection.CollectionSpaces);	// Always add the collection's query spaces.
		}

		public override void Resolve(bool generateJoin, bool implicitJoin, string classAlias, IASTNode parent)
		{
			if (!IsResolved)
			{
				if (Walker.CurrentFromClause.IsFromElementAlias(Text))
				{
					if (ResolveAsAlias())
					{
						IsResolved = true;
						// We represent a from-clause alias
					}
				}
				else if (parent != null && parent.Type == HqlSqlWalker.DOT)
				{
					DotNode dot = (DotNode)parent;
					if (parent.GetFirstChild() == this)
					{
						if (ResolveAsNakedComponentPropertyRefLhs(dot))
						{
							// we are the LHS of the DOT representing a naked comp-prop-ref
							IsResolved = true;
						}
					}
					else
					{
						if (ResolveAsNakedComponentPropertyRefRhs(dot))
						{
							// we are the RHS of the DOT representing a naked comp-prop-ref
							IsResolved = true;
						}
					}
				}
				else
				{
					int result = ResolveAsNakedPropertyRef();
					if (result == PropertyRef)
					{
						// we represent a naked (simple) prop-ref
						IsResolved = true;
					}
					else if (result == ComponentRef)
					{
						// EARLY EXIT!!!  return so the resolve call explicitly coming from DotNode can
						// resolve this...
						return;
					}
				}

				// if we are still not resolved, we might represent a constant.
				//      needed to add this here because the allowance of
				//      naked-prop-refs in the grammar collides with the
				//      definition of literals/constants ("nondeterminism").
				//      TODO: cleanup the grammar so that "processConstants" is always just handled from here
				if (!IsResolved)
				{
					try
					{
						Walker.LiteralProcessor.ProcessConstant(this, false);
					}
					catch (Exception)
					{
						// just ignore it for now, it'll get resolved later...
					}
				}
			}
		}

		private int ResolveAsNakedPropertyRef()
		{
			FromElement fromElement = LocateSingleFromElement();

			if (fromElement == null)
			{
				return Unknown;
			}

			IQueryable persister = fromElement.Queryable;
			if (persister == null)
			{
				return Unknown;
			}

			IType propertyType = GetNakedPropertyType(fromElement);
			if (propertyType == null)
			{
				// assume this ident's text does *not* refer to a property on the given persister
				return Unknown;
			}

			if ((propertyType.IsComponentType || propertyType.IsAssociationType))
			{
				return ComponentRef;
			}

			FromElement = fromElement;

			string property = Text;
			string[] columns = Walker.IsSelectStatement
					? persister.ToColumns(fromElement.TableAlias, property)
					: persister.ToColumns(property);

			string text = StringHelper.Join(", ", columns);
			Text = columns.Length == 1 ? text : "(" + text + ")";
			Type = HqlSqlWalker.SQL_TOKEN;

			// these pieces are needed for usage in select clause
			DataType = propertyType;
			_nakedPropertyRef = true;

			return PropertyRef;
		}

		private bool ResolveAsNakedComponentPropertyRefLhs(DotNode parent)
		{
			FromElement fromElement = LocateSingleFromElement();
			if (fromElement == null)
			{
				return false;
			}

			IType componentType = GetNakedPropertyType(fromElement);

			if (componentType == null)
			{
				throw new QueryException("Unable to resolve path [" + parent.Path + "], unexpected token [" + OriginalText + "]");
			}
			if (!componentType.IsComponentType)
			{
				throw new QueryException("Property '" + OriginalText + "' is not a component.  Use an alias to reference associations or collections.");
			}

			IType propertyType ;  // used to set the type of the parent dot node
			string propertyPath = Text + "." + NextSibling.Text;
			try
			{
				// check to see if our "propPath" actually
				// represents a property on the persister
				propertyType = fromElement.GetPropertyType(Text, propertyPath);
			}
			catch (Exception)
			{
				// assume we do *not* refer to a property on the given persister
				return false;
			}

			FromElement = fromElement;
			parent.PropertyPath = propertyPath;
			parent.DataType = propertyType;

			return true;
		}

		private bool ResolveAsNakedComponentPropertyRefRhs(DotNode parent)
		{
			FromElement fromElement = LocateSingleFromElement();
			if (fromElement == null)
			{
				return false;
			}

			IType propertyType;
			string propertyPath = parent.GetLhs().Text + "." + Text;
			try
			{
				// check to see if our "propPath" actually
				// represents a property on the persister
				propertyType = fromElement.GetPropertyType(Text, propertyPath);
			}
			catch (Exception)
			{
				// assume we do *not* refer to a property on the given persister
				return false;
			}

			FromElement = fromElement;
			// this piece is needed for usage in select clause
			DataType = propertyType;

			_nakedPropertyRef = true;

			return true;
		}


		private IType GetNakedPropertyType(FromElement fromElement)
		{
			if (fromElement == null)
			{
				return null;
			}

			string property = OriginalText;
			IType propertyType = null;

			try
			{
				propertyType = fromElement.GetPropertyType(property, property);
			}
			catch (Exception)
			{
			}
			return propertyType;
		}

		private FromElement LocateSingleFromElement()
		{
			IList<IASTNode> fromElements = Walker.CurrentFromClause.GetFromElements();
			if (fromElements == null || fromElements.Count != 1)
			{
				// TODO : should this be an error?
				return null;
			}

			FromElement element = (FromElement)fromElements[0];
			if (element.ClassAlias != null)
			{
				// naked property-refs cannot be used with an aliased from element
				return null;
			}
			return element;
		}

		private bool ResolveAsAlias()
		{
			// This is not actually a constant, but a reference to FROM element.
			FromElement element = Walker.CurrentFromClause.GetFromElement(Text);
			if (element != null)
			{
				FromElement = element;
				Text = element.GetIdentityColumn();
				Type = HqlSqlWalker.ALIAS_REF;
				return true;
			}
			return false;
		}
	}
}
