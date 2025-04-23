# FormlySharp

A .NET library for building [Angular Formly](https://formly.dev/) configurations with a strongly-typed C# API.

## Overview

FormlySharp is a C# library that enables strongly-typed form configuration for Angular Formly. It provides a fluent API to define form fields, field groups, and arrays in a type-safe manner, producing JSON configurations compatible with Angular Formly.

## Installation

```bash
dotnet add package FormlySharp
```

## Core Components

### FormlyBuilder

The main entry point for creating form configurations.

```csharp
// Create a form builder for type T
var builder = FormlyBuilder.For<YourModel>();
```

#### Methods:

- **`For<T>()`**: Static method that creates a new builder for type T.
- **`Field<TProp>(Expression<Func<T, TProp>> expr, Action<FieldBuilder<T, TProp>> configure)`**: Defines a primitive or simple field.
- **`FieldGroup<TNested>(Expression<Func<T, TNested>> expr, Action<FormlyBuilder<TNested>> configure)`**: Defines nested fields for a complex object property.
- **`FieldArray<TNested>(Expression<Func<T, TNested[]>> expr, Action<FormlyBuilder<TNested>> configure)`**: Defines nested fields for an array of objects.
- **`Build()`**: Returns the array of FormlyFieldConfig objects.
- **`BuildJson(JsonSerializerOptions? opts = null)`**: Serializes the form configuration to JSON.

### FieldBuilder

Used to configure individual form fields with a fluent API.

#### Core Field Properties:

- **`Id(string id)`**: Sets the field's ID.
- **`Name(string name)`**: Sets the field's name.
- **`ClassName(string css)`**: Sets CSS class names for the field.
- **`FieldGroupClassName(string css)`**: Sets CSS class names for field groups.
- **`Type(string type)`**: Sets the field type (e.g., "input", "select", etc.).
- **`DefaultValue(object? val)`**: Sets the default value for the field.
- **`Template(string tpl)`**: Sets a custom template.

#### Template Options:

- **`Label(string label)`**: Sets the field label.
- **`Placeholder(string placeholder)`**: Sets the field placeholder text.
- **`Description(string description)`**: Sets the field description.
- **`Required(bool required = true)`**: Marks the field as required.
- **`Disabled(bool disabled = true)`**: Disables the field.

#### Validation:

- **`Validator(string name, object validator)`**: Adds a validator.
- **`AsyncValidator(string name, object validator)`**: Adds an asynchronous validator.
- **`ValidationMessage(string key, string message)`**: Adds a validation message.
- **`ShowValidation(bool show = true)`**: Controls validation visibility.

#### Visibility:

- **`Hide(bool hide = true)`**: Controls field visibility.
- **`HideExpression(object expression)`**: Sets an expression to dynamically control visibility.
- **`Wrappers(params string[] wrappers)`**: Sets field wrappers.

#### Reactivity:

- **`ExpressionProperty(string key, object expression)`**: Adds dynamic property expressions.
- **`Debounce(int ms)`**: Sets input debounce time in milliseconds.
- **`UpdateOn(string event_)`**: Controls when the field updates (e.g., "blur", "change").

#### Custom Properties:

- **`Prop(string key, object value)`**: Sets custom properties.

### FormlyFieldConfig

Represents the core configuration for a formly field, mirroring Angular Formly's FormlyFieldConfig.

Main properties include:
- **`Key`**: The field key (usually the property name).
- **`Type`**: The field type.
- **`TemplateOptions`**: Configuration for the field's appearance.
- **`Validation`**: Validation settings.
- **`FieldGroup`**: Nested fields for complex objects.
- **`FieldArray`**: Configuration for array fields.

## Usage Examples

### Basic Field

```csharp
var config = FormlyBuilder
    .For<User>()
    .Field(u => u.Name, f => f
        .Type("input")
        .Label("Full Name")
        .Required()
        .Placeholder("Enter your full name"))
    .BuildJson();
```

### Field with Validation

```csharp
var config = FormlyBuilder
    .For<User>()
    .Field(u => u.Email, f => f
        .Type("input")
        .Label("Email Address")
        .Required()
        .Validator("email", true)
        .ValidationMessage("email", "Must be a valid email")
        .Placeholder("Enter your email"))
    .BuildJson();
```

### Nested Object

```csharp
var config = FormlyBuilder
    .For<User>()
    .FieldGroup(u => u.Address, address => address
        .Field(a => a.Street, f => f
            .Type("input")
            .Label("Street"))
        .Field(a => a.City, f => f
            .Type("input")
            .Label("City")))
    .BuildJson();
```

### Array of Objects

```csharp
var config = FormlyBuilder
    .For<User>()
    .FieldArray(u => u.PhoneNumbers, phone => phone
        .Field(p => p.Number, f => f
            .Type("input")
            .Label("Phone Number"))
        .Field(p => p.Type, f => f
            .Type("select")
            .Label("Type")))
    .BuildJson();
```

## License

MIT