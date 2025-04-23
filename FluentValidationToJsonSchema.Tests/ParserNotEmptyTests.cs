namespace FluentValidatorToJsonSchema.Tests;

using FluentValidation;
using FluentValidationToJsonSchema.Tests.TestClasses;
using Newtonsoft.Json.Linq;
using Xunit;

public class ParserNotEmptyTests : TestBase
{
    [Fact]
    public void Parse_ForStringNotEmptyValidator_ReturnsProperSchema()
        => Test<NotEmptyStringValidator>(ExpectedSchemaForString());

    [Fact]
    public void Parse_ForArrayNotEmptyValidator_ReturnsProperSchema()
        => Test<NotEmptyArrayValidator>(ExpectedSchemaForArray());

    private JObject ExpectedSchemaForString() => new JObject
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
                        { "type", "string" },
                        { "minLength", 1 }
                    }
                }
            }
        }
    };

    private JObject ExpectedSchemaForArray() => new JObject
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
                        { "type", "array" },
                        { "minItems", 1 }
                    }
                }
            }
        }
    };

    public class NotEmptyStringValidator : AbstractValidator<PropsOfType<string>>
    {
        public NotEmptyStringValidator()
        {
            RuleFor(x => x.Property1).NotEmpty();
        }
    }

    public class NotEmptyArrayValidator : AbstractValidator<PropsOfType<List<string>>>
    {
        public NotEmptyArrayValidator()
        {
            RuleFor(x => x.Property1).NotEmpty();
        }
    }
}
