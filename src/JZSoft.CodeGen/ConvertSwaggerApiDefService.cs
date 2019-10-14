
using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JZSoft.CodeGen
{
    public class ConvertSwaggerApiDefService
    {
        public JsonSchema SchemaData { get; set; } = new JsonSchema();
        public ConvertSwaggerApiDefService(JsonSchema schemaData)
        { 
            var t = Task.Run(async () =>
            {
                SchemaData = schemaData;
                _apiModelDtoDefList = await GetDefinition();
            });
            Task.WaitAll(t);
        }

        public static async Task<ConvertSwaggerApiDefService> FromFileAsync(string swaggerFile)
        {
            var schemaData = await JsonSchema.FromFileAsync(swaggerFile);
            return new ConvertSwaggerApiDefService(schemaData);
        }
        public static async Task<ConvertSwaggerApiDefService> FromJson(string json)
        {
            var schemaData = await JsonSchema.FromJsonAsync(json);
            return new ConvertSwaggerApiDefService(schemaData);
        }

        public static async Task<ConvertSwaggerApiDefService> FromUrlAsync(string apiUrl)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var res = await httpClient.GetAsync(apiUrl);
                    var response = await res.Content.ReadAsStringAsync();
                    var schemaData = await JsonSchema.FromJsonAsync(response);
                    return new ConvertSwaggerApiDefService(schemaData);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        private List<ModelDtoDef> _apiModelDtoDefList;
        public List<ModelDtoDef> ApiModelDtoDefList
        {
            get
            {
                return _apiModelDtoDefList;
            }
        }

        public async Task<List<ApiModel>> GetApiListAsync()
        {
            return await Task.Run(() =>
            {
                var schema = SchemaData;
                var allpaths = schema.ExtensionData["paths"] as Dictionary<string, object>;
                var list = new List<ApiModel>();
                Parallel.ForEach(allpaths, (item) =>
                {
                    ApiModel apiModel = new ApiModel();
                    apiModel.Path = item.Key;
                    Parallel.ForEach(item.Value as Dictionary<string, object>, (m) =>
                    {
                        var method = new MethodDef();
                        method.Method = m.Key;
                        var mInfo = m.Value as Dictionary<string, object>;
                        var tags = mInfo["tags"] as object[];
                        method.Tags = string.Join(",", tags);
                        #region parameters
                        if (mInfo.ContainsKey("parameters"))
                        {
                            var mpList = mInfo["parameters"] as object[];
                            Parallel.ForEach(mpList, (mp) =>
                            {
                                var mpInfo = mp as Dictionary<string, object>;
                                ParametersDef parametersDef = new ParametersDef();
                                if (mpInfo.ContainsKey("schema"))
                                {
                                    var s = mpInfo["schema"] as Dictionary<string, object>;
                                    var schemaName = s["__referencePath"].ToString().Replace("#/definitions/", string.Empty);
                                    parametersDef.SchemaName = schemaName;
                                    parametersDef.InputModel = ApiModelDtoDefList.FirstOrDefault(o => o.Name == schemaName);
                                    parametersDef.IsComplexModel = true;
                                }
                                else
                                {
                                    parametersDef.IsComplexModel = false;
                                }
                                if (mpInfo.ContainsKey("name"))
                                {
                                    parametersDef.Name = mpInfo["name"].ToString();
                                }
                                if (mpInfo.ContainsKey("type"))
                                {
                                    parametersDef.TypeName = mpInfo["type"].ToString();
                                }
                                if (mpInfo.ContainsKey("format"))
                                {
                                    parametersDef.Format = mpInfo["format"].ToString();
                                }
                                if (mpInfo.ContainsKey("required"))
                                {
                                    parametersDef.IsRequired = (bool)mpInfo["required"];
                                }
                                if (mpInfo.ContainsKey("maximum"))
                                {
                                    parametersDef.Maximum = (double)mpInfo["maximum"];
                                }
                                if (mpInfo.ContainsKey("minimum"))
                                {
                                    parametersDef.Minimum = (double)mpInfo["minimum"];
                                }
                                method.Parameters.Add(parametersDef);
                            });
                        }
                        if (mInfo.ContainsKey("responses"))
                        {
                            var res = mInfo["responses"] as Dictionary<string, object>;
                            if (res != null && res.ContainsKey("200"))
                            {
                                var resSchema = res["200"] as Dictionary<string, object>;
                                if (resSchema != null && resSchema.ContainsKey("schema"))
                                {
                                    if (resSchema["schema"] is Dictionary<string, object>)
                                    {
                                        var resPath = resSchema["schema"] as Dictionary<string, object>;
                                        var dotName = resPath["__referencePath"].ToString().Replace("#/definitions/", "");
                                        method.Reponse = new ReponseDef()
                                        {
                                            SchemaName = dotName,
                                            IsComplexModel = true,
                                            ObjectType = JsonObjectType.Object
                                        };
                                        method.Reponse.ResultModel = ApiModelDtoDefList.FirstOrDefault(o => o.Name == dotName); ;

                                    }
                                    else if (resSchema["schema"] is JsonSchema)
                                    {
                                        var resDef = resSchema["schema"] as JsonSchema;
                                        method.Reponse = new ReponseDef()
                                        {
                                            Format = resDef.Format,
                                            ObjectType = resDef.Type,
                                            IsComplexModel = false
                                        };
                                    }
                                }
                            }
                        }
                        #endregion
                        apiModel.Methods.Add(method);
                    });
                    list.Add(apiModel);

                });
                return list;
            });

        }


        public async Task<List<ModelDtoDef>> GetDefinition()
        {
            return await Task.Run(() =>
            {
                var models = new List<ModelDtoDef>();
                Parallel.ForEach(SchemaData.Definitions.Keys, async (dtoName) =>
                {
                    ModelDtoDef modelDto = await GenModelDto(SchemaData, dtoName);
                    models.Add(modelDto);
                });
                return models;
            });

        }

        private async Task<ModelDtoDef> GenModelDto(JsonSchema jsonSchema, string dtoName)
        {
            return await Task.Run(async () =>
            {
                ModelDtoDef modelDto = new ModelDtoDef();
                modelDto.Name = dtoName;
                var jsonProp = jsonSchema.Definitions[dtoName];
                foreach (var it1Key in jsonProp.Properties.Values)
                {
                    var prop = GetProps(it1Key).Result;
                    modelDto.Properties.Add(prop);
                }

                return modelDto;
            });

        }

        private async Task<PropertiesDef> GetProps(JsonSchemaProperty jsonSchemaProp)
        {
            return await Task.Run(() =>
            {
                var property = new PropertiesDef();
                property.Name = jsonSchemaProp.Name;
                property.ObjectType = jsonSchemaProp.Type;
                property.IsRequired = jsonSchemaProp.IsRequired;
                property.Format = jsonSchemaProp.Format;
                property.MaxLength = jsonSchemaProp.MaxLength;
                property.MinLength = jsonSchemaProp.MinLength;
                if (jsonSchemaProp.Type == JsonObjectType.Array)
                {
                    var popRef = jsonSchemaProp.Item.Reference;
                    if (popRef != null)
                    {
                        foreach (var item in popRef.Properties.Values)
                        {
                            var subProp = GetProps(item).Result;
                            property.Items.Add(subProp);
                        }
                    }
                }
                return property;
            });

        }


    }

}
