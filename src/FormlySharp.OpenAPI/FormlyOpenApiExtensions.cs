// using Microsoft.OpenApi.Models;
// using Microsoft.OpenApi.Any;
// using System.Text.Json;

// namespace FormlySharp.OpenAPI;

// /// <summary>
// /// Extension methods to facilitate working with OpenAPI schemas and Formly configurations.
// /// </summary>
// public static class FormlyOpenApiExtensions
// {
//     /// <summary>
//     /// Converts a FormlyFieldConfig to an OpenAPI schema, mapping compatible properties directly
//     /// and putting Formly-specific properties in the x-formly extension.
//     /// </summary>
//     /// <param name="config">The Formly field configuration to convert</param>
//     /// <returns>An OpenAPI schema representing the Formly field</returns>
//     private static OpenApiSchema ConvertFormlyConfigToOpenApiSchema(FormlyFieldConfig config)
//     {
//         var schema = new OpenApiSchema();
//         var formlyExtension = new OpenApiObject();
//         bool hasFormlySpecificProperties = false;

//         // Handle key as property name - this will be handled by the parent schema
//         // when adding this schema to its properties

//         // Map standard properties that have direct OpenAPI equivalents
//         if (config.DefaultValue != null)
//             schema.Default = ConvertValueToOpenApiAny(config.DefaultValue);
            
//         // Map type if provided
//         if (config.Type != null)
//         {
//             // Map Formly types to OpenAPI types
//             switch (config.Type.ToLowerInvariant())
//             {
//                 case "input":
//                 case "textarea":
//                     schema.Type = "string";
//                     break;
//                 case "number":
//                     schema.Type = "number";
//                     break;
//                 case "integer":
//                     schema.Type = "integer";
//                     break;
//                 case "boolean":
//                 case "checkbox":
//                     schema.Type = "boolean";
//                     break;
//                 case "select":
//                 case "radio":
//                 case "multicheckbox":
//                     // These could be strings or arrays depending on context
//                     // Default to string, but might need adjustment based on options
//                     schema.Type = "string";
//                     break;
//                 case "array":
//                     schema.Type = "array";
//                     if (config.FieldArray != null)
//                     {
//                         schema.Items = ConvertFormlyConfigToOpenApiSchema(config.FieldArray);
//                     }
//                     break;
//                 case "object":
//                 case "fieldgroup":
//                     schema.Type = "object";
//                     break;
//                 default:
//                     // For custom types, store in extension
//                     formlyExtension.Add("type", new OpenApiString(config.Type));
//                     hasFormlySpecificProperties = true;
//                     schema.Type = "object"; // Default to object for unknown types
//                     break;
//             }
//         }
        
//         // Handle props that map to standard OpenAPI properties
//         if (config.Props != null)
//         {
//             // Description, title (label), etc.
//             if (config.Props.Description != null)
//                 schema.Description = config.Props.Description;
                
//             if (config.Props.Label != null)
//                 schema.Title = config.Props.Label;
                
//             // Handle validation props
//             if (config.Props.Required.HasValue && config.Props.Required.Value)
//             {
//                 // In a parent schema, this property would be added to the required array
//                 // We'll handle this special case in WithFormlyConfig
//             }
            
//             if (config.Props.ReadOnly.HasValue)
//                 schema.ReadOnly = config.Props.ReadOnly.Value;
                
//             // Min/Max for numbers
//             if (schema.Type == "number" || schema.Type == "integer")
//             {
//                 if (config.Props.Min != null)
//                 {
//                     if (config.Props.Min is int minInt)
//                         schema.Minimum = minInt;
//                     else if (config.Props.Min is double minDouble)
//                         schema.Minimum = minDouble;
//                 }
                
//                 if (config.Props.Max != null)
//                 {
//                     if (config.Props.Max is int maxInt)
//                         schema.Maximum = maxInt;
//                     else if (config.Props.Max is double maxDouble)
//                         schema.Maximum = maxDouble;
//                 }
                
