using MTCG.BL.Http;

namespace MTCG.BL.EndpointController
{
    public abstract class Controller { }

    public interface IHttpGet
    {
        HttpResponse Get(HttpRequest request);
    }
    public interface IHttpPost
    {
        HttpResponse Post(HttpRequest request);
    }
    public interface IHttpDelete
    {
        HttpResponse Delete(HttpRequest request);
    }
    public interface IHttpPut
    {
        HttpResponse Put(HttpRequest request);
    }
    public interface IHttpPatch
    {
        HttpResponse Patch(HttpRequest request);
    }

}
