using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Score_voice
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //if (!SQLHelper.TestSqlConn())
            //{
            //    MessageBox.Show("连接数据库失败！！");
            //    return;
            //}

            if (!MySqlHelper.Testcon())
            {
                MessageBox.Show("连接数据库失败！！");
                return;
            }
            Formlogin flogin = new Formlogin();
            if (flogin.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new FormMain());
            }
            foreach (var a in session.taskIdlist)
            {
                try
                {
                    string sqlstr = string.Format("UPDATE ExamScore SET statu='0' WHERE Id={0} and statu='1' ", a);
                    //SQLHelper.Update(sqlstr);
                    MySqlHelper.ExecuteScalar(CommandType.Text, sqlstr);
                }
                catch (Exception)
                {
                }
            }

        }
    }
}
