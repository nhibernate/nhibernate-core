﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;

using NHibernate.Cfg;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH965
{
    using System.Threading.Tasks;
    [TestFixture]
    public class NH965FixtureAsync
    {
        [Test]
        public async Task BugAsync()
        {
            Configuration cfg = new Configuration();
            cfg.AddResource(GetType().Namespace + ".Mappings.hbm.xml", GetType().Assembly);
            await (cfg.BuildSessionFactory().CloseAsync());
        }
    }
}
