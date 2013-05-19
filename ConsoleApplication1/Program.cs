using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grandpree.Test;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Shard;
using Raven.Json.Linq;

namespace ConsoleApplication1
{
	class Program
	{
		static void Main()
		{
		    var shardStrategy = ShardStrategySetup();


            var documentStore = new Lazy<IDocumentStore>(() =>
            {
                var store = new DocumentStore
                {
                    Url = "http://localhost:8080",
                    DefaultDatabase = "dogs"
                };
                store.Initialize();

                //IndexCreation.CreateIndexes(typeof(TestPetshopTests).Assembly, store);

                return store;
            });


		    //using (ShardedDocumentStore store = new ShardedDocumentStore(shardStrategy))
		    using (var store = documentStore.Value)
			{
			    store.Initialize();


			    store.DatabaseCommands.Patch("owners/1", new ScriptedPatchRequest()
			        {
			            Script = @"this.Phone.push(tba);",
                        Values =
                            {
                                {"tba", "700-901"}
                            }

                        
			        });

			    //op.WaitForCompletion();

			    //var result = FindDogOwnerAsync(store, "dogs/3");

			    //Console.Out.WriteLine("result : " + result.Result);

                /**
			    var session = store.OpenSession();
			    GetUserById(session);

			    //Users by name
                UsersByName(session);

			    //Dogs by owner
                DogsByOwner(session);   

			    // Count of dogs by name
			    CountODogsByName(session);
                */

			    Console.ReadLine();
			    //CreateData(store);
			}
		}

	    private static ShardStrategy ShardStrategySetup()
	    {
	        var shards = new Dictionary<string, IDocumentStore>
	            {
	                {"vet1", new DocumentStore {Url = "http://localhost:8080", DefaultDatabase = "Vet1"}},
	                {"vet2", new DocumentStore {Url = "http://localhost:8080", DefaultDatabase = "Vet2"}},
	                {"vet3", new DocumentStore {Url = "http://localhost:8080", DefaultDatabase = "Vet3"}}
	            };

	        var shardStrategy = new ShardStrategy(shards)
	            .ShardingOn<User>()
	            .ShardingOn<Dog>(x => x.OwnerId)
	            .ShardingOn<Cat>(x => x.OwnerId);
	        return shardStrategy;
	    }

        private async static Task<String> FindDogOwnerAsync(IDocumentStore store, string dogId)
        {
            var asyncSession = store.OpenAsyncSession();

            var dog = await asyncSession.Include<Owner>(o => o.Id).LoadAsync<Dog>(dogId);
            var owner = await asyncSession.LoadAsync<Owner>(dog.OwnerId);

            //var owner = await asyncSession.Query<Owner>().Where(o => dog.);

            //var owner = asyncSession.Query<User>().Where(u => u.Name == dog.OwnerId).FirstOrDefault();

            return dog.Name + " owned by " + owner.Name;
            
            //+" owned by " + owner.Name;

        }


	    private static void GetUserById(IDocumentSession session)
	    {
	        var user = session.Load<User>("vet2/users/1");
	        Console.Out.WriteLine("" + user.Name);
	    }

	    private static void UsersByName(IDocumentSession session)
	    {
	        var user = session.Query<User>().Where(u => u.Name == "Jo").FirstOrDefault();
	        Console.Out.WriteLine("" + user.Name);
	    }

	    private static void CountODogsByName(IDocumentSession session)
	    {
	        var dogsWithName = session.Query<Dog>().ToList()
	                                  .GroupBy(d => d.Name, (k, g) => new {Name = k, Number = g.Count()});

	        foreach (var dogName in dogsWithName)
	        {
	            Console.Out.WriteLine("Number of dogs with name : " + dogName.Name + ":" + dogName.Number);
	        }
	    }

	    private static void DogsByOwner(IDocumentSession session)
	    {
	        var users = session.Query<User>().Lazily();
	        var dogs = session.Query<Dog>().Lazily();

	        foreach (var user in users.Value)
	        {
	            var dogsForUser = dogs.Value.Where(d => d.OwnerId == user.Id);
	            var dogString = string.Join(",", dogsForUser.Select(d => d.Name));

	            Console.WriteLine("dogs for {0} : {1}", user.Name, dogString);
	        }
	    }

	    private static void CreateData(ShardedDocumentStore store)
	    {
	        for (int i = 0; i < 3; i++)
	        {
	            var user = new User();
	            using (var session = store.OpenSession())
	            {
	                session.Store(user);
	                session.SaveChanges();
	            }

	            using (var session = store.OpenSession())
	            {
	                session.Store(new Dog
	                    {
	                        OwnerId = user.Id
	                    });
	                session.SaveChanges();
	            }
	        }
	    }
	}

    internal class Cat
    {
        public string OwnerId { get; set; }
        public string Name { get; set; }
    }

    /**
    internal class Dog
    {
        public string OwnerId 
        {
            get; set; 
        }
        public string Name { get; set; }
    }
    */

    internal class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    
}