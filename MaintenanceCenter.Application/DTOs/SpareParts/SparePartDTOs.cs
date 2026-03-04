namespace MaintenanceCenter.Application.DTOs.SpareParts
{
    public class SparePartDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal CurrentCost { get; set; }
    }

    public class CreateSparePartDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal CurrentCost { get; set; }
    }

    public class UpdateSparePartDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal CurrentCost { get; set; }
    }
}