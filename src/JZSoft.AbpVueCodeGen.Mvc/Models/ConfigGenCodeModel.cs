using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JZSoft.CodeGen;

namespace JZSoft.AbpVueCodeGen.Mvc.Models
{
    public class ConfigGenCodeModel
    {
        public string TagName { get; internal set; }
        public List<ModelDtoDef> DtoDefList { get; internal set; }
        public List<ApiModel> ApiModels { get; internal set; }
    }
}
