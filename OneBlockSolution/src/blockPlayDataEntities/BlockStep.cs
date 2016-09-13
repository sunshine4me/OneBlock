using System;
using System.Collections.Generic;

namespace blockPlayDataEntities
{
    public partial class BlockStep {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public string Body { get; set; }
        public string Attrs { get; set; }
        public int UserId { get; set; }
        
        public virtual PlayUser User { get; set; }
    }
}