//                 if (config.Props.Step != null)
//                 {
//                     // No direct OpenAPI equivalent for step, add to extension
//                     var propsExt = formlyExtension.ContainsKey("props") 
//                         ? (OpenApiObject)formlyExtension["props"] 
//                         : new OpenApiObject();
                        
//                     propsExt.Add("step", ConvertValueToOpenApiAny(config.Props.Step));
                    
//                     if (!formlyExtension.ContainsKey("props"))
//                         formlyExtension.Add("props", propsExt);
                        
//                     hasFormlySpecificProperties = true;
//                 }
//             }
            
//             // MinLength/MaxLength for strings
//             if (schema.Type == "string")
//             {
//                 if (config.Props.MinLength.HasValue)
//                     schema.MinLength = config.Props.MinLength.Value;
                    
//                 if (config.Props.MaxLength.HasValue)
//                     schema.MaxLength = config.Props.MaxLength.Value;
                    
//                 if (config.Props.Pattern != null)
//                     schema.Pattern = config.Props.Pattern;
//             }
            
//             // Options for select/radio/checkbox as enum
//             if (config.Props.Options != null && (schema.Type == "string" || schema.Type == "number" || schema.Type == "integer"))
//             {
//                 var enumValues = new List<IOpenApiAny>();
                
//                 foreach (var option in config.Props.Options)
//                 {
//                     if (option is IDictionary<string, object> optionDict)
//                     {
//                         // Standard format is { value: any, label: string }
//                         if (optionDict.TryGetValue("value", out var value))
//                         {
//                             enumValues.Add(ConvertValueToOpenApiAny(value));
//                         }
//                     }
//                     else
//                     {
//                         enumValues.Add(ConvertValueToOpenApiAny(option));
//                     }
//                 }
                
//                 if (enumValues.Count > 0)
//                     schema.Enum = enumValues;
                    
//                 // Store full options in extension as they contain additional info like labels
//                 var propsExt = formlyExtension.ContainsKey("props") 
//                     ? (OpenApiObject)formlyExtension["props"] 
//                     : new OpenApiObject();
                    
//                 propsExt.Add("options", ConvertArrayToOpenApiArray(config.Props.Options));
                
//                 if (!formlyExtension.ContainsKey("props"))
//                     formlyExtension.Add("props", propsExt);
                    
//                 hasFormlySpecificProperties = true;
//             }
            
//             // Add all other props to extension
//             var allPropsToExtension = new Dictionary<string, bool>
//             {
//                 { "label", false },            // Already handled via schema.Title
//                 { "description", false },      // Already handled via schema.Description
//                 { "required", false },         // Handled at parent schema level
//                 { "min", false },              // Already handled via schema.Minimum
//                 { "max", false },              // Already handled via schema.Maximum
//                 { "minLength", false },        // Already handled via schema.MinLength
//                 { "maxLength", false },        // Already handled via schema.MaxLength
//                 { "pattern", false },          // Already handled via schema.Pattern
//                 { "readonly", false }          // Already handled via schema.ReadOnly
//             };
            
//             // Add remaining props to extension
//             var remainingProps = new OpenApiObject();
//             bool hasRemainingProps = false;
            
//             if (config.Props.Placeholder != null)
//             {
//                 remainingProps.Add("placeholder", new OpenApiString(config.Props.Placeholder));
//                 hasRemainingProps = true;
//             }
            
//             if (config.Props.Disabled.HasValue)
//             {
//                 remainingProps.Add("disabled", new OpenApiBoolean(config.Props.Disabled.Value));
//                 hasRemainingProps = true;
//             }
            
//             if (config.Props.Rows.HasValue)
//             {
//                 remainingProps.Add("rows", new OpenApiInteger(config.Props.Rows.Value));
//                 hasRemainingProps = true;
//             }
            
//             if (config.Props.Cols.HasValue)
//             {
//                 remainingProps.Add("cols", new OpenApiInteger(config.Props.Cols.Value));
//                 hasRemainingProps = true;
//             }
            
