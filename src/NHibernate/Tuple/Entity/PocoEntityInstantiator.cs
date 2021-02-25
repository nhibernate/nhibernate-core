using System;
using NHibernate.Bytecode;
using NHibernate.Mapping;
using NHibernate.Proxy;

namespace NHibernate.Tuple.Entity
{
	/// <summary> Defines a POCO-based instantiator for use from the <see cref="PocoEntityTuplizer"/>.</summary>
	[Serializable]
	public class PocoEntityInstantiator : PocoInstantiator
	{
		private readonly EntityMetamodel _entityMetamodel;
		private readonly System.Type _proxyInterface;
		private readonly bool _enhancedForLazyLoading;
		private readonly IProxyFactory _proxyFactory;

		public PocoEntityInstantiator(
			EntityMetamodel entityMetamodel,
			PersistentClass persistentClass,
			IInstantiationOptimizer optimizer,
			IProxyFactory proxyFactory)
			: base(
				persistentClass.MappedClass,
				optimizer,
				persistentClass.HasEmbeddedIdentifier)
		{
			_entityMetamodel = entityMetamodel;
			_proxyInterface = persistentClass.ProxyInterface;
			_enhancedForLazyLoading = entityMetamodel.BytecodeEnhancementMetadata.EnhancedForLazyLoading;
			_proxyFactory = proxyFactory;
		}

		protected override object CreateInstance()
		{
			if (!_enhancedForLazyLoading)
			{
				return base.CreateInstance();
			}

			var entity = _proxyFactory.GetFieldInterceptionProxy(base.CreateInstance);
			_entityMetamodel.BytecodeEnhancementMetadata.InjectInterceptor(entity, null);
			return entity;
		}

		public override bool IsInstance(object obj)
		{
			return base.IsInstance(obj) ||
			       // this one needed only for guessEntityMode()
			       (_proxyInterface != null && _proxyInterface.IsInstanceOfType(obj));
		}
	}
}
