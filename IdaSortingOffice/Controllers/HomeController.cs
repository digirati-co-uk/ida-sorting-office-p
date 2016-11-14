using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
            var roll = GetRoll(Request.Url);
            var manifestJson = GetCachedManifest(roll.DlcsManifest);
            var jo = JObject.Parse(manifestJson);
            EnhanceManifest(jo, roll);
            Response.Headers["Access-Control-Allow-Origin"] = "*";
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetMaxAge(TimeSpan.FromMinutes(30));
            return Content(jo.ToString(Formatting.Indented), "application/json");
        }

        private Roll GetRoll(Uri uri)
        {
            Roll roll = null;
            var uriAuthority = uri.GetLeftPart(UriPartial.Authority);
            var requestAuthority = Request.Url.GetLeftPart(UriPartial.Authority);
            if (uriAuthority == requestAuthority && uri.AbsolutePath.StartsWith("/roll/"))
            {
                var id = uri.AbsolutePath.Substring("/roll/".Length);
                roll = GetCachedRolls().FirstOrDefault(r => r.Id == id);
            }
            return roll;
        }

        public ActionResult Manifest()
        {
            var urlPath = Request.Url.AbsolutePath.Substring("manifest/".Length + 1);
            var rangeFile = HttpContext.Server.MapPath("~/App_Data/ranges/" + urlPath + ".json");
            if (!System.IO.File.Exists(rangeFile))
            {
                return HttpNotFound("Could not find stored range");
            }
            var json = System.IO.File.ReadAllText(rangeFile);
            var range = JsonConvert.DeserializeObject<RangeData>(json);

            // first get the source manifest
            var manifestJson = GetCachedManifest(range.ManifestId);
            var sourceManifest = JObject.Parse(manifestJson);
            // Now take the chosen canvases out of the source manifest and insert them into our template
            var newManifest = GetManifestTemplate();
            var sourceCanvases = sourceManifest["sequences"][0]["canvases"];
            var newCanvases = new JArray();
            bool adding = false;
            foreach (var canvas in sourceCanvases)
            {
                if (canvas["@id"].ToString() == range.StartCanvas)
                {
                    adding = true;
                }
                if (canvas["@id"].ToString() == range.EndCanvas)
                {
                    adding = false;
                    newCanvases.Add(canvas);
                }
                if (adding)
                {
                    newCanvases.Add(canvas);    
                }
            }
            newManifest["@id"] = Request.Url.AbsoluteUri;
            newManifest["sequences"][0]["@id"] = Request.Url.AbsoluteUri + "/sequence";
            newManifest["sequences"][0]["canvases"] = newCanvases;
            newManifest["label"] = range.Label;
            newManifest["metadata"] = new JArray { Metadata("Type", range.UnitType) };
            Response.Headers["Access-Control-Allow-Origin"] = "*";
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetMaxAge(TimeSpan.FromMinutes(30));
            return Content(newManifest.ToString(Formatting.Indented), "application/json");
        }

        private JObject Metadata(string label, string value)
        {
            return new JObject
            {
                ["label"] = label,
                ["value"] = value
            };
        }

        private JObject GetManifestTemplate()
        {
            var templateFile = HttpContext.Server.MapPath("~/App_Data/manifest-template.json");
            var json = System.IO.File.ReadAllText(templateFile);
            return JObject.Parse(json);
        }

        private DirectoryInfo GetDirectoryForRanges(string manifestId)
        {
            var folder = manifestId.GetFileSafeName();
            return new DirectoryInfo(HttpContext.Server.MapPath("~/App_Data/ranges/" + folder));
        }

        [HttpPost]
        public ActionResult SaveRange(RangeData range)
        {
            if (string.IsNullOrWhiteSpace(range.ManifestId) 
                || string.IsNullOrWhiteSpace(range.StartCanvas) 
                || string.IsNullOrWhiteSpace(range.EndCanvas))
            {
                throw new ApplicationException("Missing range data");
            }
            var di = GetDirectoryForRanges(range.ManifestId);
            int index = 1;
            if (di.Exists)
            {
                var existingFileNames = di.GetFiles().Select(fi => fi.Name.Replace(".json", "")).OrderBy(s => s);
                var max = existingFileNames.LastOrDefault();
                if (max != null)
                {
                    index = int.Parse(max) + 1;
                }
            }
            else
            {
                di.Create();
            }
            var rangeFilename = index.ToString("D4");
            range.Id = string.Format("{0}/{1}", di.Name, rangeFilename);
            range.RangeManifestId = Request.Url.GetLeftPart(UriPartial.Authority) + "/manifest/" + range.Id;
            var json = JsonConvert.SerializeObject(range, Formatting.Indented);
            var target = Path.Combine(di.FullName, rangeFilename + ".json");
            System.IO.File.WriteAllText(target, json, Encoding.UTF8);
            return Content(json, "application/json");
        }

        public ActionResult Ranges(string id)
        {
            var ranges = new List<RangeData>();
            var di = GetDirectoryForRanges(id);
            if (di.Exists)
            {
                foreach (var file in di.GetFiles())
                {
                    var data = System.IO.File.ReadAllText(file.FullName);
                    ranges.Add(JsonConvert.DeserializeObject<RangeData>(data));
                }
            }
            return Content(
                JsonConvert.SerializeObject(ranges.ToArray(), Formatting.Indented), 
                "application/json");
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
                const string n2Param = "&n2=";
                var canvasId = canvas["@id"].ToString();
                var n2 = canvasId.Substring(canvasId.IndexOf(n2Param) + n2Param.Length);
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
        
        private string GetCachedManifest(string url)
        {
            // Not suitable for a production system, will run out of memory...
            string key = "manifest-" + url;
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