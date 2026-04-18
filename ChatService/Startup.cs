using System;
using System.Collections.Generic;
using ChatService.Hubs;
using ChatService.Messaging.RabbitMq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Steeltoe.Discovery.Client;
using PolicyService.Api.Events;

namespace ChatService;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var appSettingsSection = Configuration.GetSection("AppSettings");
        services.Configure<AppSettings>(appSettingsSection);

        services.AddMediatR(opts => opts.RegisterServicesFromAssemblyContaining<Startup>());

        services.AddCors(opt => opt.AddPolicy("CorsPolicy",
            builder =>
            {
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .AllowAnyOrigin(); // 🔥 FIX CORS cho nhanh
            }));

        services.AddMvc()
            .AddNewtonsoftJson();

        // ❌ TẮT AUTH để tránh lỗi 401
        // services.AddAuthentication(...)
        // services.AddAuthorization()

        services.AddSignalR();

        services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

        services.AddRabbitListeners();

        // ✅ Eureka vẫn giữ
        services.AddDiscoveryClient(Configuration);

        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        // ✅ đăng ký Eureka
        app.UseDiscoveryClient();

        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
            app.UseHsts();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("CorsPolicy");

        // ❌ TẮT AUTH
        // app.UseAuthentication();
        // app.UseAuthorization();

        app.UseHttpsRedirection();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<AgentChatHub>("/agentsChat");
        });

        app.UseRabbitListeners(new List<Type> { typeof(PolicyCreated) });
    }
}