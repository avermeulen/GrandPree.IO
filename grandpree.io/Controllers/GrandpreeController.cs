using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using grandpree.io.Models;

namespace grandpree.io.Controllers
{
    public class GrandpreeController : RavenController
    {
        
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public object Drivers()
        {
            var drivers = session.Query<Driver>().ToList();
            return Json(drivers, JsonRequestBehavior.AllowGet);
        }
        
        public object Races()
        {
            var races = session.Query<Race>().ToList();
            return Json(races, JsonRequestBehavior.AllowGet);
        }
        
        public object Context()
        {
            var races = session.Query<Race>().ToList();
            var drivers = session.Query<Driver>().ToList();
            var users = session.Query<User>().ToList();

            return Json(new
                {
                    races, 
                    drivers,
                    users
                }, JsonRequestBehavior.AllowGet);
        }

        public object PastRaces()
        {
            var races = session.Query<Race>().Where(r => r.StartTime < DateTime.Today).ToList();
            return Json(races, JsonRequestBehavior.AllowGet);
        }

        public object UpcomingRaces(int inDays)
        {
            var races = session.Query<Race>()
                .Where(r => r.StartTime > DateTime.Today && r.StartTime <= DateTime.Today.AddDays(inDays)).ToList();
            return Json(races, JsonRequestBehavior.AllowGet);
        }

    }

    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class Prediction
    {
        public string Id { get; set; }
        public string RaceId { get; set; }
        public string UserId { get; set; }
        public IEnumerable<PredictionLine> Predictions { get; set; }
    }

    public class PredictionLine
    {
        
        public string DriverId { get; set; }
        public int Podium { get; set; }
        public int Retire { get; set; }
        public int Grid { get; set; }
        public int Total { get; set; }
    }
}
