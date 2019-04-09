using System;
using System.Data.Common;
using NHibernate.Connection;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.MultiTenancy
{
	/// <summary>
	/// Base implementation for <seealso cref="MultiTenancyStrategy.Database"/> multi-tenancy strategy
	/// </summary>
	[Serializable]
	public abstract partial class AbstractMultiTenantConnectionProvider : IMultiTenantConnectionProvider
	{
		/// <summary>
		/// Tenant connection string
		/// </summary>
		protected abstract string TenantConnectionString { get; }

		/// <inheritdoc />
		public abstract string TenantIdentifier { get; }

		/// <inheritdoc />
		public IConnectionAccess GetConnectionAccess()
		{
			//TODO 6.0: Remove check
			ReflectHelper.CastOrThrow<ConnectionProvider>(SessionFactory.ConnectionProvider, $"multi-tenancy. For custom connection provider please implement IMultiTenantConnectionProvider directly for '{GetType().Name}'  and do not use {nameof(AbstractMultiTenantConnectionProvider)} as base class."); 
			return new ContextualConnectionAccess(TenantConnectionString, SessionFactory);
		}

		protected  abstract ISessionFactoryImplementor SessionFactory { get; }

		[Serializable]
		partial class ContextualConnectionAccess : IConnectionAccess
		{
			private readonly ISessionFactoryImplementor _factory;

			public ContextualConnectionAccess(string connectionString, ISessionFactoryImplementor factory)
			{
				_factory = factory;
				ConnectionString = connectionString;
			}

			public DbConnection GetConnection()
			{
				return _factory.ConnectionProvider.GetConnection(ConnectionString);
			}

			public void CloseConnection(DbConnection connection)
			{
				_factory.ConnectionProvider.CloseConnection(connection);
			}

			public string ConnectionString { get; }
		}  
	}
}
