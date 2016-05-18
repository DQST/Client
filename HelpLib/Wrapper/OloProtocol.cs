using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Collections;

namespace HelpLib.Wrapper
{
    public class OloProtocol
    {
        public static string GetOlo(string method, params object[] args)
        {
            return "{\"ver\":\"0042\", \"method\": \"" + method + "\", \"params\":" +
                JsonConvert.SerializeObject(args) + "}";
        }

        public static IEnumerable Encode(string input)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(input) as IEnumerable;
        }
    }

    public class OloFieldAttribute : Attribute
    {
        private string name;
        public OloFieldAttribute() { }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }

    public class OloService
    {
        private Type type;
        private object obj;
        private Dictionary<string, MethodInfo> methods;

        public OloService(object obj)
        {
            type = obj.GetType();
            this.obj = obj;
            methods = new Dictionary<string, MethodInfo>();
            Init();
        }

        private void Init()
        {
            var info = type.GetRuntimeMethods();
            var rez = from i in info where i.GetCustomAttributes(typeof(OloFieldAttribute), false).Length > 0 select i;
            foreach (var item in rez)
            {
                var obj = item.GetCustomAttribute(typeof(OloFieldAttribute)) as OloFieldAttribute;
                if (obj.Name != null)
                    methods.Add(obj.Name, item);
                else
                    methods.Add(item.Name, item);
            }
        }

        public void Parse(IEnumerable collection)
        {
            var col = collection as Dictionary<string, object>;
            if (col.ContainsKey("ver") && col["ver"].Equals("0042"))
            {
                if (col.ContainsKey("method"))
                {
                    var methodName = col["method"].ToString();
                    var o = col["params"];
                    var args = JsonConvert.DeserializeObject(o.ToString());
                    if (methods.ContainsKey(methodName))
                    {
                        var arr = args as JArray;
                        if (arr != null)
                        {
                            var v = arr.ToObject<List<object>>().ToArray();
                            methods[methodName].Invoke(obj, new object[] { v });
                        }
                    }
                }
            }
        }
    }
}
