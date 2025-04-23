using Xunit;
using FormlySharp;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FormlySharp.Tests;

public class Address
{
    public string Street { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty; 
}

public class User
{
    public string Username { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }
}

public class Person
{
    public string Name { get; set; } = string.Empty;

    public int Age { get; set; }

    public Address Address { get; set; } = new Address();
    
    public string[] Tags { get; set; } = Array.Empty<string>();
    
    public User[] Friends { get; set; } = Array.Empty<User>();
}

/// <summary>
/// Tests for the basic FormlyBuilder functionality
/// </summary>
public class FormlyBuilderTests
{
    [Fact]
    public void Builder_Creates_Simple_Field_Config()
    {
        // Arrange & Act
        var builder = FormlyBuilder.For<Person>()
            .Field(x => x.Name, f => f.Type("input").Label("Full Name"));
            
        var config = builder.Build();
        
        // Assert
        Assert.Single(config);
        Assert.Equal("Name", config[0].Key);
        Assert.Equal("input", config[0].Type);
        Assert.Equal("Full Name", config[0].Props?.Label);
    }
    
    [Fact]
    public void Builder_Creates_Multiple_Simple_Fields()
    {
        // Arrange & Act
        var builder = FormlyBuilder.For<Person>()
            .Field(x => x.Name, f => f.Type("input").Label("Full Name"))
            .Field(x => x.Age, f => f.Type("number").Label("Age"));
            
        var config = builder.Build();
        
        // Assert
        Assert.Equal(2, config.Length);
        
        Assert.Equal("Name", config[0].Key);
        Assert.Equal("input", config[0].Type);
        Assert.Equal("Full Name", config[0].Props?.Label);
        
        Assert.Equal("Age", config[1].Key);
        Assert.Equal("number", config[1].Type);
        Assert.Equal("Age", config[1].Props?.Label);
    }
    
    [Fact]
    public void Builder_Creates_Required_Field()
    {
        // Arrange & Act
        var builder = FormlyBuilder.For<Person>()
            .Field(x => x.Name, f => f.Type("input").Label("Full Name").Required());
            
        var config = builder.Build();
        
        // Assert
        Assert.Single(config);
        Assert.Equal("Name", config[0].Key);
        Assert.True(config[0].Props?.Required);
    }
    
    [Fact]
    public void Builder_Creates_Field_Group()
    {
        // Arrange & Act
        var builder = FormlyBuilder.For<Person>()
            .FieldGroup(x => x.Address, a => a
                .Field(x => x.Street, f => f.Type("input").Label("Street"))
                .Field(x => x.City, f => f.Type("input").Label("City")));
            
        var config = builder.Build();
        
        // Assert
        Assert.Single(config);
        Assert.Equal("Address", config[0].Key);
        Assert.NotNull(config[0].FieldGroup);
        Assert.Equal(2, config[0].FieldGroup?.Length);
        
        Assert.Equal("Street", config[0].FieldGroup?[0].Key);
        Assert.Equal("input", config[0].FieldGroup?[0].Type);
        Assert.Equal("Street", config[0].FieldGroup?[0].Props?.Label);
        
        Assert.Equal("City", config[0].FieldGroup?[1].Key);
    }
    
    [Fact]
    public void Builder_Creates_Field_Array()
    {
        // Arrange & Act
        var builder = FormlyBuilder.For<Person>()
            .FieldArray(x => x.Friends, f => f
                .Field(x => x.Username, f => f.Type("input").Label("Username"))
                .Field(x => x.Email, f => f.Type("input").Label("Email")));
            
        var config = builder.Build();
        
        // Assert
        Assert.Single(config);
        Assert.Equal("Friends", config[0].Key);
        Assert.NotNull(config[0].FieldArray);
        Assert.NotNull(config[0].FieldArray?.FieldGroup);
        Assert.Equal(2, config[0].FieldArray?.FieldGroup?.Length);
        
        Assert.Equal("Username", config[0].FieldArray?.FieldGroup?[0].Key);
        Assert.Equal("Email", config[0].FieldArray?.FieldGroup?[1].Key);
    }
    
