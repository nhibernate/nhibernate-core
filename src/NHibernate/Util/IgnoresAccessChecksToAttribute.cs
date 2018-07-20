// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class IgnoresAccessChecksToAttribute : Attribute
	{
		public string AssemblyName { get; }

		public IgnoresAccessChecksToAttribute(string assemblyName)
		{
			AssemblyName = assemblyName;
		}
	}
}
