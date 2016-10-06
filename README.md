# .net-client

## CI status

<img src=https://ci.appveyor.com/api/projects/status/github/splitio/.net-client?branch=master&svg=true&passingText=master%20-%20ok&pendingText=master%20-%20running&failingText=master%20-%20failing>

<img src=https://ci.appveyor.com/api/projects/status/github/splitio/.net-client?branch=development&svg=true&passingText=development%20-%20ok&pendingText=development%20-%20running&failingText=development%20-%20failing>


## Installing Split SDK using Nuget

To install Splitio, run the following command in the Package Manager Console

```
Install-Package Splitio
```

## Write your code!

SDK Configuration options

```cs
 var configurations = new ConfigurationOptions();
					  configurations.FeaturesRefreshRate = 30;
					  configurations.SegmentsRefreshRate = 30;

```

Create the Split Client instance. 

```cs
var sdk = factory.BuildSplitClient("API_KEY", configurations);
```

Checking if the key belongs to treatment 'on' in sample_feature. 

```cs
if (sdk.GetTreatment("key", "sample_feature") == "on") 
{
    //Code for enabled feature
} 
else 
{
    //Code for disabled feature
}
```

### Attributes support
Set the attributes values as a dictionary 

```cs
attributes = new Dictionary<string, object>();
attributes.Add("age", "21");
```

Checking if the attribute 'age' belongs to treatment 'young' in sample_feature. 

```cs
if (sdk.GetTreatment("key", "sample_feature", attributes) == "young") 
{
    //Code for young feature
} 
else 
{
    //Code for old feature
}
```

Using matching key and bucketing key

```cs
Key key = new Key("sample_matching_key", "sample_bucketing_key");

if (sdk.GetTreatment(key, "sample_feature") == "on") 
{
    //Code for enabled feature
} 
else 
{
    //Code for disabled feature
}
```

**NOTE:** For date and time values the attribute should be set as Unix Timestamp in UTC.

### Advanced Configuration of the SDK 

The SDK can configured for performance. Each configuration has a default, however, you can provide an override value while instantiating the SDK.


```cs
	var factory = new SplitFactory();
    var configurations = new ConfigurationOptions();
            configurations.FeaturesRefreshRate = 30;
            configurations.SegmentsRefreshRate = 30;
			configurations.ImpressionsRefreshRate = 30;
            configurations.MetricsRefreshRate = 30;
            configurations.ReadTimeout = 15000;
            configurations.ConnectionTimeOut = 15000;

	var sdk = factory.BuildSplitClient("API_KEY", configurations);
```

###  Blocking the SDK Until It Is Ready 

When the SDK is instantiated, it kicks off background tasks to update an in-memory cache with small amounts of data fetched from Split servers. This process can take up to a few hundred milliseconds, depending on the size of data. While the SDK is in this intermediate state, if it is asked to evaluate which treatment to show to a customer for a specific feature, it may not have data necessary to run the evaluation. In this circumstance, the SDK does not fail, rather it returns The Control Treatment.

If you would rather wait to send traffic till the SDK is ready, you can do that by blocking until the SDK is ready. This is best done as part of the startup sequence of your application. Here is an example:

```cs
		ISplitClient client = null;

		try
		{
			var factory = new SplitFactory();
			var configurations = new ConfigurationOptions();
			configurations.Ready = 1000;
			client = factory.BuildSplitClient(apikey, configurations);
		}
		catch (TimeoutException t)
		{
			// SDK was not ready in 1000 miliseconds
		}
```

###  Running the SDK in 'off-the-grid' Mode 

Features start their life on one developer's machine. A developer should be able to put a feature behind Split on their development machine without the SDK requiring network connectivity. To achieve this, Split SDK can be started in 'localhost' (aka off-the-grid mode). In this mode, the SDK neither polls nor updates Split servers, rather it uses an in-memory data structure to determine what treatments to show to the logged in customer for each of the features. Here is how you can start the SDK in 'localhost' mode:

```cs
	var factory = new SplitFactory();
	var client = factory.BuildSplitClient("localhost", configurations);
```

In this mode, the SDK loads a mapping of feature name to treatment from a file at $HOME/.split. For a given feature, the specified treatment will be returned for every customer. In Split terms, the roll-out plan for that feature becomes:

```
if user is in segment all then split 100%:treatment
```

Any feature that is not mentioned in the file is assumed to not exist. The SDK returns The Control Treatment for every customer of that feature.

The format of this file is two columns separated by whitespace. The left column is the feature name, the right column is the treatment name. Here is a sample feature file:

```
# this is a comment

reporting_v2 on # sdk.getTreatment(*, reporting_v2) will return 'on'

double_writes_to_cassandra off

new-navigation v3
```

###  Split Manager 

In order to obtain a list of Split features available in the in-memory dataset used by Split client to evaluate treatments, use the Split Manager.

```cs
    var factory = new SplitFactory();
    var client = factory.BuildSplitClient("API_KEY", null);
    var splitManager = factory.GetSplitManager();
```

Currently, SplitManager exposes the following interface:

```cs
	List<LightSplit> Splits();
```

calling splitManager.Splits() will return the following structure:

```cs
    public class LightSplit
    {
        public string name { get; set; }
        public string trafficType { get; set; }
        public bool killed { get; set; }
        public List<string> treatments { get; set; }
        public long changeNumber { get; set; }
    }
```