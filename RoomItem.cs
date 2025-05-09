namespace RockTalk
{
    public class RoomItem
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public RoomItem(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => Name;
    }
}
