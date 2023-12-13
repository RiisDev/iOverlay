﻿using System.Text.Json.Serialization;

namespace iOverlay.Logic.ValorantApi.DataTypes
{
    public record Entitlement(
        [property: JsonPropertyName("accessToken")] string AccessToken,
        [property: JsonPropertyName("entitlements")] IReadOnlyList<object> Entitlements,
        [property: JsonPropertyName("issuer")] string Issuer,
        [property: JsonPropertyName("subject")] string Subject,
        [property: JsonPropertyName("token")] string Token
    );
}
