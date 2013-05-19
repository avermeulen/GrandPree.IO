using System;
using System.Web.Mvc;
using Raven.Client;
using Raven.Client.Document;

namespace grandpree.io.Controllers
{
    public class RavenController : Controller
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

        protected IDocumentSession session;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            session = documentStore.Value.OpenSession();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            session.SaveChanges();
        }
    }
}