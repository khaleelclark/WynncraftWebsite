namespace ImperialBackend.DTOs
{
    public class RankWithMembersDTO
    {
        public int RankId { get; set; }
        public string? RankName { get; set; }
        public List<RankMemberDTO> Members { get; set; } = new();
    }
}
