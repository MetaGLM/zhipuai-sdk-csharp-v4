# 智谱大模型开放接口SDK
智谱开放平台大模型接口 C# SDK (Big Model API SDK in C#)，让开发者更便捷的调用智谱开放API

## 简介

如果有 bug 或者缺少需要的功能请提 issues

## 安装

1. 在VisualStudio打开您的解决方案(.sln)。

2. 右键单击解决方案并在弹出的菜单中选择 添加->现有项目。

3. 在弹出的对话框中，选择ZhipuApi.csproj文件，然后单击“打开”。

4. 右键单击您的项目并选择添加->项目引用。添加ZhipuApi的引用，然后单击确定。

## 使用

### 对话补全

```csharp
var clientV4 = new ClientV4(API_KEY);
var response = clientV4.chat.Completion(
    new TextRequestBase()
        .SetModel("glm-4")
        .SetMessages(new[] { new MessageItem("user", "你好，你是谁？") })
        .SetTemperature(0.7)
        .SetTopP(0.7)
);

Console.WriteLine(JsonSerializer.Serialize(response, DEFAULT_SERIALIZER_OPTION));
```

### 对话补全（流式）

```csharp
var clientV4 = new ClientV4(API_KEY);
var responseIterator = clientV4.chat.Stream(
    new TextRequestBase()
        .SetModel("glm-4")
        .SetMessages(new[]
        {
            new MessageItem("user", "1+1等于多少"),
            new MessageItem("assistant", "1+1等于2。"),
            new MessageItem("user", "再加2呢？"),
        })
        .SetTemperature(0.7)
        .SetTopP(0.7)
);

foreach  (var response in responseIterator)
{
    Console.WriteLine(JsonSerializer.Serialize(response, DEFAULT_SERIALIZER_OPTION));
}
```

### 工具调用

```csharp
var clientV4 = new ClientV4(API_KEY);
var response = clientV4.chat.Completion(
    new TextRequestBase()
        .SetModel("glm-4")
        .SetMessages(new[] { new MessageItem("user", "北京今天的天气如何？") })
        .SetTools(new[]
        {
            new FunctionTool().SetName("get_weather")
                .SetDescription("根据提供的城市名称，提供未来的天气数据")
                .SetParameters(new FunctionParameters()
                    .AddParameter("city", ParameterType.String, "搜索的城市名称")
                    .AddParameter("days", ParameterType.Integer, "要查询的未来的天数，默认为0")
                    .SetRequiredParameter(new string[] { "city" }))
        })
        .SetToolChoice("auto")
        .SetTemperature(0.7)
        .SetTopP(0.7)
);

Console.WriteLine(JsonSerializer.Serialize(response,DEFAULT_SERIALIZER_OPTION));
```

### CogView图像生成

```csharp
var clientV4 = new ClientV4(API_KEY);
var response = clientV4.images.Generation(new ImageRequestBase()
    .SetModel("cogview")
    .SetPrompt("一只可爱的科幻风格小猫咪"));

Console.WriteLine(JsonSerializer.Serialize(response,DEFAULT_SERIALIZER_OPTION));
```

### CogVLM图像识别

```csharp
var clientV4 = new ClientV4(API_KEY);
var response = clientV4.chat.Completion(
    new TextRequestBase()
        .SetModel("cogvlm_28b")
        .SetMessages(new []
        {
            new ImageToTextMessageItem("user")
                .setText("这是什么")
                .setImageUrl("<image_url>")
            
        })
        .SetTemperature(0.7)
        .SetTopP(0.7)
);

Console.WriteLine(JsonSerializer.Serialize(response, DEFAULT_SERIALIZER_OPTION));
```

### CogVLM图像识别（流式）

```csharp
var clientV4 = new ClientV4(API_KEY);
var responseIterator = clientV4.chat.Stream(
    new TextRequestBase()
        .SetModel("cogvlm_28b")
        .SetMessages(new []
        {
            new ImageToTextMessageItem("user")
                .setText("这是什么")
                .setImageUrl("<image_url>")
            
        })
        .SetTemperature(0.7)
        .SetTopP(0.7)
);

foreach  (var response in responseIterator)
{
    Console.WriteLine(JsonSerializer.Serialize(response, DEFAULT_SERIALIZER_OPTION));
}
```

### 文本向量化

```csharp
var clientV4 = new ClientV4(API_KEY);
var response = clientV4.embeddings.Process(new EmbeddingRequestBase()
    .SetModel("embedding-2")
    .SetInput("一只可爱的科幻风格小猫咪"));

Console.WriteLine(JsonSerializer.Serialize(response,DEFAULT_SERIALIZER_OPTION));
```
