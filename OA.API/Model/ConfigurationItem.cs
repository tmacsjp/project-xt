namespace OA.API.Model
{
    public class ConfigurationItem<T>
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public T DefaultValue { get; set; }
        public ConfigValueType ValueType { get; set; } = ConfigValueType.text;
        public string? Remark { get; set; } = String.Empty;

        public ConfigurationItem(string key, string title, T defaultValue, ConfigValueType valueType, string remark = "")
        {
            Key = key;
            Title = title;
            DefaultValue = defaultValue;
            ValueType = valueType;
            Remark = remark;
        }
    }

    public enum ConfigValueType
    {
        text,
        longtext,
        json,
        boolean,
        @int,
        @decimal,
        other
    }
}
