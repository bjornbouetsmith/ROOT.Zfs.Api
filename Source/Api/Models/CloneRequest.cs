namespace Api.Models
{
    /// <summary>
    /// Encapsulates the data required to clone a dataset to be independent
    /// </summary>
    public class CloneRequest
    {
        /// <summary>
        /// Name of the dataset that should be promoted
        /// </summary>
        public string Dataset { get; set; }

        /// <summary>
        /// Optional properties to set on the promoted dataset, if they should be different from original
        /// </summary>
        public PropertyData[] Properties { get; set; }

    }
}
