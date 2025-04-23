namespace FormlySharp;

using System.Collections.Generic;

/// <summary>
/// Provides a fluent API for configuring individual form fields.
/// </summary>
/// <typeparam name="TModel">The model type that contains this field</typeparam>
/// <typeparam name="TProp">The property type for this field</typeparam>
public class FieldBuilder<TModel, TProp>
{
    private FormlyFieldConfig _c;
    
    /// <summary>
    /// Initializes a new instance of the FieldBuilder class with the specified key.
    /// </summary>
    /// <param name="key">The field key, typically the property name</param>
    public FieldBuilder(string key) => _c = new() { Key = key };

    /// <summary>
    /// Sets the HTML ID attribute for the field.
    /// </summary>
    /// <param name="id">The HTML ID value</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> Id(string id)       { _c = _c with { Id = id }; return this; }
    
    /// <summary>
    /// Sets the name attribute for the field.
    /// </summary>
    /// <param name="name">The name value</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> Name(string name)   { _c = _c with { Name = name }; return this; }
    
    /// <summary>
    /// Sets CSS class names for the field.
    /// </summary>
    /// <param name="css">CSS class names</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> ClassName(string css){ _c = _c with { ClassName = css }; return this; }
    
    /// <summary>
    /// Sets CSS class names for field groups.
    /// </summary>
    /// <param name="css">CSS class names</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> FieldGroupClassName(string css) { _c = _c with { FieldGroupClassName = css }; return this; }
    
    /// <summary>
    /// Sets the field type (e.g., "input", "select", "textarea", etc.).
    /// </summary>
    /// <param name="type">The field type value</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> Type(string type)   { _c = _c with { Type = type }; return this; }
    
    /// <summary>
    /// Sets the default value for the field.
    /// </summary>
    /// <param name="val">The default value</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> DefaultValue(object? val) { _c = _c with { DefaultValue = val }; return this; }
    
    /// <summary>
    /// Sets a custom template for the field.
    /// </summary>
    /// <param name="tpl">The template string</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> Template(string tpl) { _c = _c with { Template = tpl }; return this; }

    /// <summary>
    /// Ensures that Props dictionary exists, creating it if needed.
    /// </summary>
    /// <returns>The Props dictionary</returns>
    private FormlyFieldProps EnsureProps()
    {
        var p = _c.Props ?? new FormlyFieldProps();
        _c = _c with { Props = p };
        return p;
    }
    
    /// <summary>
    /// Sets a custom property for the field.
    /// </summary>
    /// <param name="key">The property key</param>
    /// <param name="value">The property value</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> Prop(string key, object value) { var p = EnsureProps(); p.AdditionalProperties[key] = value; return this; }

    /// <summary>
    /// Ensures that Validators dictionary exists, creating it if needed.
    /// </summary>
    /// <returns>The Validators dictionary</returns>
    private Dictionary<string, object> EnsureValidators()
    {
        var v = _c.Validators ?? new Dictionary<string, object>();
        _c = _c with { Validators = v };
        return v;
    }
    
    /// <summary>
    /// Adds a validator to the field.
    /// </summary>
    /// <param name="name">The validator name</param>
    /// <param name="validator">The validator configuration</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> Validator(string name, object validator) { var v = EnsureValidators(); v[name] = validator; return this; }

    /// <summary>
    /// Ensures that AsyncValidators dictionary exists, creating it if needed.
    /// </summary>
    /// <returns>The AsyncValidators dictionary</returns>
    private Dictionary<string, object> EnsureAsyncValidators()
    {
        var a = _c.AsyncValidators ?? new Dictionary<string, object>();
        _c = _c with { AsyncValidators = a };
        return a;
    }
    
    /// <summary>
    /// Adds an asynchronous validator to the field.
    /// </summary>
    /// <param name="name">The validator name</param>
    /// <param name="validator">The validator configuration</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> AsyncValidator(string name, object validator) { var a = EnsureAsyncValidators(); a[name] = validator; return this; }

    /// <summary>
    /// Ensures that Validation options exist, creating them if needed.
    /// </summary>
    /// <returns>The Validation options</returns>
    private FormlyValidationOptions EnsureValidation()
    {
        var vo = _c.Validation ?? new FormlyValidationOptions();
        _c = _c with { Validation = vo };
        return vo;
    }
    
