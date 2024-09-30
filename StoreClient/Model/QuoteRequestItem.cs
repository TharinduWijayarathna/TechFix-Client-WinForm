using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreClient.Model
{
    internal class QuoteRequestItem
    {
        public int Id { get; set; }
        public int QuoteRequestId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
    }
}