    [Fact]
    public void Builder_Serializes_To_Valid_Json()
    {
        // Arrange
        var builder = FormlyBuilder.For<Person>()
            .Field(x => x.Name, f => f.Type("input").Label("Full Name"));
        
        // Act
        var json = builder.BuildJson();
        
        // Assert
        Assert.Contains("\"key\":\"Name\"", json);
        Assert.Contains("\"type\":\"input\"", json);
        Assert.Contains("\"label\":\"Full Name\"", json);
    }
    
    [Fact]
    public void Builder_Creates_Complex_Form()
    {
        // Arrange & Act
        var builder = FormlyBuilder.For<Person>()
            .Field(x => x.Name, f => f.Type("input").Label("Full Name").Required())
            .Field(x => x.Age, f => f.Type("number").Label("Age"))
            .FieldGroup(x => x.Address, a => a
                .Field(x => x.Street, f => f.Type("input").Label("Street"))
                .Field(x => x.City, f => f.Type("input").Label("City"))
                .Field(x => x.State, f => f.Type("input").Label("State"))
                .Field(x => x.Country, f => f.Type("input").Label("Country"))
                .Field(x => x.ZipCode, f => f.Type("input").Label("Zip Code")))
            .FieldArray(x => x.Friends, f => f
                .Field(x => x.Username, f => f.Type("input").Label("Username"))
                .Field(x => x.Email, f => f.Type("input").Label("Email"))
                .Field(x => x.IsActive, f => f.Type("checkbox").Label("Active")));
            
        var config = builder.Build();
        
        // Assert
        Assert.Equal(4, config.Length);
        
        // Basic fields
        Assert.Equal("Name", config[0].Key);
        Assert.Equal("Age", config[1].Key);
        
        // Field group
        Assert.Equal("Address", config[2].Key);
        Assert.Equal(5, config[2].FieldGroup?.Length);
        
        // Field array
        Assert.Equal("Friends", config[3].Key);
        Assert.Equal(3, config[3].FieldArray?.FieldGroup?.Length);
    }
}

/// <summary>
/// Tests for the FieldBuilder functionality 
/// </summary>
public class FieldBuilderTests
{
    [Fact]
    public void FieldBuilder_Sets_Core_Properties()
    {
        // Arrange & Act
        var builder = FormlyBuilder.For<Person>()
            .Field(x => x.Name, f => f
                .Id("person-name")
                .Name("personName")
                .ClassName("form-control")
                .Type("input"));
                
        var config = builder.Build();
        
        // Assert
        Assert.Single(config);
        Assert.Equal("Name", config[0].Key);
        Assert.Equal("person-name", config[0].Id);
        Assert.Equal("personName", config[0].Name);
        Assert.Equal("form-control", config[0].ClassName);
        Assert.Equal("input", config[0].Type);
    }
    
    [Fact]
    public void FieldBuilder_Sets_Template_Options()
    {
        // Arrange & Act
        var builder = FormlyBuilder.For<Person>()
            .Field(x => x.Name, f => f
                .Label("Full Name")
                .Placeholder("Enter your full name")
                .Description("Please enter your legal full name")
                .Required()
                .Disabled());
                
        var config = builder.Build();
        var props = config[0].Props;
        
        // Assert
        Assert.Equal("Full Name", props?.Label);
        Assert.Equal("Enter your full name", props?.Placeholder);
        Assert.Equal("Please enter your legal full name", props?.Description);
        Assert.True(props?.Required);
        Assert.True(props?.Disabled);
    }
    
    [Fact]
    public void FieldBuilder_Sets_Validators()
    {
        // Arrange & Act
        var builder = FormlyBuilder.For<Person>()
            .Field(x => x.Name, f => f
                .Validator("required", true)
                .Validator("maxLength", 50));
                
        var config = builder.Build();
        
        // Assert
        Assert.NotNull(config[0].Validators);
        Assert.True((bool)config[0].Validators!["required"]);
        Assert.Equal(50, config[0].Validators["maxLength"]);
    }
    
