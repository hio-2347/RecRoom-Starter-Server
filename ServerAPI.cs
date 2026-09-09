using Microsoft.AspNetCore.Http.Extensions;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace RecRoom
{
    public class ServerAPI(WebApplication webApplication)
    {
        private readonly WebApplication webApplication = webApplication;
        private bool hasInit = false;

        public RouteHandlerBuilder GET(string endpoint, Delegate action) => webApplication.MapGet(endpoint, action);

        public RouteHandlerBuilder POST(string endpoint, Delegate action) => webApplication.MapPost(endpoint, action);

        public RouteHandlerBuilder PUT(string endpoint, Delegate action) => webApplication.MapPut(endpoint, action);

        public RouteHandlerBuilder MAP(string endpoint, Delegate action) => webApplication.Map(endpoint, action);

        private static readonly object ConsoleLock = new();

        public void Init()
        {
            webApplication.Use(async (context, next) =>
            {
                var request = context.Request;

                context.Response.OnStarting(() =>
                {
                    lock (ConsoleLock)
                    {
                        var color = context.Response.StatusCode switch
                        {
                            200 => ConsoleColor.Green,
                            405 or 404 or 403 => ConsoleColor.Red,
                            101 => ConsoleColor.Blue,
                            _ => ConsoleColor.White
                        };

                        Console.ForegroundColor = color;
                        Console.WriteLine($"{(context.Response.StatusCode == 101 ? "WS" : request.Method)} {request.GetEncodedPathAndQuery()} {context.Response.StatusCode}");
                        if (request.Method == "POST")
                        {

                        }
                        Console.ResetColor();
                    }

                    return Task.CompletedTask;
                });

                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    lock (ConsoleLock)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{request.Method} {request.GetEncodedPathAndQuery()} => {ex.Message}");
                        Console.ResetColor();
                    }
                    throw;
                }
            });


            var assembly = Assembly.GetExecutingAssembly();
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                DictionaryKeyPolicy = null,
                WriteIndented = true
            };

            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass) continue;

                object? instance = null;
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

                foreach (var method in methods)
                {
                    if (method.GetCustomAttribute<GETAttribute>() is { } getAttr)
                    {
                        instance ??= method.IsStatic ? null : Activator.CreateInstance(type);

                        var del = CreateCompatibleDelegate(method, instance, jsonOptions);

                        Console.WriteLine($"{type.FullName}.{method.Name}  =>  GET {getAttr.Path}");
                        GET(getAttr.Path, del);
                    }

                    if (method.GetCustomAttribute<POSTAttribute>() is { } postAttr)
                    {
                        instance ??= method.IsStatic ? null : Activator.CreateInstance(type);

                        var del = CreateCompatibleDelegate(method, instance, jsonOptions);

                        Console.WriteLine($"{type.FullName}.{method.Name}  =>  POST {postAttr.Path}");
                        POST(postAttr.Path, del);
                    }

                    if (method.GetCustomAttribute<PUTAttribute>() is { } putAttr)
                    {
                        instance ??= method.IsStatic ? null : Activator.CreateInstance(type);

                        var del = CreateCompatibleDelegate(method, instance, jsonOptions);

                        Console.WriteLine($"{type.FullName}.{method.Name}  =>  POST {putAttr.Path}");
                        PUT(putAttr.Path, del);
                    }

                    if (method.GetCustomAttribute<MAPAttribute>() is { } mapAttr)
                    {
                        instance ??= method.IsStatic ? null : Activator.CreateInstance(type);

                        var del = CreateCompatibleDelegate(method, instance, jsonOptions);

                        Console.WriteLine($"{type.FullName}.{method.Name}  =>  POST {mapAttr.Path}");
                        MAP(mapAttr.Path, del);
                    }
                }
            }

            //webApplication.UseHttpsRedirection();

            webApplication.UseWebSockets(); // we use a custom SignalR because recrooms is so old its out of date...

            hasInit = true;
        }

        public void Run(string? url = null)
        {
            if (!hasInit)
                throw new InvalidOperationException("Call ServerAPI.Init() before Run().");

            webApplication.Run(url);
        }

        private static Delegate CreateCompatibleDelegate(MethodInfo method, object? instance, JsonSerializerOptions? options = null)
        {
            options ??= new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                DictionaryKeyPolicy = null,
                WriteIndented = true
            };

            var parameters = method.GetParameters();
            bool requiresAuth = method.GetCustomAttribute<UseAuthorizationAttribute>() != null;

            bool isTask = typeof(Task).IsAssignableFrom(method.ReturnType);
            Type? taskResultType = null;

            if (isTask && method.ReturnType.IsGenericType)
            {
                taskResultType = method.ReturnType.GetGenericArguments()[0];
            }

            return new Func<HttpContext, Task<IResult>>(async ctx =>
            {
                IDictionary<string, object>? info = null;
                MongoDB.User? user = null;

                if (requiresAuth)
                {
                    var authHeader = ctx.Request.Headers.Authorization;

                    info = JsonWebToken.VerifyAndDecode(authHeader);
                    if (info == null)
                        return Results.Unauthorized();

                    long sub = long.Parse((string)info["sub"]);

                    user = MongoDB.usersCollection
                        .Find(u => u.AccountId == sub)
                        .FirstOrDefault();

                    if (user == null)
                        return Results.Unauthorized();
                }

                object?[] args = [.. parameters.Select(p =>
                {
                    if (p.ParameterType == typeof(HttpContext))
                        return ctx;

                    if (requiresAuth && p.ParameterType == typeof(MongoDB.User))
                        return user;

                    if (ctx.Request.RouteValues.TryGetValue(p.Name, out var value))
                    {
                        if (value == null) return null;

                        if (p.ParameterType.IsEnum)
                            return Enum.Parse(p.ParameterType, value.ToString()!);

                        return Convert.ChangeType(value, p.ParameterType);
                    }

                    return null;
                })];

                object? result = method.Invoke(instance, args);

                if (isTask)
                {
                    if (result is Task task)
                    {
                        await task.ConfigureAwait(false);

                        if (taskResultType != null)
                        {
                            var resultProperty = task.GetType().GetProperty("Result");
                            result = resultProperty?.GetValue(task);
                        }
                        else
                        {
                            result = null;
                        }
                    }
                }

                if (result is IResult iResult)
                    return iResult;

                return Results.Json(result, options);
            });
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class GETAttribute(string path) : Attribute
        {
            public string Path { get; } = path;
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class MAPAttribute(string path) : Attribute
        {
            public string Path { get; } = path;
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class POSTAttribute(string path) : Attribute
        {
            public string Path { get; } = path;
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class PUTAttribute(string path) : Attribute
        {
            public string Path { get; } = path;
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class UseAuthorizationAttribute() : Attribute
        {
        }
    }
}
