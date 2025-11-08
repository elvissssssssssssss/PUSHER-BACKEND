


using System.Collections.Generic;

namespace apitextil.DTOs.Orders
{
    public class CreatePreferenceRequestDto
    {
        public List<PreferenceItemDto> Items { get; set; }
    }

    public class PreferenceItemDto
    {
        public string Title { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
