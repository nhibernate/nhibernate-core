using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Engine
{
	public static class JoinHelper
	{
		public static ILhsAssociationTypeSqlInfo GetLhsSqlInfo(string alias, int property,
																										IOuterJoinLoadable lhsPersister, IMapping mapping)
		{
			return new PropertiesLhsAssociationTypeSqlInfo(alias, property, lhsPersister, mapping);
		}

		public static ILhsAssociationTypeSqlInfo GetIdLhsSqlInfo(string alias, IOuterJoinLoadable lhsPersister, IMapping mapping)
		{
			return new IdPropertiesLhsAssociationTypeSqlInfo(alias, lhsPersister, mapping);
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
				return joinable.JoinColumnNames;
			}
			else
			{
				return ((IOuterJoinLoadable)joinable).GetPropertyColumnNames(uniqueKeyPropertyName);
			}
		}
	}

	public interface ILhsAssociationTypeSqlInfo
	{
		/// <summary>
		/// Get the aliased columns of the owning entity which are to 
		/// be used in the join
		/// </summary>
		string[] GetAliasedColumnNames(IAssociationType type, int begin);

		/// <summary>
		/// Get the columns of the owning entity which are to 
		/// be used in the join
		/// </summary>
		string[] GetColumnNames(IAssociationType type, int begin);

		string GetTableName(IAssociationType type);
	}

	public abstract class AbstractLhsAssociationTypeSqlInfo : ILhsAssociationTypeSqlInfo
	{
		protected AbstractLhsAssociationTypeSqlInfo(string @alias, IOuterJoinLoadable persister, IMapping mapping)
		{
			Alias = alias;
			Persister = persister;
			Mapping = mapping;
		}

		public string Alias { get; private set; }

		public IOuterJoinLoadable Persister { get; private set; }

		public IMapping Mapping { get; private set; }

		#region Implementation of ILhsAssociationTypeSqlInfo


		public string[] GetAliasedColumnNames(IAssociationType type, int begin)
		{
			if (type.UseLHSPrimaryKey)
			{
				return StringHelper.Qualify(Alias, Persister.IdentifierColumnNames);
			}
			else
			{
				string propertyName = type.LHSPropertyName;
				if (propertyName == null)
				{
					return ArrayHelper.Slice(GetAliasedColumns(), begin, type.GetColumnSpan(Mapping));
				}
				else
				{
					return ((IPropertyMapping)Persister).ToColumns(Alias, propertyName); //bad cast
				}
			}
		}

		public string[] GetColumnNames(IAssociationType type, int begin)
		{
			if (type.UseLHSPrimaryKey)
			{
				return Persister.IdentifierColumnNames;
			}
			else
			{
				string propertyName = type.LHSPropertyName;
				if (propertyName == null)
				{
					//slice, to get the columns for this component property
					return ArrayHelper.Slice(GetColumns(), begin, type.GetColumnSpan(Mapping));
				}
				else
				{
					//property-refs for associations defined on a
					//component are not supported, so no need to slice
					return Persister.GetPropertyColumnNames(propertyName);
				}
			}
		}

		protected abstract string[] GetAliasedColumns();
		protected abstract string[] GetColumns();

		public abstract string GetTableName(IAssociationType type);

		#endregion
	}

	public class PropertiesLhsAssociationTypeSqlInfo : AbstractLhsAssociationTypeSqlInfo
	{
		private readonly int propertyIdx;

		public PropertiesLhsAssociationTypeSqlInfo(string alias,
			int propertyIdx, IOuterJoinLoadable persister, IMapping mapping)
			: base(alias, persister, mapping)
		{
			this.propertyIdx = propertyIdx;
		}

		#region Overrides of AbstractLhsAssociationTypeSqlInfo

		protected override string[] GetAliasedColumns()
		{
			return Persister.ToColumns(Alias, propertyIdx);
		}

		protected override string[] GetColumns()
		{
			return Persister.GetSubclassPropertyColumnNames(propertyIdx);
		}

		public override string GetTableName(IAssociationType type)
		{
			if (type.UseLHSPrimaryKey)
			{
				return Persister.TableName;
			}
			else
			{
				string propertyName = type.LHSPropertyName;
				if (propertyName == null)
				{
					//if there is no property-ref, assume the join
					//is to the subclass table (ie. the table of the
					//subclass that the association belongs to)
					return Persister.GetSubclassPropertyTableName(propertyIdx);
				}
				else
				{
					//handle a property-ref
					string propertyRefTable = Persister.GetPropertyTableName(propertyName);
					if (propertyRefTable == null)
					{
						//it is possible that the tree-walking in OuterJoinLoader can get to
						//an association defined by a subclass, in which case the property-ref
						//might refer to a property defined on a subclass of the current class
						//in this case, the table name is not known - this temporary solution 
						//assumes that the property-ref refers to a property of the subclass
						//table that the association belongs to (a reasonable guess)
						//TODO: fix this, add: IOuterJoinLoadable.getSubclassPropertyTableName(string propertyName)
						propertyRefTable = Persister.GetSubclassPropertyTableName(propertyIdx);
					}
					return propertyRefTable;
				}
			}
		}

		#endregion
	}

	public class IdPropertiesLhsAssociationTypeSqlInfo : AbstractLhsAssociationTypeSqlInfo
	{
		public IdPropertiesLhsAssociationTypeSqlInfo(string alias, IOuterJoinLoadable persister, IMapping mapping) : base(alias, persister, mapping) {}

		#region Overrides of AbstractLhsAssociationTypeSqlInfo

		protected override string[] GetAliasedColumns()
		{
			return Persister.ToIdentifierColumns(Alias);
		}

		protected override string[] GetColumns()
		{
			return Persister.IdentifierColumnNames;
		}

		public override string GetTableName(IAssociationType type)
		{
			return Persister.TableName;
		}

		#endregion
	}

}