namespace NHibernate.Test.Criteria
{
	internal static class SessionExtender
	{
		public static void SaveOrUpdate<T>(this ISession session, params T[] models)
		{
			for (int i = 0; i < models.Length; i++)
			{
				var model = models[i];
				session.SaveOrUpdate(model);
			}
		}

		public static void Save<T>(this ISession session, params T[] models)
		{
			for (int i = 0; i < models.Length; i++)
			{
				var model = models[i];
				session.Save(model);
			}
		}

		public static void Delete<T>(this ISession session, params T[] models)
		{
			for (int i = 0; i < models.Length; i++)
			{
				var model = models[i];
				session.Delete(model);
			}
		}
	}
}
