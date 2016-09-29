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
            configurations.ReadTimeoutInMs = 15000;
            configurations.ConnectionTimeOutInMs = 15000;

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