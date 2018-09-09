using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;

namespace HelloGraphQL
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                // await context.Response.WriteAsync("Hello World!");

                // Basic
                /*
                var schema = new Schema {
                    Query = new HelloWorldQuery()
                };

                var result = await new DocumentExecuter().ExecuteAsync(doc =>
                {
                    doc.Schema = schema;
                    doc.Query = @"
                        query {
                            hello
                            howdy
                        }
                    ";
                }).ConfigureAwait(false);

                var json = new DocumentWriter(indent: true).Write(result);
                Console.WriteLine(json);
                await context.Response.WriteAsync(json);
                */

                // Match route and method of GraphQL request
                var req = context.Request;
                if (req.Path.StartsWithSegments("/api/graphql") &&
                    string.Equals(req.Method, "POST", StringComparison.OrdinalIgnoreCase))
                {
                    string body;
                    using (var streamReader = new StreamReader(req.Body))
                    {
                        body = await streamReader.ReadToEndAsync();

                        var request = JsonConvert.DeserializeObject<GraphQLRequest>(body);
                        var schema = new Schema { Query = new HelloWorldQuery() };

                        var result = await new DocumentExecuter().ExecuteAsync(doc =>
                        {
                            doc.Schema = schema;
                            doc.Query = request.Query;
                        }).ConfigureAwait(false);

                        var json = new DocumentWriter(indent: true).Write(result);
                        await context.Response.WriteAsync(json);
                    }
                }
                else
                {
                    await context.Response.WriteAsync("Hello World!");
                }

            });
        }
    }
}
