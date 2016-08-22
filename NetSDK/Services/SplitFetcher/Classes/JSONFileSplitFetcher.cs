using NetSDK.Domain;
using NetSDK.Services.Parsing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetSDK.Services.SplitFetcher.Classes
{
    public class JSONFileSplitFetcher: InMemorySplitFetcher
    {
        private SplitParser splitParser;
        public JSONFileSplitFetcher(string filePath, SplitParser splitParser)
        {
            this.splitParser = splitParser;
            var json = File.ReadAllText(filePath);
            var splitChangesResult = JsonConvert.DeserializeObject<SplitChangesResult>(json);
            splits = splitChangesResult.splits
                            .Select(x => new { Key = x.name, Value = splitParser.Parse(x)})
                            .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
