using System;
using System.Collections.Generic;

namespace NHibernate.SqlCommand
{
	public class DisjunctionFragment
	{
		private SqlStringBuilder buffer = new SqlStringBuilder();

		public DisjunctionFragment()
		{
		}

		public DisjunctionFragment(IEnumerable<ConditionalFragment> fragments)
		{
			foreach (var conditionalFragment in fragments)
				AddCondition(conditionalFragment);
		}

		public DisjunctionFragment AddCondition(ConditionalFragment fragment)
		{
			if (buffer.Count > 0)
			{
				buffer.Add(" or ");
			}

			buffer.Add("(")
				.Add(fragment.ToSqlStringFragment())
				.Add(")");

			return this;
		}

		public SqlString ToFragmentString()
		{
			return buffer.ToSqlString();
		}
	}
}
