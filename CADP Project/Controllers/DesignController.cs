using CADP_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CADP_Project.Controllers
{
    [Authorize]
    public class DesignController : Controller
    {
        private CADPEntities db = new CADPEntities();

        public ActionResult MyDesigns()
        {
            string username = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var designs = (from d in db.Designs
                           join f in db.Fabrics on d.FabricId equals f.FabricId
                           where d.UserId == user.UserId
                           select new DesignViewModel
                           {
                               DesignId = d.DesignId,
                               DesignName = d.DesignName,
                               FabricName = f.FabricName,
                               FabricColor = f.Color
                           }).ToList();

            return View(designs);
        }

        public ActionResult ViewDesign(int id)
        {
            using (CADPEntities db = new CADPEntities())
            {
                var design = (from d in db.Designs
                              join f in db.Fabrics on d.FabricId equals f.FabricId
                              where d.DesignId == id
                              select new DesignDetailViewModel
                              {
                                  DesignId = d.DesignId,
                                  DesignName = d.DesignName,
                                  DesignData = d.DesignData,
                                  FabricName = f.FabricName,
                                  FabricColor = f.Color
                              }).FirstOrDefault();

                if (design == null)
                {
                    return Json(new { success = false, message = "Design not found" }, JsonRequestBehavior.AllowGet);
                }

                return Json(design, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteDesign(int id)
        {
            var design = db.Designs.Find(id);
            if (design != null)
            {
                db.Designs.Remove(design);
                db.SaveChanges();
                return Json(new { success = true, message = "Design deleted successfully!" });
            }
            return Json(new { success = false, message = "Design not found!" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class DesignViewModel
    {
        public int DesignId { get; set; }
        public string DesignName { get; set; }
        public string FabricName { get; set; }
        public string FabricColor { get; set; }
    }

    public class DesignDetailViewModel
    {
        public int DesignId { get; set; }
        public string DesignName { get; set; }
        public string DesignData { get; set; }
        public string FabricName { get; set; }
        public string FabricColor { get; set; }
    }
}