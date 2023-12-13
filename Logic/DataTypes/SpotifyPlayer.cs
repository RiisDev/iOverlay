using System.Text.Json.Serialization;
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

namespace iOverlay.Logic.DataTypes
{
    public record Actions(
        [property: JsonPropertyName("disallows")] Disallows Disallows
    );

    public record Album(
        [property: JsonPropertyName("album_type")] string AlbumType,
        [property: JsonPropertyName("artists")] IReadOnlyList<Artist> Artists,
        [property: JsonPropertyName("available_markets")] IReadOnlyList<string> AvailableMarkets,
        [property: JsonPropertyName("external_urls")] ExternalUrls ExternalUrls,
        [property: JsonPropertyName("href")] string Href,
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("images")] IReadOnlyList<Image> Images,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("release_date")] string ReleaseDate,
        [property: JsonPropertyName("release_date_precision")] string ReleaseDatePrecision,
        [property: JsonPropertyName("total_tracks")] int? TotalTracks,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("uri")] string Uri
    );

    public record Artist(
        [property: JsonPropertyName("external_urls")] ExternalUrls ExternalUrls,
        [property: JsonPropertyName("href")] string Href,
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("uri")] string Uri
    );

    public record Context(
        [property: JsonPropertyName("external_urls")] ExternalUrls ExternalUrls,
        [property: JsonPropertyName("href")] string Href,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("uri")] string Uri
    );

    public record Device(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("is_active")] bool? IsActive,
        [property: JsonPropertyName("is_private_session")] bool? IsPrivateSession,
        [property: JsonPropertyName("is_restricted")] bool? IsRestricted,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("supports_volume")] bool? SupportsVolume,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("volume_percent")] int? VolumePercent
    );

    public record Disallows(
        [property: JsonPropertyName("resuming")] bool? Resuming
    );

    public record ExternalIds(
        [property: JsonPropertyName("isrc")] string Isrc
    );

    public record ExternalUrls(
        [property: JsonPropertyName("spotify")] string Spotify
    );

    public record Image(
        [property: JsonPropertyName("height")] int? Height,
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("width")] int? Width
    );

    public record Track(
        [property: JsonPropertyName("album")] Album Album,
        [property: JsonPropertyName("artists")] IReadOnlyList<Artist> Artists,
        [property: JsonPropertyName("available_markets")] IReadOnlyList<string> AvailableMarkets,
        [property: JsonPropertyName("disc_number")] int? DiscNumber,
        [property: JsonPropertyName("duration_ms")] double? DurationMs,
        [property: JsonPropertyName("explicit")] bool? Explicit,
        [property: JsonPropertyName("external_ids")] ExternalIds ExternalIds,
        [property: JsonPropertyName("external_urls")] ExternalUrls ExternalUrls,
        [property: JsonPropertyName("href")] string Href,
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("is_local")] bool? IsLocal,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("popularity")] int? Popularity,
        [property: JsonPropertyName("preview_url")] string PreviewUrl,
        [property: JsonPropertyName("track_number")] int? TrackNumber,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("uri")] string Uri
    );

    public record SpotifyPlayer(
        [property: JsonPropertyName("device")] Device Device,
        [property: JsonPropertyName("shuffle_state")] bool? ShuffleState,
        [property: JsonPropertyName("repeat_state")] string RepeatState,
        [property: JsonPropertyName("timestamp")] long? Timestamp,
        [property: JsonPropertyName("context")] Context Context,
        [property: JsonPropertyName("progress_ms")] double? ProgressMs,
        [property: JsonPropertyName("item")] Track Track,
        [property: JsonPropertyName("currently_playing_type")] string CurrentlyPlayingType,
        [property: JsonPropertyName("actions")] Actions Actions,
        [property: JsonPropertyName("is_playing")] bool? IsPlaying
    );


}
