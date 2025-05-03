using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.API.Utilities.Healthcheck
{
    public static class HeartbeatUtility
    {
        public const string Path = "/api/healthcheck";

        public const string ContentType = "application/json; charset=utf-8";
        public const string Status = "status";
        public const string TotalTime = "totalDuration";
        public const string Results = "entries";
        public const string Name = "Name";
        public const string Duration = "duration";
        public const string Data = "data";
        public const string Tags = "tags";
        public const string Description = "description";

        public static Task WriteResponse(HttpContext context, HealthReport healthReport)
        {
            context.Response.ContentType = ContentType;

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, CreateJsonOptions()))
                {
                    writer.WriteStartObject();

                    writer.WriteString(Status, healthReport.Status.ToString("G"));
                    writer.WriteString(TotalTime, healthReport.TotalDuration.ToString("c"));

                    if (healthReport.Entries.Count > 0)
                        writer.WriteEntries(healthReport.Entries);

                    writer.WriteEndObject();
                }

                var json = Encoding.UTF8.GetString(stream.ToArray());

                return context.Response.WriteAsync(json);
            }
        }

        private static JsonWriterOptions CreateJsonOptions()
        {
            return new JsonWriterOptions
            {
                Indented = true
            };
        }

        private static void WriteEntryData(this Utf8JsonWriter writer, IReadOnlyDictionary<string, object> data)
        {
            writer.WriteStartObject(Data);

            foreach (var item in data)
            {
                writer.WritePropertyName(item.Key);

                var type = item.Value?.GetType() ?? typeof(object);
                JsonSerializer.Serialize(writer, item.Value, type);
            }

            writer.WriteEndObject();
        }

        private static void WriteEntries(this Utf8JsonWriter writer, IReadOnlyDictionary<string, HealthReportEntry> healthReportEntries)
        {
            writer.WriteStartArray(Results);

            foreach (var entry in healthReportEntries)
            {
                writer.WriteStartObject();

                writer.WriteString(Name, entry.Key);
                writer.WriteString(Status, entry.Value.Status.ToString("G"));

                if (entry.Value.Description != null)
                    writer.WriteString(Duration, entry.Value.Description);

                if (entry.Value.Data.Count > 0)
                    writer.WriteEntryData(entry.Value.Data);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }
}
