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

        public Prediction GetPredictionByUserNameAndRaceId(string userName, string raceId)
        {
            using (var session = documentStore.Value.OpenSession())
            {
                    return session.Query<Prediction>().FirstOrDefault(p => 
                            p.RaceId == raceId 
                            && userName == p.UserId);
            }

        }

        public Prediction PostPrediction(Prediction predictionParam)
        {

            
            var prediction = GetPredictionByUserNameAndRaceId(predictionParam.UserId, predictionParam.RaceId);

            using (var session = documentStore.Value.OpenSession())
            {
                if (prediction == null)
                {
                    session.Store(predictionParam);
                }
                else
                {
                    session.Store(predictionParam, prediction.Id);
                }
                session.SaveChanges();
            }

            return predictionParam;
        } 


    }
}
