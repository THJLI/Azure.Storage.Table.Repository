namespace Azure.Storage.Table.Repository
{
    public class RepositorySettings
    {
        public RepositorySettings(string azureWebJobsStorage)
        {
            this.AzureWebJobsStorage = azureWebJobsStorage;
        }

        public string AzureWebJobsStorage { get; private set; }
    }

}
