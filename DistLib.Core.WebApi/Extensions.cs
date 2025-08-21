using Microsoft.Extensions.DependencyInjection;

namespace DistLib.Core.WebApi;

public static class Extensions
{
    // add json serializer
    public static IDistLibBuilder AddWebApi(this IDistLibBuilder builder)
    {
        builder.Services.AddH
        return builder;
    }
    
    // add http context accessor
    
    
    // stack middleware
    
    // command and query dispatcher
    
    // a clean api for binding properties to commands || queries
    
    //  common http response => Accepted / Created / Unauthorised / Forbidden
    // Ok / Bad Request / Redirect / Moved Permanently / Not Found / Internal Server Error
    
    // Exception handling middle ware
    
    // read json async
    
    // ability to read a request (either iquery or icommand) from query string, body or other http input formats
    
}
