/// <summary>
/// HTTP service for serving patch manifest, news, and file verification data to the launcher.
/// </summary>
/// <remarks>
/// Runs on port 8080 alongside the game server.
/// Endpoints:
/// - GET /patch/manifest.json - Patch version info
/// - GET /news - Server news/announcements (from MongoDB)
/// - GET /patch/files.json - File verification data
/// </remarks>
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_server.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace kakia_lime_odyssey_server.Services.Patch;

/// <summary>
/// HTTP server for launcher patch and news services.
/// </summary>
public class PatchHttpService
{
    private readonly HttpListener _listener;
    private readonly int _port;
    private CancellationTokenSource? _cts;
    private Task? _listenerTask;
    private readonly IMongoCollection<NewsDocument>? _newsCollection;

    /// <summary>
    /// Current patch version.
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Creates a new patch HTTP service.
    /// </summary>
    /// <param name="port">Port to listen on (default 8080).</param>
    public PatchHttpService(int port = 8080)
    {
        _port = port;
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://127.0.0.1:{_port}/");
        _listener.Prefixes.Add($"http://localhost:{_port}/");

        // Get news collection from database
        try
        {
            var db = DatabaseFactory.Instance;
            if (db is MongoDBService mongoDb)
            {
                _newsCollection = mongoDb.GetCollection<NewsDocument>("news");
                EnsureDefaultNews();
            }
        }
        catch (Exception ex)
        {
            Logger.Log($"[PATCH] Could not connect to database for news: {ex.Message}", LogLevel.Warning);
        }
    }

    /// <summary>
    /// Ensures default news exists in database.
    /// </summary>
    private void EnsureDefaultNews()
    {
        if (_newsCollection == null) return;

        var count = _newsCollection.CountDocuments(FilterDefinition<NewsDocument>.Empty);
        if (count == 0)
        {
            var defaultNews = new List<NewsDocument>
            {
                new NewsDocument
                {
                    Title = "Welcome to Lime Odyssey",
                    Content = "The server is now online! Create your character and begin your adventure in this beautiful fantasy world.",
                    Type = "announcement",
                    ImageUrl = "",
                    Links = new List<NewsLink>
                    {
                        new NewsLink { Text = "Getting Started Guide", Url = "#guide" },
                        new NewsLink { Text = "Join Discord", Url = "https://discord.gg/limeodyssey" }
                    },
                    Priority = 100,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = null
                },
                new NewsDocument
                {
                    Title = "Server Features",
                    Content = "Current features include:\n- Character creation with multiple races and classes\n- Combat and skill system\n- Questing with hunt and collect objectives\n- Party and guild systems\n- Trading and crafting\n- Mount system",
                    Type = "info",
                    ImageUrl = "",
                    Links = new List<NewsLink>(),
                    Priority = 50,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = null
                }
            };

            _newsCollection.InsertMany(defaultNews);
            Logger.Log($"[PATCH] Inserted {defaultNews.Count} default news items", LogLevel.Information);
        }
    }

    /// <summary>
    /// Starts the HTTP listener.
    /// </summary>
    public void Start()
    {
        try
        {
            _cts = new CancellationTokenSource();
            _listener.Start();
            _listenerTask = Task.Run(() => ListenLoop(_cts.Token));
            Logger.Log($"[PATCH] HTTP service running on http://127.0.0.1:{_port}", LogLevel.Information);
        }
        catch (HttpListenerException ex)
        {
            Logger.Log($"[PATCH] Failed to start HTTP service: {ex.Message}", LogLevel.Warning);
            Logger.Log($"[PATCH] Launcher patch/news features will be unavailable", LogLevel.Warning);
        }
    }

    /// <summary>
    /// Stops the HTTP listener.
    /// </summary>
    public void Stop()
    {
        _cts?.Cancel();
        _listener.Stop();
        _listenerTask?.Wait(1000);
    }

    /// <summary>
    /// Adds a news item to the database.
    /// </summary>
    public void AddNews(string title, string content, string type = "announcement", string? imageUrl = null, List<NewsLink>? links = null, int priority = 0, DateTime? expiresAt = null)
    {
        if (_newsCollection == null) return;

        var news = new NewsDocument
        {
            Title = title,
            Content = content,
            Type = type,
            ImageUrl = imageUrl ?? "",
            Links = links ?? new List<NewsLink>(),
            Priority = priority,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt
        };

        _newsCollection.InsertOne(news);
        Logger.Log($"[PATCH] Added news: {title}", LogLevel.Information);
    }

