using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class CollectionIdMapper: ICollectionIdMapper
	{
		private readonly HbmCollectionId hbmId;
		private const string DefaultColumnName = "collection_key";
		private string autosetType;

		public CollectionIdMapper(HbmCollectionId hbmId)
		{
			this.hbmId = hbmId;
			this.hbmId.column1 = DefaultColumnName;
			Generator(Generators.Guid);
		}

		#region Implementation of IIdMapper

		public void Generator(IGeneratorDef generator)
		{
			Generator(generator, x => { });
		}

		public void Generator(IGeneratorDef generator, Action<IGeneratorMapper> generatorMapping)
		{
			if (generator == null)
			{
				return;
			}
			if(!generator.SupportedAsCollectionElementId)
			{
				throw new NotSupportedException("The generator '" + generator.Class + "' cannot be used as collection element id.");
			}
			ApplyGenerator(generator);
			generatorMapping(new GeneratorMapper(hbmId.generator));
		}

		public void Type(IIdentifierType persistentType)
		{
			if (persistentType != null)
			{
				hbmId.type = persistentType.Name;
			}
		}

		public void Column(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return;
			}
			hbmId.column1 = name;
		}

		public void Length(int length)
		{
			hbmId.length = length.ToString();
		}

		private void ApplyGenerator(IGeneratorDef generator)
		{
			var hbmGenerator = new HbmGenerator { @class = generator.Class };
			object generatorParameters = generator.Params;
			if (generatorParameters != null)
			{
				hbmGenerator.param = (from pi in generatorParameters.GetType().GetProperties()
															let pname = pi.Name
															let pvalue = pi.GetValue(generatorParameters, null)
															select
																new HbmParam { name = pname, Text = new[] { ReferenceEquals(pvalue, null) ? "null" : pvalue.ToString() } }).
					ToArray();
			}
			else
			{
				hbmGenerator.param = null;
			}
			hbmId.generator = hbmGenerator;
			AutosetTypeThroughGeneratorDef(generator);
		}

		#endregion

		private void AutosetTypeThroughGeneratorDef(IGeneratorDef generator)
		{
			if (Equals(hbmId.type, autosetType) && generator.DefaultReturnType != null)
			{
				autosetType = generator.DefaultReturnType.GetNhTypeName();
				hbmId.type = autosetType;
			}
		}
	}
}