
namespace NHibernate.Cfg
{
    public enum MultiTenancyStrategy
    {
        /// <summary>
        /// Multi-tenancy implemented by use of discriminator columns (i.e. tenantId)
        /// </summary>
        Discriminator,

        /// <summary>
        /// Tenant per-schema
        /// </summary>
        Schema,

        /// <summary>
        /// Tenant per-database
        /// </summary>
        Database,

        /// <summary>
        /// No multi-tenancy (default)
        /// </summary>
        None
    }

    public static class MultiTenancyStrategyParser
    {
        public const string DiscriminatorXmlName = "discriminator";
        public const string SchemaXmlName = "per-schema";
        public const string DatabaseXmlName = "per-database";
        public const string NoneXmlName = "none";

        public static MultiTenancyStrategy Convert(string value)
        {
            switch (value)
            {
                case NoneXmlName:
                    return MultiTenancyStrategy.None;
                case SchemaXmlName:
                    throw new HibernateException("multi-tenancy strategies are not yet supported");
                case DatabaseXmlName:
                    throw new HibernateException("multi-tenancy strategies are not yet supported");
                case DiscriminatorXmlName:
                    throw new HibernateException("multi-tenancy strategies are not yet supported");
                default:
                    throw new HibernateException(string.Format("unknown multi-tenancy strategy: {0}", value));
            }
        }
    }
}
