using Microsoft.OpenApi.Models;

namespace FormlySharp.OpenAPI;

/// <summary>
/// Provides a fluent API for building Formly configurations from OpenAPI schemas.
/// </summary>
public class FormlyOpenApiBuilder
{
    private readonly FormlyOpenApiParser _parser;
    private readonly Dictionary<string, FormlyFieldConfig[]> _formlyConfigs = new();

    /// <summary>
    /// Creates a new FormlyOpenApiBuilder from an OpenAPI specification file.
    /// </summary>
    /// <param name="filePath">Path to the OpenAPI specification file</param>
    public FormlyOpenApiBuilder(string filePath)
    {
        _parser = new FormlyOpenApiParser(filePath);
    }

    /// <summary>
    /// Creates a new FormlyOpenApiBuilder from a stream containing an OpenAPI specification.
    /// </summary>
    /// <param name="stream">Stream containing the OpenAPI specification</param>
    public FormlyOpenApiBuilder(Stream stream)
    {
        _parser = new FormlyOpenApiParser(stream);
    }

    /// <summary>
    /// Creates a new FormlyOpenApiBuilder from an existing FormlyOpenApiParser.
    /// </summary>
    /// <param name="parser">An existing FormlyOpenApiParser instance</param>
    public FormlyOpenApiBuilder(FormlyOpenApiParser parser)
    {
        _parser = parser;
    }

    /// <summary>
    /// Gets Formly configuration for a schema reference and performs additional customization.
    /// </summary>
    /// <param name="schemaReference">Reference to the schema, e.g. "#/components/schemas/Pet"</param>
    /// <param name="customize">Optional action to customize the generated FormlyFieldConfig array</param>
    /// <returns>The builder instance for method chaining</returns>
    public FormlyOpenApiBuilder AddSchema(string schemaReference, Func<FormlyFieldConfig[], FormlyFieldConfig[]> customize)
    {
        ArgumentException.ThrowIfNullOrEmpty(schemaReference);
        ArgumentNullException.ThrowIfNull(customize);

        var configs = _parser.GetFormlyConfig(schemaReference);
        
        // Apply customization if provided
        configs = customize.Invoke(configs);
        
        _formlyConfigs[schemaReference] = configs;
        return this;
    }

    /// <summary>
    /// Gets the Formly configuration for a previously added schema reference.
    /// </summary>
    /// <param name="schemaReference">Reference to the schema, e.g. "#/components/schemas/Pet"</param>
    /// <returns>The FormlyFieldConfig array for the referenced schema</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the schema reference hasn't been added</exception>
    public FormlyFieldConfig[]? GetFormlyConfig(string schemaReference)
    {
        if (_formlyConfigs.TryGetValue(schemaReference, out var configs))
        {
            return configs;
        }

        // Auto-add the schema if not found
        return null;
    }

    /// <summary>
    /// Adds validation options to a field based on OpenAPI schema constraints.
    /// </summary>
    /// <param name="field">The field to add validation to</param>
    /// <param name="schema">The OpenAPI schema containing validation constraints</param>
    public static void AddValidationFromSchema(FormlyFieldConfig field, OpenApiSchema schema)
    {
        if (field.Props == null)
        {
            field.Props = new FormlyFieldProps();
        }

        var validation = new FormlyValidationOptions();
        bool hasValidation = false;

        // Add validations based on schema constraints
        if (schema.MinLength.HasValue)
        {
            field.Props.MinLength = schema.MinLength.Value;
            hasValidation = true;
        }

        if (schema.MaxLength.HasValue)
        {
            field.Props.MaxLength = schema.MaxLength.Value;
            hasValidation = true;
        }

        if (schema.Pattern != null)
        {
            field.Props.Pattern = schema.Pattern;
            hasValidation = true;
        }

        if (schema.Minimum.HasValue)
        {
            field.Props.Min = schema.Minimum.Value;
            hasValidation = true;
        }

        if (schema.Maximum.HasValue)
        {
            field.Props.Max = schema.Maximum.Value;
            hasValidation = true;
        }

        if (schema.Required.Contains(field.Key ?? string.Empty))
        {
            field.Props.Required = true;
        }

        if (hasValidation)
        {
            field.Validation = validation;
        }
    }
}