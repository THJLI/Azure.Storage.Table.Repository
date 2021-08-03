# Azure.Storage.Table.Repository
This project helps the developer on CRUD in Azure Storage Tables

Example:

In **Program > ConfigureServices**

### Inject RepositorySettings with "AzureWebJobsStorage"

```C#
services.AddSingleton(new RepositorySettings(AzureWebJobsStorage));
```

### Entity Class

```C#
public class ConfigEntity : EntityBase
{
    public string Name { get; set; }
}
```

### Repository

```C#
public class ConfigRepository : RepositoryBase<ConfigEntity>
{
    public ConfigRepository(RepositorySettings repositorySettings)
        :base(repositorySettings)
    {
    }
}
```
### After config, create entity and repository, you can create/read/update/delete entity in Azure Storage Table

```C#
var entity = new ConfigEntity();
entity.Name = "teste entity";
_configRepository.Insert(entity);

var lst = _configRepository.GetAll();

foreach (var item in lst)
    Console.WriteLine(item.Name);
    
```

### Output

```Output
teste entity
```








