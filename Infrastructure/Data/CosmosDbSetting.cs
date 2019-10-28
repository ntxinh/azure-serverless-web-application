using System;

namespace Infrastructure.Data
{
    public class CosmosDbSetting
    {
        public CosmosDbSetting(string uri, string key, string databaseName)
        {
            DatabaseUri = new Uri(uri);
            DatabaseKey = key;
            DatabaseName = databaseName;
        }

        public string DatabaseName { get; private set; }
        public Uri DatabaseUri { get; private set; }
        public string DatabaseKey { get; private set; }

    }
}
