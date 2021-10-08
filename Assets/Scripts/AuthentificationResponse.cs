using System.Collections.Generic;
using Newtonsoft.Json;

public class AuthentificationResponse
{
    [JsonProperty("access_token", NullValueHandling = NullValueHandling.Ignore)]
    public string AccessToken;
    [JsonProperty("expires_in", NullValueHandling = NullValueHandling.Ignore)]
    public int ExpiresIn;
    [JsonProperty("token_type", NullValueHandling = NullValueHandling.Ignore)]
    public string TokenType;
}
