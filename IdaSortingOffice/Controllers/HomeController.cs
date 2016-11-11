using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CoreTechs.Common.Text;
using IdaSortingOffice.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IdaSortingOffice.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(GetCachedRolls());
        }

        public ActionResult Roll()
        {
            var id = Request.Url.AbsolutePath.Substring("roll/".Length + 1);
            var roll = GetCachedRolls().FirstOrDefault(r => r.Id == id);
            var manifestJson = GetDlcsRollJson(roll.DlcsManifest);
            var jo = JObject.Parse(manifestJson);
            EnhanceManifest(jo, roll);
            return new ContentResult
            {
                Content = jo.ToString(Formatting.Indented),
                ContentType = "application/json"
            };
        }

        private void EnhanceManifest(JObject manifest, Roll roll)
        {
            Dictionary<string, JToken> ocr = null; 
            if (roll.OcrData != null)
            {
                var file = HttpContext.Server.MapPath("~/App_Data/ocrdata/" + roll.OcrData);
                var ocrArray = JToken.Parse(System.IO.File.ReadAllText(file));
                ocr = ocrArray.ToDictionary(ocrInfo => ocrInfo["id"].ToString());
            }

            var manifestId = Request.Url.AbsoluteUri;
            manifest["@id"] = manifestId;
            manifest["metadata"][0]["value"] = string.Format(
                "{0} {1} {2} {3}", roll.Title, roll.Dates, roll.State, roll.PubNo);
            var canvases = manifest["sequences"][0]["canvases"];
            foreach (var canvas in canvases)
            {
                // dirty...
                const string n2param = "&n2=";
                var canvasId = canvas["@id"].ToString();
                var n2 = canvasId.Substring(canvasId.IndexOf(n2param) + n2param.Length);
                var newCanvasId = string.Format("{0}canvas/{1}", manifestId, n2);
                canvas["@id"] = newCanvasId;
                canvas["images"][0]["on"] = newCanvasId;

                if (ocr != null)
                {
                    var key = canvas["images"][0]["resource"]["service"]["@id"].ToString();
                    if (ocr.ContainsKey(key))
                    {
                        var ocrInfo = ocr[key];
                        ocrInfo["@context"] = "https://dlcs-ida.org/ocr-info";
                        canvas["service"] = ocrInfo;
                    }
                }
            }
        }

        private void DecorateManifest(JObject manifest, string ocrData)
        {
            // todo
        }

        private string GetDlcsRollJson(string url)
        {
            string key = "roll-" + url;
            string json = (string)HttpContext.Cache.Get(key);
            if (json == null)
            {
                using (var wc = new WebClient())
                {
                    json = wc.DownloadString(url);
                }
                HttpContext.Cache.Insert(key, json, null, DateTime.UtcNow.AddHours(1), 
                    System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return json;
        }

        private List<Roll> GetCachedRolls()
        {
            const string key = "rolls";
            var rolls = HttpContext.Cache.Get(key);
            if (rolls == null)
            {
                rolls = GetRolls();
                HttpContext.Cache.Insert(key, rolls, null, DateTime.UtcNow.AddMinutes(1),
                    System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return (List<Roll>)rolls;
        } 

        private List<Roll> GetRolls()
        {
            var ocrData = new DirectoryInfo(HttpContext.Server.MapPath("~/App_Data/ocrdata"));
            var ocrFiles = ocrData.GetFiles().Select(fi => fi.Name).ToArray();
            const string prefix = "https://dlcs-ida.org/iiif-resource/ida/roll/";
            var rolls = new List<Roll>();
            using (TextReader csvReader = System.IO.File.OpenText(HttpContext.Server.MapPath("~/App_Data/rolls.csv")))
            {
                foreach (var line in csvReader.ReadCsv())
                {
                    var roll = new Roll
                    {
                        Title = line[0],
                        Dates = line[1],
                        Number = line[2],
                        State = line[3],
                        PubNo = line[4],
                        DlcsManifest = line[5],
                        Notes = line[6]
                    };
                    if (roll.DlcsManifest.StartsWith(prefix))
                    {
                        roll.Id = roll.DlcsManifest.Substring(prefix.Length);
                        roll.OcrData = roll.Id.Trim('/').Replace("/", "_R-") + ".json";
                        if (!ocrFiles.Contains(roll.OcrData))
                        {
                            roll.OcrData = null;
                        }
                    }
                    rolls.Add(roll);
                }
            }
            return rolls;
        }

        
    }
}