    /// <summary>
    /// Adds a validation message for a specific validation rule.
    /// </summary>
    /// <param name="key">The validation rule name</param>
    /// <param name="message">The validation error message</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> ValidationMessage(string key, string message)
    {
        var vo = EnsureValidation();
        var m = vo.Messages ?? new Dictionary<string, string>(); m[key] = message;
        vo = vo with { Messages = m };
        _c = _c with { Validation = vo };
        return this;
    }
    
    /// <summary>
    /// Controls whether validation messages should be displayed.
    /// </summary>
    /// <param name="show">Whether to show validation messages</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> ShowValidation(bool show = true)
    {
        var vo = EnsureValidation();
        vo = vo with { Show = show };
        _c = _c with { Validation = vo };
        return this;
    }

    /// <summary>
    /// Ensures that TemplateOptions exist, creating them if needed.
    /// </summary>
    /// <returns>The TemplateOptions</returns>
    private FormlyFieldProps EnsureTemplateOptions()
    {
        var to = _c.Props ?? new FormlyFieldProps();
        _c = _c with { Props = to };
        return to;
    }

    /// <summary>
    /// Ensures that TemplateOptions exist, creating them if needed. Accessible to extensions.
    /// </summary>
    /// <returns>The TemplateOptions</returns>
    public FormlyFieldProps GetTemplateOptions() => EnsureTemplateOptions();
    
    /// <summary>
    /// Updates the field's template options with the provided options. Accessible to extensions.
    /// </summary>
    /// <param name="props">The template options to set</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> WithProps(FormlyFieldProps props)
    {
        _c = _c with { Props = props };
        return this;
    }

    /// <summary>
    /// Sets the label for the field.
    /// </summary>
    /// <param name="label">The label text</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> Label(string label)
    {
        var to = EnsureTemplateOptions();
        to = to with { Label = label };
        _c = _c with { Props = to };
        return this;
    }

    /// <summary>
    /// Sets the placeholder text for the field.
    /// </summary>
    /// <param name="placeholder">The placeholder text</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> Placeholder(string placeholder)
    {
        var to = EnsureTemplateOptions();
        to = to with { Placeholder = placeholder };
        _c = _c with { Props = to };
        return this;
    }

    /// <summary>
    /// Sets the description text for the field.
    /// </summary>
    /// <param name="description">The description text</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> Description(string description)
    {
        var to = EnsureTemplateOptions();
        to = to with { Description = description };
        _c = _c with { Props = to };
        return this;
    }

    /// <summary>
    /// Marks the field as required or optional.
    /// </summary>
    /// <param name="required">Whether the field is required</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> Required(bool required = true)
    {
        var to = EnsureTemplateOptions();
        to = to with { Required = required };
        _c = _c with { Props = to };
        return this;
    }

    /// <summary>
    /// Disables or enables the field.
    /// </summary>
    /// <param name="disabled">Whether the field is disabled</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> Disabled(bool disabled = true)
    {
        var to = EnsureTemplateOptions();
        to = to with { Disabled = disabled };
        _c = _c with { Props = to };
        return this;
    }

    /// <summary>
    /// Controls whether the field is hidden.
    /// </summary>
    /// <param name="hide">Whether to hide the field</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> Hide(bool hide = true)
    {
        _c = _c with { Hide = hide };
        return this;
    }

    /// <summary>
    /// Sets an expression to dynamically control field visibility.
    /// </summary>
    /// <param name="expression">A string or function expression</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> HideExpression(object expression)
    {
        _c = _c with { HideExpression = expression };
        return this;
    }

    /// <summary>
    /// Sets wrappers for the field.
    /// </summary>
    /// <param name="wrappers">The wrapper names</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> Wrappers(params string[] wrappers)
    {
        _c = _c with { Wrappers = wrappers };
        return this;
    }

    /// <summary>
    /// Ensures that ExpressionProperties dictionary exists, creating it if needed.
    /// </summary>
    /// <returns>The ExpressionProperties dictionary</returns>
    private Dictionary<string, object> EnsureExpressionProperties()
    {
        var ep = _c.ExpressionProperties ?? new Dictionary<string, object>();
        _c = _c with { ExpressionProperties = ep };
        return ep;
    }
    
    /// <summary>
    /// Adds a dynamic property expression to the field.
    /// </summary>
    /// <param name="key">The property path</param>
    /// <param name="expression">A string or function expression</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> ExpressionProperty(string key, object expression)
    {
        var ep = EnsureExpressionProperties();
        ep[key] = expression;
        return this;
    }

