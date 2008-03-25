using System;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Util;

namespace NHibernate.Loader
{
	public abstract class BasicLoader : Loader
	{
		protected static readonly string[] NoSuffix = {string.Empty};

		private IEntityAliases[] descriptors;
		private ICollectionAliases[] collectionDescriptors;

		public BasicLoader(ISessionFactoryImplementor factory)
			: base(factory)
		{
		}

		protected override sealed IEntityAliases[] EntityAliases
		{
			get { return descriptors; }
		}

		protected override sealed ICollectionAliases[] CollectionAliases
		{
			get { return collectionDescriptors; }
		}

		protected abstract string[] Suffixes { get; set; }
		protected abstract string[] CollectionSuffixes { get; set; }

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
			if (collectionPersisters != null)
			{
				String[] collectionSuffixes = CollectionSuffixes;
				collectionDescriptors = new ICollectionAliases[collectionPersisters.Length];
				for (int i = 0; i < collectionPersisters.Length; i++)
				{
					collectionDescriptors[i] = new GeneratedCollectionAliases(
						collectionPersisters[i],
						collectionSuffixes[i]
						);
				}
			}
			else
			{
				collectionDescriptors = null;
			}
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