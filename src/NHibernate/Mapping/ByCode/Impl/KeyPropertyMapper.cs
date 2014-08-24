using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace NHibernate.Mapping.ByCode.Impl
{
	/// <summary>
	/// Manage the mapping of a HbmKeyProperty but implementing <see cref="IPropertyMapper"/>
	/// instead a more limitated KeyProperty.
	/// </summary>
	public class KeyPropertyMapper : IPropertyMapper
	{
		private readonly IAccessorPropertyMapper entityPropertyMapper;
		private readonly MemberInfo member;
		private readonly HbmKeyProperty propertyMapping;

		public KeyPropertyMapper(MemberInfo member, HbmKeyProperty propertyMapping)
		{
			if (propertyMapping == null)
			{
				throw new ArgumentNullException("propertyMapping");
			}
			this.member = member;
			this.propertyMapping = propertyMapping;
			if (member == null)
			{
				this.propertyMapping.access = "none";
			}
			if (member == null)
			{
				entityPropertyMapper = new NoMemberPropertyMapper();
			}
			else
			{
				entityPropertyMapper = new AccessorPropertyMapper(member.DeclaringType, member.Name, x => propertyMapping.access = x);
			}
		}

		#region Implementation of IEntityPropertyMapper

		public void Access(Accessor accessor)
		{
			entityPropertyMapper.Access(accessor);
		}

		public void Access(System.Type accessorType)
		{
			entityPropertyMapper.Access(accessorType);
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			// not supported by HbmKeyProperty
		}

		#endregion

		#region Implementation of IPropertyMapper

		public void Type(IType persistentType)
		{
			if (persistentType != null)
			{
				propertyMapping.type1 = persistentType.Name;
				propertyMapping.type = null;
			}
		}

		public void Type<TPersistentType>()
		{
			Type(typeof (TPersistentType), null);
		}

		public void Type<TPersistentType>(object parameters)
		{
			Type(typeof (TPersistentType), parameters);
		}

		public void Type(System.Type persistentType, object parameters)
		{
			if (persistentType == null)
			{
				throw new ArgumentNullException("persistentType");
			}
			if (!typeof (IUserType).IsAssignableFrom(persistentType) && !typeof (IType).IsAssignableFrom(persistentType))
			{
				throw new ArgumentOutOfRangeException("persistentType", "Expected type implementing IUserType or IType.");
			}
			if (parameters != null)
			{
				propertyMapping.type1 = null;
				var hbmType = new HbmType
				              {
				              	name = persistentType.AssemblyQualifiedName,
				              	param = (from pi in parameters.GetType().GetProperties()
				              	         let pname = pi.Name
				              	         let pvalue = pi.GetValue(parameters, null)
				              	         select
				              	         	new HbmParam {name = pname, Text = new[] {ReferenceEquals(pvalue, null) ? "null" : pvalue.ToString()}})
				              		.ToArray()
				              };
				propertyMapping.type = hbmType;
			}
			else
			{
				propertyMapping.type1 = persistentType.AssemblyQualifiedName;
				propertyMapping.type = null;
			}
		}

		public void Column(Action<IColumnMapper> columnMapper)
		{
			if (propertyMapping.Columns.Count() > 1)
			{
				throw new MappingException("Multi-columns property can't be mapped through singlr-column API.");
			}
			HbmColumn hbm = propertyMapping.Columns.SingleOrDefault();
			hbm = hbm
			      ??
			      new HbmColumn
			      {
			      	name = propertyMapping.column1,
			      	length = propertyMapping.length,
			      };
			string defaultColumnName = member.Name;
			columnMapper(new ColumnMapper(hbm, member != null ? defaultColumnName : "unnamedcolumn"));
			if (ColumnTagIsRequired(hbm))
			{
				propertyMapping.column = new[] {hbm};
				ResetColumnPlainValues();
			}
			else
			{
				propertyMapping.column1 = defaultColumnName == null || !defaultColumnName.Equals(hbm.name) ? hbm.name : null;
				propertyMapping.length = hbm.length;
			}
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			ResetColumnPlainValues();
			int i = 1;
			var columns = new List<HbmColumn>(columnMapper.Length);
			foreach (var action in columnMapper)
			{
				var hbm = new HbmColumn();
				string defaultColumnName = (member != null ? member.Name : "unnamedcolumn") + i++;
				action(new ColumnMapper(hbm, defaultColumnName));
				columns.Add(hbm);
			}
			propertyMapping.column = columns.ToArray();
		}

		public void Column(string name)
		{
			Column(x => x.Name(name));
		}

		public void Length(int length)
		{
			Column(x => x.Length(length));
		}

		public void Precision(short precision)
		{
			Column(x => x.Precision(precision));
		}

		public void Scale(short scale)
		{
			Column(x => x.Scale(scale));
		}

		public void NotNullable(bool notnull)
		{
			Column(x => x.NotNullable(notnull));
		}

		public void Unique(bool unique)
		{
			Column(x => x.Unique(unique));
		}

		public void UniqueKey(string uniquekeyName)
		{
			Column(x => x.UniqueKey(uniquekeyName));
		}

		public void Index(string indexName)
		{
			Column(x => x.Index(indexName));
		}

		public void Formula(string formula)
		{
			// formula is not supported by HbmKeyProperty
		}

		public void Update(bool consideredInUpdateQuery)
		{
			// update is not supported by HbmKeyProperty
		}

		public void Insert(bool consideredInInsertQuery)
		{
			// insert is not supported by HbmKeyProperty
		}

		public void Lazy(bool isLazy)
		{
			// lazy is not supported by HbmKeyProperty
		}

		public void Generated(PropertyGeneration generation)
		{
			// generated is not supported by HbmKeyProperty
		}

		private bool ColumnTagIsRequired(HbmColumn hbm)
		{
			return hbm.precision != null || hbm.scale != null || hbm.notnull || hbm.unique
			       || hbm.uniquekey != null || hbm.sqltype != null || hbm.index != null || hbm.@default != null
			       || hbm.check != null;
		}

		private void ResetColumnPlainValues()
		{
			propertyMapping.column1 = null;
			propertyMapping.length = null;
		}

		#endregion
	}
}