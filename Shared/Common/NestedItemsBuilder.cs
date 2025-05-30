using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace ColorControl.Shared.Common;

public class NestedItemsBuilder
{
    public class NestedItem
    {
        public bool Expanded { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public List<NestedItem> NestedItems { get; set; } = [];
    }

    public static NestedItem CreateTree(object obj, string text)
    {
        var serialized = JsonConvert.SerializeObject(obj, Formatting.None,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Error = delegate (object sender, ErrorEventArgs args)
                {
                    //errors.Add(args.ErrorContext.Error.Message);
                    args.ErrorContext.Handled = true;
                },
                Converters = [new Newtonsoft.Json.Converters.StringEnumConverter()]
            });
        var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(serialized);
        var root = new NestedItem
        {
            Value = text
        };
        BuildTree(dic, root);
        return root;
    }

    private static void BuildTree(object item, NestedItem node)
    {
        if (item is KeyValuePair<string, object> kv)
        {
            var keyValueNode = new NestedItem();
            keyValueNode.Name = kv.Key;
            keyValueNode.Value = kv.Key + ": " + GetValueAsString(kv.Value);
            node.NestedItems.Add(keyValueNode);
            BuildTree(kv.Value, keyValueNode);
        }
        else if (item.GetType() == typeof(JArray))
        {
            var list = (JArray)item;
            int index = 0;
            foreach (object value in list)
            {
                var arrayItem = new NestedItem();
                arrayItem.Name = $"[{index}]";
                arrayItem.Value = $"[{index}]";
                node.NestedItems.Add(arrayItem);
                BuildTree(value, arrayItem);
                index++;
            }
        }
        else if (item is Dictionary<string, object> dictionary)
        {
            foreach (KeyValuePair<string, object> d in dictionary)
            {
                BuildTree(d, node);
            }
        }
        else if (item.GetType() == typeof(JObject))
        {
            var jobject = item as JObject;
            foreach (var property in jobject.Properties())
            {
                BuildTree(new KeyValuePair<string, object>(property.Name, property.Value), node);
            }
        }
    }

    private static string GetValueAsString(object value)
    {
        if (value == null)
            return "null";
        var type = value.GetType();
        if (type.IsArray)
        {
            return "[]";
        }

        if (value.GetType() == typeof(JArray))
        {
            var arr = value as JArray;
            return $"[{arr.Count}]";
        }

        if (type.IsGenericType || value.GetType() == typeof(JObject))
        {
            return "{}";
        }

        return value.ToString();
    }
}
