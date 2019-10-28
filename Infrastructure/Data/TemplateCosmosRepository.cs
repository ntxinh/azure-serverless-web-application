using Core.Collections;
using Core.Interfaces;
using Microsoft.Azure.Documents;

namespace Infrastructure.Data
{
    public class TemplateCosmosRepository : CosmosDbRepository<Template>, ITemplateCosmosRepository
    {
        public TemplateCosmosRepository(ICosmosDbClientFactory factory) : base(factory) { }

        public override string CollectionName { get; } = "template";
        public override PartitionKey ResolvePartitionKey(string entityId) => new PartitionKey(entityId.Split(':')[0]);
    }
}
