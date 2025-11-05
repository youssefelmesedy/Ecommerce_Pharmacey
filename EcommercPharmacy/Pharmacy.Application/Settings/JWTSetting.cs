namespace Pharmacy.Application.Settings;
public record JWTSetting
{
    public string Key { get; init; } 
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public string Subject { get; init; } 
    public int AccessTokenDurationInMinutes { get; init; }
    public int RefreshTokenDurationInDays { get; init; }
}
