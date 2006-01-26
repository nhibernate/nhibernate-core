using System;

namespace Refly.CodeDom
{
	/// <summary>
	/// Different possible output types
	/// </summary>
	public enum ClassOutputType
	{
		/// <summary>
		/// Generates a class
		/// </summary>
		Class,
		/// <summary>
		/// Generates a struct
		/// </summary>
		Struct,
		/// <summary>
		/// Generates a class and it's interface
		/// </summary>
		ClassAndInterface,
		/// <summary>
		/// Generates the interface only
		/// </summary>
		Interface
	}
}
