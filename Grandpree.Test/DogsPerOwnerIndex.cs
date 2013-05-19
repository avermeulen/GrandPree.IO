using System.Linq;
using Raven.Client.Indexes;

namespace Grandpree.Test
{
    public class DogsPerOwnerIndex : AbstractIndexCreationTask<Dog, PetOwnerResult>
    {
        public DogsPerOwnerIndex()
        {
            Map = docs => from dog in docs
                          from owner in dog.Owners
                          select new PetOwnerResult
                              {
                                  Owner = owner,
                                  Count = 1
                              };
            
            Reduce = results => from result in results
                                group result by result.Owner into g
                                select new PetOwnerResult
                                    {
                                        Owner = g.Key,
                                        Count = g.Sum(k => k.Count)
                                    };
        }
    }
}