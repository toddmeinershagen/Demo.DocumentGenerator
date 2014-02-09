using System;
using Antlr4.StringTemplate;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Demo.DocumentGenerator.Core.UnitTests
{
    public class StringTemplateEngine : ITemplateEngine
    {
        public string Parse(string template, dynamic model)
        {
            var group = new TemplateGroupString("group", "delimiters \"%\", \"%\"\r\nt(x) ::= \" % x % \"");

            var renderer = new AdvancedRenderer();
            group.RegisterRenderer(typeof(DateTime), renderer);
            group.RegisterRenderer(typeof(DateTimeOffset), renderer);
            group.RegisterRenderer(typeof(double), renderer);
            group.RegisterRenderer(typeof(decimal), renderer);

            group.DefineTemplate("template", template, new[] { "Model" });

            var stringTemplate = group.GetInstanceOf("template");
            stringTemplate.Add("Model", model);

            return stringTemplate.Render();
        }
    }
}
