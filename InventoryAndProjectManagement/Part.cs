namespace InventoryAndProjectManagement
{
    public class Part
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Descr { get; set; }
        public int Quantity { get; set; }

        public Part(int aId, string aNumber, string aDescr, int aQty)
        {
            Id = aId;
            Number = aNumber;
            Descr = aDescr;
            Quantity = aQty;
        }
    }
}