using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Persister.Collection
{
	/// <summary>
	/// Summary description for CompositeElementPropertyMapping.
	/// </summary>
	public class CompositeElementPropertyMapping : AbstractPropertyMapping
	{
		private readonly IAbstractComponentType compositeType;

		public CompositeElementPropertyMapping(string[] elementColumns, string[] elementFormulaTemplates,
																					 IAbstractComponentType compositeType, IMapping factory)
		{
			this.compositeType = compositeType;

			InitComponentPropertyPaths(null, compositeType, elementColumns, elementFormulaTemplates, factory);
		}

		public override IType Type
		{
			get { return compositeType; }
		}

		protected override string EntityName
		{
			get { return compositeType.Name; }
		}
	}
}