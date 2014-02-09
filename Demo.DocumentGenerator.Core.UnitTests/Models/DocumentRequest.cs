using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DocumentGenerator.Core.UnitTests.Models
{
    public class DocumentRequest
    {
        public string TemplateName { get; set; }
        public string Culture { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
