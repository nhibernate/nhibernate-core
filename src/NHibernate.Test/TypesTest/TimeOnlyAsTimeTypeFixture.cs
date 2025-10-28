#if NET6_0_OR_GREATER
using NHibernate.Mapping.ByCode;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	[TestFixture(null, false)]
	[TestFixture("1900-01-01", false)]
	[TestFixture(null, true)]
	public class TimeOnlyAsTimeTypeFixture : AbstractTimeOnlyTypeWithScaleFixture<TimeOnlyAsTimeType>
	{
		private readonly string _baseDate;

		public TimeOnlyAsTimeTypeFixture(string baseDate, bool setMaxScale) : base(setMaxScale)
		{
			_baseDate = baseDate;
		}

		protected override void ConfigurePropertyMapping<TPersistentType>(IPropertyMapper propertyMapper)
		{
			base.ConfigurePropertyMapping<TPersistentType>(propertyMapper);
			if (_baseDate != null)
			{
				propertyMapper.Type<TPersistentType>(new { BaseDateValue = _baseDate });
			}
		}
	}
}
#endif
