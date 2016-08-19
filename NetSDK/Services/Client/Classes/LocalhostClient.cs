using log4net;
using NetSDK.Domain;
using NetSDK.Services.EngineEvaluator;
using NetSDK.Services.Parsing;
using NetSDK.Services.SplitFetcher.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetSDK.Services.Client.Classes
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

        private void BuildSplitFetcher(Dictionary<string,ParsedSplit> splits)
        {
            splitFetcher = new InMemorySplitFetcher(splits);
        }

        private Dictionary<string, ParsedSplit> ParseSplitFile(string filePath)
        {
            Dictionary<string, ParsedSplit> splits = new Dictionary<string, ParsedSplit>();

            string line;

            StreamReader file = new StreamReader(filePath);
            
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                if (String.IsNullOrEmpty(line) || line.StartsWith("#"))
                {
                    continue;
                }

                String[] feature_treatment = line.Split(' ');

                if (feature_treatment.Length != 2)
                {
                    Log.Info("Ignoring line since it does not have exactly two columns: " + line);
                    continue;
                }

                splits.Add(feature_treatment[0], CreateParsedSplit(feature_treatment[0], feature_treatment[1]));
                Log.Info("100% of keys will see " + feature_treatment[1] + " for " + feature_treatment[0]);

            }

            file.Close();

            return splits;
        }

        private ParsedSplit CreateParsedSplit(string name, string treatment)
        {
            var split = new ParsedSplit()
            {
                name = name,
                seed = 0,
                killed = false,
                defaultTreatment = treatment,
                conditions = new List<ConditionWithLogic>()
            };

            var combiningMatcher = new CombiningMatcher()
            {
                delegates = new List<AttributeMatcher>()
            };

            combiningMatcher.delegates.Add(new AttributeMatcher()
            {
                attribute = null,
                negate = false,
                matcher = new AllKeysMatcher()
            }
            );

            var partitionDefinitions = new List<PartitionDefinition>();
            partitionDefinitions.Add(new PartitionDefinition()
            {
                size = 100,
                treatment = treatment
            });

            var conditionWithLogic = new ConditionWithLogic()
            {
                matcher = combiningMatcher,
                partitions = partitionDefinitions
            };

            split.conditions.Add(conditionWithLogic);
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
