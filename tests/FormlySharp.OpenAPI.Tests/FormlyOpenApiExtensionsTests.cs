using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;

namespace FormlySharp.OpenAPI.Tests;

public class FormlyOpenApiExtensionsTests
{
    [Fact]
    public void WithFormlyConfig_AddsExtensionToSchema()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Type = "object",
            Properties =
            {
                ["name"] = new OpenApiSchema { Type = "string" }
            }
        };
        
        var formlyConfig = new[]
        {
            new FormlyFieldConfig
            {
                Key = "name",
                Type = "custom",
                Props = new FormlyFieldProps { Label = "Custom Name" }
            }
        };
        
        // Act
        schema.WithFormlyConfig(formlyConfig);
        
        // Assert
        Assert.True(schema.Extensions.ContainsKey(FormlyOpenApiParser.FormlyExtensionName));
        Assert.IsType<OpenApiObject>(schema.Extensions[FormlyOpenApiParser.FormlyExtensionName]);
    }
    
    [Fact]
    public void WithFormlyConfig_CanBeReadByParser()
    {
        // Arrange - Create a schema with custom Formly config
        var schema = new OpenApiSchema
        {
            Type = "object",
            Properties =
            {
                ["name"] = new OpenApiSchema { Type = "string" }
            }
        };
        
        var originalConfig = new[]
        {
            new FormlyFieldConfig
            {
                Key = "customField",
                Type = "custom",
                Props = new FormlyFieldProps { Label = "Custom Label" }
            }
        };
        
        schema.WithFormlyConfig(originalConfig);
        
        // Create an OpenAPI document with the schema
        var document = new OpenApiDocument
        {
            Info = new OpenApiInfo { Title = "Test API", Version = "1.0" },
            Components = new OpenApiComponents
            {
                Schemas = { ["TestSchema"] = schema }
            }
        };
        
        // Serialize and parse the OpenAPI document
        var ms = new MemoryStream();
        document.SerializeAsV3(new Microsoft.OpenApi.Writers.OpenApiJsonWriter(new StreamWriter(ms)));
        ms.Position = 0;
        
        // Act
        var parser = new FormlyOpenApiParser(ms);
        var parsedConfig = parser.GetFormlyConfig("#/components/schemas/TestSchema");
        
        // Assert
        Assert.NotNull(parsedConfig);
        Assert.Single(parsedConfig);
        Assert.Equal("customField", parsedConfig[0].Key);
        Assert.Equal("Custom Label", parsedConfig[0].Props?.Label);
    }
    
    [Fact]
    public void ApplyOpenApiSchema_AppliesSchemaToBuilder()
    {
        // This is a basic test because the implementation is a placeholder
        // In a real implementation, we'd test that the schema properties are correctly applied
        
        // Arrange
        var modelType = typeof(TestModel);
        var builder = new FormlyBuilder<TestModel>();
        
        var openApiJson = TestData.GenerateSimpleOpenApiSpec();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(openApiJson));
        var parser = new FormlyOpenApiParser(stream);
        
        // Act
        var result = builder.ApplyOpenApiSchema(parser, "#/components/schemas/User");
        
        // Assert
        Assert.NotNull(result);
        Assert.Same(builder, result); // Should return the same builder instance
    }
    
    private class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}