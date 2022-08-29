using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class NaturalIdMapper : AbstractBasePropertyContainerMapper, INaturalIdMapper
	{
		private readonly HbmClass classMapping;
		private readonly HbmNaturalId naturalIdmapping;

		public NaturalIdMapper(System.Type rootClass, HbmClass classMapping, HbmMapping mapDoc) : base(rootClass, mapDoc)
		{
			if (classMapping == null)
			{
				throw new ArgumentNullException("classMapping");
			}
			this.classMapping = classMapping;
			naturalIdmapping = new HbmNaturalId();
		}

		#region Overrides of AbstractBasePropertyContainerMapper

		protected override void AddProperty(object property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			if (classMapping.naturalid == null)
			{
				classMapping.naturalid = naturalIdmapping;
			}

			naturalIdmapping.Items = ArrayHelper.Append(naturalIdmapping.Items, property);
		}

		#endregion

		#region Implementation of INaturalIdAttributesMapper

		public void Mutable(bool isMutable)
		{
			naturalIdmapping.mutable = isMutable;
		}

		#endregion
	}
}
