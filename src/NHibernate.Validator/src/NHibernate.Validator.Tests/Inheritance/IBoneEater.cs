namespace NHibernate.Validator.Tests.Inheritance
{
    using System;

    public interface IBoneEater : IEater
    {
        [NotNull]
        String FavoriteBone { get; set; }
    }
}