using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTallerAutomorizWPF.Models
{
    public class UserAccountModel
    {
        public String UserName { get; set; }
        public String DisplayName { get; set; }
        public byte[] ProfilePicture { get; set; }
        public string Rol { get; set; }

    }
}
