using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib
{
    public class CurrentUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }

        public string Email { get; set; }
        public DateTime LoginTime { get; set; }

        public List<string> Datas { get; set; }
    }
}
