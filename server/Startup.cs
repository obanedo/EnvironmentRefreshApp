﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EnvironmentRefresh.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebEssentials.AspNetCore.Pwa;



namespace EnvironmentRefresh {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddMvc ();
            services.AddProgressiveWebApp();
            services.AddDbContextPool<EnvironmentRefreshContext>(opt => opt.UseInMemoryDatabase("BeltServiceQuotes"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }

            app.Use (async (context, next) => {
                await next ();
                if (context.Response.StatusCode == 404 &&
                    !Path.HasExtension (context.Request.Path.Value) &&
                    !context.Request.Path.Value.StartsWith ("/api/")) {
                    context.Request.Path = "/index.html";
                    await next ();
                }
            });

            app.UseMvc ();
            app.UseDefaultFiles ();
            app.UseStaticFiles ();

            app.UseEnvironmentRefreshSampleData();
        }
    }
}
