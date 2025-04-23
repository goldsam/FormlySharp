using Xunit;
using FormlySharp;
using System.Text.Json;
using System.Collections.Generic;

namespace FormlySharp.Tests;

public class Address
{
    public string Street { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty; 


}
public class Person
{
    public string Name { get; set; } = string.Empty;

    public int Age { get; set; }

    public Address Address { get; set; } = new Address();   
}

public class UnitTest1
{
    [Fact]
    public void Test1()
    {

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
            

           //.ForField(x => x.Address, f => f.WithType("input").WithLabel("Full Name"));
            //.ForField(x => x.Name, f => f.WithType("input").WithLabel("Full Name").WithRequired())
            //.ForField(x => x.Age, f => f.WithType("number").WithLabel("Age"));



        var config = builder.Build();
        Assert.Equal(3, config.Length);
        
        Assert.Equal("Name", config[0].Key);
        Assert.Equal("input", config[0].Type);
        Assert.Equal("Full Name", config[0].Props.Label);
        Assert.True(config[0].Props.Required);

        Assert.Equal("Age", config[1].Key);
        Assert.Equal("number", config[1].Type);

        Assert.Equal("Address", config[2].Key);
     //   Assert.Equal("number", config[2].Type);


        var json = builder.BuildJson();
    }

    //[Fact]
    //public void Builder_Serializes_To_Valid_Json()
    //{
    //    var builder = new FormlyFieldConfigBuilder<Person>()
    //        .ForField(x => x.Name, f => f.WithType("input").WithLabel("Full Name"));
    //    var config = builder.Build();
    //    var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { PropertyNamingPolicy = null });
    //    Assert.Contains("\"key\":\"Name\"", json);
    //    Assert.Contains("\"type\":\"input\"", json);
    //    Assert.Contains("\"label\":\"Full Name\"", json);
    //}
}