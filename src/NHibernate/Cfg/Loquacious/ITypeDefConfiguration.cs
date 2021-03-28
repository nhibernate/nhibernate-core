using System;

namespace NHibernate.Cfg.Loquacious
{
	/// <summary>
	/// Properties of TypeDef configuration.
	/// </summary>
	/// <seealso cref="ConfigurationExtensions.TypeDefinition{TDef}"/>
	//Since 5.3
	[Obsolete("Replaced by direct class usage")]
	public interface ITypeDefConfigurationProperties
	{
		/// <summary>
		/// The key to use the type-definition inside not strongly typed mappings (XML mapping).
		/// </summary>
		string Alias { get; set; }

		/// <summary>
		/// An <see cref="object"/> which public properties are used as 
		/// type-definition pareneters or null where type-definition does not need parameters or you want use default values.
		/// </summary>
		/// <remarks>
		/// <example>
		/// As <paramref name="value"/> an anonimous object can be used:
		/// <code>
		///	configure.TypeDefinition&lt;TableHiLoGenerator&gt;(c=>
		///	                                             	{
		///	                                             		c.Alias = "HighLow";
		///	                                             		c.Properties = new {max_lo = 99};
		///	                                             	});
		/// </code>
		/// </example>
		/// </remarks>
		object Properties { get; set; }
	}

	/// <summary>
	/// Properties of TypeDef configuration.
	/// </summary>
	/// <seealso cref="ConfigurationExtensions.TypeDefinition{TDef}"/>
	public class TypeDefConfigurationProperties 
#pragma warning disable 618
		: ITypeDefConfigurationProperties
#pragma warning restore 618
	{
		internal static TypeDefConfigurationProperties Create<T>()
		{
			return new TypeDefConfigurationProperties {Alias = typeof(T).Name};
		}

		#region Implementation of ITypeDefConfigurationProperties

		public string Alias { get; set; }
		public object Properties { get; set; }

		#endregion
	}
}
