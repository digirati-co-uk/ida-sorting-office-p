using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IdaSortingOffice.Controllers
{
    public class BaseController : Controller
    {
        private static string ManifestJson;
        private static string SimpleManifestJson;
        private static string CollectionJson;

        protected JObject GetManifestTemplate()
        {
            if (ManifestJson == null)
            {
                ManifestJson = GetJsonFromTemplate("~/App_Data/manifest-template.json");
            }
            return GetJsonTemplate(ManifestJson);
        }

        protected JObject GetCollectionTemplate()
        {
            if (CollectionJson == null)
            {
                CollectionJson = GetJsonFromTemplate("~/App_Data/collection-template.json");
            }
            return GetJsonTemplate(CollectionJson);
        }

        protected JObject GetSimpleManifestTemplate()
        {
            if (SimpleManifestJson == null)
            {
                SimpleManifestJson = GetJsonFromTemplate("~/App_Data/simple-manifest-template.json");
            }
            return GetJsonTemplate(SimpleManifestJson);

        }

        private JObject GetJsonTemplate(string path)
        {
            string json = GetJsonFromTemplate(path);
            return JObject.Parse(json);
        }

        private string GetJsonFromTemplate(string path)
        {
            var templateFile = HttpContext.Server.MapPath(path);
            var json = System.IO.File.ReadAllText(templateFile);
            return json;
        }
    }
}