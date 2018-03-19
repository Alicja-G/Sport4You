using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Sport4You.Models.Data
{
    public class DB : DbContext
    {
        public DbSet<PageDTO> Pages { get; set; }

    }
}