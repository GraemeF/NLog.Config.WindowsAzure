using System.IO;
using System.Xml;

using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace NLog.Config.WindowsAzure
{
    public class BlobStorageNLogConfiguration
    {
        private const string ConnectionStringSettingName = "NLogConfig.ConnectionString";
        private const string ContainerNameSettingName = "NLogConfig.ContainerName";
        private const string BlobNameSettingName = "NLogConfig.BlobName";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string blobName;
        private readonly string connectionString;
        private readonly string containerName;

        public BlobStorageNLogConfiguration(string connectionString,
                                            string containerName,
                                            string blobName)
        {
            this.connectionString = connectionString;
            this.containerName = containerName;
            this.blobName = blobName;
        }

        public static void ConfigureLoggingWithBlobSpecifiedBySettings()
        {
            if (IsNLogConfigSpecified())
                new BlobStorageNLogConfiguration(CloudConfigurationManager.GetSetting(ConnectionStringSettingName),
                                                 CloudConfigurationManager.GetSetting(ContainerNameSettingName),
                                                 CloudConfigurationManager.GetSetting(BlobNameSettingName))
                    .ConfigureLogging();
            else
                Logger.Info(
                    "Blob for NLog configuration is not specified so configuration is not being loaded.\nSet {0}, {1} and {2} in your cloud service configuration to specify an NLog configuration file in blob storage.",
                    ConnectionStringSettingName,
                    ContainerNameSettingName,
                    BlobNameSettingName);
        }

        private static bool IsNLogConfigSpecified()
        {
            return !(string.IsNullOrWhiteSpace(CloudConfigurationManager.GetSetting(ConnectionStringSettingName)) ||
                     string.IsNullOrWhiteSpace(CloudConfigurationManager.GetSetting(ContainerNameSettingName)) ||
                     string.IsNullOrWhiteSpace(CloudConfigurationManager.GetSetting(BlobNameSettingName)));
        }

        public void ConfigureLogging()
        {
            ConfigureLoggingWithBlob(GetBlobReference());
        }

        private CloudBlockBlob GetBlobReference()
        {
            Logger.Debug("Getting reference to blob \"{0}\" in container \"{1}\" using connection string \"{2}\".",
                         this.blobName,
                         this.containerName,
                         this.connectionString);

            return CloudStorageAccount.Parse(this.connectionString)
                                      .CreateCloudBlobClient()
                                      .GetContainerReference(this.containerName)
                                      .GetBlockBlobReference(this.blobName);
        }

        private static void ConfigureLoggingWithBlob(CloudBlockBlob blob)
        {
            XmlLoggingConfiguration configuration = ReadLoggingConfiguration(blob);

            Logger.Debug("Applying new configuration to NLog.");
            LogManager.Configuration = configuration;
            Logger.Info("NLog is now using the configuration loaded from {0}.", blob.Uri);
        }

        private static XmlLoggingConfiguration ReadLoggingConfiguration(CloudBlockBlob blob)
        {
            Logger.Debug("Opening blob for reading.");
            Stream stream = blob.OpenRead();

            Logger.Debug("Reading logging configuration from blob.");
            return new XmlLoggingConfiguration(new XmlTextReader(stream), null);
        }
    }
}