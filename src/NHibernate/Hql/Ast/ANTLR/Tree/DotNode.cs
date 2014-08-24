using System;
using Antlr.Runtime;

using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents a reference to a property or alias expression.  This should duplicate the relevant behaviors in
	/// PathExpressionParser.
	/// Author: Joshua Davis
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class DotNode : FromReferenceNode 
	{
		private static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof(DotNode));

		private const int DerefUnknown = 0;
		private const int DerefEntity = 1;
		private const int DerefComponent = 2;
		private const int DerefCollection = 3;
		private const int DerefPrimitive = 4;
		private const int DerefIdentifier = 5;
		private const int DerefJavaConstant = 6;

		public static bool UseThetaStyleImplicitJoins;
		public static bool REGRESSION_STYLE_JOIN_SUPPRESSION;

		/// <summary>
		/// The full path, to the root alias of this dot node.
		/// </summary>
		private string _path;

		/// <summary>
		/// The type of dereference that hapened (DEREF_xxx).
		/// </summary>
		private int _dereferenceType = DerefUnknown;

		/// <summary>
		/// The identifier that is the name of the property.
		/// </summary>
		private string _propertyName;

		/// <summary>
		/// The unresolved property path relative to this dot node.
		/// </summary>
		private string _propertyPath;

		/// <summary>
		/// The column names that this resolves to.
		/// </summary>
		private string[] _columns;

		/// <summary>
		/// Fetch join or not.
		/// </summary>
		private bool _fetch;

		private FromElement _impliedJoin;

		/// <summary>
		/// The type of join to create.   Default is an inner join.
		/// </summary>
		private JoinType _joinType = JoinType.InnerJoin;

		public DotNode(IToken token) : base(token)
		{
		}

		/// <summary>
		/// Sets the join type for this '.' node structure.
		/// </summary>
		public JoinType JoinType
		{
			set { _joinType = value; }
		}

		public bool Fetch
		{
			set { _fetch = value;  }
		}

		public override FromElement GetImpliedJoin()
		{
			return _impliedJoin;
		}

		/// <summary>
		/// Returns the full path of the node.
		/// </summary>
		public override string Path
		{
			get
			{
				if (_path == null)
				{
					var lhs = GetLhs();
					if (lhs == null)
					{
						_path = Text;
					}
					else
					{
						var rhs = (SqlNode) lhs.NextSibling;
						_path = lhs.Path + "." + rhs.OriginalText;
					}
				}
				return _path;
			}
		}


		public string PropertyPath
		{
			get { return _propertyPath; }
			set { _propertyPath = value; }
		}

		public override void SetScalarColumnText(int i)
		{
			string[] sqlColumns = GetColumns();
			ColumnHelper.GenerateScalarColumns(Walker.ASTFactory, this, sqlColumns, i);
		}

		public override void ResolveIndex(IASTNode parent) 
		{
			if (IsResolved) 
			{
				return;
			}

			IType propertyType = PrepareLhs();			// Prepare the left hand side and get the data type.

			DereferenceCollection( ( CollectionType ) propertyType, true, true, null);
		}

		public override void ResolveFirstChild()
		{
			var lhs = (FromReferenceNode)GetFirstChild();
			var property = (SqlNode) GetChild(1);

			// Set the attributes of the property reference expression.
			string propName = property.Text;
			_propertyName = propName;

			// If the uresolved property path isn't set yet, just use the property name.
			if (_propertyPath == null)
			{
				_propertyPath = propName;
			}

			// Resolve the LHS fully, generate implicit joins.  Pass in the property name so that the resolver can
			// discover foreign key (id) properties.
			lhs.Resolve(true, true, null, this);

			FromElement = lhs.FromElement;			// The 'from element' that the property is in.

			CheckSubclassOrSuperclassPropertyReference(lhs, propName);
		}

		public override void ResolveInFunctionCall(bool generateJoin, bool implicitJoin)
		{
			if (IsResolved)
			{
				return;
			}

			IType propertyType = PrepareLhs();			// Prepare the left hand side and get the data type.

			if (propertyType != null && propertyType.IsCollectionType)
			{
				ResolveIndex(null);
			}
			else
			{
				ResolveFirstChild();
				Resolve(generateJoin, implicitJoin);
			}
		}

		public override void Resolve(bool generateJoin, bool implicitJoin, string classAlias, IASTNode parent)
		{
			// If this dot has already been resolved, stop now.
			if ( IsResolved ) 
			{
				return;
			}

			IType propertyType = PrepareLhs(); // Prepare the left hand side and get the data type.

			// If there is no data type for this node, and we're at the end of the path (top most dot node), then
			// this might be a Java constant.
			if ( propertyType == null ) 
			{
				if ( parent == null ) 
				{
					Walker.LiteralProcessor.LookupConstant( this );
				}
				// If the propertyType is null and there isn't a parent, just
				// stop now... there was a problem resolving the node anyway.
				return;
			}

			if ( propertyType.IsComponentType ) 
			{
				// The property is a component...
				CheckLhsIsNotCollection();
				DereferenceComponent( parent );
				InitText();
			}
			else if ( propertyType.IsEntityType ) 
			{
				// The property is another class..
				CheckLhsIsNotCollection();
				DereferenceEntity( ( EntityType ) propertyType, implicitJoin, classAlias, generateJoin, parent );
				InitText();
			}
			else if (propertyType.IsCollectionType)
			{
				// The property is a collection...
				CheckLhsIsNotCollection();
				DereferenceCollection((CollectionType)propertyType, implicitJoin, false, classAlias);
			}
			else
			{
				// Otherwise, this is a primitive type.
				if (!CollectionProperties.IsAnyCollectionProperty(_propertyName))
				{
					CheckLhsIsNotCollection();
				}
				_dereferenceType = DerefPrimitive;
				InitText();
			}

			IsResolved = true;
		}

		
		public FromReferenceNode GetLhs()
		{
			var lhs = ((FromReferenceNode)GetChild(0));
			if (lhs == null)
			{
				throw new InvalidOperationException("DOT node with no left-hand-side!");
			}
			return lhs;
		}

		private IType GetDataType()
		{
			if (DataType == null)
			{
				FromElement fromElement = GetLhs().FromElement;

				if (fromElement == null)
				{
					return null;
				}

				// If the lhs is a collection, use CollectionPropertyMapping
				IType propertyType = fromElement.GetPropertyType(_propertyName, _propertyPath);
				if (Log.IsDebugEnabled)
				{
					Log.Debug("getDataType() : " + _propertyPath + " -> " + propertyType);
				}

				DataType = propertyType;
			}

			return DataType;
		}

		public void SetResolvedConstant(string text)
		{
			_path = text;
			_dereferenceType = DerefJavaConstant;
			IsResolved = true; // Don't resolve the node again.
		}

		private static QueryException BuildIllegalCollectionDereferenceException(string propertyName, IASTNode lhs)
		{
			string lhsPath = ASTUtil.GetPathText(lhs);
			return new QueryException("illegal attempt to dereference collection [" + lhsPath + "] with element property reference [" + propertyName + "]");
		}

		private void DereferenceCollection(CollectionType collectionType, bool implicitJoin, bool indexed, string classAlias)
		{
			_dereferenceType = DerefCollection;
			string role = collectionType.Role;

			//foo.bars.size (also handles deprecated stuff like foo.bars.maxelement for backwardness)
			IASTNode sibling = NextSibling;
			bool isSizeProperty = sibling != null && CollectionProperties.IsAnyCollectionProperty( sibling.Text );

			if (isSizeProperty)
			{
				indexed = true; //yuck!}
			}

			IQueryableCollection queryableCollection = SessionFactoryHelper.RequireQueryableCollection( role );
			string propName = Path;
			FromClause currentFromClause = Walker.CurrentFromClause;

			if ( Walker.StatementType != HqlSqlWalker.SELECT && indexed && classAlias == null ) 
			{
				// should indicate that we are processing an INSERT/UPDATE/DELETE
				// query with a subquery implied via a collection property
				// function. Here, we need to use the table name itself as the
				// qualification alias.
				// TODO : verify this works for all databases...
				// TODO : is this also the case in non-"indexed" scenarios?
				string alias = GetLhs().FromElement.Queryable.TableName;
				_columns = FromElement.ToColumns(alias, _propertyPath, false, true);
			}

			//We do not look for an existing join on the same path, because
			//it makes sense to join twice on the same collection role
			var factory = new FromElementFactory(
					currentFromClause,
					GetLhs().FromElement,
					propName,
					classAlias,
					GetColumns(),
					implicitJoin
			);
			FromElement elem = factory.CreateCollection( queryableCollection, role, _joinType, _fetch, indexed );

			if ( Log.IsDebugEnabled ) 
			{
				Log.Debug( "dereferenceCollection() : Created new FROM element for " + propName + " : " + elem );
			}

			SetImpliedJoin( elem );
			FromElement = elem;	// This 'dot' expression now refers to the resulting from element.

			if ( isSizeProperty ) 
			{
				elem.Text = "";
				elem.UseWhereFragment = false;
			}

			if ( !implicitJoin ) 
			{
				IEntityPersister entityPersister = elem.EntityPersister;
				if ( entityPersister != null ) 
				{
					Walker.AddQuerySpaces( entityPersister.QuerySpaces );
				}
			}
			Walker.AddQuerySpaces( queryableCollection.CollectionSpaces );	// Always add the collection's query spaces.
		}


		private void DereferenceEntity(EntityType entityType, bool implicitJoin, string classAlias, bool generateJoin, IASTNode parent) 
		{
			CheckForCorrelatedSubquery( "dereferenceEntity" );

			// three general cases we check here as to whether to render a physical SQL join:
			// 1) is our parent a DotNode as well?  If so, our property reference is
			// 		being further de-referenced...
			// 2) is this a DML statement
			// 3) we were asked to generate any needed joins (generateJoins==true) *OR*
			//		we are currently processing a select or from clause
			// (an additional check is the REGRESSION_STYLE_JOIN_SUPPRESSION check solely intended for the test suite)
			//
			// The REGRESSION_STYLE_JOIN_SUPPRESSION is an additional check
			// intended solely for use within the test suite.  This forces the
			// implicit join resolution to behave more like the classic parser.
			// The underlying issue is that classic translator is simply wrong
			// about its decisions on whether or not to render an implicit join
			// into a physical SQL join in a lot of cases.  The piece it generally
			// tends to miss is that INNER joins effect the results by further
			// restricting the data set!  A particular manifestation of this is
			// the fact that the classic translator will skip the physical join
			// for ToOne implicit joins *if the query is shallow*; the result
			// being that Query.list() and Query.iterate() could return
			// different number of results!

			DotNode parentAsDotNode = null;
			string property = _propertyName;
			bool joinIsNeeded;

			if ( IsDotNode( parent ) ) 
			{
				// our parent is another dot node, meaning we are being further dereferenced.
				// thus we need to generate a join unless the parent refers to the associated
				// entity's PK (because 'our' table would know the FK).
				parentAsDotNode = ( DotNode ) parent;
				property = parentAsDotNode._propertyName;
				joinIsNeeded = generateJoin && !IsReferenceToPrimaryKey( parentAsDotNode._propertyName, entityType );
			}
			else if ( ! Walker.IsSelectStatement ) 
			{
				// in non-select queries, the only time we should need to join is if we are in a subquery from clause
				joinIsNeeded = Walker.CurrentStatementType == HqlSqlWalker.SELECT && Walker.IsInFrom;
			}
			else if ( REGRESSION_STYLE_JOIN_SUPPRESSION ) {
				// this is the regression style determination which matches the logic of the classic translator
				joinIsNeeded = generateJoin && ( !Walker.IsInSelect || !Walker.IsShallowQuery);
			}
			else 
			{
				joinIsNeeded = generateJoin || ( Walker.IsInSelect || Walker.IsInFrom );
			}

			if ( joinIsNeeded ) 
			{
				DereferenceEntityJoin( classAlias, entityType, implicitJoin, parent );
			}
			else 
			{
				DereferenceEntityIdentifier( property, parentAsDotNode );
			}
		}

		private void DereferenceEntityIdentifier(string propertyName, DotNode dotParent)
		{
			// special shortcut for id properties, skip the join!
			// this must only occur at the _end_ of a path expression
			if (Log.IsDebugEnabled)
			{
				Log.Debug("dereferenceShortcut() : property " +
					propertyName + " in " + FromElement.ClassName +
					" does not require a join.");
			}

			InitText();
			SetPropertyNameAndPath(dotParent); // Set the unresolved path in this node and the parent.
			// Set the text for the parent.
			if (dotParent != null)
			{
				dotParent._dereferenceType = DerefIdentifier;
				dotParent.Text = Text;
				dotParent._columns = GetColumns();
			}
		}

		private void DereferenceEntityJoin(string classAlias, EntityType propertyType, bool impliedJoin, IASTNode parent)
		{
			_dereferenceType = DerefEntity;
			if ( Log.IsDebugEnabled ) 
			{
				Log.Debug( "dereferenceEntityJoin() : generating join for " + _propertyName + " in "
						+ FromElement.ClassName + " "
						+ ( ( classAlias == null ) ? "{no alias}" : "(" + classAlias + ")" )
						+ " parent = " + ASTUtil.GetDebugstring( parent )
				);
			}

			// Create a new FROM node for the referenced class.
			string associatedEntityName = propertyType.GetAssociatedEntityName();
			string tableAlias = AliasGenerator.CreateName( associatedEntityName );

			string[] joinColumns = GetColumns();
			string joinPath = Path;

			if ( impliedJoin && Walker.IsInFrom ) 
			{
				_joinType = Walker.ImpliedJoinType;
			}

			FromClause currentFromClause = Walker.CurrentFromClause;
			FromElement elem = currentFromClause.FindJoinByPath( joinPath );

			///////////////////////////////////////////////////////////////////////////////
			//
			// This is the piece which recognizes the condition where an implicit join path
			// resolved earlier in a correlated subquery is now being referenced in the
			// outer query.  For 3.0final, we just let this generate a second join (which
			// is exactly how the old parser handles this).  Eventually we need to add this
			// logic back in and complete the logic in FromClause.promoteJoin; however,
			// FromClause.promoteJoin has its own difficulties (see the comments in
			// FromClause.promoteJoin).
			//
			//		if ( elem == null ) {
			//			// see if this joinPath has been used in a "child" FromClause, and if so
			//			// promote that element to the outer query
			//			FromClause currentNodeOwner = getFromElement().getFromClause();
			//			FromClause currentJoinOwner = currentNodeOwner.locateChildFromClauseWithJoinByPath( joinPath );
			//			if ( currentJoinOwner != null && currentNodeOwner != currentJoinOwner ) {
			//				elem = currentJoinOwner.findJoinByPathLocal( joinPath );
			//				if ( elem != null ) {
			//					currentFromClause.promoteJoin( elem );
			//					// EARLY EXIT!!!
			//					return;
			//				}
			//			}
			//		}
			//
			///////////////////////////////////////////////////////////////////////////////

			bool found = elem != null;
			// even though we might find a pre-existing element by join path, for FromElements originating in a from-clause
			// we should only ever use the found element if the aliases match (null != null here).  
			// Implied joins are ok to reuse only if in same from clause (are there any other cases when we should reject implied joins?).
			bool useFoundFromElement = found &&
									   (elem.IsImplied && elem.FromClause == currentFromClause || // NH different behavior (NH-3002)
										AreSame(classAlias, elem.ClassAlias));

			if ( ! useFoundFromElement )
			{
				// If this is an implied join in a from element, then use the impled join type which is part of the
				// tree parser's state (set by the gramamar actions).
				JoinSequence joinSequence = SessionFactoryHelper
					.CreateJoinSequence( impliedJoin, propertyType, tableAlias, _joinType, joinColumns );

				var factory = new FromElementFactory(
						currentFromClause,
						GetLhs().FromElement,
						joinPath,
						classAlias,
						joinColumns,
						impliedJoin
				);
				elem = factory.CreateEntityJoin(
						associatedEntityName,
						tableAlias,
						joinSequence,
						_fetch,
						Walker.IsInFrom,
						propertyType
				);
			}
			else 
			{
				currentFromClause.AddDuplicateAlias(classAlias, elem);
			}
		

			SetImpliedJoin( elem );
			Walker.AddQuerySpaces( elem.EntityPersister.QuerySpaces );
			FromElement = elem;	// This 'dot' expression now refers to the resulting from element.
		}

		private bool AreSame(String alias1, String alias2) {
			// again, null != null here
			return !StringHelper.IsEmpty( alias1 ) && !StringHelper.IsEmpty( alias2 ) && alias1.Equals( alias2 );
		}

		private void SetImpliedJoin(FromElement elem)
		{
			_impliedJoin = elem;
			if (GetFirstChild().Type == HqlSqlWalker.DOT)
			{
				var dotLhs = (DotNode)GetFirstChild();
				if (dotLhs.GetImpliedJoin() != null)
				{
					_impliedJoin = dotLhs.GetImpliedJoin();
				}
			}
		}


		/// <summary>
		/// Is the given property name a reference to the primary key of the associated
		/// entity construed by the given entity type?
		/// For example, consider a fragment like order.customer.id
		/// (where order is a from-element alias).  Here, we'd have:
		/// propertyName = "id" AND
		/// owningType = ManyToOneType(Customer)
		/// and are being asked to determine whether "customer.id" is a reference
		/// to customer's PK...
		/// </summary>
		/// <param name="propertyName">The name of the property to check.</param>
		/// <param name="owningType">The type represeting the entity "owning" the property</param>
		/// <returns>True if propertyName references the entity's (owningType->associatedEntity) primary key; false otherwise.</returns>
		private bool IsReferenceToPrimaryKey(string propertyName, EntityType owningType)
		{
			IEntityPersister persister = SessionFactoryHelper.Factory.GetEntityPersister(owningType.GetAssociatedEntityName());
			if (persister.EntityMetamodel.HasNonIdentifierPropertyNamedId)
			{
				// only the identifier property field name can be a reference to the associated entity's PK...
				return propertyName == persister.IdentifierPropertyName && owningType.IsReferenceToPrimaryKey;
			}
			
			// here, we have two possibilities:
			// 		1) the property-name matches the explicitly identifier property name
			//		2) the property-name matches the implicit 'id' property name
			if (EntityPersister.EntityID == propertyName)
			{
				// the referenced node text is the special 'id'
				return owningType.IsReferenceToPrimaryKey;
			}
			
			string keyPropertyName = SessionFactoryHelper.GetIdentifierOrUniqueKeyPropertyName(owningType);
			return keyPropertyName != null && keyPropertyName == propertyName && owningType.IsReferenceToPrimaryKey;
		}

		private void CheckForCorrelatedSubquery(string methodName)
		{
			if (IsCorrelatedSubselect)
			{
				if (Log.IsDebugEnabled)
				{
					Log.Debug(methodName + "() : correlated subquery");
				}
			}
		}

		private bool IsCorrelatedSubselect
		{
			get { return Walker.IsSubQuery && FromElement.FromClause != Walker.CurrentFromClause; }
		}


		private void CheckLhsIsNotCollection()
		{
			FromReferenceNode lhs = GetLhs();
			if ( lhs.DataType != null && lhs.DataType.IsCollectionType ) 
			{
				throw BuildIllegalCollectionDereferenceException( _propertyName, lhs );
			}
		}

		private IType PrepareLhs()
		{
			FromReferenceNode lhs = GetLhs();
			lhs.PrepareForDot( _propertyName );
			return GetDataType();
		}

		private void DereferenceComponent(IASTNode parent)
		{
			_dereferenceType = DerefComponent;
			SetPropertyNameAndPath(parent);
		}

		private void SetPropertyNameAndPath(IASTNode parent)
		{
			if (IsDotNode(parent))
			{
				var dotNode = (DotNode)parent;

				IASTNode rhs = dotNode.GetChild(1);
				_propertyName = rhs.Text;
				_propertyPath = _propertyPath + "." + _propertyName; // Append the new property name onto the unresolved path.
				dotNode._propertyPath = _propertyPath;
				if (Log.IsDebugEnabled)
				{
					Log.Debug("Unresolved property path is now '" + dotNode._propertyPath + "'");
				}
			}
			else
			{
				if (Log.IsDebugEnabled)
				{
					Log.Debug("terminal propertyPath = [" + _propertyPath + "]");
				}
			}
		}

		private void InitText()
		{
			string[] cols = GetColumns();
			string text = StringHelper.Join(", ", cols);
			if (cols.Length > 1 && Walker.IsComparativeExpressionClause)
			{
				text = "(" + text + ")";
			}
			Text = text;
		}

		private string[] GetColumns()
		{
			if ( _columns == null ) 
			{
				// Use the table fromElement and the property name to get the array of column names.
				string tableAlias = GetLhs().FromElement.TableAlias;
				_columns = FromElement.ToColumns( tableAlias, _propertyPath, false );
			}
			return _columns;
		}

		private static void CheckSubclassOrSuperclassPropertyReference(AbstractSelectExpression lhs, String propertyName) 
		{
			if ( lhs != null && !( lhs is IndexNode ) ) 
			{
				FromElement source = lhs.FromElement;
				if ( source != null ) 
				{
					source.HandlePropertyBeingDereferenced( lhs.DataType, propertyName );
				}
			}
		}

		private static bool IsDotNode(IASTNode n)
		{
			return n != null && n.Type == HqlSqlWalker.DOT;
		}

		public void ResolveSelectExpression()
		{
			if (Walker.IsShallowQuery || Walker.CurrentFromClause.IsSubQuery)
			{
				Resolve(false, true);
			}
			else
			{
				Resolve(true, false);
				IType type = GetDataType();

				if (type.IsEntityType)
				{
					FromElement fromElement = FromElement;
					fromElement.IncludeSubclasses = true; // Tell the destination fromElement to 'includeSubclasses'.

					if (UseThetaStyleImplicitJoins)
					{
						fromElement.JoinSequence.SetUseThetaStyle(true);	// Use theta style (for regression)

						// Move the node up, after the origin node.
						FromElement origin = fromElement.Origin;

						if (origin != null)
						{
							ASTUtil.MakeSiblingOfParent(origin, fromElement);
						}
					}
				}
			}

			FromReferenceNode lhs = GetLhs();
			while (lhs != null)
			{
				CheckSubclassOrSuperclassPropertyReference(lhs, lhs.NextSibling.Text);
				lhs = (FromReferenceNode)lhs.GetChild(0);
			}
		}
	}
}
