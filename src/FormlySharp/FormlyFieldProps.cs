namespace FormlySharp;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Configuration options for the appearance and behavior of a form field.
/// This mirrors Angular Formly's template options and is used to customize the field template.
/// </summary>
public record FormlyFieldProps
{
    /// <summary>
    /// The field label text displayed to the user.
    /// </summary>
    [JsonPropertyName("label")]         public string? Label { get; set; }
    
    /// <summary>
    /// Placeholder text displayed when the field is empty.
    /// </summary>
    [JsonPropertyName("placeholder")]   public string? Placeholder { get; set; }
    
    /// <summary>
    /// Description text that provides additional information about the field.
    /// </summary>
    [JsonPropertyName("description")]   public string? Description { get; set; }
    
    /// <summary>
    /// Whether the field is required. Used for validation.
    /// </summary>
    [JsonPropertyName("required")]      public bool? Required { get; set; }
    
    /// <summary>
    /// Whether the field is disabled. Prevents user interaction with the field.
    /// </summary>
    [JsonPropertyName("disabled")]      public bool? Disabled { get; set; }
    
    /// <summary>
    /// Minimum value for numeric fields. Can be a number or Date object.
    /// </summary>
    [JsonPropertyName("min")]           public object? Min { get; set; }
    
    /// <summary>
    /// Maximum value for numeric fields. Can be a number or Date object.
    /// </summary>
    [JsonPropertyName("max")]           public object? Max { get; set; }
    
    /// <summary>
    /// Minimum length for text input.
    /// </summary>
    [JsonPropertyName("minLength")]     public int? MinLength { get; set; }
    
    /// <summary>
    /// Maximum length for text input.
    /// </summary>
    [JsonPropertyName("maxLength")]     public int? MaxLength { get; set; }
    
    /// <summary>
    /// Regular expression pattern for text validation.
    /// </summary>
    [JsonPropertyName("pattern")]       public string? Pattern { get; set; }
    
    /// <summary>
    /// Options for select, radio, or checkbox fields.
    /// </summary>
    [JsonPropertyName("options")]       public IEnumerable<object>? Options { get; set; }
    
    /// <summary>
    /// Number of rows for textarea fields.
    /// </summary>
    [JsonPropertyName("rows")]          public int? Rows { get; set; }
    
    /// <summary>
    /// Number of columns for textarea fields.
    /// </summary>
    [JsonPropertyName("cols")]          public int? Cols { get; set; }
    
    /// <summary>
    /// The tabindex attribute for the field.
    /// </summary>
    [JsonPropertyName("tabindex")]      public int? TabIndex { get; set; }
    
    /// <summary>
    /// Whether the field is read-only.
    /// </summary>
    [JsonPropertyName("readonly")]      public bool? ReadOnly { get; set; }
    
    /// <summary>
    /// Step value for numeric inputs. Controls the granularity when using up/down arrows.
    /// </summary>
    [JsonPropertyName("step")]          public object? Step { get; set; }
    
    /// <summary>
    /// JavaScript function or expression to execute when the field receives focus.
    /// </summary>
    [JsonPropertyName("focus")]         public string? OnFocus { get; set; }
    
    /// <summary>
    /// JavaScript function or expression to execute when the field loses focus.
    /// </summary>
    [JsonPropertyName("blur")]          public string? OnBlur { get; set; }
    
    /// <summary>
    /// JavaScript function or expression to execute when the field value changes.
    /// </summary>
    [JsonPropertyName("change")]        public string? OnChange { get; set; }
    
    /// <summary>
    /// JavaScript function or expression to execute when a key is released in the field.
    /// </summary>
    [JsonPropertyName("keyup")]         public string? OnKeyUp { get; set; }
    
    /// <summary>
    /// JavaScript function or expression to execute when a key is pressed down in the field.
    /// </summary>
    [JsonPropertyName("keydown")]       public string? OnKeyDown { get; set; }
    
    /// <summary>
    /// JavaScript function or expression to execute when a key is pressed in the field.
    /// </summary>
    [JsonPropertyName("keypress")]      public string? OnKeyPress { get; set; }
    
    /// <summary>
    /// JavaScript function or expression to execute when the field is clicked.
    /// </summary>
    [JsonPropertyName("click")]         public string? OnClick { get; set; }

    /// <summary>
    /// Internationalization options for the field. Contains translation keys for field properties.
    /// </summary>
    [JsonPropertyName("i18n")]          public FormlyI18nOptions? I18n { get; set; }

    /// <summary>
    /// Additional properties that can be used to extend the field configuration.
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, object> AdditionalProperties { get; set; } =
        new Dictionary<string, object>(StringComparer.Ordinal);
}