using System.Collections.Concurrent;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Any;
using System.Text.Json;
using FormlySharp;
using Microsoft.OpenApi.Interfaces;

namespace FormlySharp.OpenAPI;

/// <summary>
/// Parser that extracts FormlyFieldConfig arrays from OpenAPI specifications.
/// </summary>
public class FormlyOpenApiParser
{
    private readonly OpenApiDocument _document;
    private readonly ConcurrentDictionary<string, FormlyFieldConfig[]> _cache = new();

    // Extension property name for Formly configuration in OpenAPI schemas
    public const string FormlyExtensionName = "x-formly";

    /// <summary>
    /// Creates a new FormlyOpenApiParser from an OpenAPI specification file.
    /// </summary>
    /// <param name="filePath">Path to the OpenAPI specification file</param>
    public FormlyOpenApiParser(string filePath)
    {
        using var fileStream = File.OpenRead(filePath);
        var reader = new OpenApiStreamReader();
        var result = reader.Read(fileStream, out _);
        _document = result;
    }

    /// <summary>
    /// Creates a new FormlyOpenApiParser from a stream containing an OpenAPI specification.
    /// </summary>
    /// <param name="stream">Stream containing the OpenAPI specification</param>
    public FormlyOpenApiParser(Stream stream)
    {
        var reader = new OpenApiStreamReader();
        var result = reader.Read(stream, out _);
        _document = result;
    }

    /// <summary>
    /// Gets FormlyFieldConfig array for a schema reference in the OpenAPI document.
    /// Results are cached for subsequent calls.
    /// </summary>
    /// <param name="schemaReference">Reference to the schema, e.g. "#/components/schemas/Pet"</param>
    /// <returns>Array of FormlyFieldConfig objects for the referenced schema</returns>
    public FormlyFieldConfig[] GetFormlyConfig(string schemaReference)
    {
        return _cache.GetOrAdd(schemaReference, reference =>
        {
            // Extract schema from reference
            var schema = ResolveSchemaReference(reference);
            if (schema == null)
                throw new ArgumentException($"Schema reference '{reference}' not found in the document");

            return GenerateFormlyConfig(schema);
        });
    }

    private OpenApiSchema? ResolveSchemaReference(string reference)
    {
        if (!reference.StartsWith("#/components/schemas/"))
        {
            throw new ArgumentException(
                "Only local references to components/schemas are supported (format: #/components/schemas/Name)",
                nameof(reference));
        }

        var schemaName = reference.Substring("#/components/schemas/".Length);
        
        if (_document.Components?.Schemas?.TryGetValue(schemaName, out var schema) == true)
        {
            return schema;
        }

        return null;
    }

    private FormlyFieldConfig[] GenerateFormlyConfig(OpenApiSchema schema)
    {
        // Check if there's a custom formly extension
        if (schema.Extensions.TryGetValue(FormlyExtensionName, out var extension))
        {
            return ParseFormlyExtension(extension);
        }

        // Otherwise, generate FormlyFieldConfig based on the schema properties
        return GenerateFormlyConfigFromSchema(schema);
    }

