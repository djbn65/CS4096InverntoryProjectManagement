using System.Linq;

namespace InventoryAndProjectManagement
{
    internal class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Step { get; set; }
        public bool IsComplete { get; set; }
        public Machine MachineData { get; set; }

        public Project(int aId, string aName, string aDescription, int aStep, bool aIsComplete, Machine aMachineData = null)
        {
            Id = aId;
            Name = aName;
            Description = aDescription;
            Step = aStep;
            IsComplete = aIsComplete;

            if (aMachineData != null)
            {
                MachineData = new Machine(aMachineData.Id, aMachineData.Name, aMachineData.Description, new System.Collections.Generic.List<Part>());

                foreach (Part part in aMachineData.PartList)
                {
                    MachineData.PartList.Add(new Part(part.Id, part.Number, part.Description, part.Quantity));
                }
            }
        }
    }
}