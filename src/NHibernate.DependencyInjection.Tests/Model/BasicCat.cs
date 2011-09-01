using Iesi.Collections.Generic;

namespace NHibernate.DependencyInjection.Tests.Model
{
    public class BasicCat
    {
        public virtual int Id { get; protected set; }

        public virtual string Name { get; set; }

        private ISet<BasicCat> _kittens = new HashedSet<BasicCat>();
        public virtual ISet<BasicCat> Kittens
        {
            get { return _kittens; }
            set { _kittens = value; }
        }

        public virtual BasicCat Parent { get; set; }
    }
}