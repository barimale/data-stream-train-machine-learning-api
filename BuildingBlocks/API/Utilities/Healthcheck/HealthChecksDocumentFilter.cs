using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BuildingBlocks.API.Utilities.Healthcheck
{
    public class HealthChecksDocumentFilter : IDocumentFilter
    {
        private const string _name = "Heartbeat";
        private const string _operationId = "GetHeartbeat";
        private const string _summary = "Get System Heartbeat";
        private const string _description = "Get the heartbeat of the system.";

        private const string _okCode = "200";
        private const string _okDescription = "Healthy";
        private const string _notOkCode = "503";
        private const string _notOkDescription = "Not Healthy";

        private const string _typeString = "string";
        private const string _typeArray = "array";
        private const string _typeObject = "object";
        private const string _applicationJson = "application/json";
        private const string _timespanFormat = "[-][d'.']hh':'mm':'ss['.'fffffff]";


        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            ApplyComponentHealthStatus(swaggerDoc);
            ApplyComponentHealthReportEntry(swaggerDoc);
            ApplyComponentHealthReport(swaggerDoc);

            ApplyPathHeartbeat(swaggerDoc);
        }

        private IList<IOpenApiAny> GetHealthStatusValues()
        {
            return typeof(HealthStatus)
                .GetEnumValues()
                .Cast<object>()
                .Select(value => (IOpenApiAny)new OpenApiString(value.ToString()))
                .ToList();
        }

        private void ApplyComponentHealthStatus(OpenApiDocument swaggerDoc)
        {
            swaggerDoc?.Components.Schemas.Add(nameof(HealthStatus), new OpenApiSchema
            {
                Type = _typeString,
                Enum = GetHealthStatusValues()
            });
        }

        private void ApplyComponentHealthReportEntry(OpenApiDocument swaggerDoc)
        {
            swaggerDoc?.Components.Schemas.Add(nameof(HealthReportEntry), new OpenApiSchema
            {
                Type = _typeObject,
                Properties = new Dictionary<string, OpenApiSchema>
            {
                {
                    HeartbeatUtility.Name,
                    new OpenApiSchema
                    {
                        Type = _typeObject,
                        Properties = new Dictionary<string, OpenApiSchema>
                {
                {
                    HeartbeatUtility.Data,
                    new OpenApiSchema
                    {
                        Type = _typeObject,
                        Nullable = true,
                        AdditionalProperties = new OpenApiSchema()
                    }
                },
                                {
                    HeartbeatUtility.Description,
                    new OpenApiSchema
                    {
                        Type = _typeString,
                        Nullable = true
                    }
                },
                {
                    HeartbeatUtility.Duration,
                    new OpenApiSchema
                    {
                        Type = _typeString,
                        Nullable = true
                    }
                },
                {
                    HeartbeatUtility.Status,
                    new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = nameof(HealthStatus)
                        }
                    }
                },
                {
                    HeartbeatUtility.Tags,
                    new OpenApiSchema
                    {
                        Type = _typeArray,
                        Nullable = true,
                        Items  = new OpenApiSchema()
                    }
                },
                }}}},
            });
        }

        private void ApplyComponentHealthReport(OpenApiDocument swaggerDoc)
        {
            swaggerDoc?.Components.Schemas.Add(nameof(HealthReport), new OpenApiSchema()
            {
                Type = _typeObject,
                Properties = new Dictionary<string, OpenApiSchema>
            {
                {
                    HeartbeatUtility.Status,
                    new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = nameof(HealthStatus)
                        }
                    }
                },
                {
                    HeartbeatUtility.TotalTime,
                    new OpenApiSchema
                    {
                        Type = _typeString,
                        Format = _timespanFormat,
                        Nullable = true
                    }
                },
                {
                    HeartbeatUtility.Results,
                    new OpenApiSchema
                    {
                        Type = _typeArray,
                        Nullable = true,
                        Items = new OpenApiSchema
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = nameof(HealthReportEntry)
                            }
                        }
                    }
                }
            }
            });

        }

        private void ApplyPathHeartbeat(OpenApiDocument swaggerDoc)
        {
            swaggerDoc?.Paths.Add(HeartbeatUtility.Path, new OpenApiPathItem
            {
                Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                {
                    OperationType.Get,
                    new OpenApiOperation
                    {
                        Summary = _summary,
                        Description = _description,
                        OperationId = _operationId,
                        Tags = new List<OpenApiTag>
                        {
                            new OpenApiTag
                            {
                                Name = _name
                            }
                        },
                        Responses = new OpenApiResponses
                        {
                            {
                                _okCode,
                                new OpenApiResponse
                                {
                                    Description = _okDescription,
                                    Content = new Dictionary<string, OpenApiMediaType>
                                    {
                                        {
                                            _applicationJson,
                                            new OpenApiMediaType
                                            {
                                                Schema = new OpenApiSchema
                                                {
                                                    Reference = new OpenApiReference
                                                    {
                                                        Type = ReferenceType.Schema,
                                                        Id = nameof(HealthReport)
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            {
                                _notOkCode,
                                new OpenApiResponse
                                {
                                    Description = _notOkDescription,
                                    Content = new Dictionary<string, OpenApiMediaType>
                                    {
                                        {
                                            _applicationJson,
                                            new OpenApiMediaType
                                            {
                                                Schema = new OpenApiSchema
                                                {
                                                    Reference = new OpenApiReference
                                                    {
                                                        Type = ReferenceType.Schema,
                                                        Id = nameof(HealthReport)
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            });
        }
    }
}
