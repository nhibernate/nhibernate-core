#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;

using NHibernate.Mapping.ByCode;
using NHibernate.Type;

namespace NHibernate.Test.TypesTest
{
	public abstract class AbstractTimeOnlyTypeWithScaleFixture<TType> : AbstractTimeOnlyTypeFixture<TType> where TType : IType
	{
		private readonly bool _setMaxScale;
		protected AbstractTimeOnlyTypeWithScaleFixture(bool setMaxScale)
		{
			_setMaxScale = setMaxScale;
		}

		/// <summary>
		/// The resolution used when setMaxScale is true
		/// </summary>
		protected virtual long MaxTimestampResolutionInTicks => Dialect.TimestampResolutionInTicks;

		/// <summary>
		/// Add fractional seconds to the test values when setMaxScale is true
		/// </summary>
		protected override IReadOnlyList<TimeOnly> TestValues => [.. base.TestValues.Select(x => _setMaxScale ? AdjustTestValueWithFractionalSeconds(x) : x)];

		private TimeOnly AdjustTestValueWithFractionalSeconds(TimeOnly value)
		{
			value = new TimeOnly(value.Hour,value.Minute,value.Second);
			var ticks = value.Ticks + MaxTimestampResolutionInTicks;
			if (ticks + MaxTimestampResolutionInTicks > TimeOnly.MaxValue.Ticks)
			{
				ticks = value.Ticks - MaxTimestampResolutionInTicks;
			}
			return new TimeOnly(ticks);
		}

		protected override void ConfigurePropertyMapping<TPersistentType>(IPropertyMapper propertyMapper)
		{
			if (_setMaxScale)
			{
				propertyMapper.Scale((short) Math.Floor(Math.Log10(TimeSpan.TicksPerSecond / MaxTimestampResolutionInTicks)));
			}
		}
	}
}
#endif
