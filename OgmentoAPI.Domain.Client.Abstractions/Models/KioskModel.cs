
namespace OgmentoAPI.Domain.Client.Abstractions.Models
{
    public class KioskModel
    {
        public int ID { get; set; }

        public string KioskName { get; set; }

        public Tuple<Guid, string> SalesCenter { get; set; }

        public int SalesCenterId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }


    }
}
