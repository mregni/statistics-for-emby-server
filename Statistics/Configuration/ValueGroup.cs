namespace Statistics.Configuration
{
    public class ValueGroup
    {
        public string Title { get; set; }

        public string ValueLineOne { get; set; }
        public string ValueLineTwo { get; set; }
        public string Size { get; set; }
        public object Raw { get; set; }
        public string ExtraInformation { get; set; }
        public string Id { get; set; }

        public ValueGroup()
        {
            Size = "small";
        }
    }
}