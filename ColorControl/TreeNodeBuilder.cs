using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ColorControl
{
    public class TreeNodeBuilder
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();

        public static TreeNode CreateTree(object obj, string text)
        {
            var serialized = JsonConvert.SerializeObject(obj, Formatting.None,
                new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Error = delegate (object sender, ErrorEventArgs args)
                    {
                        //errors.Add(args.ErrorContext.Error.Message);
                        args.ErrorContext.Handled = true;
                    },
                    Converters = new [] { new Newtonsoft.Json.Converters.StringEnumConverter() }
                });
            var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(serialized);
            var root = new TreeNode();
            root.Text = text;
            BuildTree(dic, root);
            return root;
        }

        private static void BuildTree(object item, TreeNode node)
        {
            if (item is KeyValuePair<string, object>)
            {
                KeyValuePair<string, object> kv = (KeyValuePair<string, object>)item;
                TreeNode keyValueNode = new TreeNode();
                keyValueNode.Name = kv.Key;
                keyValueNode.Text = kv.Key + ": " + GetValueAsString(kv.Value);
                node.Nodes.Add(keyValueNode);
                BuildTree(kv.Value, keyValueNode);
            }
            else if (item.GetType() == typeof(JArray))
            {
                var list = (JArray)item;
                int index = 0;
                foreach (object value in list)
                {
                    TreeNode arrayItem = new TreeNode();
                    arrayItem.Name = $"[{index}]";
                    arrayItem.Text = $"[{index}]";
                    node.Nodes.Add(arrayItem);
                    BuildTree(value, arrayItem);
                    index++;
                }
            }
            else if (item is Dictionary<string, object>)
            {
                Dictionary<string, object> dictionary = (Dictionary<string, object>)item;
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
}
