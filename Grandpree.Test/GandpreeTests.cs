using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Document;



namespace Grandpree.Test
{
    


    /*
    [TestFixture]
    public class TestMe
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
						DefaultDatabase = "Grandpree"
					};
				store.Initialize();

                //IndexCreation.CreateIndexes(typeof(RavenController).Assembly, store); 

				return store;
			});

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
        public void AddRaces()
        {
            var lines = File.ReadAllLines("./Races.txt");
            foreach (var line in lines)
            {
                var parts = line.Split('\t');

                var race = new Race()
                    {
                        StartTime = DateTime.Parse(parts[1]),
                        Name = parts[0]
                    };
                
                session.Store(race);
            }
        }

        [Test]
        public void AddDrivers()
        {
            var lines = File.ReadAllLines("./Drivers.txt");
            string group = null;

            foreach (var line in lines)
            {
                if (line.Contains("Group"))
                {
                    group = line.Trim();
                    continue;
                }

                if ("".Equals(line.Trim()))
                {
                    continue;
                }

                var driver = new Driver()
                    {
                        Group = group,
                        Name = line.Trim()
                    };

                session.Store(driver);

            }

            //file.Read

            session.Store(new Driver(){Group = "Group A", Name="Seb Vettel"});
            session.Store(new Driver(){Group = "Group B", Name="Fernando Alonso"});
            session.Store(new Driver(){Group = "Group B", Name="Jenson Button"});

        }
        
        [Test]
        public void DeleteAllDrivers()
        {
            session.Query<Driver>().ToList().ForEach(
                driver => session.Delete(driver));
        }

        [Test]
        public void CreateLoadsOfUsers()
        {
            for (int i = 0; i < 200; i++)
            {

                session = documentStore.Value.OpenSession();
                for (int j = 0; j < 1000; j++)
                {
                    var user = new User()
                    {
                        Name = "user-3-" + i + "-" + j,
                        Uid = i
                    };
                    session.Store(user);
                }

                session.SaveChanges();
                session.Advanced.Clear();
                

            }
        }

    }


    class User
    {
        public int Uid { get; set; }
        public string Name { get; set; }
    }
     * */

}
