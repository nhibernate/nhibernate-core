using System.Data;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Param
{
	public class VersionTypeSeedParameterSpecification : IParameterSpecification
	{
		private readonly IVersionType type;

		public VersionTypeSeedParameterSpecification(IVersionType type)
		{
			this.type = type;
		}

		public int Bind(IDbCommand statement, QueryParameters qp, ISessionImplementor session, int position)
		{
			type.NullSafeSet(statement, type.Seed(session), position, session);
			return 1;
		}

		public IType ExpectedType
		{
			get { return type; }
			set
			{
				// expected type is intrinsic here...
			}
		}

		public string RenderDisplayInfo()
		{
			return "version-seed, type=" + type;
		}
	}
}