    /// <summary>
    /// Ensures that ModelOptions exist, creating them if needed.
    /// </summary>
    /// <returns>The ModelOptions</returns>
    private FormlyModelOptions EnsureModelOptions()
    {
        var mo = _c.ModelOptions ?? new FormlyModelOptions();
        _c = _c with { ModelOptions = mo };
        return mo;
    }

    /// <summary>
    /// Sets the debounce time for updating the model.
    /// </summary>
    /// <param name="ms">Time in milliseconds</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> Debounce(int ms)
    {
        var mo = EnsureModelOptions();
        mo = mo with { Debounce = ms };
        _c = _c with { ModelOptions = mo };
        return this;
    }

    /// <summary>
    /// Controls when the field updates the model.
    /// </summary>
    /// <param name="event_">The update event (e.g., "blur", "change")</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> UpdateOn(string event_)
    {
        var mo = EnsureModelOptions();
        mo = mo with { UpdateOn = event_ };
        _c = _c with { ModelOptions = mo };
        return this;
    }

    /// <summary>
    /// Sets a translation key for the field label. The key will be resolved on the client side.
    /// </summary>
    /// <param name="key">The translation key</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> LabelKey(string key)
    {
        var to = EnsureTemplateOptions();
        var i18n = EnsureI18nOptions(to);
        i18n = i18n with { LabelKey = key };
        to = to with { I18n = i18n };
        _c = _c with { Props = to };
        return this;
    }

    /// <summary>
    /// Sets a translation key for the field placeholder. The key will be resolved on the client side.
    /// </summary>
    /// <param name="key">The translation key</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> PlaceholderKey(string key)
    {
        var to = EnsureTemplateOptions();
        var i18n = EnsureI18nOptions(to);
        i18n = i18n with { PlaceholderKey = key };
        to = to with { I18n = i18n };
        _c = _c with { Props = to };
        return this;
    }

    /// <summary>
    /// Sets a translation key for the field description. The key will be resolved on the client side.
    /// </summary>
    /// <param name="key">The translation key</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> DescriptionKey(string key)
    {
        var to = EnsureTemplateOptions();
        var i18n = EnsureI18nOptions(to);
        i18n = i18n with { DescriptionKey = key };
        to = to with { I18n = i18n };
        _c = _c with { Props = to };
        return this;
    }

    /// <summary>
    /// Sets the locale for this specific field. Can be used for field-specific locale settings.
    /// </summary>
    /// <param name="locale">The locale code (e.g., "en-US", "fr-FR")</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> Locale(string locale)
    {
        var to = EnsureTemplateOptions();
        var i18n = EnsureI18nOptions(to);
        i18n = i18n with { Locale = locale };
        to = to with { I18n = i18n };
        _c = _c with { Props = to };
        return this;
    }

    /// <summary>
    /// Sets a translation key for validation messages by validator name.
    /// </summary>
    /// <param name="validator">The validator name</param>
    /// <param name="translationKey">The translation key</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> ValidationMessageKey(string validator, string translationKey)
    {
        var to = EnsureTemplateOptions();
        var i18n = EnsureI18nOptions(to);
        var messages = i18n.ValidationMessages ?? new Dictionary<string, string>();
        messages[validator] = translationKey;
        i18n = i18n with { ValidationMessages = messages };
        to = to with { I18n = i18n };
        _c = _c with { Props = to };
        return this;
    }

    /// <summary>
    /// Sets a custom translation key to the field.
    /// </summary>
    /// <param name="key">The translation property name</param>
    /// <param name="translationKey">The translation key</param>
    /// <returns>This builder instance for method chaining</returns>
    public FieldBuilder<TModel, TProp> TranslationKey(string key, string translationKey)
    {
        var to = EnsureTemplateOptions();
        var i18n = EnsureI18nOptions(to);
        i18n.AdditionalTranslations[key] = translationKey;
        to = to with { I18n = i18n };
        _c = _c with { Props = to };
        return this;
    }
    
    /// <summary>
    /// Ensures that I18n options exist, creating them if needed.
    /// </summary>
    /// <param name="templateOptions">The template options object</param>
    /// <returns>The I18n options</returns>
    private FormlyI18nOptions EnsureI18nOptions(FormlyFieldProps templateOptions)
    {
        var i18n = templateOptions.I18n ?? new FormlyI18nOptions();
        return i18n;
    }

    /// <summary>
    /// Builds and returns the final FormlyFieldConfig.
    /// </summary>
    /// <returns>The configured FormlyFieldConfig</returns>
    public FormlyFieldConfig Build() => _c;
}