using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Score_voice
{
    public class AI_listenScore
    {

        //        //Id, InsertDT, papercode, itemno, itemcode, encodeno, filename, 
        //--Score1, Score2, Score3, Score4, Statu, listener, listenDT, batchno, batchdesc, sign

        public int Id { get; set; }

        public DateTime InsertDT { get; set; }

        public string papercode { get; set; }

        public string itemno { get; set; }

        public string itemcode { get; set; }

        public string encodeno { get; set; }

        public string filename { get; set; }

        public string Score1 { get; set; }

        public string Score2 { get; set; }

        public string Score3 { get; set; }

        public string Score4 { get; set; }


        public string Statu { get; set; }

        public string listener { get; set; }

        public DateTime listenDT { get; set; }

        public int batchno { get; set; }

        public string batchdesc { get; set; }

        public string sign { get; set; }
        public int scoreid { get; set; }
        public string recheck { get; set; }
        public string validflag { get; set; }

    }
}