//             if (config.Props.TabIndex.HasValue)
//             {
//                 remainingProps.Add("tabindex", new OpenApiInteger(config.Props.TabIndex.Value));
//                 hasRemainingProps = true;
//             }
            
//             // Add event handlers
//             if (config.Props.OnFocus != null || 
//                 config.Props.OnBlur != null || 
//                 config.Props.OnChange != null ||
//                 config.Props.OnKeyUp != null ||
//                 config.Props.OnKeyDown != null ||
//                 config.Props.OnKeyPress != null ||
//                 config.Props.OnClick != null)
//             {
//                 var events = new OpenApiObject();
                
//                 if (config.Props.OnFocus != null)
//                     events.Add("focus", new OpenApiString(config.Props.OnFocus));
                    
//                 if (config.Props.OnBlur != null)
//                     events.Add("blur", new OpenApiString(config.Props.OnBlur));
                    
//                 if (config.Props.OnChange != null)
//                     events.Add("change", new OpenApiString(config.Props.OnChange));
                    
//                 if (config.Props.OnKeyUp != null)
//                     events.Add("keyup", new OpenApiString(config.Props.OnKeyUp));
                    
//                 if (config.Props.OnKeyDown != null)
//                     events.Add("keydown", new OpenApiString(config.Props.OnKeyDown));
                    
//                 if (config.Props.OnKeyPress != null)
//                     events.Add("keypress", new OpenApiString(config.Props.OnKeyPress));
                    
//                 if (config.Props.OnClick != null)
//                     events.Add("click", new OpenApiString(config.Props.OnClick));
                
//                 remainingProps.Add("events", events);
//                 hasRemainingProps = true;
//             }
            
//             // Add i18n options
//             if (config.Props.I18n != null)
//             {
//                 remainingProps.Add("i18n", ConvertI18nOptionsToOpenApiObject(config.Props.I18n));
//                 hasRemainingProps = true;
//             }
            
//             // Add any additional props
//             foreach (var prop in config.Props.AdditionalProperties)
//             {
//                 if (!allPropsToExtension.ContainsKey(prop.Key) || allPropsToExtension[prop.Key])
//                 {
//                     remainingProps.Add(prop.Key, ConvertValueToOpenApiAny(prop.Value));
//                     hasRemainingProps = true;
//                 }
//             }
            
//             if (hasRemainingProps)
//             {
//                 if (!formlyExtension.ContainsKey("props"))
//                     formlyExtension.Add("props", remainingProps);
//                 else
//                 {
//                     var existingProps = (OpenApiObject)formlyExtension["props"];
//                     foreach (var prop in remainingProps)
//                     {
//                         existingProps.Add(prop.Key, prop.Value);
//                     }
//                 }
                
//                 hasFormlySpecificProperties = true;
//             }
//         }
        
//         // Handle field groups for object types
//         if (config.FieldGroup != null && config.FieldGroup.Length > 0)
//         {
//             schema.Type = "object";
//             schema.Properties = new Dictionary<string, OpenApiSchema>();
//             var requiredProperties = new List<string>();
            
//             foreach (var field in config.FieldGroup)
//             {
//                 if (field.Key != null)
//                 {
//                     var propertyKey = field.Key.ToString();
//                     var propertySchema = ConvertFormlyConfigToOpenApiSchema(field);
//                     schema.Properties.Add(propertyKey, propertySchema);
                    
//                     // Check if this field is required
//                     if (field.Props?.Required == true)
//                     {
//                         requiredProperties.Add(propertyKey);
//                     }
//                 }
//             }
            
//             if (requiredProperties.Count > 0)
//             {
//                 schema.Required = requiredProperties;
//             }
//         }
        
//         // Add Formly-specific properties to extension
//         // ID
//         if (config.Id != null)
//         {
//             formlyExtension.Add("id", new OpenApiString(config.Id));
//             hasFormlySpecificProperties = true;
//         }
        
//         // Name
//         if (config.Name != null)
//         {
//             formlyExtension.Add("name", new OpenApiString(config.Name));
//             hasFormlySpecificProperties = true;
//         }
        
