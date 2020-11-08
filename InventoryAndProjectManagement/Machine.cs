namespace InventoryAndProjectManagement
{
    internal class Machine
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Machine(string aName, string aDescr)
        {
            Name = aName;
            Description = aDescr;
        }
    }
}