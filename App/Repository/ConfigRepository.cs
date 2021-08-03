using Azure.Storage.Table.Repository;

namespace App
{
    public class ConfigRepository : RepositoryBase<ConfigEntity>
    {
        public ConfigRepository(RepositorySettings repositorySettings)
            :base(repositorySettings)
        {

        }

    }
}
