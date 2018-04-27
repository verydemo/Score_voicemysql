using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Score_voice
{
    public partial class txt_edit : Form
    {
        public txt_edit()
        {
            InitializeComponent();
        }

        private void txt_edit_Load(object sender, EventArgs e)
        {
            List<string> heardlist = new List<string>();
            tmpdata td = session.blist.First();
            System.Type t = td.GetType();
            PropertyInfo[] ps = t.GetProperties();
            foreach (PropertyInfo i in ps)
            {
                object value = i.GetValue(td, null);
                if (value != null)
                {
                    string name = i.Name;
                    heardlist.Add(name);
                }
            }

            for(int i=0;i< heardlist.Count;i++)
            {
                DataGridViewButtonColumn Column = new DataGridViewButtonColumn();
                string name = string.Format("tmpfiled{0}",i+1);
                Column.Text = name;
                Column.Name = name;
                Column.HeaderText = name;
                Column.DataPropertyName = name;
                dataGridView1.Columns.Add(Column);
            }
            dataGridView1.Font = new Font("宋体", 11, FontStyle.Regular);
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = session.blist.Take(5).ToList();
            comboBox1.DataSource = heardlist.Take(heardlist.Count()).ToList();
            comboBox1.Text = heardlist[0].ToString();
            comboBox2.DataSource = heardlist.Take(heardlist.Count()).ToList();
            comboBox2.Text = heardlist[1].ToString();
            comboBox3.DataSource = heardlist.Take(heardlist.Count()).ToList();
            comboBox3.Text = heardlist[2].ToString();
            comboBox4.DataSource = heardlist.Take(heardlist.Count()).ToList();
            comboBox4.Text = heardlist[3].ToString();
            comboBox5.DataSource = heardlist.Take(heardlist.Count()).ToList();
            comboBox5.Text = heardlist[4].ToString();
            //comboBox6.DataSource = heardlist.Take(heardlist.Count()).ToList();
            //comboBox6.Text = heardlist[5].ToString();
        }

        private void btntest_Click(object sender, EventArgs e)
        {
            tmpdata td = session.blist.First();
            System.Type t = td.GetType();
            PropertyInfo pi = t.GetProperty(comboBox4.Text);
            string filepath = pi.GetValue(td, null).ToString();
            filepath = filepath.Replace(textBox4.Text,textBox9.Text);
            try
            {
                System.Diagnostics.Process.Start(filepath);
            }
            catch (Exception)
            {
                MessageBox.Show("系统找不到指定文件!");
            }
            
        }

        private void btnSub_Click(object sender, EventArgs e)
        {
            DialogResult dialogClose = MessageBox.Show("确定提交！！！", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dialogClose == DialogResult.OK)
            {
                if (txtbatchdesc.Text.Trim() == "")
                {
                    MessageBox.Show("请填写数据描述!!!");
                }
                import_data();
            }
            else {

            }
           
        }

        #region 将tmpdata插入表
        private void import_data()
        {
            List<tmpdata> alist = session.blist;
            List<AI_listenScore> blist = new List<AI_listenScore>();
            string sqlstr = string.Format("SELECT batchno FROM AI_listenScore ORDER BY batchno desc LIMIT 1");
            var a = MySqlHelper.ExecuteScalar(CommandType.Text, sqlstr);
            int batchno = 1;
            string batchdesc = txtbatchdesc.Text.Trim();
            if (a != null)
            {
                batchno = Convert.ToInt32(a) + 1;
            }
            List<string> sqllist = new List<string>();
            foreach (var item1 in alist)
            {
                AI_listenScore es = new AI_listenScore();

                try
                {
                    es.papercode = getproval(item1,comboBox1.Text);
                    if (textBox1.Text != "")
                    {
                        es.papercode = es.papercode.Replace(textBox1.Text, textBox12.Text);
                    }
                    es.itemno = getproval(item1, comboBox2.Text);
                    if (textBox2.Text != "")
                    {
                        es.itemno = es.itemno.Replace(textBox2.Text, textBox11.Text);
                    }
                    es.encodeno = getproval(item1, comboBox3.Text);
                    if (textBox3.Text != "")
                    {
                        es.encodeno = es.encodeno.Replace(textBox3.Text, textBox10.Text);
                    }
                    es.filename = getproval(item1, comboBox4.Text);
                    if (textBox4.Text != "")
                    {
                        es.filename = es.filename.Replace(textBox4.Text, textBox9.Text);
                    }
                    es.Score1 = getproval(item1, comboBox5.Text);
                    if (textBox5.Text != "")
                    {
                        es.Score1 = es.Score1.Replace(textBox5.Text, textBox8.Text);
                    }

                    es.Statu = "0";

                    string sql1 = string.Format("INSERT INTO AI_listenScore ( papercode, itemno, encodeno, filename, Score1, Statu,batchno,batchdesc) VALUES ('{0}','{1}','{2}','{3}','{4}' ,'{5}','{6}','{7}')", es.papercode,es.itemno,es.encodeno,es.filename.Replace(@"\", "/"), es.Score1,es.Statu, batchno, batchdesc);

                    sqllist.Add(sql1);
                }
                catch (Exception)
                {
                    //blist.Add(es);
                }

            }

            MySqlHelper.ExecuteSqlTran(sqllist);
            MessageBox.Show("导入成功！！！");
        }

        #endregion


        #region 获取字段的值
        private string getproval(object td, string fieldname)
        {
            System.Type t = td.GetType();
            PropertyInfo pi = t.GetProperty(fieldname);
            string value = pi.GetValue(td, null).ToString();
            return value;
        } 
        #endregion


    }
}
