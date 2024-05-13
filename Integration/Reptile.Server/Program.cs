var builder = WebApplication.CreateBuilder(args);

// Add services to the DI container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMasaBlazor();

// Add CORS services and configure a very permissive policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("OpenCorsPolicy", builder => builder
        .AllowAnyOrigin()   // Allow requests from any origin
        .AllowAnyMethod()   // Allow any HTTP method
        .AllowAnyHeader()
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .SetIsOriginAllowed(s=>true)
        .WithExposedHeaders("*")
    
    
    ); // Allow any header
});
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();



// Apply the CORS policy globally
app.UseCors("OpenCorsPolicy");

// Map Blazor Hub and fallback page
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Run the application
app.Run();