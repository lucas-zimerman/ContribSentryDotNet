# sentry dotnet transaction addon
### Unnoficial addon for adding Peformance support to Sentry-dotnet

Official Docs: https://docs.sentry.io/performance-monitoring/getting-started/

##### Usage:

```C#
SentryTracingSDK.Init(dsn, tracesSampleRate); can be called before or after SentrySdk.Init.

var transaction = SentryTracingSDK.StartTransaction( name );// return a new transaction.
var child = transaction.StartChild( name );// return a new child
child.Finish();// finishes the child
// You can add as many childs as you wish on a transaction
transaction.Finish();// finishes and sends the transaction
```

##### You'll need to add an additional code inside of your BeforeSend code
```C#
if (arg is SentryTransaction transaction)
{
   transaction.SendTransaction();
   return null;
}
```
