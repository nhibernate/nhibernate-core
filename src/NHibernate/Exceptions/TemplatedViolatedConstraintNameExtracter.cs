using System.Data.Common;

namespace NHibernate.Exceptions
{
    /// <summary> 
    /// Knows how to extract a violated constraint name from an error message based on the
    /// fact that the constraint name is templated within the message.
    /// </summary>
    public abstract class TemplatedViolatedConstraintNameExtracter : IViolatedConstraintNameExtracter
    {

        /// <summary>
        /// Extracts the constraint name based on a template (i.e., <i>templateStart</i><b>constraintName</b><i>templateEnd</i>).
        /// </summary>
        /// <param name="templateStart">The pattern denoting the start of the constraint name within the message.</param>
        /// <param name="templateEnd">The pattern denoting the end of the constraint name within the message.</param>
        /// <param name="message">The templated error message containing the constraint name.</param>
        /// <returns>The found constraint name, or null.</returns>
        protected string ExtractUsingTemplate(string templateStart, string templateEnd, string message)
        {
            int templateStartPosition = message.IndexOf(templateStart);
            if (templateStartPosition < 0)
            {
                return null;
            }

            int start = templateStartPosition + templateStart.Length;
            int end = message.IndexOf(templateEnd, start);
            if (end < 0)
            {
                end = message.Length;
            }

            return message.Substring(start, end);
        }

        /// <summary> 
        /// Extract the name of the violated constraint from the given SQLException. 
        /// </summary>
        /// <param name="sqle">The exception that was the result of the constraint violation. </param>
        /// <returns> The extracted constraint name. </returns>
        public abstract string ExtractConstraintName(DbException sqle);
    }
}
