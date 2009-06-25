using NHibernate.Exceptions;
namespace NHibernate.Cfg.Loquacious
{
	public interface ICommandsConfiguration
	{
		ICommandsConfiguration Preparing();
		ICommandsConfiguration WithTimeout(int seconds);
		ICommandsConfiguration ConvertingExpetionsTrough<TExceptionConverter>() where TExceptionConverter : ISQLExceptionConverter;
		ICommandsConfiguration AutoCommentingSql();
		IDbIntegrationConfiguration WithHqlToSqlSubstitutions(string csvQuerySubstitutions);
		IDbIntegrationConfiguration WithDefaultHqlToSqlSubstitutions();
		
		/// <summary>
		/// Maximum depth of outer join fetching
		/// </summary>
		/// <remarks>
		/// 0 (zero) disable the usage of OuterJoinFetching
		/// </remarks>
		ICommandsConfiguration WithMaximumDepthOfOuterJoinFetching(byte maxFetchDepth);
	}
}