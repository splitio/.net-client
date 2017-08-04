using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit.Extensions;

namespace Splitio.TestSupport
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SplitTest : ClassDataAttribute
    {
        public string feature { get; private set; }
        public string[] treatments { get; private set; }

        [JsonConstructor]
        public SplitTest(string feature, string[] treatments)
            : base(typeof(object))
        {
            this.feature = feature;
            this.treatments = treatments;
        }
 
        public SplitTest(string test):base(typeof(object))
        {
            var splitTest = JsonConvert.DeserializeObject<SplitTest>(test);
            this.feature = splitTest.feature;
            this.treatments = splitTest.treatments;
        }

        public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest, Type[] parameterTypes)
        {
            var data = new List<string[]>();
            foreach (var treatment in treatments)
            {
                data.Add(new string[] { feature, treatment });
            }
            return data;
        }
    }
}