    /// <summary>
    /// Gets all active news from database.
    /// </summary>
    public List<NewsDocument> GetActiveNews()
    {
        if (_newsCollection == null) return GetDefaultNews();

        try
        {
            var filter = Builders<NewsDocument>.Filter.And(
                Builders<NewsDocument>.Filter.Eq(n => n.IsActive, true),
                Builders<NewsDocument>.Filter.Or(
                    Builders<NewsDocument>.Filter.Eq(n => n.ExpiresAt, null),
                    Builders<NewsDocument>.Filter.Gt(n => n.ExpiresAt, DateTime.UtcNow)
                )
            );

            return _newsCollection
                .Find(filter)
                .SortByDescending(n => n.Priority)
                .ThenByDescending(n => n.CreatedAt)
                .ToList();
        }
        catch (Exception ex)
        {
            Logger.Log($"[PATCH] Error fetching news: {ex.Message}", LogLevel.Warning);
            return GetDefaultNews();
        }
    }

    private List<NewsDocument> GetDefaultNews()
    {
        return new List<NewsDocument>
        {
            new NewsDocument
            {
                Id = ObjectId.GenerateNewId(),
                Title = "Server Online",
                Content = "Welcome to Lime Odyssey! The server is now running.",
                Type = "announcement",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        };
    }

    private async Task ListenLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested && _listener.IsListening)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                _ = Task.Run(() => HandleRequest(context), ct);
            }
            catch (HttpListenerException) when (ct.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                Logger.Log($"[PATCH] HTTP error: {ex.Message}", LogLevel.Debug);
            }
        }
    }

    private void HandleRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        try
        {
            response.ContentType = "application/json";
            response.AddHeader("Access-Control-Allow-Origin", "*");

            string responseBody;
            var path = request.Url?.AbsolutePath ?? "/";

            switch (path)
            {
                case "/patch/manifest.json":
                    responseBody = GetManifestJson();
                    break;
                case "/news":
                    responseBody = GetNewsJson();
                    break;
                case "/patch/files.json":
                    responseBody = GetFilesJson();
                    break;
                default:
                    response.StatusCode = 404;
                    responseBody = JsonSerializer.Serialize(new { error = "Not found" });
                    break;
            }

            var buffer = Encoding.UTF8.GetBytes(responseBody);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);

            Logger.Log($"[PATCH] {request.HttpMethod} {path} -> {response.StatusCode}", LogLevel.Debug);
        }
        catch (Exception ex)
        {
            Logger.Log($"[PATCH] Request error: {ex.Message}", LogLevel.Debug);
        }
        finally
        {
            response.Close();
        }
    }

    private string GetManifestJson()
    {
        var manifest = new
        {
            version = Version,
            patches = Array.Empty<object>()
        };
        return JsonSerializer.Serialize(manifest);
    }

    private string GetNewsJson()
    {
        var news = GetActiveNews();
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return JsonSerializer.Serialize(news.Select(n => new
        {
            id = n.Id.ToString(),
            title = n.Title,
            content = n.Content,
            type = n.Type,
            date = n.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
            imageUrl = n.ImageUrl,
            links = n.Links.Select(l => new { text = l.Text, url = l.Url }).ToList(),
            priority = n.Priority
        }), options);
    }

    private string GetFilesJson()
    {
        return JsonSerializer.Serialize(new { files = Array.Empty<object>() });
    }
}

/// <summary>
/// News document stored in MongoDB.
/// </summary>
public class NewsDocument
{
    /// <summary>MongoDB document ID.</summary>
    [BsonId]
    public ObjectId Id { get; set; }

    /// <summary>News title.</summary>
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>News content (supports markdown/newlines).</summary>
    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>News type: announcement, update, maintenance, event, info.</summary>
    [BsonElement("type")]
    public string Type { get; set; } = "announcement";

    /// <summary>Optional image URL for the news item.</summary>
    [BsonElement("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>Clickable links associated with this news.</summary>
    [BsonElement("links")]
    public List<NewsLink> Links { get; set; } = new();

    /// <summary>Display priority (higher = shown first).</summary>
    [BsonElement("priority")]
    public int Priority { get; set; }

    /// <summary>Whether the news is active/visible.</summary>
    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    /// <summary>When the news was created.</summary>
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Optional expiration date.</summary>
    [BsonElement("expiresAt")]
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// Clickable link in a news item.
/// </summary>
public class NewsLink
{
    /// <summary>Display text for the link.</summary>
    [BsonElement("text")]
    public string Text { get; set; } = string.Empty;

    /// <summary>URL the link points to.</summary>
    [BsonElement("url")]
    public string Url { get; set; } = string.Empty;
}
