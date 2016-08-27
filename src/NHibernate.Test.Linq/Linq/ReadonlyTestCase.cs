using System;
using System.Collections;
using System.Reflection;
using log4net;
using log4net.Config;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
    public abstract class ReadonlyTestCase : TestCase
    {
        protected override void CreateSchema()
        {
        }

        protected override void DropSchema()
        {
        }

        protected override bool CheckDatabaseWasCleaned()
        {
            // We are read-only, so we're theoretically always clean.
            return true;
        }

        protected override void ApplyCacheSettings(Configuration configuration)
        {
            // Patrick Earl: I wasn't sure if making this do nothing was important, but I left it here since it wasn't running in the code when I changed it.
        }
    }
}