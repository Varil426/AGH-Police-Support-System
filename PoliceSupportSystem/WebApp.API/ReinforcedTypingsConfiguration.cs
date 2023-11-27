using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;
using ConfigurationBuilder = Reinforced.Typings.Fluent.ConfigurationBuilder;

namespace WebApp.API;

public class ReinforcedTypingsConfiguration
{
    public static void Configure(ConfigurationBuilder builder)
    {
        builder.Substitute(typeof(Guid), new RtSimpleTypeName("string"));

        builder.Global(config => config
            .CamelCaseForProperties()
            .CamelCaseForMethods()
            .AutoOptionalProperties()
            .UseModules());
    }
}