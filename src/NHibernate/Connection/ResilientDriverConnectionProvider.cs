using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using NHibernate;
using NHibernate.Connection;

namespace NHibernate.Connection
{
	public class ResilientDriverConnectionProvider : DriverConnectionProvider
	{
		public const String ConnectionDelayBetweenTries = "connection.delay_between_tries";
		public const String ConnectionMaxTries = "connection.max_tries";

		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(ResilientDriverConnectionProvider));

		public ResilientDriverConnectionProvider()
		{
			this.MaxTries = 3;
			this.DelayBetweenTries = TimeSpan.FromSeconds(5);
		}

		public Int32 MaxTries { get; set; }

		public TimeSpan DelayBetweenTries { get; set; }

		public override void Configure(IDictionary<String, String> settings)
		{
			String maxTries;
			String delayBetweenTries;

			if (settings.TryGetValue(ConnectionMaxTries, out maxTries) == true)
			{
				this.MaxTries = Int32.Parse(maxTries);
			}

			if (settings.TryGetValue(ConnectionDelayBetweenTries, out delayBetweenTries) == true)
			{
				TimeSpan timespan;
				Int32 delay;

				if (TimeSpan.TryParse(delayBetweenTries, out timespan) == true)
				{
					this.DelayBetweenTries = timespan;
				}
				else if (Int32.TryParse(delayBetweenTries, out delay) == true)
				{
					this.DelayBetweenTries = TimeSpan.FromSeconds(delay);
				}
				else
				{
					throw (new Exception("Invalid delay"));
				}
			}

			base.Configure(settings);
		}

		public override IDbConnection GetConnection()
		{
			IDbConnection con = null;

			for (var i = 0; i < this.MaxTries; ++i)
			{
				try
				{
					log.Debug(String.Format("Attempting to get connection, {0} of {1}", (i + 1), this.MaxTries));
					con = base.GetConnection();

					if (con.State != ConnectionState.Open)
					{
						con.Open();
					}

					log.Debug(String.Concat("Got a connection after {0} tries", (i + 1)));

					break;
				}
				catch (Exception ex)
				{
					if (i == this.MaxTries - 1)
					{
						log.Error(String.Concat("Could not get connection after {0} tries", this.MaxTries), ex);
						throw;
					}
					else
					{
						Thread.Sleep(this.DelayBetweenTries);
					}
				}
			}

			return (con);
		}
	}
}