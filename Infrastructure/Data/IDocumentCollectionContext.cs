using Core.Collections;
using Microsoft.Azure.Documents;

namespace Infrastructure.Data
{
    public interface IDocumentCollectionContext<in T> where T : BaseCollection
    {
        string CollectionName { get; }

        string GenerateId(T entity);

        PartitionKey ResolvePartitionKey(string entityId);
    }
}
