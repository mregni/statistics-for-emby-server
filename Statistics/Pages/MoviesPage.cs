using System;
using System.IO;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Plugins;

namespace Statistics.Pages
{
    public class MoviesPage : IPluginConfigurationPage
    {
        public string Name => Constants.Moviepage;

        public ConfigurationPageType ConfigurationPageType => ConfigurationPageType.PluginConfiguration;

        public IPlugin Plugin => Statistics.Plugin.Instance;

        public Stream GetHtmlStream()
        {
            var type = GetType();
            return type.Assembly.GetManifestResourceStream(type.Namespace + ".MoviesPage.html");
        }
    }
}
