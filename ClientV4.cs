using ZhipuApi.Modules;

namespace ZhipuApi
{
    public class ClientV4
    {
        private string _apiKey;
        public Chat chat { get; private set; }
        
        public Images images { get; private set; }
        
        public Embeddings embeddings { get; private set; }
        
        public ClientV4(string apiKey)
        {
            this._apiKey = apiKey;
            this.chat = new Chat(apiKey);
            this.images = new Images(apiKey);
            this.embeddings = new Embeddings(apiKey);
        }
        
        
    }
}