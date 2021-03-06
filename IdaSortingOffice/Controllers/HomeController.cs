﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
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
    public class HomeController : BaseController
    {
        protected class SiteCredentials
        {
            public string Prefix;
            public string Username;
            public string Password;
        }

        private static readonly object _siteCredentialsCacheLock = new object();
        protected static List<SiteCredentials> SiteCredentialsCache = null;

        [Authorize]
        public ActionResult Index()
        {
            return View(GetCachedRolls());
        }

        public ActionResult RollCollection()
        {
            SetManifestHeaders();
            var collection = GetCollectionTemplate();
            collection["@id"] = Request.Url.AbsoluteUri;
            var tableService = new JObject
            {
                ["profile"] = "table-profile-here",
                ["headers"] = new JArray("Id", "Title", "Dates", "Number", "State", "PubNo", "Notes")
            };
            collection["service"] = tableService;
            var rolls = GetCachedRolls();
            var prefix = Request.Url.GetLeftPart(UriPartial.Authority) + "/roll/";
            var orderedRolls = new JObject[rolls.Count];
            int index = 5;
            foreach (var roll in rolls)
            {
                var member = GetSimpleManifestTemplate();
                member["@id"] = prefix + roll.Id;
                var rowService = new JObject
                {
                    ["profile"] = "row-profile-here",
                    ["values"] = new JArray(roll.Id, roll.Title, roll.Dates, roll.Number, roll.State, roll.PubNo, roll.Notes),
                    ["highlight"] = roll.OcrData == null ? "" : "info"
                };
                member["service"] = rowService;
                member["label"] = $"{roll.Title}, {roll.Dates} | {roll.Number} {roll.State} | {roll.PubNo}";
                switch (roll.Id)
                {
                    case "M-1011/127/":
                        orderedRolls[0] = member;
                        break;
                    case "M-1473/24/":
                        orderedRolls[1] = member;
                        break;
                    case "T-21/02/":
                        orderedRolls[2] = member;
                        break;
                    case "M-941/03/":
                        orderedRolls[3] = member;
                        break;
                    case "M-1304/25/":
                        orderedRolls[4] = member;
                        break;
                    default:
                        orderedRolls[index++] = member;
                        break;
                }
            }
            var members = new JArray();
            foreach (JObject roll in orderedRolls)
            {
                if(roll != null)
                {
                    // TODO - if there were dupes in rolls, there will be fewer in orderedRolls than rolls.
                    members.Add(roll);
                }
            }
            collection["members"] = members;
            return Content(collection.ToString(Formatting.Indented), "application/json");
        }

        [Authorize]
        public ActionResult SortingOffice()
        {
            return View();
        }

        public ActionResult Roll()
        {
            var roll = GetRoll(Request.Url);
            var manifestJson = GetCachedManifest(roll.DlcsManifest);
            var jo = JObject.Parse(manifestJson);
            EnhanceManifest(jo, roll);
            SetManifestHeaders();
            return Content(jo.ToString(Formatting.Indented), "application/json");
        }

        private void SetManifestHeaders()
        {
            Response.Headers["Access-Control-Allow-Origin"] = "*";
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetMaxAge(TimeSpan.FromMinutes(30));
            Response.Cache.SetSlidingExpiration(true);
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
            SetManifestHeaders();
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
            manifest["sequences"][0]["@id"] = manifestId + "sequence";
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
                LoadSiteCredentials();

                using (var wc = new WebClient())
                {
                    SetWebClientAuthentication(wc, url);
                    json = wc.DownloadString(url);
                }
                HttpContext.Cache.Insert(key, json, null, DateTime.UtcNow.AddHours(1), 
                    System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return json;
        }

        private void SetWebClientAuthentication(WebClient wc, string url)
        {
            SiteCredentials credentials =
                SiteCredentialsCache.FirstOrDefault(
                    sc => url.StartsWith(sc.Prefix, StringComparison.InvariantCultureIgnoreCase));

            if (credentials == null)
            {
                return;
            }

            wc.Credentials = new NetworkCredential(credentials.Username, credentials.Password);
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

        private void LoadSiteCredentials()
        {
            if (SiteCredentialsCache == null)
            {
                lock (_siteCredentialsCacheLock)
                {
                    if (SiteCredentialsCache != null)
                    {
                        return;
                    }

                    SiteCredentialsCache = new List<SiteCredentials>();
                    DirectoryInfo di = new DirectoryInfo(HttpContext.Server.MapPath("~"));
                    string json = System.IO.File.ReadAllText(Path.Combine(di.FullName, ConfigurationManager.AppSettings["CredentialsFile"]));
                    JObject j = JObject.Parse(json);
                    foreach (JToken token in j["credentials"].Children())
                    {
                        SiteCredentials credential = new SiteCredentials
                        {
                            Prefix = token.Value<string>("prefix"),
                            Username = token.Value<string>("username"),
                            Password = token.Value<string>("password")
                        };

                        SiteCredentialsCache.Add(credential);
                    }
                }
            }            
        }
    }
}