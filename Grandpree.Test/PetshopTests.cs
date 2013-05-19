using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Client.Linq;

namespace Grandpree.Test
{
    

    [TestFixture]
    public class TestPetshopTests
    {
        private Lazy<IDocumentStore> documentStore;
        private IDocumentSession session;

        [TestFixtureSetUp]
        public void GlobalSetup()
        {
            Console.WriteLine("Global Setup");

            documentStore = new Lazy<IDocumentStore>(() =>
			{
				var store = new DocumentStore
					{
						Url = "http://localhost:8080",
						DefaultDatabase = "dogs"
					};
				store.Initialize();

                IndexCreation.CreateIndexes(typeof(TestPetshopTests).Assembly, store); 

				return store;
			});

        }


        public class OwnerNameTextSearchIndex : AbstractIndexCreationTask<Owner>
        {
            public OwnerNameTextSearchIndex()
            {

                Map = users => from user in users select new {user.Name};

                Index(x => x.Name, FieldIndexing.Analyzed);
                Index(x => x.Hobby, FieldIndexing.Analyzed);

            }
        }

        [SetUp]
        public void BeforeTest()
        {
            Console.WriteLine("Setup");
            session = documentStore.Value.OpenSession();
        }

        [TearDown]
        public void AfterTest()
        {
            session.SaveChanges();
        }

        [Test]
        public void GetUsernamesWithDogNames()
        {
            Assert.AreEqual(3, session.Query<Owner>().ToList().Count());
            Assert.AreEqual(3, session.Query<Dog>().ToList().Count());

            Assert.AreEqual(3, session.Query<PetOwnerResult, DogsPerOwnerIndex>().Count());

            var petOwnerResults = session.Query<PetOwnerResult, PetsPerOwnerIndex>().ToList();

            foreach (var petOwnerResult in petOwnerResults)
            {
                Console.WriteLine(petOwnerResult);
            }
            
            //Assert.AreEqual(3, petOwnerResults.Count());

        }

        [Test]
        public void OwnersWithPets()
        {
            var dogWithOwners = session.Query<Dog>().TransformWith<DogOwnersUniteTransformer,DogWithOwners>().ToList();
            foreach (var dogWithOwner in dogWithOwners)
            {
                Console.WriteLine(dogWithOwner);
            }
        }
        
        [Test]
        public void WithTextIndex()
        {
            var dogWithOwners = session.Query<Dog>().TransformWith<DogOwnersUniteTransformer,DogWithOwners>().ToList();
            foreach (var dogWithOwner in dogWithOwners)
            {
                Console.WriteLine(dogWithOwner);
            }
        }

        [Test]
        public void TestConcurrency()
        {
            var dog = session.Load<Dog>("dogs/1");

            dog.Name = "Rakayana-1";

            session.Store(dog);

        } 

        [Test]
        public void GetUsersDogCounInOneRequest()
        {
            var user = session.Advanced.Lazily.Load<Owner>("owners/1");
            var list = session.Query<Dog>().Where(x => x.Owners.Any(y => y == "owners/1")).Lazily();

            Console.WriteLine(user.Value.Name + " : " + list.Value.Count());

        }



    }


    public class Pet
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Owners { get; set; }
    }

    public class Dog : Pet
    {
        public string OwnerId { get; set; }
    }

    public class Cat : Pet
    {
        
    }

    public class Owner
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Hobby { get; set; }
    }

    public class PetOwnerResult
    {
        public string OwnerName { get; set; }
        public string Owner { get; set; }
        public int Count { get; set; }

        public override string ToString()
        {
            return string.Format("OwnerName: {0}, Owner: {1}, Count: {2}", OwnerName, Owner, Count);
        }
    }
}
