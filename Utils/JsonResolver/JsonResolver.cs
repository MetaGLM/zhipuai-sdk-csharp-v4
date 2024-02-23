using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ZhipuApi.Models;
using ZhipuApi.Models.RequestModels;
using ZhipuApi.Models.RequestModels.ImageToTextModels;

namespace ZhipuApi.Utils.JsonResolver
{
    public class JsonResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType == typeof(MessageItem) && property.PropertyName == "content")
            {
                Predicate<object> shouldSerialize = instance =>
                {
                    if (instance is ImageToTextMessageItem)
                    {
                        return false;
                    }
                    return true;
                };
                property.ShouldSerialize = shouldSerialize;
            }

            return property;
        }
    }
}