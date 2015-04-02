### Serilog.Sinks.CouchDB

[![Build status](https://ci.appveyor.com/api/projects/status/i0u84k30h2ab3lbp/branch/master?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-couchdb/branch/master)

A Serilog sink that writes events to Apache [CouchDB](http://couchdb.org).

**Package** - [Serilog.Sinks.CouchDB](http://nuget.org/packages/serilog.sinks.couchdb)
| **Platforms** - .NET 4.5

You'll need to create a database on your CouchDB server. In the example shown, it is called `log`.

```csharp
var log = new LoggerConfiguration()
    .WriteTo.CouchDB("http://mycouchdb/log/")
    .CreateLogger();
```
