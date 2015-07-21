using System;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Test.NHSpecificTest.NH3171
{
	public class Artist
	{
		public virtual string Name { get; set; }
		public virtual Song Song { get; set; }
		public virtual Country Country { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			Artist other = obj as Artist;
			return other.Song.Id == Song.Id && other.Country.Id == Country.Id;
		}

		public override int GetHashCode()
		{
			const int hashMultiplier = 31;

			var items = new object[] { Song.Id, Country.Id };
			return items.Where(item => item != null).Aggregate(0, (accumulator, next) => accumulator = (accumulator * hashMultiplier) ^ next.GetHashCode());
		}
	}

	public class Song
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class Country
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }

		public virtual ICollection<Artist> Artists { get; set; }
	}
}