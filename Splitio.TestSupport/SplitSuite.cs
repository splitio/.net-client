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
    public class SplitSuite : ClassDataAttribute
    {
        public SplitScenario[] splitScenarios { get; private set; }

        public SplitSuite(string scenarios): base(typeof(object))
        {
            this.splitScenarios = JsonConvert.DeserializeObject<SplitScenario[]>(scenarios);
        }

        public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest, Type[] parameterTypes)
        {
            var data = new List<string[]>();
            foreach (var splitScenario in this.splitScenarios)
            {
                foreach (var splitTest in splitScenario.features)
                {
                    foreach (var treatment in splitTest.treatments)
                    {
                        data.Add(new string[] { splitTest.feature, treatment });
                    }
                }
            }
            return data;
        }
    }
}
