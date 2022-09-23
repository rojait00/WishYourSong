using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Radzen;
using WishYourSong.Data;
using Amazon;
using SpotifyAPI.Web;
using WishYourSong;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();


var secrets = new Secrets(builder.Configuration);

// AWS
var awsCredentials = new BasicAWSCredentials(secrets.AwsUserId, secrets.AwsUserPassword);
var awsConfig = new AmazonDynamoDBConfig()
{
    RegionEndpoint = RegionEndpoint.EUCentral1
};
var awsClient = new AmazonDynamoDBClient(awsCredentials, awsConfig);
builder.Services.AddSingleton<IAmazonDynamoDB>(awsClient);
builder.Services.AddSingleton<IDynamoDBContext, DynamoDBContext>();

// Spotify 
var spotifyClientConfig = SpotifyClientConfig.CreateDefault().WithAuthenticator(new ClientCredentialsAuthenticator(secrets.SpotifyUserId, secrets.SpotifyUserPassword));
var spotifyClient = new SpotifyClient(spotifyClientConfig);
builder.Services.AddSingleton<ISpotifyClient>(spotifyClient);

// WishYourSong
builder.Services.AddSingleton<SongDatabase>();
builder.Services.AddSingleton<Votes>();
builder.Services.AddScoped<User>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
