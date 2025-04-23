namespace FormlySharp;

using System;

/// <summary>
/// Provides extension methods for FieldBuilder with type-specific functionality.
/// </summary>
public static class FieldBuilderExtensions
{
    /// <summary>
    /// Sets the minimum value for the field. Only available when the field type implements IComparable.
    /// </summary>
    /// <typeparam name="TModel">The model type that contains this field</typeparam>
    /// <typeparam name="TProp">The property type for this field, which must implement IComparable</typeparam>
    /// <param name="builder">The field builder</param>
    /// <param name="min">The minimum value (must be the same type as the field)</param>
    /// <returns>The field builder for method chaining</returns>
    public static FieldBuilder<TModel, TProp> Min<TModel, TProp>(
        this FieldBuilder<TModel, TProp> builder, 
        TProp min) where TProp : IComparable<TProp>
    {
        var prop = builder.GetTemplateOptions();
        prop = prop with { Min = min! };
        return builder.WithProps(prop);
    }

    /// <summary>
    /// Sets the maximum value for the field. Only available when the field type implements IComparable.
    /// </summary>
    /// <typeparam name="TModel">The model type that contains this field</typeparam>
    /// <typeparam name="TProp">The property type for this field, which must implement IComparable</typeparam>
    /// <param name="builder">The field builder</param>
    /// <param name="max">The maximum value (must be the same type as the field)</param>
    /// <returns>The field builder for method chaining</returns>
    public static FieldBuilder<TModel, TProp> Max<TModel, TProp>(
        this FieldBuilder<TModel, TProp> builder, 
        TProp max) where TProp : IComparable<TProp>
    {
        var prop = builder.GetTemplateOptions();
        prop = prop with { Max = max! };
        return builder.WithProps(prop);
    }
}