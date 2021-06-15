using System;

namespace MyStore.Business.Data.Entities
{
    public class OrderHistory
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Items { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? PrintDate { get; set; }
        public DateTime ArchiveDate { get; set; }
    }
}