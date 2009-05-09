using System.Reflection;
using NHibernate.Transform;

namespace NHibernate.Hql
{
	public sealed class HolderInstantiator
	{
		public static readonly HolderInstantiator NoopInstantiator = new HolderInstantiator(null, null);

		private readonly IResultTransformer transformer;
		private readonly string[] queryReturnAliases;

		public static HolderInstantiator GetHolderInstantiator(IResultTransformer selectNewTransformer,
		                                                       IResultTransformer customTransformer,
		                                                       string[] queryReturnAliases)
		{
			if (selectNewTransformer != null)
			{
				return new HolderInstantiator(selectNewTransformer, queryReturnAliases);
			}
			else
			{
				return new HolderInstantiator(customTransformer, queryReturnAliases);
			}
		}

		public static IResultTransformer CreateSelectNewTransformer(ConstructorInfo constructor, bool returnMaps,
		                                                            bool returnLists)
		{
			if (constructor != null)
			{
				return new AliasToBeanConstructorResultTransformer(constructor);
			}
			else if (returnMaps)
			{
				return Transformers.AliasToEntityMap;
			}
			else if (returnLists)
			{
				return Transformers.ToList;
			}
			else
			{
				return null;
			}
		}

		public static HolderInstantiator CreateClassicHolderInstantiator(ConstructorInfo constructor,
		                                                                 IResultTransformer transformer)
		{
			if (constructor != null)
			{
				return new HolderInstantiator(new AliasToBeanConstructorResultTransformer(constructor), null);
			}
			else
			{
				return new HolderInstantiator(transformer, null);
			}
		}

		public HolderInstantiator(IResultTransformer transformer, string[] queryReturnAliases)
		{
			this.transformer = transformer;
			this.queryReturnAliases = queryReturnAliases;
		}

		public bool IsRequired
		{
			get { return transformer != null; }
		}

		public object Instantiate(object[] row)
		{
			if (transformer == null)
			{
				return row;
			}
			else
			{
				return transformer.TransformTuple(row, queryReturnAliases);
			}
		}

		public string[] QueryReturnAliases
		{
			get { return queryReturnAliases; }
		}

		public IResultTransformer ResultTransformer
		{
			get { return transformer; }
		}
	}
}