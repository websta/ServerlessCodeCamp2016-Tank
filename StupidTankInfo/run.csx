#load "Models\TankInfo.csx"

using System.Net;
using System.Threading.Tasks;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    var tankInfo = new TankInfo()
    {
        name = "SpupidTank",
        owner = "Roman & Gerald"
    };

    return req.CreateResponse(HttpStatusCode.OK, tankInfo);
}