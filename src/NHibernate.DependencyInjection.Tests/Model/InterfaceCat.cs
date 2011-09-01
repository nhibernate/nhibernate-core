using Iesi.Collections.Generic;

namespace NHibernate.DependencyInjection.Tests.Model
{
    public class InterfaceCat : IInterfaceCat
    {
        public int Id { get; protected set; }

        public string Name { get; set; }

        private ISet<IInterfaceCat> _kittens = new HashedSet<IInterfaceCat>();
        public ISet<IInterfaceCat> Kittens
        {
            get { return _kittens; }
            set { _kittens = value; }
        }

        public IInterfaceCat Parent { get; set; }
    }
}