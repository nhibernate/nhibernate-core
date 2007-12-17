using System;
using System.Collections;
using System.IO;
using log4net;
using NHibernate.Search.Impl;
using NHibernate.Util;

namespace NHibernate.Search
{
    public class DirectoryProviderHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DirectoryProviderHelper));

        /// <summary>
        /// Build a directory name out of a root and relative path, guessing the significant part
        /// and checking for the file availability
        /// </summary>
        public static String GetSourceDirectory(
            String rootPropertyName, String relativePropertyName,
            String directoryProviderName, IDictionary properties)
        {
            //TODO check that it's a directory
            String root = (string) properties[rootPropertyName];
            String relative = (string) properties[relativePropertyName];
            if (log.IsDebugEnabled)
            {
                log.Debug(
                    "Guess source directory from " + rootPropertyName + " " + root != null
                        ? root
                        : "<null>" + " and " + relativePropertyName + " " + (relative != null ? relative : "<null>")
                    );
            }
            if (relative == null) relative = directoryProviderName;
            if (StringHelper.IsEmpty(root))
            {
                log.Debug("No root directory, go with relative " + relative);
                DirectoryInfo sourceFile = new DirectoryInfo(relative);
                if (! sourceFile.Exists)
                {
                    throw new HibernateException("Unable to read source directory: " + relative);
                }
                //else keep source as it
            }
            else
            {
                DirectoryInfo rootDir = new DirectoryInfo(root);
                if (rootDir.Exists)
                {
                    DirectoryInfo sourceFile = new DirectoryInfo(Path.Combine(root, relative));
                    if (! sourceFile.Exists) sourceFile.Create();
                    log.Debug("Get directory from root + relative");
                    try
                    {
                        relative = sourceFile.FullName;
                    }
                    catch (IOException e)
                    {
                        throw new AssertionFailure("Unable to get canonical path: " + root + " + " + relative);
                    }
                }
                else
                {
                    throw new SearchException(rootPropertyName + " does not exist");
                }
            }
            return relative;
        }

        public static DirectoryInfo DetermineIndexDir(String directoryProviderName, IDictionary properties)
        {
            bool createIfMissing;
            string indexBase = (string) properties["indexBase"] ?? ".";
            string shouldCreate = (string) properties["indexBase.create"] ?? "false";
            bool.TryParse(shouldCreate, out createIfMissing);

            //We need this to allow using the search from the web, where the "." directory is 
            //somewhere in the system root.
            indexBase = indexBase.Replace("~", AppDomain.CurrentDomain.BaseDirectory);
            DirectoryInfo indexDir = new DirectoryInfo(indexBase);
            if (!(indexDir.Exists))
            {
                if (!createIfMissing)
                    throw new HibernateException(String.Format("Index directory does not exists: {0}", indexBase));
                indexDir.Create();
            }

            if (!HasWriteAccess(indexDir))
            {
                throw new HibernateException("Cannot write into index directory: " + indexBase);
            }

            indexDir = new DirectoryInfo(Path.Combine(indexDir.FullName, directoryProviderName));
            return indexDir;
        }

        private static bool HasWriteAccess(DirectoryInfo indexDir)
        {
            string tempFileName = Path.Combine(indexDir.FullName, Guid.NewGuid().ToString());
            //Yuck! but it is the simplest way
            try
            {
                File.CreateText(tempFileName).Close();
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            try
            {
                File.Delete(tempFileName);
            }
            catch (UnauthorizedAccessException)
            {
                //we may have permissions to create but not delete, ignoring
            }
            return true;
        }
    }
}