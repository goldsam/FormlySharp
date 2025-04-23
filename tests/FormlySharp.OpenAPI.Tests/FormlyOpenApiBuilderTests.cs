using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Microsoft.OpenApi.Models;

namespace FormlySharp.OpenAPI.Tests;

public class FormlyOpenApiBuilderTests
{
    [Fact]
    public void Constructor_WithStream_CreatesBuilder()
    {
        // Arrange
        var openApiJson = TestData.GenerateSimpleOpenApiSpec();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(openApiJson));
        
        // Act
        var builder = new FormlyOpenApiBuilder(stream);
        
        // Assert
        Assert.NotNull(builder);
    }
    
    [Fact]
    public void Constructor_WithFilePath_CreatesBuilder()
    {
        // Arrange
        var openApiJson = TestData.GenerateSimpleOpenApiSpec();
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, openApiJson);
        
        try
        {
            // Act
            var builder = new FormlyOpenApiBuilder(tempFile);
            
            // Assert
            Assert.NotNull(builder);
        }
        finally
        {
            // Cleanup
            File.Delete(tempFile);
        }
    }
    
    [Fact]
    public void Constructor_WithParser_CreatesBuilder()
    {
        // Arrange
        var openApiJson = TestData.GenerateSimpleOpenApiSpec();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(openApiJson));
        var parser = new FormlyOpenApiParser(stream);
        
        // Act
        var builder = new FormlyOpenApiBuilder(parser);
        
        // Assert
        Assert.NotNull(builder);
    }
    
    
    [Fact]
    public void AddSchema_WithCustomizeAction_ModifiesSchema()
    {
        // Arrange
        var openApiJson = TestData.GenerateSimpleOpenApiSpec();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(openApiJson));
        var builder = new FormlyOpenApiBuilder(stream);
        
        // Act
        builder.AddSchema("#/components/schemas/User", configs => 
        {
            // Add a custom field
            var customField = new FormlyFieldConfig
            {
                Key = "customField",
                Type = "custom",
                Props = new FormlyFieldProps { Label = "Custom Field" }
            };
            
            Array.Resize(ref configs, configs.Length + 1);
            configs[configs.Length - 1] = customField;
            return configs;
        });
        
        var configs = builder.GetFormlyConfig("#/components/schemas/User");
        
        // Assert
        Assert.NotNull(configs);
        Assert.Equal(4, configs.Length);
        
        var customField = configs.FirstOrDefault(f => f.Key?.ToString() == "customField");
        Assert.NotNull(customField);
        Assert.Equal("custom", customField.Type);
    }
    
    [Fact]
    public void GetFormlyConfig_WithUnaddedSchema_ReturnsNull()
    {
        // Arrange
        var openApiJson = TestData.GenerateSimpleOpenApiSpec();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(openApiJson));
        var builder = new FormlyOpenApiBuilder(stream);
        
        // Act - Not calling AddSchema first
        var configs = builder.GetFormlyConfig("#/components/schemas/User");
        
        // Assert
        Assert.Null(configs);
    }
    
    [Fact]
    public void AddValidationFromSchema_WithValidationRules_AddsValidationToField()
    {
        // Arrange
        var field = new FormlyFieldConfig
        {
            Key = "testField",
            Type = "input"
        };
        
        var schema = new OpenApiSchema
        {
            Type = "string",
            MinLength = 5,
            MaxLength = 10,
            Pattern = "^[a-z]+$"
        };
        
        // Act
        FormlyOpenApiBuilder.AddValidationFromSchema(field, schema);
        
        // Assert
        Assert.NotNull(field.Props);
        Assert.Equal(5, field.Props.MinLength);
        Assert.Equal(10, field.Props.MaxLength);
        Assert.Equal("^[a-z]+$", field.Props.Pattern);
        Assert.NotNull(field.Validation);
    }
    
    [Fact]
    public void AddValidationFromSchema_WithNumericValidation_AddsMinMaxToField()
    {
        // Arrange
        var field = new FormlyFieldConfig
        {
            Key = "testField",
            Type = "number"
        };
        
        var schema = new OpenApiSchema
        {
            Type = "number",
            Minimum = 0,
            Maximum = 100
        };
        
        // Act
        FormlyOpenApiBuilder.AddValidationFromSchema(field, schema);
        
        // Assert
        Assert.NotNull(field.Props);
        Assert.Equal(new decimal(0), field.Props.Min);
        Assert.Equal(new decimal(100), field.Props.Max);
        Assert.NotNull(field.Validation);
    }
    
    [Fact]
    public void AddValidationFromSchema_WithRequiredField_SetsRequiredProperty()
    {
        // Arrange
        var field = new FormlyFieldConfig
        {
            Key = "name"
        };
        
        var schema = new OpenApiSchema
        {
            Required = { "name", "email" }
        };
        
        // Act
        FormlyOpenApiBuilder.AddValidationFromSchema(field, schema);
        
        // Assert
        Assert.NotNull(field.Props);
        Assert.True(field.Props.Required);
    }
}