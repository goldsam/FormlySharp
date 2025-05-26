using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using Xunit;

namespace FormlySharp.OpenAPI.Tests;

public class FormlyOpenApiParserTests
{
    [Fact]
    public void Constructor_WithValidOpenApiStream_CreatesParser()
    {
        // Arrange
        var openApiJson = TestData.GenerateSimpleOpenApiSpec();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(openApiJson));
        
        // Act
        var parser = new FormlyOpenApiParser(stream);
        
        // Assert
        Assert.NotNull(parser);
    }
    
    [Fact]
    public void GetFormlyConfig_WithValidSchemaReference_ReturnsFieldConfigs()
    {
        // Arrange
        var openApiJson = TestData.GenerateSimpleOpenApiSpec();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(openApiJson));
        var parser = new FormlyOpenApiParser(stream);
        
        // Act
        var configs = parser.GetFormlyConfig("#/components/schemas/User");
        
        // Assert
        Assert.NotNull(configs);
        Assert.Equal(3, configs.Length);
        
        // Verify the generated fields match our schema
        var idField = configs.FirstOrDefault(f => f.Key?.ToString() == "id");
        Assert.NotNull(idField);
        Assert.Equal("number", idField.Type);
        
        var nameField = configs.FirstOrDefault(f => f.Key?.ToString() == "name");
        Assert.NotNull(nameField);
        Assert.Equal("input", nameField.Type);
        Assert.True(nameField.Props?.Required);
        
        var emailField = configs.FirstOrDefault(f => f.Key.ToString() == "email");
        Assert.NotNull(emailField);
        Assert.Equal("input", emailField.Type);
    }
    
    [Fact]
    public void GetFormlyConfig_WithInvalidSchemaReference_ThrowsArgumentException()
    {
        // Arrange
        var openApiJson = TestData.GenerateSimpleOpenApiSpec();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(openApiJson));
        var parser = new FormlyOpenApiParser(stream);
        
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            parser.GetFormlyConfig("invalid-reference"));
            
        Assert.Contains("Only local references to components/schemas are supported", exception.Message);
    }
    
    [Fact]
    public void GetFormlyConfig_WithNonexistentSchemaReference_ThrowsArgumentException()
    {
        // Arrange
        var openApiJson = TestData.GenerateSimpleOpenApiSpec();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(openApiJson));
        var parser = new FormlyOpenApiParser(stream);
        
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            parser.GetFormlyConfig("#/components/schemas/NonExistentSchema"));
            
        Assert.Contains("not found in the document", exception.Message);
    }
    
    //[Fact]
    //public void GetFormlyConfig_WithCustomFormlyExtension_ReturnsExtensionConfig()
    //{
    //    // Arrange
    //    var openApiJson = TestData.GenerateOpenApiSpecWithFormlyExtension();
    //    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(openApiJson));
    //    var parser = new FormlyOpenApiParser(stream);
        
    //    // Act
    //    var configs = parser.GetFormlyConfig("#/components/schemas/Product");
        
    //    // Assert
    //    Assert.NotNull(configs);
    //    Assert.Single(configs);
        
    //    var field = configs[0];
    //    Assert.Equal("customField", field.Key);
    //    Assert.Equal("custom", field.Type);
    //    Assert.Equal("Custom Label", field.Props?.Label);
    //}
    
    [Fact]
    public void GetFormlyConfig_WithNestedObjectSchema_GeneratesFieldGroup()
    {
        // Arrange
        var openApiJson = TestData.GenerateOpenApiSpecWithNestedObjects();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(openApiJson));
        var parser = new FormlyOpenApiParser(stream);
        
        // Act
        var configs = parser.GetFormlyConfig("#/components/schemas/Person");
        
        // Assert
        Assert.NotNull(configs);
        Assert.Equal(2, configs.Length);
        
        var nameField = configs.FirstOrDefault(f => f.Key?.ToString() == "name");
        Assert.NotNull(nameField);
        
        var addressField = configs.FirstOrDefault(f => f.Key?.ToString() == "address");
        Assert.NotNull(addressField);
        Assert.Equal("object", addressField.Type);
        Assert.NotNull(addressField.FieldGroup);
        Assert.Equal(3, addressField.FieldGroup.Length);
        
        var streetField = addressField.FieldGroup.FirstOrDefault(f => f.Key?.ToString() == "street");
        Assert.NotNull(streetField);
        Assert.Equal("input", streetField.Type);
    }
    
    [Fact]
    public void GetFormlyConfig_WithArraySchema_GeneratesFieldArray()
    {
        // Arrange
        var openApiJson = TestData.GenerateOpenApiSpecWithArray();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(openApiJson));
        var parser = new FormlyOpenApiParser(stream);
        
        // Act
        var configs = parser.GetFormlyConfig("#/components/schemas/Team");
        
        // Assert
        Assert.NotNull(configs);
        Assert.Single(configs);
        
        var membersField = configs.FirstOrDefault(f => f.Key?.ToString() == "members");
        Assert.NotNull(membersField);
        Assert.Equal("array", membersField.Type);
        Assert.NotNull(membersField.FieldArray);
        Assert.NotNull(membersField.FieldArray.FieldGroup);
    }
    
    [Fact]
    public void GetFormlyConfig_WithEnumSchema_GeneratesOptionsArray()
    {
        // Arrange
        var openApiJson = TestData.GenerateOpenApiSpecWithEnum();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(openApiJson));
        var parser = new FormlyOpenApiParser(stream);
        
        // Act
        var configs = parser.GetFormlyConfig("#/components/schemas/Status");
        
        // Assert
        Assert.NotNull(configs);
        Assert.Single(configs);
        
        var statusField = configs.FirstOrDefault(f => f.Key?.ToString() == "status");
        Assert.NotNull(statusField);
        Assert.Equal("input", statusField.Type);
        Assert.NotNull(statusField.Props?.Options);
        Assert.Equal(3, statusField.Props.Options.Count());
    }
    
    [Fact]
    public void GetFormlyConfig_WithValidationConstraints_GeneratesValidation()
    {
        // Arrange
        var openApiJson = TestData.GenerateOpenApiSpecWithValidation();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(openApiJson));
        var parser = new FormlyOpenApiParser(stream);
        
        // Act
        var configs = parser.GetFormlyConfig("#/components/schemas/ValidatedModel");
        
        // Assert
        Assert.NotNull(configs);
        Assert.Equal(4, configs.Length);
        
        var nameField = configs.FirstOrDefault(f => f.Key?.ToString() == "name");
        Assert.NotNull(nameField);
        Assert.Equal(3, nameField.Props?.MinLength);
        Assert.Equal(50, nameField.Props?.MaxLength);
        
        var emailField = configs.FirstOrDefault(f => f.Key?.ToString() == "email");
        Assert.NotNull(emailField);
        Assert.NotNull(emailField.Props?.Pattern);
        
        var ageField = configs.FirstOrDefault(f => f.Key?.ToString() == "age");
        Assert.NotNull(ageField);
        Assert.Equal(18, ageField.Props?.Min);
        Assert.Equal(120, ageField.Props?.Max);
    }
}