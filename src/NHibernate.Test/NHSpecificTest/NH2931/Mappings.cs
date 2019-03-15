using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.NH2931
{
	public class AMapping : JoinedSubclassMapping<A> {}

	public class BMapping : ClassMapping<B> {}

	public class CMapping : JoinedSubclassMapping<C> {}

	public class DMapping : JoinedSubclassMapping<D> {}

	public class EMapping : JoinedSubclassMapping<E> {}

	public class FMapping : JoinedSubclassMapping<F> {}

	public class GMapping : ClassMapping<G> {}
}
