using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Bytecode;
using NHibernate.Engine;
using NHibernate.Intercept;
using NHibernate.Util;

namespace NHibernate.Tuple.Entity
{
	/// <summary>
	/// Author: Steve Ebersole
	/// </summary>
	public class BytecodeEnhancementMetadataNonPocoImpl : IBytecodeEnhancementMetadata
	{
		private readonly string _errorMessage;

		public BytecodeEnhancementMetadataNonPocoImpl(string entityName)
		{
			EntityName = entityName;
			LazyPropertiesMetadata = LazyPropertiesMetadata.NonEnhanced(entityName);
			UnwrapProxyPropertiesMetadata = UnwrapProxyPropertiesMetadata.NonEnhanced(entityName);
			_errorMessage = $"Entity [{entityName}] is non-poco, and therefore not instrumented";
		}

		/// <inheritdoc />
		public string EntityName { get; }

		/// <inheritdoc />
		public bool EnhancedForLazyLoading => false;

		/// <inheritdoc />
		public LazyPropertiesMetadata LazyPropertiesMetadata { get; }

		/// <inheritdoc />
		public UnwrapProxyPropertiesMetadata UnwrapProxyPropertiesMetadata { get; }

		/// <inheritdoc />
		public IFieldInterceptor InjectInterceptor(object entity, bool lazyPropertiesAreUnfetched, ISessionImplementor session)
		{
			throw new NotInstrumentedException(_errorMessage);
		}

		/// <inheritdoc />
		public IFieldInterceptor ExtractInterceptor(object entity)
		{
			throw new NotInstrumentedException(_errorMessage);
		}

		/// <inheritdoc />
		public ISet<string> GetUninitializedLazyProperties(object entity)
		{
			return CollectionHelper.EmptySet<string>();
		}

		/// <inheritdoc />
		public ISet<string> GetUninitializedLazyProperties(object[] entityState)
		{
			return CollectionHelper.EmptySet<string>();
		}
	}
}