//         // Class name
//         if (config.ClassName != null)
//         {
//             formlyExtension.Add("className", new OpenApiString(config.ClassName));
//             hasFormlySpecificProperties = true;
//         }
        
//         // Field group class name
//         if (config.FieldGroupClassName != null)
//         {
//             formlyExtension.Add("fieldGroupClassName", new OpenApiString(config.FieldGroupClassName));
//             hasFormlySpecificProperties = true;
//         }
        
//         // Template
//         if (config.Template != null)
//         {
//             formlyExtension.Add("template", new OpenApiString(config.Template));
//             hasFormlySpecificProperties = true;
//         }
        
//         // Validators and AsyncValidators
//         if (config.Validators != null)
//         {
//             formlyExtension.Add("validators", ConvertIDictionaryToOpenApiObject(config.Validators));
//             hasFormlySpecificProperties = true;
//         }
        
//         if (config.AsyncValidators != null)
//         {
//             formlyExtension.Add("asyncValidators", ConvertIDictionaryToOpenApiObject(config.AsyncValidators));
//             hasFormlySpecificProperties = true;
//         }
        
//         // Validation
//         if (config.Validation != null)
//         {
//             formlyExtension.Add("validation", ConvertValidationToOpenApiObject(config.Validation));
//             hasFormlySpecificProperties = true;
//         }
        
//         // Expression Properties
//         if (config.ExpressionProperties != null)
//         {
//             formlyExtension.Add("expressionProperties", ConvertIDictionaryToOpenApiObject(config.ExpressionProperties));
//             hasFormlySpecificProperties = true;
//         }
        
//         // Hide and HideExpression
//         if (config.Hide.HasValue)
//         {
//             formlyExtension.Add("hide", new OpenApiBoolean(config.Hide.Value));
//             hasFormlySpecificProperties = true;
//         }
        
//         if (config.HideExpression != null)
//         {
//             formlyExtension.Add("hideExpression", ConvertValueToOpenApiAny(config.HideExpression));
//             hasFormlySpecificProperties = true;
//         }
        
//         // Wrappers
//         if (config.Wrappers != null)
//         {
//             formlyExtension.Add("wrappers", ConvertArrayToOpenApiArray(config.Wrappers));
//             hasFormlySpecificProperties = true;
//         }
        
//         // Focus
//         if (config.Focus.HasValue)
//         {
//             formlyExtension.Add("focus", new OpenApiBoolean(config.Focus.Value));
//             hasFormlySpecificProperties = true;
//         }
        
//         // ModelOptions
//         if (config.ModelOptions != null)
//         {
//             formlyExtension.Add("modelOptions", ConvertModelOptionsToOpenApiObject(config.ModelOptions));
//             hasFormlySpecificProperties = true;
//         }
        
//         // Lifecycle and Hooks
//         if (config.Hooks != null)
//         {
//             formlyExtension.Add("hooks", ConvertLifecycleOptionsToOpenApiObject(config.Hooks));
//             hasFormlySpecificProperties = true;
//         }
        
//         if (config.Lifecycle != null)
//         {
//             formlyExtension.Add("lifecycle", ConvertLifecycleOptionsToOpenApiObject(config.Lifecycle));
//             hasFormlySpecificProperties = true;
//         }
        
//         // Handle field array for array types
//         if (config.FieldArray != null && schema.Type != "array")
//         {
//             // If we didn't already set this as an array type,
//             // we need to include the fieldArray in the extension
//             formlyExtension.Add("fieldArray", ConvertFormlyConfigToOpenApiObject(config.FieldArray));
//             hasFormlySpecificProperties = true;
//         }
        
//         // Parsers
//         if (config.Parsers != null)
//         {
//             formlyExtension.Add("parsers", ConvertArrayToOpenApiArray(config.Parsers));
//             hasFormlySpecificProperties = true;
//         }
        
//         // Only add extension if we have Formly-specific properties
//         if (hasFormlySpecificProperties)
//         {
//             schema.Extensions[FormlyOpenApiParser.FormlyExtensionName] = formlyExtension;
//         }
        
