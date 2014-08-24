using System;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class IdMapper : IIdMapper
	{
		private readonly IAccessorPropertyMapper accessorMapper;
		private readonly HbmId hbmId;

		public IdMapper(HbmId hbmId)
			: this(null, hbmId) {}

		public IdMapper(MemberInfo member, HbmId hbmId)
		{
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

		public void UnsavedValue(object value)
		{
			hbmId.unsavedvalue = value != null ? value.ToString() : "null";
		}

		public void Column(string name)
		{
			hbmId.column1 = name;
		}

		public void Length(int length)
		{
			hbmId.length = length.ToString();
		}

		private void ApplyGenerator(IGeneratorDef generator)
		{
			var hbmGenerator = new HbmGenerator {@class = generator.Class};
			if(hbmId.name == null)
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
				hbmGenerator.param = (from pi in generatorParameters.GetType().GetProperties()
									  let pname = pi.Name
									  let pvalue = pi.GetValue(generatorParameters, null)
									  select
										new HbmParam {name = pname, Text = new[] {ReferenceEquals(pvalue, null) ? "null" : pvalue.ToString()}}).
					ToArray();
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

			public void Access(Accessor accessor) {}

			public void Access(System.Type accessorType) {}

			#endregion
		}

		#endregion
	}
}