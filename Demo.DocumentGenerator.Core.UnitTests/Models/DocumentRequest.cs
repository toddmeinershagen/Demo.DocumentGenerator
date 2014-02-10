using System.Collections.Generic;

namespace Demo.DocumentGenerator.Core.UnitTests.Models
{
    public class DocumentRequest
    {
        public string TemplateName { get; set; }
        public string Culture { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