//         return schema;
//     }

//     /// <summary>
//     /// Adds Formly configuration to an OpenAPI Schema as an extension property.
//     /// </summary>
//     /// <param name="schema">The OpenAPI schema to extend</param>
//     /// <param name="formlyConfig">The Formly field configuration to add</param>
//     /// <returns>The modified schema for method chaining</returns>
//     public static OpenApiSchema WithFormlyConfig(this OpenApiSchema schema, FormlyFieldConfig[] formlyConfig)
//     {
//         // Initialize properties collection if it doesn't exist
//         schema.Properties ??= new Dictionary<string, OpenApiSchema>();
        
//         // Store any Formly-specific configuration that doesn't map directly to OpenAPI
//         var formlyExtension = new OpenApiObject();
//         bool hasFormlySpecificProperties = false;
        
//         foreach (var config in formlyConfig)
//         {
//             if (config.Key != null)
//             {
//                 // Convert the Formly config to an OpenAPI schema
//                 var propertySchema = ConvertFormlyConfigToOpenApiSchema(config);
//                 string key = config.Key.ToString();
                
//                 // Add as a property in the schema
//                 schema.Properties[key] = propertySchema;
                
//                 // Mark as required if specified
//                 if (config.Props?.Required == true)
//                 {
//                     schema.Required ??= new List<string>();
//                     if (!schema.Required.Contains(key))
//                     {
//                         schema.Required.Add(key);
//                     }
//                 }
//             }
//             else
//             {
//                 // For items without a key, we'll need to store them in the extension
//                 var configObject = ConvertFormlyConfigToOpenApiObject(config);
                
//                 // Add to a formly extension array
//                 if (!formlyExtension.ContainsKey("additionalFields"))
//                 {
//                     formlyExtension.Add("additionalFields", new OpenApiArray());
//                 }
                
//                 ((OpenApiArray)formlyExtension["additionalFields"]).Add(configObject);
//                 hasFormlySpecificProperties = true;
//             }
//         }
        
//         // Only add the extension if we have Formly-specific properties 
//         // that couldn't be encoded in standard OpenAPI
//         if (hasFormlySpecificProperties)
//         {
//             schema.Extensions[FormlyOpenApiParser.FormlyExtensionName] = formlyExtension;
//         }
        
//         return schema;
//     }

//     /// <summary>
//     /// Converts a FormlyFieldConfig object to an OpenApiObject
//     /// </summary>
//     private static OpenApiObject ConvertFormlyConfigToOpenApiObject(FormlyFieldConfig config)
//     {
//         var openApiObject = new OpenApiObject();

//         // Add properties if they're not null
//         if (config.Key != null)
//             openApiObject.Add("key", ConvertValueToOpenApiAny(config.Key));
            
//         if (config.Id != null)
//             openApiObject.Add("id", new OpenApiString(config.Id));
            
//         if (config.Name != null)
//             openApiObject.Add("name", new OpenApiString(config.Name));
            
//         if (config.ClassName != null)
//             openApiObject.Add("className", new OpenApiString(config.ClassName));
            
//         if (config.FieldGroupClassName != null)
//             openApiObject.Add("fieldGroupClassName", new OpenApiString(config.FieldGroupClassName));
            
//         if (config.Type != null)
//             openApiObject.Add("type", new OpenApiString(config.Type));
            
//         if (config.DefaultValue != null)
//             openApiObject.Add("defaultValue", ConvertValueToOpenApiAny(config.DefaultValue));
            
//         if (config.Template != null)
//             openApiObject.Add("template", new OpenApiString(config.Template));
            
//         if (config.Props != null)
//             openApiObject.Add("props", ConvertFormlyPropsToOpenApiObject(config.Props));
            
//         if (config.Validators != null)
//             openApiObject.Add("validators", ConvertIDictionaryToOpenApiObject(config.Validators));
            
