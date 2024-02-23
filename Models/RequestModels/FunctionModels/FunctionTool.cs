using System.Collections.Generic;

namespace ZhipuApi.Models.RequestModels.FunctionModels
{
    public class FunctionTool
    {
        public string type { get; set; }
        public Dictionary<string, object> function { get; set; }

        public FunctionTool()
        {
            this.type = "function";
            this.function = new Dictionary<string, object>();
        }

        public FunctionTool SetName(string name)
        {
            this.function["name"] = name;
            return this;
        }

        public FunctionTool SetDescription(string desc)
        {
            this.function["description"] = desc;
            return this;
        }

        public FunctionTool SetParameters(FunctionParameters param)
        {
            this.function["parameters"] = param;
            return this;
        }
    }
}