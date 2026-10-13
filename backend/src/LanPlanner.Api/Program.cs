using LanPlanner.Api.Authentication;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpContextCurrentUser>();
builder.Services.AddAccessJwtAuthentication(builder.Configuration);

LanPlanner.Application.Installer.DependencyInjection.RegisterApplicationServices(builder.Services, builder.Environment);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("AccessJwt", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = AuthenticationExtensions.AccessJwtHeader,
        Description = "JWT, wie es Cloudflare Access im Header mitschickt (lokal: Test-Token)."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("AccessJwt", document)] = []
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    //(c =>
    //{
    //    c.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    //});
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();