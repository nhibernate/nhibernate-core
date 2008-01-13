using System;
using NHibernate.SqlCommand;
using NHibernate.Type;
using System.Collections.Generic;

namespace NHibernate.Expressions
{
	[Serializable]
	public class ProjectionList : IProjection
	{
		private IList<IProjection> elements = new List<IProjection>();

		protected internal ProjectionList()
		{
		}

		public ProjectionList Create()
		{
			return new ProjectionList();
		}

		public ProjectionList Add(IProjection proj)
		{
			elements.Add(proj);
			return this;
		}

		public ProjectionList Add(IProjection projection, String alias)
		{
			return Add(Projections.Alias(projection, alias));
		}

		public IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			IList<IType> types = new List<IType>(Length);
			
			for (int i = 0; i < Length; i++)
			{
				IType[] elemTypes = this[i].GetTypes(criteria, criteriaQuery);
				foreach (IType elemType in elemTypes)
					types.Add(elemType);
			}
			IType[] result = new IType[types.Count];
			types.CopyTo(result, 0);
			return result;
		}

		public SqlString ToSqlString(ICriteria criteria, int loc, ICriteriaQuery criteriaQuery)
		{
			SqlStringBuilder buf = new SqlStringBuilder();
			for (int i = 0; i < Length; i++)
			{
				IProjection proj = this[i];
				buf.Add(proj.ToSqlString(criteria, loc, criteriaQuery));
				loc += proj.GetColumnAliases(loc).Length;
				if (i < elements.Count - 1)
				{
					buf.Add(", ");
				}
			}
			return buf.ToSqlString();
		}

		public SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			SqlStringBuilder buf = new SqlStringBuilder();
			for (int i = 0; i < Length; i++)
			{
				IProjection proj = this[i];
				if (proj.IsGrouped)
				{
					buf.Add(proj.ToGroupSqlString(criteria, criteriaQuery))
						.Add(", ");
				}
			}
			if (buf.Count >= 2)
			{
				buf.RemoveAt(buf.Count - 1);
			}
			//if (buf.Length > 2) buf.Length = buf.Length - 2; //pull off the last ", "
			return buf.ToSqlString();
		}

		public String[] GetColumnAliases(int loc)
		{
			IList<string> aliases = new List<string>(Length);

			for (int i = 0; i < Length; i++)
			{
				String[] colAliases = this[i].GetColumnAliases(loc);
				foreach (string alias in colAliases)
					aliases.Add(alias);
				loc += colAliases.Length;
			}
			string[] result = new string[aliases.Count];
			aliases.CopyTo(result, 0);
			return result;
		}

		public String[] GetColumnAliases(string alias, int loc)
		{
			for (int i = 0; i < Length; i++)
			{
				String[] result = this[i].GetColumnAliases(alias, loc);
				if (result != null) return result;
				loc += this[i].GetColumnAliases(loc).Length;
			}
			return null;
		}

		public IType[] GetTypes(string alias, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			for (int i = 0; i < Length; i++)
			{
				IType[] result = this[i].GetTypes(alias, criteria, criteriaQuery);
				if (result != null) return result;
			}
			return null;
		}

		public String[] Aliases
		{
			get
			{
				IList<string> aliasList = new List<string>(Length);

				for (int i = 0; i < Length; i++)
				{
					String[] aliases = this[i].Aliases;
					foreach (string alias in aliases)
						aliasList.Add(alias);
				}
				string[] result = new string[aliasList.Count];
				aliasList.CopyTo(result, 0);
				return result;
			}
		}

		public IProjection this[int index]
		{
			get
			{
				return elements[index];
			}
		}

		public int Length
		{
			get { return elements.Count; }
		}

		public override String ToString()
		{
			return elements.ToString();
		}

		public bool IsGrouped
		{
			get
			{
				for (int i = 0; i < Length; i++)
				{
					if (this[i].IsGrouped)
						return true;
				}
				return false;
			}
		}
	}
}