//         if (config.AsyncValidators != null)
//             openApiObject.Add("asyncValidators", ConvertIDictionaryToOpenApiObject(config.AsyncValidators));
            
//         if (config.Validation != null)
//             openApiObject.Add("validation", ConvertValidationToOpenApiObject(config.Validation));
            
//         if (config.ExpressionProperties != null)
//             openApiObject.Add("expressionProperties", ConvertIDictionaryToOpenApiObject(config.ExpressionProperties));
            
//         if (config.Hide.HasValue)
//             openApiObject.Add("hide", new OpenApiBoolean(config.Hide.Value));
            
//         if (config.HideExpression != null)
//             openApiObject.Add("hideExpression", ConvertValueToOpenApiAny(config.HideExpression));
            
//         if (config.Wrappers != null)
//             openApiObject.Add("wrappers", ConvertArrayToOpenApiArray(config.Wrappers));
            
//         if (config.Focus.HasValue)
//             openApiObject.Add("focus", new OpenApiBoolean(config.Focus.Value));
            
//         if (config.ModelOptions != null)
//             openApiObject.Add("modelOptions", ConvertModelOptionsToOpenApiObject(config.ModelOptions));
            
//         if (config.Hooks != null)
//             openApiObject.Add("hooks", ConvertLifecycleOptionsToOpenApiObject(config.Hooks));
            
//         if (config.Lifecycle != null)
//             openApiObject.Add("lifecycle", ConvertLifecycleOptionsToOpenApiObject(config.Lifecycle));
            
//         if (config.FieldGroup != null)
//         {
//             var fieldGroupArray = new OpenApiArray();
//             foreach (var field in config.FieldGroup)
//             {
//                 fieldGroupArray.Add(ConvertFormlyConfigToOpenApiObject(field));
//             }
//             openApiObject.Add("fieldGroup", fieldGroupArray);
//         }
            
//         if (config.FieldArray != null)
//             openApiObject.Add("fieldArray", ConvertFormlyConfigToOpenApiObject(config.FieldArray));
            
//         if (config.Parsers != null)
//             openApiObject.Add("parsers", ConvertArrayToOpenApiArray(config.Parsers));

//         return openApiObject;
//     }

//     private static OpenApiObject ConvertFormlyPropsToOpenApiObject(FormlyFieldProps props)
//     {
//         var openApiObject = new OpenApiObject();
        
//         // Add FormlyFieldProps properties
//         if (props.Label != null)
//             openApiObject.Add("label", new OpenApiString(props.Label));
            
//         if (props.Placeholder != null)
//             openApiObject.Add("placeholder", new OpenApiString(props.Placeholder));
            
//         if (props.Description != null)
//             openApiObject.Add("description", new OpenApiString(props.Description));
            
//         if (props.Required.HasValue)
//             openApiObject.Add("required", new OpenApiBoolean(props.Required.Value));
            
//         if (props.Disabled.HasValue)
//             openApiObject.Add("disabled", new OpenApiBoolean(props.Disabled.Value));
            
//         if (props.Min != null)
//             openApiObject.Add("min", ConvertValueToOpenApiAny(props.Min));
            
//         if (props.Max != null)
//             openApiObject.Add("max", ConvertValueToOpenApiAny(props.Max));
            
//         if (props.MinLength.HasValue)
//             openApiObject.Add("minLength", new OpenApiInteger(props.MinLength.Value));
            
//         if (props.MaxLength.HasValue)
//             openApiObject.Add("maxLength", new OpenApiInteger(props.MaxLength.Value));
            
//         if (props.Pattern != null)
//             openApiObject.Add("pattern", new OpenApiString(props.Pattern));
            
//         if (props.Options != null)
//             openApiObject.Add("options", ConvertArrayToOpenApiArray(props.Options));
            
//         if (props.Rows.HasValue)
//             openApiObject.Add("rows", new OpenApiInteger(props.Rows.Value));
            
//         if (props.Cols.HasValue)
//             openApiObject.Add("cols", new OpenApiInteger(props.Cols.Value));
            
