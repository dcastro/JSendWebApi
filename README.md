# JSendWebApi

JSendWebApi extends [ASP.NET Web API 2][0]'s [`ApiController`][2] and enables easy generation of [JSend-formatted responses]
[1].


The return value of a `JSendApIController` action is converted to a HTTP response as follows:

## Void Actions

Actions that don't return anything are converted to a 200 response with its status set to `success`.  

```csharp
public class ArticlesController : JSendApiController
{
    public void Post()
    {
    }
}
```
```
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

{
    "status" : "success",
    "data" : null
}
```

## `IHttpActionResult`

The `JSendApiController` provides several helper methods to easily build JSend-formatted responses.  
[Here's a full list of these helpers methods and examples responses][3].

A simple example:

```csharp
public class ArticlesController : JSendApiController
{
    private readonly IRepository<Article> _repo = //...;

    public IHttpActionResult Post(Article article)
    {
        if (!ModelState.IsValid)
            return JSendBadRequest(ModelState); // Returns a 400 "fail" response

        _repo.Store(article);
        return JSendCreatedAtRoute("GetArticle", new {id = article.Id}, article); // Returns a 201 "success" response
    }

    [Route("articles/{id:int}", Name = "GetArticle")]
    public IHttpActionResult Get(int id)
    {
        var article = _repo.Get(id);
        return JSendOk(article); // Returns a 200 "success" response
    }
}
```

The `Post` action above will return one of the following HTTP responses:

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
The `Get` action will return:
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

## Other return types

For all other return types (*), they'll be wrapped in a 200 response with its status set to `success` , 


```csharp
public class ArticlesController : JSendApiController
{
    public IEnumerable<Article> Get()
    {
        return GetAllArticlesFromDb();
    }
}
```
```
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

{
    "status": "success",
    "data": [
        {
            "title": "Why Are So Many Of The Framework Classes Sealed?"
        },     
        {
            "title": "C# Performance Benchmark Mistakes, Part One"
        }
    ]
}
```

(*) Except `HttpResponseMessage`, which is converted directly to an HTTP response.

## Exceptions

Depending on the current [`IncludeErrorDetailPolicy`][4] and on whether the client is local or remote, 
exceptions thrown by `JSendApiController` actions will be formatted as either:

```
HTTP/1.1 500 Internal Server Error
Content-Type: application/json; charset=utf-8

{                                                                                                                       
  "status": "error",                                                                                                    
  "message": "Operation is not valid due to the current state of the object.",                                          
  "data": "System.InvalidOperationException: Operation is not valid due to the current state of the object.
}
```

or as:

```
HTTP/1.1 500 Internal Server Error
Content-Type: application/json; charset=utf-8

{                                               
  "status": "error",                            
  "message": "An error has occurred."           
}
```

The default behavior is to show exception details to local clients and hide them from remote clients.

## Other stuff

* The `JSendAuthorize` attribute is available and replaces the `Authorize` attribute



 [0]: http://www.asp.net/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api
 [1]: http://labs.omniti.com/labs/jsend
 [2]: https://msdn.microsoft.com/en-us/library/system.web.http.apicontroller%28v=vs.118%29.aspx
 [3]: https://github.com/dcastro/JSendWebApi/wiki#list-of-helper-methods
 [4]: https://msdn.microsoft.com/en-us/library/system.web.http.httpconfiguration.includeerrordetailpolicy%28v=vs.118%29.aspx
