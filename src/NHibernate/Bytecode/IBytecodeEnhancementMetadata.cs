using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Engine;
using NHibernate.Intercept;
using NHibernate.Tuple.Entity;

namespace NHibernate.Bytecode
{
	/// <summary>
	/// Encapsulates bytecode enhancement information about a particular entity.
	/// 
	/// Author: Steve Ebersole
	/// </summary>
	public interface IBytecodeEnhancementMetadata
	{
		/// <summary>
		/// The name of the entity to which this metadata applies.
		/// </summary>
		string EntityName { get; }

		/// <summary>
		/// Has the entity class been bytecode enhanced for lazy loading?
		/// </summary>
		bool EnhancedForLazyLoading { get; }

		/// <summary>
		/// Has the information about all lazy properties
		/// </summary>
		LazyPropertiesMetadata LazyPropertiesMetadata { get; }

		/// <summary>
		/// Has the information about all properties mapped as lazy="no-proxy"
		/// </summary>
		UnwrapProxyPropertiesMetadata UnwrapProxyPropertiesMetadata { get; }

		/// <summary>
		/// Build and inject an interceptor instance into the enhanced entity.
		/// </summary>
		/// <param name="entity">The entity into which built interceptor should be injected.</param>
		/// <param name="lazyPropertiesAreUnfetched">Whether all lazy properties were unfetched or not.</param>
		/// <param name="session">The session to which the entity instance belongs.</param>
		/// <returns>The built and injected interceptor.</returns>
		// TODO: Remove lazyPropertiesAreUnfetched when interceptor will be injected on entity instantiation
		IFieldInterceptor InjectInterceptor(object entity, bool lazyPropertiesAreUnfetched, ISessionImplementor session);

		/// <summary>
		/// Extract the field interceptor instance from the enhanced entity.
		/// </summary>
		/// <param name="entity">The entity from which to extract the interceptor.</param>
		/// <returns>The extracted interceptor.</returns>
		IFieldInterceptor ExtractInterceptor(object entity);

		/// <summary>
		/// Retrieve the uninitialized lazy properties from the enhanced entity.
		/// </summary>
		/// <param name="entity">The entity from which to retrieve the uninitialized lazy properties.</param>
		/// <returns></returns>
		ISet<string> GetUninitializedLazyProperties(object entity);

		/// <summary>
		/// Retrieve the uninitialized lazy properties from the entity state.
		/// </summary>
		/// <param name="entityState">The entity state from which to retrieve the uninitialized lazy properties.</param>
		/// <returns></returns>
		ISet<string> GetUninitializedLazyProperties(object[] entityState);
	}
}
