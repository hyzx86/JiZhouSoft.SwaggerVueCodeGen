using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JZSoft.AbpVueCodeGen.Mvc.Models;
using JZSoft.CodeGen;
using Newtonsoft.Json.Linq;

namespace JZSoft.AbpVueCodeGen.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            //if (ApiList.Count == 0)
            //{
            //    ApiList = GetApiListAsync().Result;
            //}

        }
         
        public IActionResult GetCodeFromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return View();
            }
            JObject jObject = JObject.Parse(json);
            return View(jObject);
        }

        public static List<ApiModel> ApiList { get; set; } = new List<ApiModel>();
        public static ConvertSwaggerApiDefService service { get; set; }
        public async Task<List<ApiModel>> GetApiListAsync()
        {
            service = await ConvertSwaggerApiDefService.
                   FromUrlAsync("http://localhost:21021/swagger/v1/swagger.json");
            ApiList = await service.GetApiListAsync();
            return ApiList;

        }

        public IActionResult Index()
        {
            var ApiModels = ApiList.OrderBy(o => o.Methods.FirstOrDefault().Tags).ToList();
            return View(ApiModels);
        } 
        public JsonResult ConfigGenCode(string tagName)
        {
            var config = new ConfigGenCodeModel();
            config.TagName = tagName;
            List<ModelDtoDef> modelDtoDefs = new List<ModelDtoDef>();
            List<ApiModel> apiModels = new List<ApiModel>();
            foreach (var item in ApiList.Where(o => o.Methods.FirstOrDefault().Tags == tagName))
            {
                foreach (var method in item.Methods)
                {
                    foreach (var parameters in method.Parameters)
                    {
                        if (parameters.IsComplexModel)
                        {
                            modelDtoDefs.Add(parameters.InputModel);
                        }
                    }
                    var res = method.Reponse;
                    if (res != null && res.IsComplexModel)
                    {
                        modelDtoDefs.Add(res.ResultModel);
                    }
                }
                apiModels.Add(item);
            }
            config.DtoDefList = modelDtoDefs.Distinct().ToList();
            config.ApiModels = apiModels;
            return new JsonResult(config);
        }
        public IActionResult GenCode(GenCodeInput input)
        {
            var configData = new GenCodeOutput();
            configData.Config = input;
            configData.CreateApi = ApiList.FirstOrDefault(o => o.Path == input.CreateMethod);
            configData.DeleteApi = ApiList.FirstOrDefault(o => o.Path == input.DeleMethod);
            configData.UpdateApi = ApiList.FirstOrDefault(o => o.Path == input.UpdateMethod);
            configData.ListApi = ApiList.FirstOrDefault(o => o.Path == input.ListMethod);
            Response.ContentType = "text/plian;charset=utf-8";
            return View(configData);
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
