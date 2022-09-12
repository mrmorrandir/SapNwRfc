namespace SapNwRfc.Internal
{
    internal class StringTableElement
    {
        [SapName("")]
        public string Data { get; set; }

        public override string ToString()
        {
            return Data;
        }

        public static implicit operator string(StringTableElement element) => element.Data;
    }
}
