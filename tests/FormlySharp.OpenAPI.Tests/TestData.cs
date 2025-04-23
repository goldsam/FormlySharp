namespace FormlySharp.OpenAPI.Tests;

/// <summary>
/// Contains test data that can be reused across multiple test classes
/// </summary>
public static class TestData
{
    /// <summary>
    /// Generates a simple OpenAPI specification with a User schema
    /// </summary>
    public static string GenerateSimpleOpenApiSpec()
    {
        return @"{
  ""openapi"": ""3.0.0"",
  ""info"": {
    ""title"": ""Test API"",
    ""version"": ""1.0.0""
  },
  ""paths"": {},
  ""components"": {
    ""schemas"": {
      ""User"": {
        ""type"": ""object"",
        ""required"": [""name""],
        ""properties"": {
          ""id"": {
            ""type"": ""integer"",
            ""format"": ""int64""
          },
          ""name"": {
            ""type"": ""string""
          },
          ""email"": {
            ""type"": ""string""
          }
        }
      }
    }
  }
}";
    }
    
    /// <summary>
    /// Generates an OpenAPI specification with custom x-formly extension
    /// </summary>
    public static string GenerateOpenApiSpecWithFormlyExtension()
    {
        return @"{
  ""openapi"": ""3.0.0"",
  ""info"": {
    ""title"": ""Test API"",
    ""version"": ""1.0.0""
  },
  ""paths"": {},
  ""components"": {
    ""schemas"": {
      ""Product"": {
        ""type"": ""object"",
        ""properties"": {
          ""id"": {
            ""type"": ""integer""
          }
        },
        ""x-formly"": [
          {
            ""key"": ""customField"",
            ""type"": ""custom"",
            ""props"": {
              ""label"": ""Custom Label""
            }
          }
        ]
      }
    }
  }
}";
    }
    
    /// <summary>
    /// Generates an OpenAPI specification with nested object schemas
    /// </summary>
    public static string GenerateOpenApiSpecWithNestedObjects()
    {
        return @"{
  ""openapi"": ""3.0.0"",
  ""info"": {
    ""title"": ""Test API"",
    ""version"": ""1.0.0""
  },
  ""paths"": {},
  ""components"": {
    ""schemas"": {
      ""Person"": {
        ""type"": ""object"",
        ""properties"": {
          ""name"": {
            ""type"": ""string""
          },
          ""address"": {
            ""type"": ""object"",
            ""properties"": {
              ""street"": {
                ""type"": ""string""
              },
              ""city"": {
                ""type"": ""string""
              },
              ""zipCode"": {
                ""type"": ""string""
              }
            }
          }
        }
      }
    }
  }
}";
    }
    
    /// <summary>
    /// Generates an OpenAPI specification with array schema
    /// </summary>
    public static string GenerateOpenApiSpecWithArray()
    {
        return @"{
  ""openapi"": ""3.0.0"",
  ""info"": {
    ""title"": ""Test API"",
    ""version"": ""1.0.0""
  },
  ""paths"": {},
  ""components"": {
    ""schemas"": {
      ""Team"": {
        ""type"": ""object"",
        ""properties"": {
          ""members"": {
            ""type"": ""array"",
            ""items"": {
              ""type"": ""object"",
              ""properties"": {
                ""name"": {
                  ""type"": ""string""
                },
                ""role"": {
                  ""type"": ""string""
                }
              }
            }
          }
        }
      }
    }
  }
}";
    }
    
    /// <summary>
    /// Generates an OpenAPI specification with enum schema
    /// </summary>
    public static string GenerateOpenApiSpecWithEnum()
    {
        return @"{
  ""openapi"": ""3.0.0"",
  ""info"": {
    ""title"": ""Test API"",
    ""version"": ""1.0.0""
  },
  ""paths"": {},
  ""components"": {
    ""schemas"": {
      ""Status"": {
        ""type"": ""object"",
        ""properties"": {
          ""status"": {
            ""type"": ""string"",
            ""enum"": [""active"", ""pending"", ""inactive""]
          }
        }
      }
    }
  }
}";
    }
    
    /// <summary>
    /// Generates an OpenAPI specification with validation constraints
    /// </summary>
    public static string GenerateOpenApiSpecWithValidation()
    {
        return @"{
  ""openapi"": ""3.0.0"",
  ""info"": {
    ""title"": ""Test API"",
    ""version"": ""1.0.0""
  },
  ""paths"": {},
  ""components"": {
    ""schemas"": {
      ""ValidatedModel"": {
        ""type"": ""object"",
        ""required"": [""name"", ""email""],
        ""properties"": {
          ""name"": {
            ""type"": ""string"",
            ""minLength"": 3,
            ""maxLength"": 50
          },
          ""email"": {
            ""type"": ""string"",
            ""pattern"": ""^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$""
          },
          ""age"": {
            ""type"": ""integer"",
            ""minimum"": 18,
            ""maximum"": 120
          },
          ""score"": {
            ""type"": ""number"",
            ""format"": ""float"",
            ""minimum"": 0,
            ""maximum"": 100
          }
        }
      }
    }
  }
}";
    }
}