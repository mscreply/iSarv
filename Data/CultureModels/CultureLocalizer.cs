using System.Globalization;
using iSarv.Resources;
using Microsoft.Extensions.Localization;
using System.Reflection;
using System.Resources;

namespace iSarv.Data.CultureModels
{
    public class CultureLocalizer
    {
        private readonly IStringLocalizer _localizer;
        private readonly ResourceManager _resourceManager;
        public CultureLocalizer(IStringLocalizerFactory factory)
        {
            var type = typeof(ViewResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("ViewResource", assemblyName.Name);
            _resourceManager = new ResourceManager(type.FullName, type.Assembly);
        }

        // if we have formatted string we can provide arguments         
        // e.g.: @Localizer.Text("Hello {0}", User.Name)
        public LocalizedString Text(string key, params string[] arguments)
        {
            return arguments == null
                ? _localizer[key]
                : _localizer[key, arguments];
        }

        public string TextByLang(string key, string lang)
        {
            return _resourceManager.GetString(key, new CultureInfo(lang)) ?? key;
        }
    }
}