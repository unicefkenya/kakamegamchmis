using System;

namespace MCHMIS.Models
{
    public class MpesaAuthorizationCode
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime UtcDateTime { get; set; }
    }
}