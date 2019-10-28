using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;

namespace Infrastructure.Services
{
    public sealed class TelemetryClientWrapper
    {
        #region Singleton
        private static readonly Lazy<TelemetryClientWrapper> lazy =
            new Lazy<TelemetryClientWrapper>(() => new TelemetryClientWrapper());

        public static TelemetryClientWrapper Instance { get { return lazy.Value; } }

        private TelemetryClientWrapper()
        {
            var config = TelemetryConfiguration.CreateDefault();
            _telemetryClient = new TelemetryClient(config);
        }

        #endregion

        private readonly TelemetryClient _telemetryClient;

        public void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            _telemetryClient.TrackEvent(eventName, properties, metrics);
        }

        public void TrackException(Exception exception, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            _telemetryClient.TrackException(exception, properties, metrics);
        }

        public void TrackHandledException(Exception exception, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            if (properties == null) properties = new Dictionary<string, string>();
            properties.Add(new KeyValuePair<string, string>("Handled", "true"));
            _telemetryClient.TrackException(exception, properties, metrics);
        }
    }
}
