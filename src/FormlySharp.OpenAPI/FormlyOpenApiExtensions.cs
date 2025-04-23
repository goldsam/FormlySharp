using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using System.Text.Json;

namespace FormlySharp.OpenAPI;

/// <summary>
/// Extension methods to facilitate working with OpenAPI schemas and Formly configurations.
/// </summary>
public static class FormlyOpenApiExtensions
{
    /// <summary>
    /// Adds Formly configuration to an OpenAPI Schema as an extension property.
    /// </summary>
    /// <param name="schema">The OpenAPI schema to extend</param>
    /// <param name="formlyConfig">The Formly field configuration to add</param>
    /// <returns>The modified schema for method chaining</returns>
    public static OpenApiSchema WithFormlyConfig(this OpenApiSchema schema, FormlyFieldConfig[] formlyConfig)
    {
        var json = JsonSerializer.Serialize(formlyConfig, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        // Parse the JSON string to create OpenApiObject
        var openApiObject = ParseJsonToOpenApiObject(json);
        
        // Add extension to the schema
        schema.Extensions[FormlyOpenApiParser.FormlyExtensionName] = openApiObject;
        
        return schema;
    }

    /// <summary>
    /// Helper method to parse JSON into an OpenApiObject
    /// </summary>
    private static OpenApiObject ParseJsonToOpenApiObject(string json)
    {
        using var document = JsonDocument.Parse(json);
        return ConvertObject(document.RootElement);
    }

    /// <summary>
    /// Converts a System.Text.Json.JsonElement to an OpenApiObject
    /// </summary>
    private static IOpenApiAny ConvertJsonElementToOpenApiObject(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => ConvertObject(element),
            JsonValueKind.Array => ConvertArray(element),
            JsonValueKind.String => new OpenApiString(element.GetString()!),
            JsonValueKind.Number => element.TryGetInt32(out var intValue) 
                ? new OpenApiInteger(intValue) 
                : new OpenApiDouble(element.GetDouble()),
            JsonValueKind.True => new OpenApiBoolean(true),
            JsonValueKind.False => new OpenApiBoolean(false),
            JsonValueKind.Null => new OpenApiNull(),
            _ => new OpenApiNull()
        };
    }

    private static OpenApiObject ConvertObject(JsonElement element)
    {
        var obj = new OpenApiObject();
        foreach (var property in element.EnumerateObject())
        {
            obj.Add(property.Name, ConvertJsonElementToOpenApiObject(property.Value));
        }
        return obj;
    }

    private static OpenApiArray ConvertArray(JsonElement element)
    {
        var array = new OpenApiArray();
        foreach (var item in element.EnumerateArray())
        {
            array.Add(ConvertJsonElementToOpenApiObject(item));
        }
        return array;
    }

    /// <summary>
    /// Extension method to generate Formly configuration for a model type using both
    /// the model's properties and any OpenAPI schema information.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="builder">The FormlyBuilder instance</param>
    /// <param name="parser">The OpenAPI parser</param>
    /// <param name="schemaReference">Reference to the schema in the OpenAPI spec</param>
    /// <returns>The FormlyBuilder for method chaining</returns>
    public static FormlyBuilder<T> ApplyOpenApiSchema<T>(
        this FormlyBuilder<T> builder,
        FormlyOpenApiParser parser,
        string schemaReference)
    {
        // This is a placeholder for a more complex implementation
        // that would merge Formly configurations from both the model properties
        // and the OpenAPI schema
        
        // In a full implementation, this would:
        // 1. Get the OpenAPI schema for the reference
        // 2. Apply any additional properties, validations, etc. from the schema
        // 3. Return the enhanced builder
        
        return builder;
    }
}