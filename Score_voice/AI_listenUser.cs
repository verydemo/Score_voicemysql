using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Score_voice
{
    public class AI_listenUser
    {
        //Id, InsertDT, UserName, UserDesc, Pwd, Isable, type
        public int Id { get; set; }
        public DateTime InsertDT { get; set; }

        public string UserName { get; set; }

        public string UserDesc { get; set; }

        public string Pwd { get; set; }

        public int Isable { get; set; }

        public string type { get; set; }

    }
}