//         if (props.TabIndex.HasValue)
//             openApiObject.Add("tabindex", new OpenApiInteger(props.TabIndex.Value));
            
//         if (props.ReadOnly.HasValue)
//             openApiObject.Add("readonly", new OpenApiBoolean(props.ReadOnly.Value));
            
//         if (props.Step != null)
//             openApiObject.Add("step", ConvertValueToOpenApiAny(props.Step));
            
//         if (props.OnFocus != null)
//             openApiObject.Add("focus", new OpenApiString(props.OnFocus));
            
//         if (props.OnBlur != null)
//             openApiObject.Add("blur", new OpenApiString(props.OnBlur));
            
//         if (props.OnChange != null)
//             openApiObject.Add("change", new OpenApiString(props.OnChange));
            
//         if (props.OnKeyUp != null)
//             openApiObject.Add("keyup", new OpenApiString(props.OnKeyUp));
            
//         if (props.OnKeyDown != null)
//             openApiObject.Add("keydown", new OpenApiString(props.OnKeyDown));
            
//         if (props.OnKeyPress != null)
//             openApiObject.Add("keypress", new OpenApiString(props.OnKeyPress));
            
//         if (props.OnClick != null)
//             openApiObject.Add("click", new OpenApiString(props.OnClick));
            
//         if (props.I18n != null)
//             openApiObject.Add("i18n", ConvertI18nOptionsToOpenApiObject(props.I18n));
        
//         // Add any additional properties from the dictionary
//         foreach (var kvp in props.AdditionalProperties)
//         {
//             openApiObject.Add(kvp.Key, ConvertValueToOpenApiAny(kvp.Value));
//         }
        
//         return openApiObject;
//     }

//     private static OpenApiObject ConvertI18nOptionsToOpenApiObject(FormlyI18nOptions i18n)
//     {
//         var openApiObject = new OpenApiObject();
        
//         if (i18n.LabelKey != null)
//             openApiObject.Add("labelKey", new OpenApiString(i18n.LabelKey));
            
//         if (i18n.PlaceholderKey != null)
//             openApiObject.Add("placeholderKey", new OpenApiString(i18n.PlaceholderKey));
            
//         if (i18n.DescriptionKey != null)
//             openApiObject.Add("descriptionKey", new OpenApiString(i18n.DescriptionKey));
            
//         if (i18n.ValidationMessages != null)
//         {
//             var messagesObject = new OpenApiObject();
//             foreach (var kvp in i18n.ValidationMessages)
//             {
//                 messagesObject.Add(kvp.Key, new OpenApiString(kvp.Value));
//             }
//             openApiObject.Add("validationMessages", messagesObject);
//         }
        
//         if (i18n.Locale != null)
//             openApiObject.Add("locale", new OpenApiString(i18n.Locale));
        
//         // Add any additional translations from the dictionary
//         foreach (var kvp in i18n.AdditionalTranslations)
//         {
//             openApiObject.Add(kvp.Key, ConvertValueToOpenApiAny(kvp.Value));
//         }
        
//         return openApiObject;
//     }

//     private static OpenApiObject ConvertValidationToOpenApiObject(FormlyValidationOptions validation)
//     {
//         var openApiObject = new OpenApiObject();
        
//         if (validation.Messages != null)
//         {
//             var messagesObject = new OpenApiObject();
//             foreach (var kvp in validation.Messages)
//             {
//                 messagesObject.Add(kvp.Key, new OpenApiString(kvp.Value));
//             }
//             openApiObject.Add("messages", messagesObject);
//         }
        
//         if (validation.Show.HasValue)
//             openApiObject.Add("show", new OpenApiBoolean(validation.Show.Value));
        
//         return openApiObject;
//     }

//     private static OpenApiObject ConvertModelOptionsToOpenApiObject(FormlyModelOptions options)
//     {
//         var openApiObject = new OpenApiObject();
        
//         if (options.Debounce.HasValue)
//             openApiObject.Add("debounce", new OpenApiInteger(options.Debounce.Value));
            
