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
using Demo.DocumentGenerator.Core.UnitTests.Models;

namespace Demo.DocumentGenerator.Core.UnitTests
{
    using System.Web.Script.Serialization;
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.Collections;
    using Newtonsoft.Json.Linq;

    [TestFixture]
    public class SerializerTests
    {
        private DateTimeOffset _createdDate = new DateTimeOffset(21.July(2014).AddHours(2));
        private string jsonValue = "{\"TemplateName\":\"First\",\"Culture\":\"en-US\",\"Data\":{\"CreatedDate\":\"2014-07-21T02:00:00-05:00\",\"NetCharges\":125.50,\"PatientFirstName\":\"Todd\",\"PatientLastName\":\"Meinershagen\",\"Procedures\":[{\"Code\":\"CD1\",\"Description\":\"Leg amputation\"},{\"Code\":\"CD2\",\"Description\":\"Pace maker replacement\"}]}}";

        [TestFixtureSetUp]
        public void Init()
        {
            FactoryGirl.NET.FactoryGirl.Define(() =>
            {
                var value = new Dictionary<string, object>();
                value.Add("CreatedDate", _createdDate);
                value.Add("NetCharges", 125.50m);
                value.Add("PatientFirstName", "Todd");
                value.Add("PatientLastName", "Meinershagen");

                //var procedures = new [] { new { Code = "CD1", Description = "Leg amputation" }, new { Code = "CD2", Description = "Pace maker replacement" } };
                var procedures = new Procedure[] { new Procedure{ Code = "CD1", Description = "Leg amputation" }, new Procedure{ Code = "CD2", Description = "Pace maker replacement" } };
                value.Add("Procedures", procedures);

                return new DocumentRequest{ TemplateName = "First", Culture = "en-US", Data = value };
            });
        }

        [Test]
        public void given_shallow_dictionary_when_serializing_to_json_should_return_json_string()
        {
            var value = FactoryGirl.NET.FactoryGirl.Build<DocumentRequest>();
            var settings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.DateTimeOffset,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
            };

            var serializedValue = JsonConvert.SerializeObject(value, settings);

            serializedValue.Should().Be(jsonValue);
        }
        
        [Test]
        public void given_json_when_deserializing_to_shallow_dictionary_should_return_object()
        {
            var settings = new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.IsoDateFormat, DateParseHandling = DateParseHandling.DateTimeOffset, DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind };
            settings.Converters.Add(new ObjectDictionaryConverter());
            var deserializedValue = JsonConvert.DeserializeObject<DocumentRequest>(jsonValue, settings);

            var expectedValue = FactoryGirl.NET.FactoryGirl.Build<DocumentRequest>();

            deserializedValue.ShouldBeEquivalentTo(expectedValue, options => options
                .Including(x => x.TemplateName)
                .Including(x => x.Culture));

            object procedures = deserializedValue.Data["Procedures"];

            procedures.Should().BeOfType(typeof(List<dynamic>));
        }

        [Test]
        public void given_request_when_templating_output_should_generate_output()
        {
            var settings = new JsonSerializerSettings() { 
                DateFormatHandling = DateFormatHandling.IsoDateFormat, 
                DateParseHandling = DateParseHandling.DateTimeOffset, 
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
            };
            settings.Converters.Add(new ObjectDictionaryConverter());

            var request = JsonConvert.DeserializeObject<DocumentRequest>(jsonValue, settings);
            
            var service = new TemplateService(new FileSystemService(), new StringTemplateEngine());
            var content = service.Parse(request.TemplateName, request.Data, new CultureInfo(request.Culture));

            content.Should().Be("Name:  Todd Meinershagen\r\nNet Charges:  $125.50\r\nCreated Date:  7/21/2014\r\nCreated Date (Formatted):  21.07.2014 02:00\r\nProcedures:\r\n    1.  CD1 - Leg amputation\r\n    2.  CD2 - Pace maker replacement\r\n");
        }
    }
}