    private FormlyFieldConfig[] ParseFormlyExtension(IOpenApiExtension extension)
    {
        //if (extension is OpenApiArray arr)
        //{
        //    // If it's an array, parse each item
        //    var configs = new List<FormlyFieldConfig>();
        //    foreach (var item in arr)
        //    {
        //        if (item is OpenApiObject obj)
        //        {
        //            var config = new FormlyFieldConfig();
        //            // Parse the object as needed
        //            configs.Add(config);
        //        }
        //    }
        //    return configs.ToArray();


        //}

        // Parse the custom extension which should contain Formly configuration
        if (extension is OpenApiObject obj)
        {
            try
            {
                // If it's a direct array in the extension
                if (obj.TryGetValue("0", out var firstItem))
                {
                    // This appears to be an array format
                    var configs = new List<FormlyFieldConfig>();
                    int index = 0;
                    
                    while (obj.TryGetValue(index.ToString(), out var item))
                    {
                        if (item is OpenApiObject fieldObj)
                        {
                            var config = new FormlyFieldConfig();
                            
                            // Parse key
                            if (fieldObj.TryGetValue("key", out var keyValue) && keyValue is OpenApiString keyString)
                            {
                                config.Key = keyString.Value;
                            }
                            
                            // Parse type
                            if (fieldObj.TryGetValue("type", out var typeValue) && typeValue is OpenApiString typeString)
                            {
                                config.Type = typeString.Value;
                            }
                            
                            // Parse props if available
                            if (fieldObj.TryGetValue("props", out var propsValue) && propsValue is OpenApiObject propsObj)
                            {
                                config.Props = new FormlyFieldProps();
                                
                                // Parse label
                                if (propsObj.TryGetValue("label", out var labelValue) && labelValue is OpenApiString labelString)
                                {
                                    config.Props.Label = labelString.Value;
                                }
                                
                                // Add more props parsing as needed
                            }
                            
                            configs.Add(config);
                        }
                        
                        index++;
                    }
                    
                    return configs.ToArray();
                }
                else
                {
                    // Try to parse as a single FormlyFieldConfig
                    var config = new FormlyFieldConfig();
                    
                    if (obj.TryGetValue("key", out var keyValue) && keyValue is OpenApiString keyString)
                    {
                        config.Key = keyString.Value;
                    }
                    
                    if (obj.TryGetValue("type", out var typeValue) && typeValue is OpenApiString typeString)
                    {
                        config.Type = typeString.Value;
                    }
                    
                    return new[] { config };
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse {FormlyExtensionName} extension: {ex.Message}", ex);
            }
        }
        
        return Array.Empty<FormlyFieldConfig>();
    }

    private FormlyFieldConfig[] GenerateFormlyConfigFromSchema(OpenApiSchema schema)
    {
        var fields = new List<FormlyFieldConfig>();

        // If it's an object, process its properties
        if (schema.Type == "object" && schema.Properties != null)
        {
            foreach (var property in schema.Properties)
            {
                var field = CreateFieldFromProperty(property.Key, property.Value, schema.Required);
                if (field != null)
                {
                    fields.Add(field);
                }
            }
        }

        return fields.ToArray();
    }

    private FormlyFieldConfig? CreateFieldFromProperty(string propertyName, OpenApiSchema propertySchema, ISet<string>? requiredProperties = null)
    {
        var field = new FormlyFieldConfig
        {
            Key = propertyName,
            Type = MapOpenApiTypeToFormlyType(propertySchema.Type),
            Props = new FormlyFieldProps
            {
                Label = propertySchema.Title ?? propertyName,
                Description = propertySchema.Description,
                Required = requiredProperties != null && requiredProperties.Contains(propertyName)
            }
        };

        // Add validation properties
        if (propertySchema.MinLength.HasValue)
            field.Props.MinLength = propertySchema.MinLength.Value;
            
        if (propertySchema.MaxLength.HasValue)
            field.Props.MaxLength = propertySchema.MaxLength.Value;
            
        if (propertySchema.Minimum.HasValue)
            field.Props.Min = (int)propertySchema.Minimum.Value;
            
        if (propertySchema.Maximum.HasValue)
            field.Props.Max = (int)propertySchema.Maximum.Value;
            
        if (!string.IsNullOrEmpty(propertySchema.Pattern))
            field.Props.Pattern = propertySchema.Pattern;

        // Handle different schema types
        switch (propertySchema.Type)
        {
            case "object" when propertySchema.Properties != null:
                field.FieldGroup = GenerateFormlyConfigFromSchema(propertySchema);
                break;
                
            case "array" when propertySchema.Items != null:
                field.FieldArray = new FormlyFieldConfig
                {
                    FieldGroup = GenerateFormlyConfigFromSchema(propertySchema.Items)
                };
                break;
                
            case "string" when propertySchema.Enum.Count > 0:
                // Handle enum type
                field.Props.Options = propertySchema.Enum
                    .OfType<OpenApiString>()
                    .Select(e => new { Value = e.Value, Label = e.Value })
                    .ToList();
                break;
        }

        return field;
    }

    private string MapOpenApiTypeToFormlyType(string openApiType)
    {
        return openApiType switch
        {
            "string" => "input",
            "number" => "number",
            "integer" => "number",
            "boolean" => "checkbox",
            "object" => "object",
            "array" => "array",
            _ => "input"
        };
    }
}
