using log4net;
using Splitio.Domain;
using Splitio.Services.EngineEvaluator;
using Splitio.Services.Parsing;
using Splitio.Services.SplitFetcher.Classes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Splitio.Services.Client.Classes
{
    public class LocalhostClient : Client
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LocalhostClient));

        public LocalhostClient(string filePath)
        {
            InitializeLogger();
            filePath = LookupFilePath(filePath);
            var splits = ParseSplitFile(filePath);
            BuildSplitFetcher(splits);
            BuildSplitter();
            BuildEngine();
        }

        private string LookupFilePath(string filePath)
        {
            if (File.Exists(filePath))
            {
                return filePath;
            }
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            var fullPath = Path.Combine(home, filePath);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }

            throw new DirectoryNotFoundException("Splits file could not be found");
        }

        private void BuildSplitFetcher(ConcurrentDictionary<string,ParsedSplit> splits)
        {
            splitFetcher = new InMemorySplitFetcher(splits);
        }

        private ConcurrentDictionary<string, ParsedSplit> ParseSplitFile(string filePath)
        {
            ConcurrentDictionary<string, ParsedSplit> splits = new ConcurrentDictionary<string, ParsedSplit>();

            string line;

            StreamReader file = new StreamReader(filePath);
            
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                if (String.IsNullOrEmpty(line) || line.StartsWith("#"))
                {
                    continue;
                }

                String[] feature_treatment = Regex.Split(line, @"\s+");

                if (feature_treatment.Length != 2)
                {
                    Log.Info("Ignoring line since it does not have exactly two columns: " + line);
                    continue;
                }

                splits.TryAdd(feature_treatment[0], CreateParsedSplit(feature_treatment[0], feature_treatment[1]));
                Log.Info("100% of keys will see " + feature_treatment[1] + " for " + feature_treatment[0]);

            }

            file.Close();

            return splits;
        }

        /// <summary>
        /// Creates a ParsedSplit instance that always returns 
        /// treatment specified in input file. It is implemented this way
        /// for simplification. When a split is killed, the engine 
        /// returns default treatment for that feature.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="treatment"></param>
        /// <returns></returns>
        private ParsedSplit CreateParsedSplit(string name, string treatment)
        {
            var split = new ParsedSplit()
            {
                name = name,
                seed = 0,
                killed = true,
                defaultTreatment = treatment,
                conditions = null
            };

            return split;
        }

        private void InitializeLogger()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        private void BuildSplitter()
        {
            splitter = new Splitter();
        }

        private void BuildEngine()
        {
            engine = new Engine(splitter);
        }

    }
}
