using System;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Hql.Util;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;
using IASTNode=NHibernate.Hql.Ast.ANTLR.Tree.IASTNode;

namespace NHibernate.Hql.Ast.ANTLR
{
	[CLSCompliant(false)]
	public class SessionFactoryHelperExtensions
	{
		private readonly ISessionFactoryImplementor _sfi;
		private readonly NullableDictionary<string, IPropertyMapping> _collectionPropertyMappingByRole;
		private readonly SessionFactoryHelper helper;

		/// <summary>
		/// Construct a new SessionFactoryHelperExtensions instance.
		/// </summary>
		/// <param name="sfi">The SessionFactory impl to be encapsulated.</param>
		public SessionFactoryHelperExtensions(ISessionFactoryImplementor sfi)
		{
			_sfi = sfi;
			helper = new SessionFactoryHelper(_sfi);
			_collectionPropertyMappingByRole = new NullableDictionary<string, IPropertyMapping>();
		}

		public ISessionFactoryImplementor Factory
		{
			get { return _sfi;}
		}

		/// <summary>
		/// Locate a registered sql function by name.
		/// </summary>
		/// <param name="functionName">The name of the function to locate</param>
		/// <returns>The sql function, or null if not found.</returns>
		public ISQLFunction FindSQLFunction(string functionName)
		{
			return _sfi.SQLFunctionRegistry.FindSQLFunction(functionName.ToLowerInvariant());
		}

		/// <summary>
		/// Locate a registered sql function by name.
		/// </summary>
		/// <param name="functionName">The name of the function to locate</param>
		/// <returns>The sql function, or throws QueryException if no matching sql functions could be found.</returns>
		private ISQLFunction RequireSQLFunction(string functionName)
		{
			ISQLFunction f = FindSQLFunction(functionName);

			if (f == null)
			{
				throw new QueryException("Unable to find SQL function: " + functionName);
			}
			return f;
		}

		/// <summary>
		/// Find the function return type given the function name and the first argument expression node.
		/// </summary>
		/// <param name="functionName">The function name.</param>
		/// <param name="first">The first argument expression.</param>
		/// <returns>the function return type given the function name and the first argument expression node.</returns>
		public IType FindFunctionReturnType(String functionName, IASTNode first)
		{
			// locate the registered function by the given name
			ISQLFunction sqlFunction = RequireSQLFunction(functionName);

			// determine the type of the first argument...
			IType argumentType = null;

			if (first != null)
			{
				if (functionName == "cast")
				{
					argumentType = TypeFactory.HeuristicType(first.NextSibling.Text);
				}
				else if (first is SqlNode)
				{
					argumentType = ((SqlNode) first).DataType;
				}
			}

			return sqlFunction.ReturnType(argumentType, _sfi);
		}

		/// <summary>
		/// Given a (potentially unqualified) class name, locate its imported qualified name.
		/// </summary>
		/// <param name="className">The potentially unqualified class name</param>
		/// <returns>The qualified class name.</returns>
		public string GetImportedClassName(string className)
		{
			return _sfi.GetImportedClassName(className);
		}

