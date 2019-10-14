using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace JZSoft.AbpVueCodeGen.Mvc.Controllers
{
    public class GenCodeController : Controller
    {
        public IActionResult Index(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return View();
            }
            JObject jObject = JObject.Parse(json);
            Response.ContentType = "text/plian;charset=utf-8";
            return View(jObject);
        } 
    }
}