using System;

namespace NHibernate.Engine
{
	[Serializable]
	public struct ExecuteUpdateResultCheckStyle
	{
		// Added Default because I would like this type to be a value type and they don't allow nulls.
		public static readonly ExecuteUpdateResultCheckStyle Default = new ExecuteUpdateResultCheckStyle("default");
		
		public static readonly ExecuteUpdateResultCheckStyle None = new ExecuteUpdateResultCheckStyle("none");
		public static readonly ExecuteUpdateResultCheckStyle Count = new ExecuteUpdateResultCheckStyle("rowcount");
		public static readonly ExecuteUpdateResultCheckStyle Param = new ExecuteUpdateResultCheckStyle("param");

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

				case "param":
					return Param;

				default:
					return Default;
			}
		}

		public static ExecuteUpdateResultCheckStyle DetermineDefault(string customSql, bool callable)
		{
			if (customSql == null)
			{
				return Count;
			}
			else
			{
				return callable ? Param : Count;
			}
		}

		public static bool operator==(ExecuteUpdateResultCheckStyle left, ExecuteUpdateResultCheckStyle right)
		{
			return left.name == right.name;
		}

		public static bool operator !=(ExecuteUpdateResultCheckStyle left, ExecuteUpdateResultCheckStyle right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			if(obj is ExecuteUpdateResultCheckStyle)
			{
				return this == (ExecuteUpdateResultCheckStyle) obj;
			}
			return false;
		}
		public override int GetHashCode()
		{
			return name.GetHashCode();
		}
	}
}
