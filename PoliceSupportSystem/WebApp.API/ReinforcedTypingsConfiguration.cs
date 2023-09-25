using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;
using Shared.Application.Integration.DTOs;
using ConfigurationBuilder = Reinforced.Typings.Fluent.ConfigurationBuilder;

namespace WebApp.API;

public class ReinforcedTypingsConfiguration
{
    public static void Configure(ConfigurationBuilder builder)
    {
        builder.Substitute(typeof(Guid), new RtSimpleTypeName("string"));

        // builder.SubstituteGeneric(
        //     typeof(IEnumerable<>),
        //     (type, typeResolver) =>
        //     {
        //         var args = type.GetGenericArguments();
        //         return new RtArrayType(typeResolver.ResolveTypeName(args[0]));
        //     });
        
        builder.Global(config => config
            .CamelCaseForProperties()
            .CamelCaseForMethods()
            .AutoOptionalProperties()
            .UseModules());
    }
}