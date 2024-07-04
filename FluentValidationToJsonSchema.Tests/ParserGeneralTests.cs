namespace FluentValidatorToJsonSchema.Tests;

using FluentValidation;
using Newtonsoft.Json.Linq;
using Xunit;

public class ParserGeneralTests : TestBase
{
    [Fact]
    public void Parse_ForNull_ReturnsMinimalConfiguration() => Test(BasicObject, null);

    [Fact]
    public void Parse_ForEmptyValidator_ReturnsMinimalConfiguration() => Test<EmptyValidator>(BasicObject);

    private JObject BasicObject = new JObject
    {
        { "$schema",  "https://json-schema.org/draft/2020-12/schema"},
        { "type", "object" },
    };

    public class EmptyValidator : AbstractValidator<object> { }

}