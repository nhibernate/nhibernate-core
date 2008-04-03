using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	[Serializable]
	public abstract class AbstractEmptinessExpression : ICriterion
	{
		private readonly TypedValue[] NO_VALUES = new TypedValue[0];
		private readonly string propertyName;

		protected abstract bool ExcludeEmpty { get; }


		protected AbstractEmptinessExpression(string propertyName)
		{
			this.propertyName = propertyName;
		}

		public TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return NO_VALUES;
		}

		public abstract IProjection[] GetProjections();

		public override sealed string ToString()
		{
			return propertyName + (ExcludeEmpty ? " is not empty" : " is empty");
		}

		public SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			string entityName = criteriaQuery.GetEntityName(criteria, propertyName);
			string actualPropertyName = criteriaQuery.GetPropertyName(propertyName);
			string sqlAlias = criteriaQuery.GetSQLAlias(criteria, propertyName);

			ISessionFactoryImplementor factory = criteriaQuery.Factory;
			IQueryableCollection collectionPersister = GetQueryableCollection(entityName, actualPropertyName, factory);

			string[] collectionKeys = collectionPersister.KeyColumnNames;
			string[] ownerKeys = ((ILoadable)factory.GetEntityPersister(entityName)).IdentifierColumnNames;

			StringBuilder innerSelect = new StringBuilder();
			innerSelect.Append("(select 1 from ")
				.Append(collectionPersister.TableName)
				.Append(" where ")
				.Append(
				new ConditionalFragment().SetTableAlias(sqlAlias).SetCondition(ownerKeys, collectionKeys).ToSqlStringFragment());
			if (collectionPersister.HasWhere)
			{
				innerSelect.Append(" and (")
					.Append(collectionPersister.GetSQLWhereString(collectionPersister.TableName))
					.Append(") ");
			}

			innerSelect.Append(")");

			return new SqlString(new string[] {ExcludeEmpty ? "exists" : "not exists", innerSelect.ToString()});
		}


		protected IQueryableCollection GetQueryableCollection(string entityName, string actualPropertyName,
		                                                      ISessionFactoryImplementor factory)
		{
			IPropertyMapping ownerMapping = (IPropertyMapping) factory.GetEntityPersister(entityName);
			IType type = ownerMapping.ToType(actualPropertyName);
			if (!type.IsCollectionType)
			{
				throw new MappingException(
					"Property path [" + entityName + "." + actualPropertyName + "] does not reference a collection"
					);
			}

			string role = ((CollectionType) type).Role;
			try
			{
				return (IQueryableCollection) factory.GetCollectionPersister(role);
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

		#region Operator Overloading

		public static AbstractCriterion operator &(AbstractEmptinessExpression lhs, AbstractEmptinessExpression rhs)
		{
			return new AndExpression(lhs, rhs);
		}

		public static AbstractCriterion operator |(AbstractEmptinessExpression lhs, AbstractEmptinessExpression rhs)
		{
			return new OrExpression(lhs, rhs);
		}


		public static AbstractCriterion operator &(AbstractEmptinessExpression lhs, AbstractCriterion rhs)
		{
			return new AndExpression(lhs, rhs);
		}

		public static AbstractCriterion operator |(AbstractEmptinessExpression lhs, AbstractCriterion rhs)
		{
			return new OrExpression(lhs, rhs);
		}


		public static AbstractCriterion operator !(AbstractEmptinessExpression crit)
		{
			return new NotExpression(crit);
		}

		/// <summary>
		/// See here for details:
		/// http://steve.emxsoftware.com/NET/Overloading+the++and++operators
		/// </summary>
		public static bool operator false(AbstractEmptinessExpression criteria)
		{
			return false;
		}

		/// <summary>
		/// See here for details:
		/// http://steve.emxsoftware.com/NET/Overloading+the++and++operators
		/// </summary>
		public static bool operator true(AbstractEmptinessExpression criteria)
		{
			return false;
		}

		#endregion
	}
}
