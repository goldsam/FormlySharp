namespace FormlySharp;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// The main entry point for creating Angular Formly configurations with a strongly-typed API.
/// </summary>
public static class FormlyBuilder
{
    /// <summary>
    /// Creates a new FormlyBuilder for the specified model type.
    /// </summary>
    /// <typeparam name="T">The model type to build a form configuration for</typeparam>
    /// <returns>A new FormlyBuilder instance</returns>
    public static FormlyBuilder<T> For<T>() => new();
}

/// <summary>
/// Provides a fluent API for building Angular Formly field configurations from a strongly-typed model.
/// </summary>
/// <typeparam name="T">The model type to build a form configuration for</typeparam>
public class FormlyBuilder<T>
{
    private readonly List<FormlyFieldConfig> _fields = new();

    /// <summary>
    /// Define a primitive or simple field based on a property of the model.
    /// </summary>
    /// <typeparam name="TProp">The property type</typeparam>
    /// <param name="expr">Expression that selects the property from the model</param>
    /// <param name="configure">Action that configures the field</param>
    /// <returns>This builder instance for method chaining</returns>
    /// <exception cref="ArgumentException">Thrown when the expression is not a member expression</exception>
    public FormlyBuilder<T> Field<TProp>(
        Expression<Func<T, TProp>> expr,
        Action<FieldBuilder<T, TProp>> configure)
    {
        if (expr.Body is not MemberExpression m)
            throw new ArgumentException("Must use a member expression", nameof(expr));
        var key = m.Member.Name;
        var fb = new FieldBuilder<T, TProp>(key);
        configure(fb);
        _fields.Add(fb.Build());
        return this;
    }

    /// <summary>
    /// Define nested fields for a complex object property.
    /// This creates a field group that contains child fields for the properties of the nested object.
    /// </summary>
    /// <typeparam name="TNested">The type of the nested object</typeparam>
    /// <param name="expr">Expression that selects the nested object property from the model</param>
    /// <param name="configure">Action that configures the fields for the nested object</param>
    /// <returns>This builder instance for method chaining</returns>
    /// <exception cref="ArgumentException">Thrown when the expression is not a member expression</exception>
    public FormlyBuilder<T> FieldGroup<TNested>(
        Expression<Func<T, TNested>> expr,
        Action<FormlyBuilder<TNested>> configure)
    {
        if (expr.Body is not MemberExpression m)
            throw new ArgumentException("Must use a member expression", nameof(expr));
        var key = m.Member.Name;
        var nested = new FormlyBuilder<TNested>();
        configure(nested);
        _fields.Add(new FormlyFieldConfig { Key = key, FieldGroup = nested.Build() });
        return this;
    }

    /// <summary>
    /// Define nested fields for an array of objects.
    /// This creates a field array configuration that allows generating forms for collections.
    /// </summary>
    /// <typeparam name="TNested">The type of objects in the array</typeparam>
    /// <param name="expr">Expression that selects the array property from the model</param>
    /// <param name="configure">Action that configures the fields for each item in the array</param>
    /// <returns>This builder instance for method chaining</returns>
    /// <exception cref="ArgumentException">Thrown when the expression is not a member expression</exception>
    public FormlyBuilder<T> FieldArray<TNested>(
        Expression<Func<T, TNested[]>> expr,
        Action<FormlyBuilder<TNested>> configure)
    {
        if (expr.Body is not MemberExpression m)
            throw new ArgumentException("Must use a member expression", nameof(expr));
        var key = m.Member.Name;
        var nested = new FormlyBuilder<TNested>();
        configure(nested);
        _fields.Add(new FormlyFieldConfig
        {
            Key = key,
            FieldArray = new FormlyFieldConfig { FieldGroup = nested.Build() }
        });
        return this;
    }

    /// <summary>
    /// Builds the Formly field configuration array.
    /// </summary>
    /// <returns>An array of FormlyFieldConfig objects</returns>
    public FormlyFieldConfig[] Build() => _fields.ToArray();

    /// <summary>
    /// Builds and serializes the Formly field configuration to JSON.
    /// </summary>
    /// <param name="opts">Optional JSON serializer options, defaults to camelCase property naming</param>
    /// <returns>A JSON string representation of the form configuration</returns>
    public string BuildJson(JsonSerializerOptions? opts = null) =>
       JsonSerializer.Serialize(_fields,
            opts ?? new JsonSerializerOptions 
            { 
              PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
              DefaultIgnoreCondition  = JsonIgnoreCondition.WhenWritingNull
            });
}