NLog.Config.WindowsAzure
========================

Host your [NLog](http://nlog-project.org/) configurations in Windows Azure blob storage.

It's nice to keep your logging configuration separate from your Windows Azure cloud service packages.
This library helps you do that by loading it from blob storage, allowing you to easily
update your logging configuration without redeploying or remoting to your cloud service.

Works great with services like [Logentries](http://logentries.com).

### Quick Start

1. Upload your NLog config file to Windows Azure blob storage.
2. Add the <code>NLog.Config.WindowsAzure</code> NuGet package to your cloud service.
3. Set the <code>NLogConfig.ConnectionString</code>, <code>NLogConfig.ContainerName</code>
and <code>NLogConfig.BlobName</code> cloud configuration settings to point at your
NLog configuration file.
4. Call <code>BlobStorageNLogConfiguration.ConfigureLoggingWithBlobSpecifiedBySettings()</code>
on startup.

### Slightly slower start

If you don't want to use the standard variables, or you just like to do stuff yourself,
you can create your own instance of <code>BlobStorageNLogConfiguration</code>
and call <code>ConfigureLogging</code> to load and initialize NLog with the configuration.

### Other things you might like to know

* NLog will use whatever configuration you give it (e.g. an <code>NLog.config</code> file in your deployment
package) until you load your configuration from blob storage. Use this default configuration
to diagnose any issues loading your configuration from blob storage.

### That's it

Please drop me a line/issue/pull request if you have any suggestions or questions.
