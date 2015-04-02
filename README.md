# JSend.WebApi & JSend.Client

`JSendApiController` extends [ASP.NET Web API 2][0]'s [`ApiController`][2] and enables easy generation of [JSend-formatted responses][1].

On the other hand, `JSendClient` wraps around `HttpClient` and provides an easy way to send HTTP requests and parse JSend-formatted responses.

 * [Example](#example)
 * [JSend.WebApi](#jsendwebapi)
   * [Return types](#return-types)
     * [Void actions](#void-actions)
     * [`IHttpActionResult`](#ihttpactionresult)
     * [Other return types](#other-return-types)
   * [Exceptions](#exceptions)
   * [Other stuff](#other-stuff)
 * [JSend.Client](#jsendclient)
   * [Querying an API](#querying-an-api)
   * [Handling the response](#handling-the-response)
   * [Configuring the client](#configuring-the-client)
 * [Download](#download)

## Example

JSend.WebApi

```csharp
public class ArticlesController : JSendApiController
{
    public IHttpActionResult Get(int id)
    {
        var article = _repo.Get(id);

        if (article != null)
            return JSendOk(article);

        return JSendNotFound();
    }
}
```

JSend.Client

```csharp
using (var client = new JSendClient())
{
    var getResponse = await client.GetAsync<Article>("http://localhost/articles/4");
    var existingArticle = getResponse.GetDataOrDefault();

    var postResponse = await client.PostAsync<Article>("http://localhost/articles/", article);
    var newArticle = postResponse.Data;

    var deleteResponse = await client.DeleteAsync("http://localhost/articles/4");
    deleteResponse.EnsureSuccessStatus(); //throws if the response's status is not "success"

    var putResponse = await client.PutAsync("http://localhost/articles/4", existingArticle);
    if (! putResponse.IsSuccess)
        Logger.Log(putResponse.Error);
}
```

## JSend.WebApi

### Return types

The return value of a `JSendApiController` action is converted to a HTTP response as follows:

#### Void actions

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

#### `IHttpActionResult`

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

#### Other return types

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

### Exceptions

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

### Other stuff

* The `JSendAuthorize` attribute is available and replaces the `Authorize` attribute

## JSend.Client

### Querying an API

```csharp
await client.GetAsync<Article>(uri);

await client.PostAsync<Article>(uri, article);   // If you expect an updated article back
await client.PostAsync(uri, article);            // If you don't expect data back

await client.PutAsync<Article>(uri, article);    // If you expect an updated article back
await client.PutAsync(uri, article);             // If you don't expect data back

await client.DeleteAsync(uri);
```

### Handling the response

If you expect the API to always return a "success response"...

```csharp
//... and you don't need any data
response.EnsureSuccessStatus();  //(throws if response was not successful)

//... and you expect the response to always contain data
var article = response.Data;     //(throws if response was not successful or did not contain data)

//... and you're not sure whether the response contains data
var article = response.GetDataOrDefault();
var article = response.GetDataOrDefault(new Article());
if (response.HasData) { ... }
```

If the API might return a "fail/error response" (e.g., because a resource was not found)...

```csharp
//... and you don't need any data
if (response.IsSuccess) { ... }

//... and you need the data
var article = response.GetDataOrDefault();
var article = response.GetDataOrDefault(new Article());
if (response.HasData) { ... }

//... and you want to handle the error
if (! response.IsSuccess) { Logger.Log(response.Error); }
```

If you want to know more details about the response, such as its status code, you can use the [`JSendResponse.HttpResponseMessage`][7] property to access the original HTTP response message.

```csharp
if (response.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
{
    ...
}
```

### Configuring the client

If you want the client to perform some additional work (e.g., add a "X-Version" header to all requests, or log all exceptions) you can do so by extending [`MessageInterceptor`][6]:

```csharp
public class MyCustomInterceptor : MessageInterceptor
{
    public override void OnSending(HttpRequestMessage request)
    {
        request.Headers.Add("X-Version", "2.0");
    }

    public override void OnException(ExceptionContext context)
    {
        Logger.Log(context.Exception);
    }
}
```

You can then configure the client like this:

```csharp
var settings = new JSendClientSettings
    {
        MessageInterceptor = new MyCustomInterceptor(),
        SerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            }
    };

var client = new JSendClient(settings);
```

## Download

To install JSend.WebApi, run the following command in the Package Manager Console

    PM> Install-Package JSend.WebApi

Or download the binaries/source code from [here][5].

The JSend.Client will soon be available on NuGet.

 [0]: http://www.asp.net/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api
 [1]: http://labs.omniti.com/labs/jsend
 [2]: https://msdn.microsoft.com/en-us/library/system.web.http.apicontroller%28v=vs.118%29.aspx
 [3]: https://github.com/dcastro/JSendWebApi/wiki#list-of-helper-methods
 [4]: https://msdn.microsoft.com/en-us/library/system.web.http.httpconfiguration.includeerrordetailpolicy%28v=vs.118%29.aspx
 [5]: https://github.com/dcastro/JSendWebApi/releases
 [6]: https://github.com/dcastro/JSendWebApi/blob/master/src/JSend.Client/MessageInterceptor.cs
 [7]: https://github.com/dcastro/JSendWebApi/blob/master/src/JSend.Client/JSendResponse.cs#L60
