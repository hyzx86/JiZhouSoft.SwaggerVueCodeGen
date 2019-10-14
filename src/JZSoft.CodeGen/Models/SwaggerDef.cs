using System;
using System.Collections.Generic;
using System.Text;

namespace JZSoft.CodeGen.Models
{
    public class SwaggerMethodDef
    {
        public string Tag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Get/SET/PUT/DELETE
        /// </summary>
        public string Method { get; set; }
        public ParameterDef Parameters { get; set; }
        public Definition Response { get; set; }
    }


    public class ParameterDef
    {
        public string Name { get; set; }
        public bool Required { get; set; }
        public string ObjectType { get; set; }
        public string Format { get; set; }
        public string In { get; set; }
        public int? Maximum { get; set; }
        public int? Minimum { get; set; }
        public bool IsComplexModel { get; set; }
        public Definition DefinitionRef { get; set; }
    }


    public class Definition
    {
        public string Name { get; set; }
        public string Format { get; set; }
        public string ObjectType { get; set; }
        public bool IsComplexModel { get; set; }
        public Definition DefinitionRef { get; set; }
    }



}
