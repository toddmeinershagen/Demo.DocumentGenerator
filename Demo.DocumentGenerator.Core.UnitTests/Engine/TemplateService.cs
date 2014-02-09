using System.Globalization;
using System.IO;

namespace Demo.DocumentGenerator.Core.UnitTests
{
    public class TemplateService
    {
        private const string DefaultLanguage = "en";
        private const string TemplatesDirectoryName = "Templates";
        private const string TemplateFileNameWithCultureTemplate = "{0}.{1}.template";
        private const string TemplateFileNameWithoutCultureTemplate = "{0}.template";

        private readonly IFileSystemService _fileSystemService;
        private readonly ITemplateEngine _templateEngine;
        private readonly string _templatesDirectoryFullName;

        public TemplateService(IFileSystemService fileSystemService, ITemplateEngine templateEngine)
        {
            _fileSystemService = fileSystemService;
            _templateEngine = templateEngine;
            _templatesDirectoryFullName = Path.Combine(_fileSystemService.GetCurrentDirectory(), TemplatesDirectoryName);
        }

        public string Parse(string templateName, dynamic model, CultureInfo cultureInfo = null)
        {
            var templateContent = GetContent(templateName, cultureInfo);
            return _templateEngine.Parse(templateContent, model);
        }

        private string GetContent(string templateName, CultureInfo cultureInfo)
        {
            var templateFileName = TryGetFileName(templateName, cultureInfo);
            if (string.IsNullOrEmpty(templateFileName))
            {
                throw new FileNotFoundException(string.Format("Template file not found for template '{0}' in '{1}'", templateName, _templatesDirectoryFullName));
            }

            return _fileSystemService.ReadAllText(templateFileName);
        }

        private string TryGetFileName(string templateName, CultureInfo cultureInfo)
        {
            var language = GetLanguageName(cultureInfo);

            // check file for current culture
            var fullFileName = GetFullFileName(templateName, language);
            if (_fileSystemService.FileExists(fullFileName))
            {
                return fullFileName;
            }

            // check file for default culture
            if (language != DefaultLanguage)
            {
                fullFileName = GetFullFileName(templateName, DefaultLanguage);
                if (_fileSystemService.FileExists(fullFileName))
                {
                    return fullFileName;
                }
            }

            // check file without culture
            fullFileName = GetFullFileName(templateName, string.Empty);
            if (_fileSystemService.FileExists(fullFileName))
            {
                return fullFileName;
            }

            return string.Empty;
        }

        private static string GetLanguageName(CultureInfo cultureInfo)
        {
            return cultureInfo != null ? cultureInfo.TwoLetterISOLanguageName.ToLower() : DefaultLanguage;
        }

        private string GetFullFileName(string templateName, string language)
        {
            var fileNameTemplate = string.IsNullOrEmpty(language) ? TemplateFileNameWithoutCultureTemplate : TemplateFileNameWithCultureTemplate;
            var templateFileName = string.Format(fileNameTemplate, templateName, language);
            return Path.Combine(_templatesDirectoryFullName, templateFileName);
        }
    }
}