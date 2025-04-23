namespace FluentValidatorToJsonSchema.Tests;

using FluentValidation;
using FluentValidationToJsonSchema.Tests.TestClasses;
using Newtonsoft.Json.Linq;
using Xunit;

public class ParserRegExpTests : TestBase
{
    [Fact]
    public void Parse_ForAlphanumericPattern_ReturnsProperSchema()
        => Test<AlphanumericValidator>(ExpectedSchema("^[a-zA-Z0-9]+$"));

    [Fact]
    public void Parse_ForEmailPattern_ReturnsProperSchema()
        => Test<EmailValidator>(ExpectedSchema(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"));

    [Fact]
    public void Parse_ForPhoneNumberPattern_ReturnsProperSchema()
        => Test<PhoneNumberValidator>(ExpectedSchema(@"^\+?[1-9]\d{1,14}$"));

    [Fact]
    public void Parse_ForHexColorPattern_ReturnsProperSchema()
        => Test<HexColorValidator>(ExpectedSchema(@"^#?([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$"));

    private JObject ExpectedSchema(string pattern) => new JObject
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
                        { "pattern", pattern }
                    }
                }
            }
        }
    };

    public class AlphanumericValidator : AbstractValidator<PropsOfType<string>>
    {
        public AlphanumericValidator()
        {
            RuleFor(x => x.Property1).Matches("^[a-zA-Z0-9]+$");
        }
    }

    public class EmailValidator : AbstractValidator<PropsOfType<string>>
    {
        public EmailValidator()
        {
            RuleFor(x => x.Property1).Matches(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
        }
    }

    public class PhoneNumberValidator : AbstractValidator<PropsOfType<string>>
    {
        public PhoneNumberValidator()
        {
            RuleFor(x => x.Property1).Matches(@"^\+?[1-9]\d{1,14}$");
        }
    }

    public class HexColorValidator : AbstractValidator<PropsOfType<string>>
    {
        public HexColorValidator()
        {
            RuleFor(x => x.Property1).Matches(@"^#?([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$");
        }
    }
}
