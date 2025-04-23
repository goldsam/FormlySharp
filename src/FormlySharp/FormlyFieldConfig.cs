namespace FormlySharp;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Represents an Angular Formly field configuration.
/// This class mirrors Angular Formly's FormlyFieldConfig interface and is used to define
/// form field configurations in a strongly-typed way.
/// </summary>
/// <remarks>
/// See https://formly.dev/docs/api/core/#formlyfieldconfig for the Angular Formly documentation.
/// </remarks>
public record FormlyFieldConfig
{
    /// <summary>
    /// The field key, typically the property name from the model.
    /// This is used to bind the field to the corresponding model property.
    /// </summary>
    [JsonPropertyName("key")]
    public object? Key { get; init; }

    /// <summary>
    /// The HTML ID attribute for the field.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// The name attribute for the field. Useful when submitting forms traditionally.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// CSS class names to apply to the field.
    /// </summary>
    [JsonPropertyName("className")]
    public string? ClassName { get; init; }

    /// <summary>
    /// CSS class names to apply to field groups.
    /// </summary>
    [JsonPropertyName("fieldGroupClassName")]
    public string? FieldGroupClassName { get; init; }

    /// <summary>
    /// The field type (e.g., "input", "select", "textarea", etc.).
    /// This must match a registered field type in the Angular Formly configuration.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// The default value for the field.
    /// </summary>
    [JsonPropertyName("defaultValue")]
    public object? DefaultValue { get; init; }

    /// <summary>
    /// Custom template HTML for the field.
    /// </summary>
    [JsonPropertyName("template")]
    public string? Template { get; init; }

    /// <summary>
    /// Field properties used to configure the field's appearance and behavior.
    /// </summary>
    [JsonPropertyName("props")]
    public FormlyFieldProps? Props { get; init; }

    /// <summary>
    /// Validators for the field. The keys are validator names and values are validator configurations.
    /// </summary>
    [JsonPropertyName("validators")]
    public Dictionary<string, object>? Validators { get; init; }

    /// <summary>
    /// Asynchronous validators for the field. The keys are validator names and values are validator configurations.
    /// </summary>
    [JsonPropertyName("asyncValidators")]
    public Dictionary<string, object>? AsyncValidators { get; init; }

    /// <summary>
    /// Validation options including validation messages and display preferences.
    /// </summary>
    [JsonPropertyName("validation")]
    public FormlyValidationOptions? Validation { get; init; }

    /// <summary>
    /// Expression properties allow dynamically setting field properties based on model values or other conditions.
    /// The keys are property paths (e.g., "props.disabled") and values are expressions.
    /// </summary>
    [JsonPropertyName("expressionProperties")]
    public Dictionary<string, object>? ExpressionProperties { get; init; }

    /// <summary>
    /// Whether the field should be hidden.
    /// </summary>
    [JsonPropertyName("hide")]
    public bool? Hide { get; init; }

    /// <summary>
    /// An expression that determines whether the field should be hidden dynamically.
    /// Can be a string or function expression.
    /// </summary>
    [JsonPropertyName("hideExpression")]
    public object? HideExpression { get; init; }

    /// <summary>
    /// Wrappers to apply to the field. These must be registered in the Angular Formly configuration.
    /// </summary>
    [JsonPropertyName("wrappers")]
    public string[]? Wrappers { get; init; }

    /// <summary>
    /// Whether the field should automatically receive focus when rendered.
    /// </summary>
    [JsonPropertyName("focus")]
    public bool? Focus { get; init; }

    /// <summary>
    /// Options for controlling how the field updates the model.
    /// </summary>
    [JsonPropertyName("modelOptions")]
    public FormlyModelOptions? ModelOptions { get; init; }

    /// <summary>
    /// Lifecycle hooks for the field.
    /// </summary>
    [JsonPropertyName("hooks")]
    public FormlyLifecycleOptions? Hooks { get; init; }

    /// <summary>
    /// Legacy lifecycle options, retained for compatibility.
    /// Prefer using Hooks for new code.
    /// </summary>
    [JsonPropertyName("lifecycle")]
    public FormlyLifecycleOptions? Lifecycle { get; init; }

    /// <summary>
    /// Child fields when this field is a group. Used for complex object properties.
    /// </summary>
    [JsonPropertyName("fieldGroup")]
    public FormlyFieldConfig[]? FieldGroup { get; init; }

    /// <summary>
    /// Configuration for array fields. Defines the field template to use for each array item.
    /// </summary>
    [JsonPropertyName("fieldArray")]
    public FormlyFieldConfig? FieldArray { get; init; }

    /// <summary>
    /// Parser functions to apply when processing field values.
    /// </summary>
    [JsonPropertyName("parsers")]
    public List<string>? Parsers { get; init; }
}