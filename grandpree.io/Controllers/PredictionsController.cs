using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Raven.Client;
using Raven.Client.Document;

namespace grandpree.io.Controllers
{
    public class PredictionsController : ApiController
    {

        private Lazy<IDocumentStore> documentStore = new Lazy<IDocumentStore>(() =>
        {
            var store = new DocumentStore
            {
                Url = "http://localhost:8080",
                DefaultDatabase = "Grandpree"
            };
            store.Initialize();

            return store;
        });


        public Prediction PostPrediction(Prediction prediction)
        {

            var session = documentStore.Value.OpenSession();
            if (!session.Query<Prediction>()
                        .Any(p => p.RaceId == prediction.RaceId && prediction.UserId == p.UserId))
            {
                session.Store(prediction);
            }

            session.SaveChanges();

            return prediction;
        } 


    }
}
