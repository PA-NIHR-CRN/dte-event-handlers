using System;
using System.Collections.Generic;
using System.Text.Json;
using Domain.Commands;
using Evento;

namespace Adapter.Mappers
{
    public class CompleteStepMapper : ICloudEventMapper
    {
        public Uri Schema => new Uri("completestep/1.0", UriKind.RelativeOrAbsolute);
        
        private readonly Uri _source = new Uri("dte-web", UriKind.RelativeOrAbsolute);
        private readonly List<string> _dataContentTypes = new List<string> { "application/json", "application/cloudevents+json" };

        public Command Map(CloudEvent request)
        {
            Ensure.NotNull(request, nameof(request));
            
            if (!_dataContentTypes.Contains(request.DataContentType))
            {
                throw new ArgumentException($"While running Map in '{nameof(CompleteStepMapper)}' I can't recognize the DataContentType:{request.DataContentType} (DataSchema:{request.DataSchema};Source:{request.Source})");
            }

            if (_source.ToString() != "*" && !request.Source.Equals(_source))
            {
                throw new ArgumentException($"While running Map in '{nameof(CompleteStepMapper)}' I can't recognize the Source:{request.Source} (DataSchema:{request.DataSchema})");
            }

            if (!request.DataSchema.Equals(Schema))
            {
                throw new ArgumentException($"While running Map in '{nameof(CompleteStepMapper)}' I can't recognize the DataSchema:{request.DataSchema} (Source:{request.Source})");
            }

            CompleteStepCommand cmd = JsonSerializer.Deserialize<CompleteStepCommand>(request.Data.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            cmd.Metadata = new Dictionary<string, string>
            {
                {"$correlationId", cmd.StudyId},
                {"source", request.Source.ToString()},
                {"$applies", request.Time.ToString("O")},
                {"cloudrequest-id", request.Id},
                {"schema", request.DataSchema.ToString()},
                {"content-type", request.DataContentType}
            };
            
            return cmd;
        }
    }
}