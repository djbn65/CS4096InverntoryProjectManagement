namespace InventoryAndProjectManagement
{
    internal class Part
    {
        public string Number { get; set; }
        public string Descr { get; set; }
        public int Quantity { get; set; }

        public Part(string aNumber, string aDescr, int aQty)
        {
            Number = aNumber;
            Descr = aDescr;
            Quantity = aQty;
        }
    }
}