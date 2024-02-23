namespace ZhipuApi.Models.RequestModels
{
    public class MessageItem
    {
        public string role { get; set; }
        public virtual string content { get; set; }

        public MessageItem(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
    }
}