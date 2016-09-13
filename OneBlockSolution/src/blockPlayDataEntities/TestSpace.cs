using System;
using System.Collections.Generic;

namespace blockPlayDataEntities
{
    public partial class TestSpace
    {
        public TestSpace()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Describe { get; set; }
        public int UserId { get; set; }
        public string SapceData { get; set; }
        
        public virtual PlayUser User { get; set; }
    }
}
