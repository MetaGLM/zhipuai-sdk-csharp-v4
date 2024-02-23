using ZhipuApi.Models.ResponseModels.ToolModels;

namespace ZhipuApi.Models.ResponseModels
{
    public class ResponseChoiceDelta
    {
        public string role { get; set; }
        public string content { get; set; }
        public ToolCallItem[] tool_calls { get; set; }
    }
}