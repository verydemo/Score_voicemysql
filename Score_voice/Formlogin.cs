using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Score_voice
{
    public partial class Formlogin : Form
    {
        public Formlogin()
        {
            InitializeComponent();
            textBox2.KeyDown += txtPassword_KeyDown;
        }



        //public static string usersess = "";
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "" || textBox2.Text.Trim() == "")
            {
                MessageBox.Show("用户名或密码为空！");
            }
            else {
                string username = textBox1.Text.Replace("-", "").Trim();
                string pwd = textBox2.Text.Replace("-", "").Trim();
                string sqlstr = string.Format("SELECT count(Id) FROM AI_listenUser WHERE UserName='{0}' AND Pwd='{1}'", username, pwd);
                //int a =SQLHelper.GetSingleResult(sqlstr);
                int a =Convert.ToInt32( MySqlHelper.ExecuteScalar(CommandType.Text,sqlstr));
                if (a > 0)
                {
                    string sqlstr1 = string.Format("SELECT * FROM AI_listenUser WHERE UserName='{0}' AND Pwd='{1}'", username, pwd);
                    DataSet dataset = MySqlHelper.GetDataSet(CommandType.Text, sqlstr1);
                    AI_listenUser lu1 = dataset.Tables[0].ToModels<AI_listenUser>().First();
                    session.listenuser = lu1;
                    session.user = username;
                    this.DialogResult = DialogResult.OK;
                }
                else {
                    MessageBox.Show("账号密码错误！！！");
                }
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                button1_Click(null, null);
            }
        }

    }
}
