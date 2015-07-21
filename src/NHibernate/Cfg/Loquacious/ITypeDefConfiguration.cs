namespace NHibernate.Cfg.Loquacious
{
	/// <summary>
	/// Properties of TypeDef configuration.
	/// </summary>
	/// <seealso cref="ConfigurationExtensions.TypeDefinition{TDef}"/>
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

	internal class TypeDefConfigurationProperties<T> : ITypeDefConfigurationProperties
		where T: class
	{
		public TypeDefConfigurationProperties()
		{
			Alias = typeof(T).Name;
		}

		#region Implementation of ITypeDefConfigurationProperties

		public string Alias { get; set; }
		public object Properties { get; set; }

		#endregion
	}
}