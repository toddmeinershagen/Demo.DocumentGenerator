namespace Demo.DocumentGenerator.Core.UnitTests
{
    public interface ITemplateEngine
    {
        string Parse(string template, dynamic model);
    }
}