    [Fact]
    public void FieldBuilder_Sets_Validation_Messages()
    {
        // Arrange & Act
        var builder = FormlyBuilder.For<Person>()
            .Field(x => x.Name, f => f
                .ValidationMessage("required", "Name is required")
                .ValidationMessage("maxLength", "Name is too long")
                .ShowValidation(true));
                
        var config = builder.Build();
        var validation = config[0].Validation;
        
        // Assert
        Assert.NotNull(validation);
        Assert.NotNull(validation!.Messages);
        Assert.Equal("Name is required", validation.Messages!["required"]);
        Assert.Equal("Name is too long", validation.Messages["maxLength"]);
        Assert.True(validation.Show);
    }
    
    [Fact]
    public void FieldBuilder_Sets_Expression_Properties()
    {
        // Arrange & Act
        var builder = FormlyBuilder.For<Person>()
            .Field(x => x.Name, f => f
                .ExpressionProperty("props.label", "model.customLabel")
                .ExpressionProperty("props.disabled", "model.isDisabled"));
                
        var config = builder.Build();
        var expressionProps = config[0].ExpressionProperties;
        
        // Assert
        Assert.NotNull(expressionProps);
        Assert.Equal("model.customLabel", expressionProps!["props.label"]);
        Assert.Equal("model.isDisabled", expressionProps["props.disabled"]);
    }
    
    [Fact]
    public void FieldBuilder_Sets_Hide_Properties()
    {
        // Arrange & Act
        var builder = FormlyBuilder.For<Person>()
            .Field(x => x.Name, f => f
                .Hide(true)
                .HideExpression("model.hideCondition"));
                
        var config = builder.Build();
        
        // Assert
        Assert.True(config[0].Hide);
        Assert.Equal("model.hideCondition", config[0].HideExpression);
    }
    
    [Fact]
    public void FieldBuilder_Sets_Model_Options()
    {
        // Arrange & Act
        var builder = FormlyBuilder.For<Person>()
            .Field(x => x.Name, f => f
                .Debounce(300)
                .UpdateOn("blur"));
                
        var config = builder.Build();
        var modelOptions = config[0].ModelOptions;
        
        // Assert
        Assert.NotNull(modelOptions);
        Assert.Equal(300, modelOptions!.Debounce);
        Assert.Equal("blur", modelOptions.UpdateOn);
    }
    
    [Fact]
    public void FieldBuilder_Sets_Wrappers()
    {
        // Arrange & Act
        var builder = FormlyBuilder.For<Person>()
            .Field(x => x.Name, f => f
                .Wrappers("form-field", "panel"));
                
        var config = builder.Build();
        
        // Assert
        Assert.NotNull(config[0].Wrappers);
        Assert.Equal(2, config[0].Wrappers!.Length);
        Assert.Equal("form-field", config[0].Wrappers[0]);
        Assert.Equal("panel", config[0].Wrappers[1]);
    }
    
    [Fact]
    public void FieldBuilder_Sets_Custom_Prop()
    {
        // Arrange & Act
        var builder = FormlyBuilder.For<Person>()
            .Field(x => x.Name, f => f
                .Prop("customOption", "customValue"));
                
        var config = builder.Build();
        var props = config[0].Props;
        
        // Assert
        Assert.NotNull(props);
        Assert.Contains("customOption", props!.AdditionalProperties.Keys);
        Assert.Equal("customValue", props.AdditionalProperties["customOption"]);
    }
}

/// <summary>
/// Integration tests for more complex scenarios
/// </summary>
public class FormlyIntegrationTests
{
    [Fact]
    public void Json_Output_Has_Correct_Structure_And_CamelCase_Properties()
    {
        // Arrange
        var builder = FormlyBuilder.For<Person>()
            .Field(x => x.Name, f => f.Type("input").Label("Full Name").Required());
            
        // Act
        var json = builder.BuildJson();
        
        // Assert
        // Check if json is properly formatted with camelCase
        Assert.Contains("\"key\":\"Name\"", json);
        Assert.Contains("\"type\":\"input\"", json);
        Assert.Contains("\"label\":\"Full Name\"", json);
        Assert.Contains("\"required\":true", json);
    }
    
