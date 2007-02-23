using System;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Engine
{
	public sealed class JoinHelper
	{
		private JoinHelper()
		{
		}

		/// <summary>
		/// Get the aliased columns of the owning entity which are to 
		/// be used in the join
		/// </summary>
		public static string[] GetAliasedLHSColumnNames(
			IAssociationType type,
			string alias,
			int property,
			IOuterJoinLoadable lhsPersister,
			IMapping mapping
			)
		{
			return GetAliasedLHSColumnNames(type, alias, property, 0, lhsPersister, mapping);
		}

		/// <summary>
		/// Get the columns of the owning entity which are to 
		/// be used in the join
		/// </summary>
		public static string[] GetLHSColumnNames(
			IAssociationType type,
			int property,
			IOuterJoinLoadable lhsPersister,
			IMapping mapping
			)
		{
			return GetLHSColumnNames(type, property, 0, lhsPersister, mapping);
		}

		/// <summary>
		/// Get the aliased columns of the owning entity which are to 
		/// be used in the join
		/// </summary>
		public static string[] GetAliasedLHSColumnNames(
			IAssociationType type,
			string alias,
			int property,
			int begin,
			IOuterJoinLoadable lhsPersister,
			IMapping mapping
			)
		{
			if (type.UseLHSPrimaryKey)
			{
				return StringHelper.Qualify(alias, lhsPersister.IdentifierColumnNames);
			}
			else
			{
				string propertyName = type.LHSPropertyName;
				if (propertyName == null)
				{
					return ArrayHelper.Slice(
						lhsPersister.ToColumns(alias, property),
						begin,
						type.GetColumnSpan(mapping)
						);
				}
				else
				{
					return ((IPropertyMapping) lhsPersister).ToColumns(alias, propertyName); //bad cast
				}
			}
		}

		/// <summary>
		/// Get the columns of the owning entity which are to 
		/// be used in the join
		/// </summary>
		public static string[] GetLHSColumnNames(
			IAssociationType type,
			int property,
			int begin,
			IOuterJoinLoadable lhsPersister,
			IMapping mapping
			)
		{
			if (type.UseLHSPrimaryKey)
			{
				//return lhsPersister.getSubclassPropertyColumnNames(property);
				return lhsPersister.IdentifierColumnNames;
			}
			else
			{
				string propertyName = type.LHSPropertyName;
				if (propertyName == null)
				{
					//slice, to get the columns for this component
					//property
					return ArrayHelper.Slice(
						lhsPersister.GetSubclassPropertyColumnNames(property),
						begin,
						type.GetColumnSpan(mapping)
						);
				}
				else
				{
					//property-refs for associations defined on a
					//component are not supported, so no need to slice
					return lhsPersister.GetPropertyColumnNames(propertyName);
				}
			}
		}

		public static string GetLHSTableName(
			IAssociationType type,
			int property,
			IOuterJoinLoadable lhsPersister
			)
		{
			if (type.UseLHSPrimaryKey)
			{
				return lhsPersister.TableName;
			}
			else
			{
				string propertyName = type.LHSPropertyName;
				if (propertyName == null)
				{
					//if there is no property-ref, assume the join
					//is to the subclass table (ie. the table of the
					//subclass that the association belongs to)
					return lhsPersister.GetSubclassPropertyTableName(property);
				}
				else
				{
					//handle a property-ref
					string propertyRefTable = lhsPersister.GetPropertyTableName(propertyName);
					if (propertyRefTable == null)
					{
						//it is possible that the tree-walking in OuterJoinLoader can get to
						//an association defined by a subclass, in which case the property-ref
						//might refer to a property defined on a subclass of the current class
						//in this case, the table name is not known - this temporary solution 
						//assumes that the property-ref refers to a property of the subclass
						//table that the association belongs to (a reasonable guess)
						//TODO: fix this, add: IOuterJoinLoadable.getSubclassPropertyTableName(string propertyName)
						propertyRefTable = lhsPersister.GetSubclassPropertyTableName(property);
					}
					return propertyRefTable;
				}
			}
		}

		/// <summary>
		/// Get the columns of the associated table which are to 
		/// be used in the join
		/// </summary>
		public static string[] GetRHSColumnNames(IAssociationType type, ISessionFactoryImplementor factory)
		{
			string uniqueKeyPropertyName = type.RHSUniqueKeyPropertyName;
			IJoinable joinable = type.GetAssociatedJoinable(factory);
			if (uniqueKeyPropertyName == null)
			{
				return joinable.KeyColumnNames;
			}
			else
			{
				return ((IOuterJoinLoadable) joinable).GetPropertyColumnNames(uniqueKeyPropertyName);
			}
		}
	}
}