using JZSoft.CodeGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JZSoft.AbpVueCodeGen.Mvc.Models
{
    public class GenCodeOutput
    {
        public GenCodeInput Config { get; set; }
        public ApiModel ListApi { get; set; }
        public ApiModel CreateApi { get; set; }
        public ApiModel DeleteApi { get; set; }
        public ApiModel UpdateApi { get; set; }

        public List<ModelDtoDef> DtoList
        {
            get
            {
                var apiList = new ApiModel[] { ListApi, CreateApi, DeleteApi, UpdateApi };
                var dtoList = new List<ModelDtoDef>();
                foreach (var item in apiList)
                {
                    var modelDto = new ModelDtoDef();
                    var m = item.Methods.FirstOrDefault();
                    foreach (var p in m.Parameters)
                    {
                        if (p.IsComplexModel)
                        {
                            dtoList.Add(p.InputModel);
                        }
                    }
                    var res = m.Reponse;
                    if (res != null && res.IsComplexModel)
                    {
                        dtoList.Add(res.ResultModel);
                    }
                }
                return dtoList.Distinct().ToList();
            }
        }
        public string ListDtoName
        {
            get
            {
                return getInputDtoName(ListApi);
            }
        }
        public string ListOutputDto
        {
            get
            {
                var m = ListApi.Methods.FirstOrDefault();
                return string.Empty;
            }
        }

        public string UpdateDtoName
        {
            get
            {
                return getInputDtoName(UpdateApi);
            }
        }
        public string CreateDtoName
        {
            get
            {
                return getInputDtoName(CreateApi);
            }
        }

        private string getInputDtoName(ApiModel apiModel)
        {
            var method = apiModel.Methods.FirstOrDefault();
            foreach (var item in method.Parameters)
            {
                if (item.IsComplexModel)
                {
                    return item.InputModel.Name;
                }
            }
            return string.Empty;
        }
    }
}
