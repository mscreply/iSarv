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
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName!);
            _localizer = factory.Create("ViewResource", assemblyName.Name!);
            _resourceManager = new ResourceManager(type.FullName!, type.Assembly);
        }

        // if we have formatted string we can provide arguments         
        // e.g.: @Localizer.TextIgnoreCase("Hello {0}", User.Name)
        public LocalizedString Text(string key, params string[] arguments)
        {
            try
            {
                return arguments == null
                    ? _localizer[key]
                    : _localizer[key, arguments];
            }
            catch (Exception e)
            {
                return new LocalizedString("", key);
            }
        }

        public string TextIgnoreCase(string key)
        {
            _resourceManager.IgnoreCase = true;
            try
            {
                return _resourceManager.GetString(key) ?? key;
            }
            catch (Exception)
            {
                return key;
            }
        }
    }
}