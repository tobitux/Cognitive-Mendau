using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MendauPrediction.Web.Models;

namespace MendauPrediction.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Upload");
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

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadDocument()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/Upload"), fileName);
                    file.SaveAs(path);

                    var prediction = new Prediction();
                    var result = prediction.Predict(path);

                    var vm = new UploadDocumentViewModel();
                    vm.Predictions = new List<Models.Prediction>();
                    foreach (var p in result)
                    {
                        vm.Predictions.Add(p);
                    }


                    return View(vm);
                }
            }

            return View();
        }
    }
}