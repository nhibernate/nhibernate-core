#if NETSTANDARD
// ReSharper disable once CheckNamespace
namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	internal class DesignerCategoryAttribute : Attribute
	{
		public DesignerCategoryAttribute(string category)
		{
		}
	}
}
#endif
