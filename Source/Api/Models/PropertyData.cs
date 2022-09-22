using ROOT.Zfs.Core.Info;

namespace Api.Models
{
    public class PropertyData
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Source { get; set; }

        public static PropertyData FromValue(PropertyValue value)
        {
            return new PropertyData { Name = value.Property.Name, Value = value.Value, Source = value.Source.Name };
        }
    }
}
