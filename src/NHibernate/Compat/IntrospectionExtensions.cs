#if FEATURE_LEGACY_REFLECTION_API
// ReSharper disable once CheckNamespace
namespace System.Reflection
{
	/// <summary>
	/// Contains methods for converting <see cref="T:System.Type" /> objects.
	/// This allows us to use the new Reflection API in .NET 4.5 back to .NET 4.0.
	/// Since this is the only way to access Reflection methods in .NET Core, we need to use it.
	/// </summary>
	internal static class IntrospectionExtensions
	{
		public static Type GetTypeInfo(this Type type)
		{
			return type;
		}
	}
}
#endif
