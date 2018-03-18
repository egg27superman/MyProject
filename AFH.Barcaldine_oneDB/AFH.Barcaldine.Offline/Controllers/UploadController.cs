using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

using AFH.Common.Serializer;
using AFH.Barcaldine.Models;

namespace AFH.Barcaldine.Offline.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/

        public ActionResult Upload()
        {
            //UploadModel uploads = new UploadModel();
            //uploads.FileName = new List<string>();

            //for (int i = 0; i < Request.Files.Count; i++)
            //{
            
            string fileName = string.Empty;
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                fileName = "Uploads/" + DateTime.Now.ToString("yyyyMMddhhmmss") + file.FileName;
                
                //uploads.FileName.Add(fileName);

                file.SaveAs(AppDomain.CurrentDomain.BaseDirectory + fileName);
                //file.SaveAs(fileName);
            }
            //}

            //JsonSerializer json = new JsonSerializer();
            //string returnJson = json.Serialize<UploadModel>(uploads);

            return Json(ConfigurationManager.AppSettings["ImageUpload"].ToString() + fileName, JsonRequestBehavior.AllowGet);
        }

    }
}
