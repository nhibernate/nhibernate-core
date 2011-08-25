using Iesi.Collections.Generic;

namespace NHibernate.DependencyInjection.Tests.Model
{
    public interface IInterfaceCat
    {
        int Id { get; }
        string Name { get; set; }
        ISet<IInterfaceCat> Kittens { get; set; }
        IInterfaceCat Parent { get; set; }
    }
}