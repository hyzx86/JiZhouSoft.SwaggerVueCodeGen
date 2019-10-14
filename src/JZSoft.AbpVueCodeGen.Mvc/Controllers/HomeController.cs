using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JiZhouSoft.SwaggerVueCodeGen.Models; 
using Newtonsoft.Json.Linq;
namespace JiZhouSoft.SwaggerVueCodeGen.Controllers
{
    public class HomeController : Controller
    {
        public static string ApiJsonUrl { get; set; } 

        public IActionResult Index()
        { 
            return View();
        }
        public IActionResult GenCode(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return View();
            }
            JObject jObject = JObject.Parse(json);
            Response.ContentType = "text/plian;charset=utf-8";
            return View(jObject);
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
