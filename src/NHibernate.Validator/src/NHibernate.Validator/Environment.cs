namespace NHibernate.Validator
{
	/// <summary>
	/// Hibernate Validator Event properties
	/// The properties are retrieved from Hibernate
	/// (hibernate.properties, hibernate.cfg.xml, persistence.xml or Configuration API)
	/// </summary>
	public class Environment
	{
		/// <summary>
		/// Apply DDL changes on Hibernate metamodel when using validator with Hibernate Annotations. Default to true.
		/// </summary>
		public static readonly string APPLY_TO_DDL = "hibernate.validator.apply_to_ddl";
		
		/// <summary>
		/// Enable listeners auto registration in Hibernate Annotations and EntityManager. Default to true.
		/// </summary>
		public static readonly string AUTOREGISTER_LISTENERS = "hibernate.validator.autoregister_listeners";

		/// <summary>
		/// Message interpolator class used. The same instance is shared across all ClassValidators 
		/// </summary>
		public static readonly string MESSAGE_INTERPOLATOR_CLASS = "hibernate.validator.message_interpolator_class";
	}
}