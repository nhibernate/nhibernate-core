using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class VersionMapper : IVersionMapper
	{
		private readonly IAccessorPropertyMapper entityPropertyMapper;
		private readonly MemberInfo member;
		private readonly HbmVersion versionMapping;

		public VersionMapper(MemberInfo member, HbmVersion hbmVersion)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			if (hbmVersion == null)
			{
				throw new ArgumentNullException("hbmVersion");
			}
			this.member = member;
			versionMapping = hbmVersion;
			versionMapping.name = member.Name;
			entityPropertyMapper = new AccessorPropertyMapper(member.DeclaringType, member.Name, x => versionMapping.access = x);
		}

		#region Implementation of IColumnsMapper

		public void Column(Action<IColumnMapper> columnMapper)
		{
			if (versionMapping.Columns.Count() > 1)
			{
				throw new MappingException("Multi-columns property can't be mapped through single-column API.");
			}
			HbmColumn hbm = versionMapping.Columns.SingleOrDefault();
			hbm = hbm
			      ??
			      new HbmColumn
			      {
			      	name = versionMapping.column1
			      };
			string defaultColumnName = member.Name;
			columnMapper(new ColumnMapper(hbm, member != null ? defaultColumnName : "unnamedcolumn"));
			if (ColumnTagIsRequired(hbm))
			{
				versionMapping.column = new[] {hbm};
				ResetColumnPlainValues();
			}
			else
			{
				versionMapping.column1 = defaultColumnName == null || !defaultColumnName.Equals(hbm.name) ? hbm.name : null;
			}
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			versionMapping.column1 = null;
			int i = 1;
			var columns = new List<HbmColumn>(columnMapper.Length);
			foreach (var action in columnMapper)
			{
				var hbm = new HbmColumn();
				string defaultColumnName = (member != null ? member.Name : "unnamedcolumn") + i++;
				action(new ColumnMapper(hbm, defaultColumnName));
				columns.Add(hbm);
			}
			versionMapping.column = columns.ToArray();
		}

		public void Column(string name)
		{
			Column(x => x.Name(name));
		}

		private bool ColumnTagIsRequired(HbmColumn hbm)
		{
			return hbm.length != null || hbm.precision != null || hbm.scale != null || hbm.notnull || hbm.unique
			       || hbm.uniquekey != null || hbm.sqltype != null || hbm.index != null || hbm.@default != null
			       || hbm.check != null;
		}

		private void ResetColumnPlainValues()
		{
			versionMapping.column1 = null;
		}

		#endregion

		#region Implementation of IVersionMapper

		public void Type(IVersionType persistentType)
		{
			if (persistentType != null)
			{
				versionMapping.type = persistentType.Name;
			}
		}

		public void Type<TPersistentType>() where TPersistentType : IUserVersionType
		{
			Type(typeof (TPersistentType));
		}

		public void Type(System.Type persistentType)
		{
			if (persistentType == null)
			{
				throw new ArgumentNullException("persistentType");
			}
			if (!typeof (IUserVersionType).IsAssignableFrom(persistentType))
			{
				throw new ArgumentOutOfRangeException("persistentType", "Expected type implementing IUserVersionType");
			}
			versionMapping.type = persistentType.AssemblyQualifiedName;
		}

		public void UnsavedValue(object value)
		{
			versionMapping.unsavedvalue = value != null ? value.ToString() : "null";
		}

		public void Insert(bool useInInsert)
		{
			versionMapping.insert = useInInsert;
			versionMapping.insertSpecified = !useInInsert;
		}

		public void Generated(VersionGeneration generatedByDb)
		{
			if (generatedByDb != null)
			{
				versionMapping.generated = generatedByDb.ToHbm();
			}
		}

		#endregion

		#region Implementation of IAccessorPropertyMapper

		public void Access(Accessor accessor)
		{
			entityPropertyMapper.Access(accessor);
		}

		public void Access(System.Type accessorType)
		{
			entityPropertyMapper.Access(accessorType);
		}

		#endregion
	}
}