		/// <summary>
		/// Does the given persister define a physical discriminator column
		/// for the purpose of inheritance discrimination?
		/// </summary>
		/// <param name="persister">The persister to be checked.</param>
		/// <returns>True if the persister does define an actual discriminator column.</returns>
		public bool HasPhysicalDiscriminatorColumn(IQueryable persister)
		{
			if (persister.DiscriminatorType != null)
			{
				string discrimColumnName = persister.DiscriminatorColumnName;
				// Needed the "clazz_" check to work around union-subclasses
				// TODO : is there a way to tell whether a persister is truly discrim-column based inheritence?
				if (discrimColumnName != null && "clazz_" != discrimColumnName)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Locate the collection persister by the collection role.
		/// </summary>
		/// <param name="collectionFilterRole">The collection role name.</param>
		/// <returns>The defined CollectionPersister for this collection role, or null.</returns>
		public IQueryableCollection GetCollectionPersister(string collectionFilterRole)
		{
			try
			{
				return (IQueryableCollection)_sfi.GetCollectionPersister(collectionFilterRole);
			}
			catch (InvalidCastException cce)
			{
				throw new QueryException("collection is not queryable: " + collectionFilterRole, cce);
			}
			catch (Exception e)
			{
				throw new QueryException("collection not found: " + collectionFilterRole, e);
			}
		}

		/// <summary>
		/// Determine the name of the property for the entity encapsulated by the
		/// given type which represents the id or unique-key.
		/// </summary>
		/// <param name="entityType">The type representing the entity.</param>
		/// <returns>The corresponding property name</returns>
		public string GetIdentifierOrUniqueKeyPropertyName(EntityType entityType)
		{
			try
			{
				return entityType.GetIdentifierOrUniqueKeyPropertyName(_sfi);
			}
			catch (MappingException me)
			{
				throw new QueryException(me);
			}
		}

		/// <summary>
		/// Retrieves the column names corresponding to the collection elements for the given
		/// collection role.
		/// </summary>
		/// <param name="role">The collection role</param>
		/// <param name="roleAlias">The sql column-qualification alias (i.e., the table alias)</param>
		/// <returns>the collection element columns</returns>
		public string[] GetCollectionElementColumns(string role, string roleAlias)
		{
			// TODO - add CollectionPropertyNames.COLLECTION_ELEMENTS (involves core NH change)
			return GetCollectionPropertyMapping(role).ToColumns(roleAlias, "elements");
		}

		/// <summary>
		/// Essentially the same as GetElementType, but requiring that the
		/// element type be an association type.
		/// </summary>
		/// <param name="collectionType">The collection type to be checked.</param>
		/// <returns>The AssociationType of the elements of the collection.</returns>
		public IAssociationType GetElementAssociationType(CollectionType collectionType)
		{
			return (IAssociationType)GetElementType(collectionType);
		}

		/// <summary>
		/// Locate the collection persister by the collection role, requiring that
		/// such a persister exist.
		/// </summary>
		/// <param name="role">The collection role name.</param>
		/// <returns>The defined CollectionPersister for this collection role.</returns>
		public IQueryableCollection RequireQueryableCollection(string role)
		{
			try
			{
				IQueryableCollection queryableCollection = (IQueryableCollection)_sfi.GetCollectionPersister(role);
				if (queryableCollection != null)
				{
					_collectionPropertyMappingByRole.Add(role, new CollectionPropertyMapping(queryableCollection));
				}
				return queryableCollection;
			}
			catch (InvalidCastException cce)
			{
				throw new QueryException("collection role is not queryable: " + role, cce);
			}
			catch (Exception e)
			{
				throw new QueryException("collection role not found: " + role, e);
			}
		}

		/// <summary>
		/// Locate the persister by class or entity name, requiring that such a persister
		/// exist.
		/// </summary>
		/// <param name="name">The class or entity name</param>
		/// <returns>The defined persister for this entity</returns>
		public IEntityPersister RequireClassPersister(string name)
		{
			IEntityPersister cp;
			try 
			{
				cp = FindEntityPersisterByName( name );
				if ( cp == null ) 
				{
					throw new QuerySyntaxException( name + " is not mapped" );
				}
			}
			catch ( MappingException e ) 
			{
				throw new QueryException( e.Message, e );
			}
			return cp;
		}

		/// <summary>
		/// Given a (potentially unqualified) class name, locate its persister.
		/// </summary>
		/// <param name="className">The (potentially unqualified) class name.</param>
		/// <returns>The defined persister for this class, or null if none found.</returns>
		public IQueryable FindQueryableUsingImports(string className)
		{
			return FindQueryableUsingImports(_sfi, className);
		}

		/// <summary>
		/// Given a (potentially unqualified) class name, locate its persister.
		/// </summary>
		/// <param name="sfi">The session factory implementor.</param>
		/// <param name="className">The (potentially unqualified) class name.</param>
		/// <returns>The defined persister for this class, or null if none found.</returns>
		private static IQueryable FindQueryableUsingImports(ISessionFactoryImplementor sfi, string className) 
		{
			return new SessionFactoryHelper(sfi).FindQueryableUsingImports(className);
		}

		/// <summary>
		/// Locate the persister by class or entity name.
		/// </summary>
		/// <param name="name">The class or entity name</param>
		/// <returns>The defined persister for this entity, or null if none found.</returns>
		private IEntityPersister FindEntityPersisterByName(string name)
		{
			return helper.FindEntityPersisterUsingImports(name);
		}

		/// <summary>
		/// Create a join sequence rooted at the given collection.
		/// </summary>
		/// <param name="collPersister">The persister for the collection at which the join should be rooted.</param>
		/// <param name="collectionName">The alias to use for qualifying column references.</param>
		/// <returns>The generated join sequence.</returns>
		public JoinSequence CreateCollectionJoinSequence(IQueryableCollection collPersister, String collectionName)
		{
			JoinSequence joinSequence = CreateJoinSequence();
			joinSequence.SetRoot(collPersister, collectionName);
			joinSequence.SetUseThetaStyle(true);		// TODO: figure out how this should be set.

			///////////////////////////////////////////////////////////////////////////////
			// This was the reason for failures regarding INDEX_OP and subclass joins on
			// theta-join dialects; not sure what behaviour we were trying to emulate ;)
			//		joinSequence = joinSequence.getFromPart();	// Emulate the old addFromOnly behavior.
			return joinSequence;
		}

		/// <summary>
		/// Generate an empty join sequence instance.
		/// </summary>
		/// <returns>The generated join sequence.</returns>
		public JoinSequence CreateJoinSequence()
		{
			return new JoinSequence(_sfi);
		}

		/// <summary>
		/// Generate a join sequence representing the given association type.
		/// </summary>
		/// <param name="implicitJoin">Should implicit joins (theta-style) or explicit joins (ANSI-style) be rendered</param>
		/// <param name="associationType">The type representing the thing to be joined into.</param>
		/// <param name="tableAlias">The table alias to use in qualifying the join conditions</param>
		/// <param name="joinType">The type of join to render (inner, outer, etc)</param>
		/// <param name="columns">The columns making up the condition of the join.</param>
		/// <returns>The generated join sequence.</returns>
		public JoinSequence CreateJoinSequence(bool implicitJoin, IAssociationType associationType, string tableAlias, JoinType joinType, string[] columns) 
		{
			JoinSequence joinSequence = CreateJoinSequence();
			joinSequence.SetUseThetaStyle(implicitJoin);	// Implicit joins use theta style (WHERE pk = fk), explicit joins use JOIN (after from)
			joinSequence.AddJoin( associationType, tableAlias, joinType, columns );
			return joinSequence;
		}

		public String[][] GenerateColumnNames(IType[] sqlResultTypes)
		{
			return NameGenerator.GenerateColumnNames(sqlResultTypes, _sfi);
		}

		public bool IsStrictJPAQLComplianceEnabled
		{
			get
			{
				// TODO - is this required at all?
				//return _sfi.Settings.isStrictJPAQLCompliance();
				return false;
			}
		}

		/// <summary>
		/// Retrieve a PropertyMapping describing the given collection role.
		/// </summary>
		/// <param name="role">The collection role for which to retrieve the property mapping.</param>
		/// <returns>The property mapping.</returns>
		private IPropertyMapping GetCollectionPropertyMapping(string role)
		{
			return _collectionPropertyMappingByRole[role];
		}

		/// <summary>
		/// Given a collection type, determine the Type representing elements
		/// within instances of that collection.
		/// </summary>
		/// <param name="collectionType">The collection type to be checked.</param>
		/// <returns>The Type of the elements of the collection.</returns>
		private IType GetElementType(CollectionType collectionType)
		{
			return collectionType.GetElementType(_sfi);
		}
	}
}
