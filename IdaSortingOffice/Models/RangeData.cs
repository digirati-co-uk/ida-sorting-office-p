using System;

namespace IdaSortingOffice.Models
{
    [Serializable]
    public class RangeData
    {
        public string Id { get; set; }
        public string ManifestId { get; set; }
        public string StartCanvas { get; set; }
        public string EndCanvas { get; set; }
        public string UnitType { get; set; }
        public string Label { get; set; }
        public string RangeManifestId { get; set; }
    }
}