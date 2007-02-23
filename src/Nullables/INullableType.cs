using System;

namespace Nullables
{
	public interface INullableType
	{
		object Value { get; }
		bool HasValue { get; }
	}
}