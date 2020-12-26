namespace FileReceiverBot.Common.Models
{
    internal class ReceivedFileInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }

        public override string ToString()
        {
            return $"FileId: {Id}\n\t" +
                $"FIle Name: {Name}\n\t" +
                $"File Label: {Label}";
        }
    }
}
