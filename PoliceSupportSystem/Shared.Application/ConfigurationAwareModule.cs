using Autofac;
using Microsoft.Extensions.Configuration;

namespace Shared.Application;

public abstract class ConfigurationAwareModule : Module
{
    public IConfiguration Configuration { get; }

    protected ConfigurationAwareModule(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected TSettings GetSettings<TSettings>(string sectionName)
    {
        var configSection = Configuration.GetRequiredSection(sectionName);
        var settings = configSection.Get<TSettings>() ?? throw new Exception();
        return settings;
    }
}