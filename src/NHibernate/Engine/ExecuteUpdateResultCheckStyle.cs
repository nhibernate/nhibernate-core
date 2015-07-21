using System;
using NHibernate.SqlCommand;

namespace NHibernate.Engine
{
	[Serializable]
	public class ExecuteUpdateResultCheckStyle
	{
		public static readonly ExecuteUpdateResultCheckStyle None = new ExecuteUpdateResultCheckStyle("none");
		public static readonly ExecuteUpdateResultCheckStyle Count = new ExecuteUpdateResultCheckStyle("rowcount");

		private readonly string name;

		private ExecuteUpdateResultCheckStyle(string name)
		{
			this.name = name;
		}

		public static ExecuteUpdateResultCheckStyle Parse(string name)
		{
			switch (name)
			{
				case "none":
					return None;

				case "rowcount":
					return Count;

				default:
					return null;
			}
		}

		public static ExecuteUpdateResultCheckStyle DetermineDefault(SqlString customSql, bool callable)
		{
			return Count;
		}

		public override bool Equals(object obj)
		{
			var castedObj = obj as ExecuteUpdateResultCheckStyle;
			if (castedObj != null)
			{
				return name == castedObj.name;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return name.GetHashCode();
		}

		public override string ToString()
		{
			return name;
		}
	}
}