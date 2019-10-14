using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JZSoft.AbpVueCodeGen.Mvc.Models
{
    public class GenCodeInput
    {
        public string TagName { get; set; }
        public string ListMethod { get; set; }
        public string CreateMethod { get; set; }
        public string UpdateMethod { get; set; }
        public string DeleMethod { get; set; }
    }
}
