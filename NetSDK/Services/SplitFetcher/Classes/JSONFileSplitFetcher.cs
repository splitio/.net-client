﻿using Splitio.Domain;
using Splitio.Services.Parsing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Splitio.Services.Cache.Classes;
using Splitio.Services.Cache.Interfaces;

namespace Splitio.Services.SplitFetcher.Classes
{
    public class JSONFileSplitFetcher
    {
        public ISplitCache splitCache { get; private set; }
        private SplitParser splitParser;
        public JSONFileSplitFetcher(string filePath, SplitParser splitParser)
        {
            this.splitParser = splitParser;
            var json = File.ReadAllText(filePath);
            var splitChangesResult = JsonConvert.DeserializeObject<SplitChangesResult>(json);
            splitCache = new InMemorySplitCache(new ConcurrentDictionary<string, ParsedSplit>(
                splitChangesResult.splits.Select(x => new KeyValuePair<string,ParsedSplit>(x.name, splitParser.Parse(x) ))
            ));
        }
    }
}
