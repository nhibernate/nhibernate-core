using Iesi.Collections.Generic;

namespace NHibernate.DependencyInjection.Tests.Model
{
    public class DependencyInjectionCat
    {
        private readonly CatBehavior _behavior;

        public DependencyInjectionCat(CatBehavior behavior)
        {
            _behavior = behavior;
        }

        public virtual string Meow()
        {
            return _behavior.Meow();
        }

        public virtual string Purr()
        {
            return _behavior.Purr();
        }

        public virtual int Id { get; protected set; }

        public virtual string Name { get; set; }

        private ISet<DependencyInjectionCat> _kittens = new HashedSet<DependencyInjectionCat>();
        public virtual ISet<DependencyInjectionCat> Kittens
        {
            get { return _kittens; }
            set { _kittens = value; }
        }

        public virtual DependencyInjectionCat Parent { get; set; }
    }
}