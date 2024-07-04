namespace FluentValidatorToJsonSchema.Tests;

using FluentValidation;
using FluentValidationToJsonSchema.Tests.TestClasses;
using Newtonsoft.Json.Linq;
using Xunit;

public class ParserNotNullTests : TestBase
{
    [Fact]
    public void Parse_ForStringNotNullValidator_ReturnsProperSchema() => Test<NotNullValidator<string>>(NotNullValidatorExpectedResult("string"));


    [Fact]
    public void Parse_ForIntNotNullValidator_ReturnsProperSchema() => Test<NotNullValidator<int?>>(NotNullValidatorExpectedResult("number"));


    [Fact]
    public void Parse_ForObjectNotNullValidator_ReturnsProperSchema() => Test<NotNullValidator<object>>(NotNullValidatorExpectedResult("object"));

    private JObject NotNullValidatorExpectedResult(string type) => new JObject
        {
            { "$schema",  "https://json-schema.org/draft/2020-12/schema"},
            { "type", "object" },
            {
                "properties",
                new JObject
                {
                    {
                        "Property1",
                        new JObject
                        {
                            { "type" , new JArray { type } }
                        }
                    }
                }
            },
        };

    public class NotNullValidator<T> : AbstractValidator<PropsOfType<T>>
    {
        public NotNullValidator()
        {
            RuleFor(x => x.Property1).NotNull();
            RuleFor(x => x.Property1).NotNull();
        }
    }
}