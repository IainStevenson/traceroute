namespace NetState.Command
{
    public class Settings
    {
        public string XmlDataPath { get; set; }
        public string TextDataPath { get; set; }
        public string JsonDataPath { get; set; }
        public int MaxHops {  get;set; }
        public int Timeout {  get;set;}
        public string LocalHostsFile { get; set; }
    }
}