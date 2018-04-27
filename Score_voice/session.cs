using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Score_voice
{
    public static class session
    {
        public static string user { get; set; }

        //登陆用户
        public static AI_listenUser listenuser = new AI_listenUser();

        //当前题目
        public static AI_listenScore listenScore = new AI_listenScore();

        //当前批次
        public static int batchno = 1;

        private static List<int> taskidlist = new List<int>();
        public static List<int>  taskIdlist
        {
            get
            {
                return taskidlist;
            }
        }

        public static List<tmpdata> blist = new List<tmpdata>();

        public static string[] first_array;

        public static string filead =MySqlHelper.config1.AppSettings.Settings["filead"].Value;

    }
}
