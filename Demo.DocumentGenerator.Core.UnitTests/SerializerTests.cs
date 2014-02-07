using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace Demo.DocumentGenerator.Core.UnitTests
{
    using FactoryGirl.NET;

    [TestFixture]
    public class SerializerTests
    {
        private DateTimeOffset _createdDate = new DateTimeOffset(21.July(2014).AddHours(2));
        private string jsonValue = "{\"TemplateName\":\"First\",\"Culture\":\"en-US\",\"Data\":{\"CreatedDate\":\"2014-07-21T02:00:00-05:00\",\"NetCharges\":125.50,\"PatientFirstName\":\"Todd\",\"PatientLastName\":\"Meinershagen\"}}";

        [TestFixtureSetUp]
        public void Init()
        {
            FactoryGirl.Define(() =>
            {
                var value = new Dictionary<string, object>();
                value.Add("CreatedDate", _createdDate);
                value.Add("NetCharges", 125.50m);
                value.Add("PatientFirstName", "Todd");
                value.Add("PatientLastName", "Meinershagen");

                return new DocumentRequest{ TemplateName = "First", Culture = "en-US", Data = value };
            });
        }

        class DocumentRequest
        {
            public string TemplateName { get; set; }
            public string Culture { get; set; }
            public Dictionary<string, object> Data { get; set; }
        }

        [Test]
        public void given_shallow_dictionary_when_serializing_to_json_should_return_json_string()
        {
            var value = FactoryGirl.Build<DocumentRequest>();

            var serializedValue = JsonConvert.SerializeObject(value);

            serializedValue.Should().Be(jsonValue);
        }
        
        [Test]
        public void given_json_when_deserializing_to_shallow_dictionary_should_return_object()
        {
            var settings = new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat, DateParseHandling = DateParseHandling.DateTimeOffset, DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind };
            var deserializedValue = JsonConvert.DeserializeObject<DocumentRequest>(jsonValue, settings);

            var expectedValue = FactoryGirl.Build<DocumentRequest>();
            deserializedValue.ShouldBeEquivalentTo(expectedValue);
        }

        [Test]
        public void given_request_when_templating_output_should_generate_output()
        {
            var request = FactoryGirl.Build<DocumentRequest>();
            var service = new TemplateService(new FileSystemService(), new StringTemplateEngine());
            var content = service.Parse(request.TemplateName, request.Data, new CultureInfo(request.Culture));

            content.Should().Be("Name:  Todd Meinershagen\r\nNet Charges:  125.50\r\nCreated Date:  7/21/2014\r\nCreated Date (Formatted):  21.07.2014 02:00");
        }
    }
}
