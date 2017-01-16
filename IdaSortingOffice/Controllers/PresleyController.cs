using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var manifestPath = HttpContext.Server.MapPath(root + containerName + ".json");
                if(System.IO.File.Exists(manifestPath))
                {
                    return new FilePathResult(manifestPath, "application/json");
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
            }
            throw new HttpException(501, "Not Implemented required");
        }        
    }
}
 