using Microsoft.AspNetCore.WebUtilities;
using System.Xml.Serialization;

namespace ProjectManagement.CustomResults
{
    public class XmlResults(object result) : IResult
    {
        private const string XmlContentType = "application/xml";

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.ContentType = XmlContentType;

            using var stream = new FileBufferingWriteStream();

            var serializer = new XmlSerializer(result.GetType());
            serializer.Serialize(stream, result);

            await stream.DrainBufferAsync(httpContext.Response.Body);
        }
    }
}