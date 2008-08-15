using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader
{
	public abstract class BasicLoader : Loader
	{
		protected static readonly string[] NoSuffix = {string.Empty};

		private IEntityAliases[] descriptors;
		private ICollectionAliases[] collectionDescriptors;

		public BasicLoader(ISessionFactoryImplementor factory) : base(factory) {}

		protected override sealed IEntityAliases[] EntityAliases
		{
			get { return descriptors; }
		}

		protected override sealed ICollectionAliases[] CollectionAliases
		{
			get { return collectionDescriptors; }
		}

		protected abstract string[] Suffixes { get; }
		protected abstract string[] CollectionSuffixes { get; }

		protected override void PostInstantiate()
		{
			ILoadable[] persisters = EntityPersisters;
			string[] suffixes = Suffixes;
			descriptors = new IEntityAliases[persisters.Length];
			for (int i = 0; i < descriptors.Length; i++)
			{
				descriptors[i] = new DefaultEntityAliases(persisters[i], suffixes[i]);
			}

			ICollectionPersister[] collectionPersisters = CollectionPersisters;
			int bagCount = 0;
			if (collectionPersisters != null)
			{
				string[] collectionSuffixes = CollectionSuffixes;
				collectionDescriptors = new ICollectionAliases[collectionPersisters.Length];
				for (int i = 0; i < collectionPersisters.Length; i++)
				{
					if (IsBag(collectionPersisters[i]))
					{
						bagCount++;
					}
					collectionDescriptors[i] = new GeneratedCollectionAliases(collectionPersisters[i], collectionSuffixes[i]);
				}
			}
			else
			{
				collectionDescriptors = null;
			}
			// H3.2 : 14.3. Associations and joins
			// Join fetching multiple collection roles also sometimes gives unexpected results for bag mappings, 
			// so be careful about how you formulate your queries in this case
			if (bagCount > 1)
			{
				throw new QueryException("Cannot simultaneously fetch multiple bags.");
			}
		}

		private static bool IsBag(ICollectionPersister collectionPersister)
		{
			return collectionPersister.CollectionType.GetType().IsAssignableFrom(typeof (BagType));
		}

		/// <summary>
		/// Utility method that generates 0_, 1_ suffixes. Subclasses don't
		/// necessarily need to use this algorithm, but it is intended that
		/// they will in most cases.
		/// </summary>
		public static string[] GenerateSuffixes(int length)
		{
			return GenerateSuffixes(0, length);
		}

		public static string[] GenerateSuffixes(int seed, int length)
		{
			if (length == 0)
			{
				return NoSuffix;
			}

			string[] suffixes = new string[length];

			for (int i = 0; i < length; i++)
			{
				suffixes[i] = (i + seed).ToString() + StringHelper.Underscore;
			}

			return suffixes;
		}
	}
}