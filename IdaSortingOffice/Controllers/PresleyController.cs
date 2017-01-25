using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace IdaSortingOffice.Controllers
{
    class PresleyOperation
    {
        public string HttpMethod;
        public string Name;
        public string[] Params;
    }

    public class PresleyController : BaseController
    {
        const string PathRoot = "~/App_Data/presley/ida/";

        private System.IO.FileInfo GetCollectionFile(string collectionName)
        {
            return new System.IO.FileInfo(HttpContext.Server.MapPath(
                PathRoot + "collections/" + collectionName + ".json"));
        }
        private System.IO.FileInfo GetManifestFile(string identifier)
        {
            return new System.IO.FileInfo(HttpContext.Server.MapPath(
                PathRoot + "manifests/" + identifier + ".json"));
        }


        /// <summary>
        /// Bypass MVC routing to think about Django paths
        /// </summary>
        private static PresleyOperation Resolve(string httpMethod, Uri url)
        {
            var routes = new Dictionary<string, Regex>();
            routes.Add("Collection", new Regex(@"presley/ida/collection/([^\/]*)"));
            routes.Add("Manifest", new Regex(@"presley/ida/([^\/]*)/manifest"));
            routes.Add("Range", new Regex(@"presley/ida/([^\/]*)/range/([^\/]*)")); // same for canvas etc..
            // Presley doesn't need to store ranges in "actual" containers as long as the URI is unique.
            // The fact that the URI has two varying parts doesn't really matter as long as the whole matches the table? 
            foreach (var kvp in routes)
            {
                var m = kvp.Value.Match(url.AbsolutePath);
                if (m.Success)
                {
                    return new PresleyOperation
                    {
                        HttpMethod = httpMethod,
                        Name = kvp.Key,
                        Params = m.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray()
                    };
                }
            }
            return null;
        }


        // GET: Presley
        public ActionResult PresleyOp()
        {
            // For now we won't support Collections that are not also containers of manifests.
            /*
             * One issue, but not a showstopper for MVP. Although a container of manifests 
             * in Presley could return a IIIF Collection as a response, Presley can have 
             * IIIF Collections that are not containers (parents) of manifests as far as URLs 
             * go. PUT a manifest to /ida/manifests/manifest1 - if you deref /ida/manifests you 
             * get a collection, but you might also want to tell Presley that manifest1 is in 
             * /ida/collections/coll1 and /ida/collections/coll2 - you don't do that by PUTting 
             * the manifest into them as well, you might do that by PATCHing a collection, or 
             * POSTing a manifest summary (e.g., just { @id, @type, label } to the collection, 
             * etc. For immediate sorty MVP we can just only support one Collection for a manifest 
             * to live in, which is also its container.
               Need to read the LDP spec some more...
               Also we are starting to solve IIIF Presentation REST here, but good to get a foot in the ground 
            */

            var op = Resolve(Request.HttpMethod, Request.Url);
            if(op == null)
            {
                return new HttpNotFoundResult();
            }

            Response.Headers["Access-Control-Allow-Origin"] = "*";

            if (op.Name == "Collection")
            {
                var collectionName = op.Params[0];
                var collFile = GetCollectionFile(collectionName);
                if (op.HttpMethod == "GET")
                {
                    return GetCollection(collFile);
                }
                if (op.HttpMethod == "POST")
                {
                    return PostToCollection(collFile);
                }
            }

            if(op.Name == "Manifest")
            {
                var identifier = op.Params[0];
                var manifestFile = GetManifestFile(identifier);
                if (op.HttpMethod == "GET")
                {
                    return GetManifest(manifestFile);
                }
                if (op.HttpMethod == "PUT")
                {
                    return PutManifest(manifestFile);
                }
            }

            throw new HttpException(501, "Not Implemented");
            
        }

        private ActionResult PutManifest(System.IO.FileInfo manifestFile)
        {
            // look for mint new ids service
            // mint new ids
            // make a canvasmap
            // save the manifest with its canvasmap

            var manifest = GetRequestBodyAsJObject();
            manifest["@id"] = Request.Url.AbsoluteUri;
            var services = new JArray();
            if (manifest["service"] != null)
            {
                if (manifest["service"] is JArray)
                {
                    services = (JArray)manifest["service"];
                }
                else
                {
                    services.Add(manifest["service"]);
                }
            }
            var mintServices = services.Where(s => s["profile"].ToString() == "https://dlcs.info/profiles/mintrequest").ToList();
            if (mintServices.Any())
            {
                foreach (var mintService in mintServices)
                {
                    services.Remove(mintService);
                }
                var canvases = manifest["sequences"][0]["canvases"] as JArray;
                var canvasmap = new JObject();
                foreach (var canvas in canvases)
                {
                    var uriAuthority = Request.Url.GetLeftPart(UriPartial.Authority);
                    var newCanvasId = uriAuthority + "/canvases/" + Guid.NewGuid().ToString("N");
                    canvasmap[newCanvasId] = canvas["@id"].ToString();
                    canvas["@id"] = newCanvasId;
                }
                var mapService = new JObject();
                mapService["@id"] = manifest["@id"] + "/canvasmap";
                mapService["@context"] = "https://dlcs.info/context/presley";
                mapService["profile"] = "https://dlcs.info/profiles/canvasmap";
                mapService["canvasmap"] = canvasmap;
                services.Add(mapService);
            }
            manifest["service"] = null;
            if (services.Any())
            {
                if (services.Count == 1)
                {
                    manifest["service"] = services[0];
                }
                else
                {
                    manifest["service"] = services;
                }
            }

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            if (!manifestFile.Exists)
            {
                Response.StatusCode = 201;
            }
            System.IO.File.WriteAllText(manifestFile.FullName, json, Encoding.UTF8);
            return Content(json, "application/json");
        }

        private ActionResult GetManifest(System.IO.FileInfo manifestFile)
        {
            if (manifestFile.Exists)
            {
                var json = System.IO.File.ReadAllText(manifestFile.FullName);
                return Content(json, "application/json");
            }
            else
            {
                return HttpNotFound("Could not find requested manifest");
            }
        }

        private ActionResult PostToCollection(System.IO.FileInfo collFile)
        {
            // we are putting a manifest into a collection - but this is not containment, it's association.
            // The JSON-LD object posted is just the @id
            var postedObject = GetRequestBodyAsJObject();
            // require for now it looks like this:
            // { "@id": ... , "@type": "sc:Manifest" }
            // we can relax this a bit later...
            var asUri = new Uri(postedObject["@id"].ToString());
            var mfOp = Resolve("POST", asUri);
            if (mfOp != null)
            {
                var filePath = GetManifestFile(mfOp.Params[0]);
                JArray manifestList = new JArray();
                if (collFile.Exists)
                {
                    manifestList = JArray.Parse(System.IO.File.ReadAllText(collFile.FullName));
                }
                if(manifestList.All(m => m.ToString() != filePath.FullName))
                {
                    manifestList.Add(filePath.FullName);
                    var json = JsonConvert.SerializeObject(manifestList, Formatting.Indented);
                    System.IO.File.WriteAllText(collFile.FullName, json, Encoding.UTF8);
                }
                Response.StatusCode = 202;
                Response.StatusDescription = "Accepted";
                return Content("\"Accepted\"", "application/json");
            }
            Response.StatusCode = 400;
            Response.StatusDescription = "Only Presley manifests for now in this mockup";
            return new EmptyResult();
        }

        private ActionResult GetCollection(System.IO.FileInfo collFile)
        {
            if (collFile.Exists)
            {
                var collList = JArray.Parse(System.IO.File.ReadAllText(collFile.FullName));
                var collection = GetCollectionTemplate();
                collection["@id"] = Request.Url.AbsoluteUri;
                var members = new JArray();
                foreach (var manifestPath in collList)
                {
                    var member = GetSimpleManifestTemplate();
                    var memberManifest = JObject.Parse(System.IO.File.ReadAllText(manifestPath.ToString()));
                    member["@id"] = memberManifest["@id"];
                    member["label"] = memberManifest["label"];
                    members.Add(member);
                }
                collection["members"] = members;
                return Content(collection.ToString(Formatting.Indented), "application/json");
            }
            else
            {
                return HttpNotFound("Could not find requested container");
            }
        }
    }
}
 