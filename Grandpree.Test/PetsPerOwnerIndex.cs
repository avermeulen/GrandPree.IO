using System.Linq;
using Raven.Client.Indexes;

namespace Grandpree.Test
{
    public class PetsPerOwnerIndex : AbstractMultiMapIndexCreationTask<PetOwnerResult>
    {
        public PetsPerOwnerIndex()
        {

            AddMap<Dog>(dogs => from dog in dogs
                                from owner in dog.Owners
                                select new PetOwnerResult()
                                    {
                                        OwnerName = null,
                                        Owner = owner,
                                        Count = 1
                                    });

            AddMap<Cat>(cats => from cat in cats
                                from owner in cat.Owners
                                select new PetOwnerResult()
                                    {
                                        OwnerName = null,
                                        Owner = owner,
                                        Count = 1
                                    });

            AddMap<Owner>(owners => from owner in owners
                                    select new PetOwnerResult()
                                        {
                                            OwnerName = owner.Name,
                                            Owner = owner.Id,
                                            Count = 1
                                        });


            Reduce = results => from result in results
                                group result by result.Owner into g
                                select new PetOwnerResult
                                    {
                                        Owner = g.Key,
                                        OwnerName = g.First(p => p.OwnerName != null).OwnerName,
                                        Count = g.Sum(k => k.Count)
                                    };
        }
    }
}