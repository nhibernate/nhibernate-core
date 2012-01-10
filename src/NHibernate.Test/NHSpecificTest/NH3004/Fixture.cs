﻿using System.Collections;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3004
{
	[TestFixture]
	public class Fixture 
	{

        protected string MappingsAssembly
        {
            get { return "NHibernate.Test"; }
        }

        public virtual string BugNumber
        {
            get
            {
                string ns = GetType().Namespace;
                return ns.Substring(ns.LastIndexOf('.') + 1);
            }
        }

        protected IList Mappings
        {
            get
            {
                return new string[]
					{
						"NHSpecificTest." + BugNumber + ".Mappings.hbm.xml"
					};
            }
        }

        [Test]
        public void RemoveUnusedCommandParametersBug_1()
        {
            /* UseNamedPrefixInSql       is true 
             * UseNamedPrefixInParameter is false
             * */
            var driver = new TestSqlClientDriver(true, false);

            RunTest(driver);
        }

        [Test]
        public void RemoveUnusedCommandParametersBug_2()
        {
            /* UseNamedPrefixInSql       is true 
             * UseNamedPrefixInParameter is true
             * */
            var driver = new TestSqlClientDriver(true, true);

            RunTest(driver);
        }

        private static void RunTest(TestSqlClientDriver driver)
        {
            var command = driver.CreateCommand();

            var param = command.CreateParameter();
            param.ParameterName = driver.FormatNameForParameter("p0");
            command.Parameters.Add(param);

            SqlString sqlString = new SqlStringBuilder()
                                            .AddParameter()
                                            .ToSqlString();


            driver.RemoveUnusedCommandParameters(command, sqlString);

            NUnit.Framework.Assert.AreEqual(command.Parameters.Count, 1);
        }

	}
}