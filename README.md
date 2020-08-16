# sentry dotnet transaction addon V2.0.0
### Unnoficial addon for adding Peformance support to Sentry-dotnet

# Status
Currently in Alpha, not all features were implemented and you may experience errors or lose of Performance events.

Official Docs: https://docs.sentry.io/performance-monitoring/getting-started/

# Configuration

To initialize the performance addon you'll need add the SentryTracingSdkIntegration to your SentryOptions integration.
```C#
sentryOptions.AddIntegration(new SentryTracingSdkIntegration());
```
you'll of course need to initialize SentrySdk giving passing the SentryOptions where you added the Integration.

# Usage
You can start/finish a Transaction by creating an transaction Object or by 'using'
```C#
var transaction = SentryTracingSDK.StartTransaction( name );// return a new transaction.
var child = transaction.StartChild( name );// return a new child
... code to be measured
child.Finish();// finishes the child
// You can add as many childs as you wish on a transaction
transaction.Finish();// finishes and sends the transaction
```
```C#
using(var transaction = SentryTracingSDK.StartTransaction( name ))
{
 var child = transaction.StartChild( name );// return a new child
 ... code to be measured
 child.Finish();// finishes the child
 // You can add as many childs as you wish on a transaction
}
```

You can also start a child anywhere in the code, as long as there's an active Isolated Transaction, else
the child will be discarted
```C#
using(var child = SentryTracingSDK.StartChild( url, Post ))
{
... your http request here
 child.Finish(httpstatuscode);// child finished with the current status code
}
```

To isolate a Transaction if you would like to start a child by not referencing the Tracing object you'll need to run the following code 
```C#
var transaction = SentryTracingSDK.StartTransaction( name );
await transaction.IsolateTracking(async ()=>{
 // your code here
});
transaction.Finish();
```
That way, if the code SentryTracingSDK.StartChild is called and the stack trace is inside of an isolated Transaction block, the Span will be attached to the Isolated Transaction. 
