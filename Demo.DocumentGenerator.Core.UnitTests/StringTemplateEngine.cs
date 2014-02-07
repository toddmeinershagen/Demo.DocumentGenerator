using System;
using Antlr4.StringTemplate;

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

            group.DefineTemplate("template", template, new[] { "Model" });

            var stringTemplate = group.GetInstanceOf("template");
            stringTemplate.Add("Model", model);

            return stringTemplate.Render();
        }
    }
}
