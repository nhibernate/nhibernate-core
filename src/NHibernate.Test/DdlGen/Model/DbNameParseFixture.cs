using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.DdlGen.Model;
using NUnit.Framework;

namespace NHibernate.Test.DdlGen.Model
{
    [TestFixture]
    public class DbNameParseFixture
    {
        private IEnumerable<DbName> GenerateDbNames(string catalog, string schema, string table)
        {
            var tables = new[] { table, "`" + table + "`" };
            var schemas = String.IsNullOrEmpty(schema) ? new string[] { null } : new[] { schema, "`" + schema + "`" };
            var catalogs = String.IsNullOrEmpty(catalog) ? new string[] { null } : new[] { catalog, "`" + catalog + "`" };

            return from t in tables
                   from s in schemas
                   from c in catalogs
                   select new DbName(c,s,t);
        }

        [Test]
        public void CanParseMultipartNames()
        {
            var names = GenerateDbNames(null, null, "MyTable")
                .Concat(GenerateDbNames(null, "dbo", "MyTable"))
                .Concat(GenerateDbNames("nhibernate", "dbo", "MyTable"))
                .Select(n => new
                {
                    AssembledName = n,
                    ParsedName = new DbName(n.ToString())
                });
            foreach (var n in names)
            {
                Assert.That(n.ParsedName.Catalog, Is.EqualTo(n.AssembledName.Catalog));
                Assert.That(n.ParsedName.Schema, Is.EqualTo(n.AssembledName.Schema));
                Assert.That(n.ParsedName.Name, Is.EqualTo(n.AssembledName.Name));
            }
        }

        [Test]
        public void CanParseTempTableNames()
        {
            var expected = "#Lizard";
            var model = new DbName(expected);
            Assert.That(model.Name, Is.EqualTo(expected));
        }

    }
}