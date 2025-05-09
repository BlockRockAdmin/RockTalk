#nullable enable
using System;
using System.Threading.Tasks;
using Supabase;
using Supabase.Realtime;
using Supabase.Realtime.PostgresChanges;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static RockTalk.Constants;

namespace RockTalk
{
    public sealed class SupabaseService
    {
        private Supabase.Client? _client;
        private readonly ILogger<SupabaseService> _logger;

        public SupabaseService(ILogger<SupabaseService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InitializeAsync()
        {
            if (_client != null)
            {
                _logger.LogInformation("Supabase client already initialized. Skipping.");
                return;
            }

            _logger.LogInformation("Initializing Supabase client...");
            try
            {
                var options = new SupabaseOptions { AutoConnectRealtime = true };
                _client = new Supabase.Client(SupabaseUrl, SupabaseKey, options);
                await _client.InitializeAsync().ConfigureAwait(false);

                var session = _client.Auth.CurrentSession;
                if (session != null)
                {
                    _logger.LogInformation("Session active: {UserId}", session.User?.Id);
                }
                else
                {
                    _logger.LogInformation("No active session.");
                }

                if (_client.Realtime != null)
                {
                    _logger.LogInformation("Realtime client initialized.");
                }
                else
                {
                    _logger.LogWarning("Realtime client not initialized.");
                }

                _logger.LogInformation("Supabase client initialized successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Supabase client.");
                throw;
            }
        }

        public async Task<RealtimeChannel?> GetMessagesChannel(
            string roomId,
            Action<Message> insertHandler,
            Action<Message> updateHandler,
            Action<Message> deleteHandler)
        {
            if (_client?.Realtime == null)
            {
                _logger.LogError("Realtime client is null or not initialized.");
                return null;
            }

            try
            {
                var channel = _client.Realtime.Channel("realtime:public:messages");

                // Register Postgres changes
                channel.Register(new PostgresChangesOptions("public", "messages"));
                channel.AddPostgresChangeHandler(PostgresChangesOptions.ListenType.All, (sender, change) =>
                {
                    try
                    {
                        _logger.LogDebug("Realtime change received: {Change}", JsonConvert.SerializeObject(change));
                        var model = change.Model<Message>();
                        if (model == null || model.RoomId != roomId) return;

                        switch (change.Event)
                        {
                            case Supabase.Realtime.Constants.EventType.Insert:
                                _logger.LogDebug("Message inserted: {Message}", JsonConvert.SerializeObject(model));
                                insertHandler(model);
                                break;
                            case Supabase.Realtime.Constants.EventType.Update:
                                _logger.LogDebug("Message updated: {Message}", JsonConvert.SerializeObject(model));
                                updateHandler(model);
                                break;
                            case Supabase.Realtime.Constants.EventType.Delete:
                                _logger.LogDebug("Message deleted: {Message}", JsonConvert.SerializeObject(model));
                                deleteHandler(model);
                                break;
                            default:
                                _logger.LogWarning("Unknown event type: {EventType}", change.Event);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing realtime change.");
                    }
                });

                // TODO: Broadcast handling - Replace with correct API
                // Check IntelliSense for channel.AddBroadcastHandler, channel.On, etc.
                /*
                channel.AddBroadcastHandler("chat_broadcast", (sender, payload) =>
                {
                    try
                    {
                        _logger.LogDebug("Broadcast received: {Payload}", JsonConvert.SerializeObject(payload));
                        var jObject = payload as JObject;
                        if (jObject == null) return;

                        var userId = jObject["user_id"]?.ToString();
                        var receivedRoomId = jObject["room_id"]?.ToString();
                        var content = jObject["content"]?.ToString();
                        var createdAtStr = jObject["created_at"]?.ToString();

                        if (receivedRoomId != roomId)
                        {
                            _logger.LogDebug("Broadcast ignored for room {RoomId}: {Payload}", roomId, JsonConvert.SerializeObject(payload));
                            return;
                        }

                        if (DateTime.TryParse(createdAtStr, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out var createdAt))
                        {
                            var broadcastMsg = new Message
                            {
                                Id = -1,
                                UserId = userId,
                                RoomId = roomId,
                                Content = $"[Broadcast] {content}",
                                CreatedAt = createdAt,
                                Extension = "broadcast"
                            };
                            insertHandler(broadcastMsg);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing broadcast.");
                    }
                });
                */

                // TODO: Presence handling - Replace with correct API
                // Check IntelliSense for channel.AddPresenceHandler, channel.On, etc.
                /*
                channel.AddPresenceHandler((sender, diff) =>
                {
                    try
                    {
                        _logger.LogDebug("Presence update received: {Diff}", JsonConvert.SerializeObject(diff));
                        var jObject = diff as JObject;
                        if (jObject == null) return;

                        var joins = jObject["joins"] as JArray;
                        if (joins == null) return;

                        foreach (var join in joins)
                        {
                            var userId = join["user_id"]?.ToString();
                            var email = join["email"]?.ToString();
                            var onlineAtStr = join["online_at"]?.ToString();

                            if (DateTime.TryParse(onlineAtStr, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out var onlineAt))
                            {
                                var presenceMsg = new Message
                                {
                                    Id = -1,
                                    UserId = userId,
                                    RoomId = roomId,
                                    Content = $"[Presence] User {email ?? userId} is online",
                                    CreatedAt = onlineAt,
                                    Extension = "presence"
                                };
                                insertHandler(presenceMsg);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing presence update.");
                    }
                });
                */

                await channel.Subscribe().ConfigureAwait(false);
                _logger.LogInformation("Realtime channel subscribed successfully for room {RoomId}.", roomId);
                return channel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create or configure realtime channel for room {RoomId}.", roomId);
                return null;
            }
        }
    }
}
