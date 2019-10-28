using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Infrastructure.Data
{
    public class CosmosDbClientFactory : ICosmosDbClientFactory
    {
        private readonly CosmosDbSetting _settings;
        private readonly string _databaseName;
        //private readonly List<string> _collectionNames;
        private readonly IDocumentClient _documentClient;

        public CosmosDbClientFactory(CosmosDbSetting settings)
        {
            _settings = settings;

            _databaseName = _settings.DatabaseName;

            //var collectionNames = new List<string>();
            //collectionNames.Add("template2");
            //_collectionNames = collectionNames;

            var _dbClient = new DocumentClient(_settings.DatabaseUri, _settings.DatabaseKey, new ConnectionPolicy()
            {
                MaxConnectionLimit = 100,
                ConnectionMode = ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Tcp
            });
            _dbClient.OpenAsync().Wait();
            _documentClient = _dbClient;
        }

        public ICosmosDbClient GetClient(string collectionName)
        {
            //if (!_collectionNames.Contains(collectionName))
            //{
            //    throw new ArgumentException($"Unable to find collection: {collectionName}");
            //}

            return new CosmosDbClient(_databaseName, collectionName, _documentClient);
        }

        //public async Task EnsureDbSetupAsync()
        //{
        //    await _documentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseName));

        //    foreach (var collectionName in _collectionNames)
        //    {
        //        await _documentClient.ReadDocumentCollectionAsync(
        //            UriFactory.CreateDocumentCollectionUri(_databaseName, collectionName));
        //    }
        //}
    }
}
