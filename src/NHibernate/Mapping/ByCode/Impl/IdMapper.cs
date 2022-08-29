using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;
using NHibernate.UserTypes;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class IdMapper : IIdMapper
	{
		private readonly IAccessorPropertyMapper accessorMapper;
		private readonly MemberInfo member;
		private readonly HbmId hbmId;

		public IdMapper(HbmId hbmId)
			: this(null, hbmId) { }

		public IdMapper(MemberInfo member, HbmId hbmId)
		{
			this.member = member;
			this.hbmId = hbmId;
			if (member != null)
			{
				System.Type idType = member.GetPropertyOrFieldType();
				hbmId.name = member.Name;
				hbmId.type1 = idType.GetNhTypeName();
				accessorMapper = new AccessorPropertyMapper(member.DeclaringType, member.Name, x => hbmId.access = x);
			}
			else
			{
				hbmId.type1 = typeof(int).GetNhTypeName();
				accessorMapper = new NoMemberPropertyMapper();
			}
		}

		#region Implementation of IIdMapper

		public void Generator(IGeneratorDef generator)
		{
			Generator(generator, x => { });
		}

		public void Generator(IGeneratorDef generator, Action<IGeneratorMapper> generatorMapping)
		{
			ApplyGenerator(generator);
			generatorMapping(new GeneratorMapper(hbmId.generator));
		}

		public void Access(Accessor accessor)
		{
			accessorMapper.Access(accessor);
		}

		public void Access(System.Type accessorType)
		{
			accessorMapper.Access(accessorType);
		}

		public void Type(IIdentifierType persistentType)
		{
			if (persistentType != null)
			{
				hbmId.type1 = persistentType.Name;
				hbmId.type = null;
			}
		}

		public void Type(System.Type persistentType, object parameters)
		{
			if (persistentType == null)
			{
				throw new ArgumentNullException(nameof(persistentType));
			}
			if (!typeof(IUserType).IsAssignableFrom(persistentType) && !typeof(IType).IsAssignableFrom(persistentType) && !typeof(ICompositeUserType).IsAssignableFrom(persistentType))
			{
				throw new ArgumentOutOfRangeException(nameof(persistentType), "Expected type implementing IUserType, ICompositeUserType or IType.");
			}
			if (parameters != null)
			{
				hbmId.type1 = null;
				var hbmType = new HbmType
				{
					name = persistentType.AssemblyQualifiedName,
					param = parameters.GetType().GetProperties().ToArray(
							pi =>
							{
								object pvalue = pi.GetValue(parameters, null);
								return new HbmParam { name = pi.Name, Text = new[] { ReferenceEquals(pvalue, null) ? "null" : pvalue.ToString() } };
							})
				};
				hbmId.type = hbmType;
			}
			else
			{
				hbmId.type1 = persistentType.AssemblyQualifiedName;
				hbmId.type = null;
			}
		}

		public void UnsavedValue(object value)
		{
			hbmId.unsavedvalue = value != null ? value.ToString() : "null";
		}

		public void Column(Action<IColumnMapper> columnMapper)
		{
			if (hbmId.Columns.Count() > 1)
				throw new MappingException("Multi-columns property can't be mapped through singlr-column API.");

			HbmColumn hbm = hbmId.Columns.SingleOrDefault() ?? new HbmColumn
			{
				name = hbmId.column1,
				length = hbmId.length
			};

			string defaultColumnName = member != null ? member.Name : null;
			columnMapper(new ColumnMapper(hbm, member != null ? defaultColumnName : "unnamedcolumn"));
			if (hbm.sqltype != null ||
				hbm.@default != null ||
				hbm.check != null ||
				hbm.precision != null ||
				hbm.scale != null ||
				hbm.notnullSpecified ||
				hbm.uniqueSpecified ||
				hbm.uniquekey != null ||
				hbm.index != null)
			{
				hbmId.column = new[] { hbm };
				ResetIdPlainValues();
			}
			else
			{
				hbmId.column1 = defaultColumnName == null || defaultColumnName != hbm.name ? hbm.name : null;
				hbmId.length = hbm.length;
			}
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			ResetIdPlainValues();
			int i = 1;
			var columns = new List<HbmColumn>(columnMapper.Length);
			foreach (var action in columnMapper)
			{
				var hbm = new HbmColumn();
				string defaultColumnName = (member != null ? member.Name : "unnamedcolumn") + i++;
				action(new ColumnMapper(hbm, defaultColumnName));
				columns.Add(hbm);
			}
			hbmId.column = columns.ToArray();
		}

		public void Column(string name)
		{
			Column(x => x.Name(name));
		}

		public void Length(int length)
		{
			Column(x => x.Length(length));
		}

		private void ResetIdPlainValues()
		{
			hbmId.column1 = null;
			hbmId.length = null;
		}

		private void ApplyGenerator(IGeneratorDef generator)
		{
			var hbmGenerator = new HbmGenerator { @class = generator.Class };
			if (hbmId.name == null)
			{
				// no member for the id
				var defaultReturnType = generator.DefaultReturnType;
				// where a DefaultReturnType is not available, let NH discover it during the mapping-binding process
				if (defaultReturnType != null)
				{
					hbmId.type1 = defaultReturnType.GetNhTypeName();
				}
			}
			object generatorParameters = generator.Params;
			if (generatorParameters != null)
			{
				hbmGenerator.param = generatorParameters.GetType().GetProperties().ToArray(
					pi =>
					{
						var pvalue = pi.GetValue(generatorParameters, null);
						return
							new HbmParam { name = pi.Name, Text = new[] { ReferenceEquals(pvalue, null) ? "null" : pvalue.ToString() } };
					});
			}
			else
			{
				hbmGenerator.param = null;
			}
			hbmId.generator = hbmGenerator;
		}

		#endregion

		#region Nested type: NoMemberPropertyMapper

		private class NoMemberPropertyMapper : IAccessorPropertyMapper
		{
			#region IAccessorPropertyMapper Members

			public void Access(Accessor accessor) { }

			public void Access(System.Type accessorType) { }

			#endregion
		}

		#endregion
	}
}
