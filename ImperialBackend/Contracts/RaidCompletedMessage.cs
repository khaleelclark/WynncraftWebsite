namespace ImperialBackend.Contracts;

public sealed class RaidCompletedMessage
{
    public int RaidId { get; set; }
    public DateTimeOffset CompletedDate { get; set; }
    public List<string> MinecraftUsernames { get; set; } = new();
}