    [Fact]
    public void Json_Output_Excludes_Null_Properties()
    {
        // Arrange
        var builder = FormlyBuilder.For<Person>()
            .Field(x => x.Name, f => f.Type("input").Label("Full Name"));
            
        // Act
        var json = builder.BuildJson();
        
        // Assert
        // Ensure null properties are not included
        Assert.DoesNotContain("\"id\":null", json);
        Assert.DoesNotContain("\"defaultValue\":null", json);
        Assert.DoesNotContain("\"validators\":null", json);
    }
    
    [Fact]
    public void Complex_Form_With_All_Features()
    {
        // Arrange & Act - Create a complex form with almost all features
        var builder = FormlyBuilder.For<Person>()
            // Basic fields
            .Field(x => x.Name, f => f
                .Id("person-name")
                .Type("input")
                .Label("Full Name")
                .Required()
                .Placeholder("Enter your full name")
                .Validator("maxLength", 50)
                .ValidationMessage("required", "Name is required")
                .ValidationMessage("maxLength", "Name cannot be longer than 50 characters")
                .ExpressionProperty("props.disabled", "model.isReadOnly"))
                
            .Field(x => x.Age, f => f
                .Type("number")
                .Label("Age")
                .Min(0)
                .Max(120))
                
            // Field group
            .FieldGroup(x => x.Address, a => a
                .Field(x => x.Street, f => f.Type("input").Label("Street").Required())
                .Field(x => x.City, f => f.Type("input").Label("City").Required())
                .Field(x => x.State, f => f.Type("input").Label("State"))
                .Field(x => x.Country, f => f.Type("input").Label("Country"))
                .Field(x => x.ZipCode, f => f.Type("input").Label("Zip Code").Required()))
                
            // Field array
            .FieldArray(x => x.Friends, f => f
                .Field(x => x.Username, f => f.Type("input").Label("Username").Required())
                .Field(x => x.Email, f => f.Type("input").Label("Email").Required())
                .Field(x => x.IsActive, f => f.Type("checkbox").Label("Active")));
        
        var config = builder.Build();
        var json = builder.BuildJson();
        
        // Assert - Basic validation of structure
        Assert.Equal(4, config.Length);
        
        // Verify JSON contains needed structures
        Assert.Contains("\"fieldGroup\":", json);
        Assert.Contains("\"fieldArray\":", json);
        Assert.Contains("\"validators\":", json);
        Assert.Contains("\"validation\":", json);
        Assert.Contains("\"messages\":", json);
        Assert.Contains("\"expressionProperties\":", json);
    }
}

public class FormlyFieldConfigBuilderTests
{
    [Fact]
    public void Builder_Creates_Simple_Field_Config()
    {
        var builder = FormlyBuilder.For<Person>()
            .Field(x => x.Name, f => f.Type("input").Label("Full Name").Required())
            .Field(x => x.Age, f => f.Type("number").Label("Age"))
            .FieldGroup(x => x.Address, a => a
                .Field(x => x.ZipCode, f => f.Label("Zip"))
                .Field(x => x.Street, f => f.Label("Street")));
            

        var config = builder.Build();
        Assert.Equal(3, config.Length);
        
        Assert.Equal("Name", config[0].Key);
        Assert.Equal("input", config[0].Type);
        Assert.Equal("Full Name", config[0].Props.Label);
        Assert.True(config[0].Props.Required);

        Assert.Equal("Age", config[1].Key);
        Assert.Equal("number", config[1].Type);

        Assert.Equal("Address", config[2].Key);
        Assert.Equal(2, config[2].FieldGroup.Length);
        Assert.Equal("ZipCode", config[2].FieldGroup[0].Key);
        Assert.Equal("Street", config[2].FieldGroup[1].Key);

        var json = builder.BuildJson();
        Assert.NotEmpty(json);
    }
}