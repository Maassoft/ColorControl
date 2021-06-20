using NvAPIWrapper.Display;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace ColorControl
{
    public class ColorDataConverter : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new List<Type>() { typeof(ColorData) }; }
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (obj == null) return result;
            return result;
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return NvPreset.GenerateColorData(dictionary);
        }
    }
}
