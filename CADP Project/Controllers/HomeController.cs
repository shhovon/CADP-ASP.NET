using CADP_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CADP_Project.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult Design()
        {
            using (CADPEntities db = new CADPEntities())
            {
                var fabrics = db.Fabrics.ToList();
                return View(fabrics);
            }
        }


        [HttpPost]
        public ActionResult SaveDesign(string designData, int fabricId, string designName)
        {
            using (CADPEntities db = new CADPEntities())
            {
                var user = db.Users.FirstOrDefault();
                if (user == null)
                {
                    return new HttpStatusCodeResult(401, "User not logged in");
                }
                var design = new Design
                {
                    DesignName = designName,
                    DesignData = designData,
                    UserId = user.UserId,
                    FabricId = fabricId
                };
                db.Designs.Add(design);
                db.SaveChanges();
                return Json(new { success = true, message = "Design saved successfully!" });
            }
        }

        public ActionResult AddFabric()
        {
            using (CADPEntities db = new CADPEntities())
            {
                var fabrics = db.Fabrics.ToList();
                return View(fabrics); 
            }
        }

        [HttpPost]
        public ActionResult AddFabric(Fabric model)
        {
            if (ModelState.IsValid)
            {
                using (CADPEntities db = new CADPEntities())
                {
                    db.Fabrics.Add(model);
                    db.SaveChanges();
                    ViewBag.Message = "Fabric added successfully!";
                }

                return RedirectToAction("AddFabric");
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult DeleteFabric(int fabricId)
        {
            using (CADPEntities db = new CADPEntities())
            {
                var fabric = db.Fabrics.Find(fabricId);
                if (fabric != null)
                {
                    db.Fabrics.Remove(fabric);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Fabric deleted successfully!" });
                }
            }
            return Json(new { success = false, message = "Fabric not found!" });
        }
    }
}