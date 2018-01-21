namespace smarthome.DataService.Models
{
    public class Light
    {
        public string lightId { get; set; }
        public string name { get; set; }
        public Code codes { get; set; }
    }

    public class Code
    {
        public int on { get; set; }
        public int off { get; set; }
    }
}