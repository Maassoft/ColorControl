using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
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
            JavaScriptSerializer jss = new JavaScriptSerializer();
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
            Dictionary<string, object> dic = jss.Deserialize<Dictionary<string, object>>(serialized);
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
            else if (item is ArrayList)
            {
                ArrayList list = (ArrayList)item;
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

            if (value is ArrayList)
            {
                var arr = value as ArrayList;
                return $"[{arr.Count}]";
            }

            if (type.IsGenericType)
            {
                return "{}";
            }

            return value.ToString();
        }
    }
}
