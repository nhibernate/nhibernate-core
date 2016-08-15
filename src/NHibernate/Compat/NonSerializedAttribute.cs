#if !FEATURE_SERIALIZATION && NETSTANDARD
// ReSharper disable once CheckNamespace
namespace System
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	internal sealed class NonSerializedAttribute : Attribute
	{
	}
}
#endif
