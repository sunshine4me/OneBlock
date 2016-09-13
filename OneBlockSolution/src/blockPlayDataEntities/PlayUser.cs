using System;
using System.Collections.Generic;

namespace blockPlayDataEntities
{
    public partial class PlayUser
    {
        public PlayUser()
        {
            TestCase = new HashSet<TestCase>();
            TestSpace = new HashSet<TestSpace>();
            BlockStep = new HashSet<BlockStep>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public DateTime JoinDate { get; set; }
        public int Lv { get; set; }
        public string Avatar { get; set; }
        public bool Locked { get; set; }

        public virtual ICollection<TestCase> TestCase { get; set; }
        public virtual ICollection<TestSpace> TestSpace { get; set; }
        public virtual ICollection<BlockStep> BlockStep { get; set; }
    }
}
