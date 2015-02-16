# Work in progress

# JSendWebApi

JSendWebApi extends [ASP.NET Web API 2][0]'s [`ApiController`][2] and provides several helper methods to easily build [JSend-formatted responses]
[1].
These methods are similar to `ApiController`'s `Ok()`, `BadRequest()`, etc.

Example:

```csharp
public class ArticlesController : JSendApiController
{
    private readonly IRepository<Article> _repo = //...;

    public IHttpActionResult Post(Article article)
    {
        if (!ModelState.IsValid)
            return JSendBadRequest(ModelState);

        _repo.Store(article);
        return JSendCreatedAtRoute("GetArticle", new {id = article.Id}, article);
    }

    [Route("articles/{id:int}", Name = "GetArticle")]
    public IHttpActionResult Get(int id)
    {
        var article = _repo.Get(id);
        return JSendOk(article);
    }
}
```

The actions above will return one of the following HTTP responses:

```
HTTP/1.1 201 Created
Content-Type: application/json; charset=utf-8
Location: http://localhost/articles/5

{
    "status" : "success",
    "data" : {
        "title" : "Ground-breaking study discovers how to exit Vim"
    }
}
```
```
HTTP/1.1 400 Bad Request
Content-Type: application/json; charset=utf-8

{
    "status" : "fail",
    "data" : { 
        "article.Title" : "The Title field is required."
    }
}
```
```
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

{
    "status" : "success",
    "data" : {
        "title" : "Ground-breaking study discovers how to exit Vim"
    }
}
```

 [0]: http://www.asp.net/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api
 [1]: http://labs.omniti.com/labs/jsend
 [2]: https://msdn.microsoft.com/en-us/library/system.web.http.apicontroller%28v=vs.118%29.aspx
