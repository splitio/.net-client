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
```cs
/** SDK Configuration options */
 var configurations = new ConfigurationOptions();
					  configurations.FeaturesRefreshRate = 30;
					  configurations.SegmentsRefreshRate = 30;

/** Create the Split Client instance. */
var sdk = factory.BuildSplitClient("API_KEY", configurations);

/** Set the attributes dictionary as null*/
attributes = null;

/** Checking if the key belongs to treatment 'on' in sample_feature. */
if (sdk.GetTreatment("key", "sample_feature", attributes) == "on") 
{
    //Code for enabled feature
} 
else 
{
    //Code for disabled feature
}
```
### Attributes support
```cs
/** Set the attributes values as a dictionary */
attributes = new Dictionary<string, object>();
attributes.Add("age", "21");

/** Checking if the attribute 'age' belongs to treatment 'young' in sample_feature. */

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