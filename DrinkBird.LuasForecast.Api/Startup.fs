namespace DrinkBird.LuasForecast.Api

open System.Net.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open DrinkBird.LuasForecast.Api.Controllers

type Startup private () =
    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection) =
        // Add framework services.
        services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2) |> ignore

        services.AddHttpClient("Luas") |> ignore

        services.AddScoped<ILuasForecastService>(fun serviceProvider ->
            let httpClient = serviceProvider.GetService<IHttpClientFactory>().CreateClient("Luas")
            let getForecastFor = LuasForecastService.getForecast httpClient
            { new ILuasForecastService with member __.GetForecastFor stopCode = getForecastFor stopCode }
        ) |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IHostingEnvironment) =
        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore

        app.UseMvc() |> ignore

    member val Configuration : IConfiguration = null with get, set
