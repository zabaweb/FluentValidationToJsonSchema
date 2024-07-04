namespace FluentValidatorToJsonSchema.Tests;

using FluentAssertions;
using FluentAssertions.Json;
using FluentValidation;
using Newtonsoft.Json.Linq;

public abstract class TestBase
{
    protected readonly IParser parser = new Parser();

    public void Test<T>(JObject expectedSchema) where T : IValidator, new()
    {
        IValidator validator = new T();
        Test(expectedSchema, validator);
    }

    public void Test(JObject expectedSchema, IValidator validator)
    {
        var schema = parser.Parse(validator);
        schema.Should().BeEquivalentTo(expectedSchema);
    }
}