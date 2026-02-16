using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<Recruitment> Recruitments { get; set; }
    }

}
