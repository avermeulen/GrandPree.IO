using System.Linq;
using Raven.Client.Indexes;

namespace Grandpree.Test
{
    public class DogOwnersUniteTransformer : AbstractTransformerCreationTask<Dog>
    {
        public DogOwnersUniteTransformer()
        {
            TransformResults = dogs => from dog in dogs
                                       let owners = dog.Owners.Select(id => LoadDocument<Owner>(id).Name)
                                       select new DogWithOwners()
                                           {
                                               DogName = dog.Name,
                                               Owners = owners
                                           };
        }
    }
}