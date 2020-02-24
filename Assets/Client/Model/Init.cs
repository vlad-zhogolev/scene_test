namespace Client.Model
{
    public class Init
    {
        public Scene scene { get; set; }
        public Object[] objects { get; set; }
        public string roomId { get; set; }
        public int maxPlayers { get; set; }
    }
}
