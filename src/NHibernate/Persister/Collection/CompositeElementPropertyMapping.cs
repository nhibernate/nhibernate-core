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

		public CompositeElementPropertyMapping( string[] elementColumns, string[] elementFormulaTemplates,
			IAbstractComponentType compositeType, ISessionFactoryImplementor factory )
		{
			this.compositeType = compositeType;

			InitComponentPropertyPaths( null, compositeType, elementColumns, elementFormulaTemplates, factory );
		}

		/// <summary></summary>
		public override IType Type
		{
			get { return compositeType; }
		}

		/// <summary></summary>
		public override string ClassName
		{
			get { return compositeType.Name; }
		}
	}
}