//         if (options.UpdateOn != null)
//             openApiObject.Add("updateOn", new OpenApiString(options.UpdateOn));
        
//         return openApiObject;
//     }

//     private static OpenApiObject ConvertLifecycleOptionsToOpenApiObject(FormlyLifecycleOptions options)
//     {
//         var openApiObject = new OpenApiObject();
        
//         if (options.OnInit != null)
//             openApiObject.Add("onInit", new OpenApiString(options.OnInit));
            
//         if (options.OnChanges != null)
//             openApiObject.Add("onChanges", new OpenApiString(options.OnChanges));
            
//         if (options.AfterContentInit != null)
//             openApiObject.Add("afterContentInit", new OpenApiString(options.AfterContentInit));
            
//         if (options.AfterViewInit != null)
//             openApiObject.Add("afterViewInit", new OpenApiString(options.AfterViewInit));
            
//         if (options.OnDestroy != null)
//             openApiObject.Add("onDestroy", new OpenApiString(options.OnDestroy));
        
//         return openApiObject;
//     }

//     private static OpenApiObject ConvertDictionaryToOpenApiObject(Dictionary<string, object> dict)
//     {
//         var openApiObject = new OpenApiObject();
//         foreach (var kvp in dict)
//         {
//             openApiObject.Add(kvp.Key, ConvertValueToOpenApiAny(kvp.Value));
//         }
//         return openApiObject;
//     }

//     private static OpenApiArray ConvertArrayToOpenApiArray<T>(IEnumerable<T> array)
//     {
//         var openApiArray = new OpenApiArray();
//         foreach (var item in array)
//         {
//             openApiArray.Add(ConvertValueToOpenApiAny(item));
//         }
//         return openApiArray;
//     }

//     private static IOpenApiAny ConvertValueToOpenApiAny(object? value)
//     {
//         if (value == null)
//             return new OpenApiNull();
            
//         return value switch
//         {
//             string s => new OpenApiString(s),
//             int i => new OpenApiInteger(i),
//             long l => new OpenApiLong(l),
//             float f => new OpenApiFloat(f),
//             double d => new OpenApiDouble(d),
//             bool b => new OpenApiBoolean(b),
//             DateTime dt => new OpenApiDateTime(dt),
//             IDictionary<string, object> dict => ConvertIDictionaryToOpenApiObject(dict),
//             IEnumerable<object> array => ConvertArrayToOpenApiArray(array),
//             // For other types, you might need more specific handling
//             // or potentially serialize complex objects
//             _ => new OpenApiString(value.ToString() ?? string.Empty)
//         };
//     }

//     private static OpenApiObject ConvertIDictionaryToOpenApiObject(IDictionary<string, object> dict)
//     {
//         var openApiObject = new OpenApiObject();
//         foreach (var kvp in dict)
//         {
//             openApiObject.Add(kvp.Key, ConvertValueToOpenApiAny(kvp.Value));
//         }
//         return openApiObject;
//     }

//     /// <summary>
//     /// Extension method to generate Formly configuration for a model type using both
//     /// the model's properties and any OpenAPI schema information.
//     /// </summary>
//     /// <typeparam name="T">The model type</typeparam>
//     /// <param name="builder">The FormlyBuilder instance</param>
//     /// <param name="parser">The OpenAPI parser</param>
//     /// <param name="schemaReference">Reference to the schema in the OpenAPI spec</param>
//     /// <returns>The FormlyBuilder for method chaining</returns>
//     public static FormlyBuilder<T> ApplyOpenApiSchema<T>(
//         this FormlyBuilder<T> builder,
//         FormlyOpenApiParser parser,
//         string schemaReference)
//     {
//         // This is a placeholder for a more complex implementation
//         // that would merge Formly configurations from both the model properties
//         // and the OpenAPI schema
        
//         // In a full implementation, this would:
//         // 1. Get the OpenAPI schema for the reference
//         // 2. Apply any additional properties, validations, etc. from the schema
//         // 3. Return the enhanced builder
        
//         return builder;
//     }
// }