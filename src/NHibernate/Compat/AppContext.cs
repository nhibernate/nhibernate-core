#if NET_4_0
namespace System
{
	internal static class AppContext
	{
		/// <summary>Gets the pathname of the base directory that the assembly resolver uses to probe for assemblies. </summary>
		public static string BaseDirectory
		{
			get { return AppDomain.CurrentDomain.BaseDirectory; }
		}
	}
}
#endif
