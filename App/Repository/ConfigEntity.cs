using Azure.Storage.Table.Repository;
using Azure.Storage.Table.Repository.Converter;

namespace App
{
    public enum enuStatus: int
    {
        None = 0,
        Enable = 1,
        Disable = 2
    }

    public class ConfigEntity : EntityBase
    {

        public string Name { get; set; }

        [EntityEnumPropertyConverter]
        public enuStatus Status { get; set; } = enuStatus.None;

    }
}
