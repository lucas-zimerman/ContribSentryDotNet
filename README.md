# sentry dotnet transaction addon
### Unnoficial addon for adding Peformance support to Sentry-dotnet

# Status
Currently in Alpha, not all features were implemented and you may experience errors or lose of Performance events.

Official Docs: https://docs.sentry.io/performance-monitoring/getting-started/

# Configuration

To initialize the performance addon you'll need to call SentryTracingSdk.Init
```C#
SentryTracingSDK.Init(dsn);
```
Also you will need to attach to your Sentry options the SentryTracingEventProcessor so the addon can consume the Tracing event when it's ready 

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

You can also start a child anywhere in the code, as long as there's an active Transaction running at the current Thread, else
the child will be discarted
```C#
using(var child = SentryTracingSDK.StartChild( url, Post ))
{
... your http request here
 child.Finish(httpstatuscode);// child finished with the current status code
}
```
