using Newtonsoft.Json;

public class CharacterMove
{
    public Character Model;
    public Move Move;
    [JsonProperty("actual_value")]
    public int ActualValue;
    public string Strength;
}