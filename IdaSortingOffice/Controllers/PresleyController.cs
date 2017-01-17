using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace IdaSortingOffice.Controllers
{
    public class PresleyController : BaseController
    {
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

            // GET Containers and Manifests, PUT Manifests.
            const string root = "~/App_Data/presley/ida/";
            Response.Headers["Access-Control-Allow-Origin"] = "*";
            var pathPart = Request.Url.AbsolutePath.Substring("presley/ida/".Length + 1);
            var parts = pathPart.Split('/');
            var containerName = parts[0];
            var manifestName = parts.Length > 1 ? parts[1] : null;
            if(manifestName == null && Request.HttpMethod == "GET")
            {
                // request for the CONTAINER
                // enumerate the manifests within and return as IIIF Collection
                var folder = HttpContext.Server.MapPath(root + containerName);
                var di = new System.IO.DirectoryInfo(folder);
                if (di.Exists)
                {
                    var collection = GetCollectionTemplate();
                    var members = new JArray();
                    foreach(var fi in di.GetFiles())
                    {
                        var member = GetSimpleManifestTemplate();
                        member["@id"] = Request.Url.AbsoluteUri + "/" + fi.Name.Replace(".json", "");
                        member["label"] = null; // come back to this; need to read each file...
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
            if(manifestName != null && Request.HttpMethod == "GET")
            {
                var manifestPath = HttpContext.Server.MapPath(root + containerName + "/" + manifestName + ".json");
                if(System.IO.File.Exists(manifestPath))
                {

                    var json = System.IO.File.ReadAllText(manifestPath);
                    return Content(json, "application/json");
                }
                else
                {
                    return HttpNotFound("Could not find requested manifest");
                }
            }
            if (manifestName != null && Request.HttpMethod == "PUT")
            {
                // look for mint new ids service
                // mint new ids
                // make a canvasmap
                // save the manifest with its canvasmap

                var folder = HttpContext.Server.MapPath(root + containerName);
                var di = new System.IO.DirectoryInfo(folder);
                if (!di.Exists) di.Create();
                var manifest = GetRequestBodyAsJObject();
                manifest["@id"] = Request.Url.AbsoluteUri;
                var services = new JArray();
                if(manifest["service"] != null)
                {
                    if(manifest["service"] is JArray)
                    {
                        services = (JArray)manifest["service"];
                    }
                    else
                    {
                        services.Add(manifest["service"]);
                    }
                }
                var mintServices = services.Where(s => s["profile"].ToString() == "https://dlcs.info/profiles/mintrequest").ToList();
                if(mintServices.Any())
                {
                    foreach(var mintService in mintServices)
                    {
                        services.Remove(mintService);
                    }
                    var canvases = manifest["sequences"][0]["canvases"] as JArray;
                    var canvasmap = new JObject();
                    foreach(var canvas in canvases)
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
                if(services.Any())
                {
                    if(services.Count == 1)
                    {
                        manifest["service"] = services[0];
                    }
                    else
                    {
                        manifest["service"] = services;
                    }
                }

                var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
                var target = HttpContext.Server.MapPath(root + containerName + "/" + manifestName + ".json");
                if (!System.IO.File.Exists(target))
                {
                    Response.StatusCode = 201;
                }
                System.IO.File.WriteAllText(target, json, Encoding.UTF8);
                return Content(json, "application/json");
            }
            throw new HttpException(501, "Not Implemented required");
        }        
    }
}
 