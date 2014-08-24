using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1612
{
	public class AreaStatistics
	{
		public virtual MonetaryValue? GDP { get; set; }
		public virtual int? CitizenCount { get; set; }
		public virtual Person Reporter { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			var other = obj as AreaStatistics;
			if (ReferenceEquals(other, null))
			{
				return false;
			}

			return CitizenCount == other.CitizenCount && GDP == other.GDP && Reporter == other.Reporter;
		}

		public override int GetHashCode()
		{
			return (CitizenCount ?? 0) ^ GDP.GetHashCode() ^ (Reporter != null ? Reporter.GetHashCode() : 0);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			if (CitizenCount.HasValue)
			{
				sb.Append("CitizenCount: ").Append(CitizenCount);
			}
			if (GDP.HasValue)
			{
				if (sb.Length > 0)
				{
					sb.Append("; ");
				}
				sb.Append("GDP: ").Append(GDP);
			}
			if (Reporter != null)
			{
				if (sb.Length > 0)
				{
					sb.Append("; ");
				}
				sb.Append("Reporter: ").Append(Reporter.Name);
			}

			return sb.ToString();
		}
	}
}