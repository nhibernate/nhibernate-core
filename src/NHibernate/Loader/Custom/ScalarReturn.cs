using NHibernate.Type;

namespace NHibernate.Loader.Custom
{
	/// <summary> Represent a scalar (AKA simple value) return within a query result. </summary>
	internal class ScalarReturn : IReturn
	{
		private readonly IType type;
		private readonly string columnAlias;

		public ScalarReturn(IType type, string columnAlias)
		{
			this.type = type;
			this.columnAlias = columnAlias;
		}

		public IType Type
		{
			get { return type; }
		}

		public string Alias
		{
			get { return columnAlias; }
		}
	}
}
