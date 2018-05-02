using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;
using AxWMPLib;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Threading;

namespace Score_voice
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            
        }

        AxWindowsMediaPlayer awmp = new AxWindowsMediaPlayer();

        
        //public static string IP = SQLHelper.config.AppSettings.Settings["IP"].Value;
        
        private void Form1_Load(object sender, EventArgs e)
        {
            string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Text = this.Text + "  v" + Version;
            var batchlist = MySqlHelper.GetDataSet(CommandType.Text, "SELECT batchno,batchdesc FROM AI_listenScore where validflag='2' GROUP BY batchno,batchdesc ORDER BY batchno").Tables[0];
            cbopici.DataSource = batchlist;
            cbopici.DisplayMember = "batchdesc";
            cbopici.ValueMember = "batchno";
            session.batchno = Convert.ToInt32(cbopici.SelectedValue);
            cbopici.SelectedValueChanged += query_combo;
            
            var quesnolist = MySqlHelper.GetDataSet(CommandType.Text, string.Format("SELECT itemno FROM AI_listenScore WHERE batchno={0} and validflag='2' GROUP BY itemno ORDER BY itemno", session.batchno)).Tables[0];

            cboitemno.DataSource = quesnolist;
            cboitemno.DisplayMember = "itemno";
            querydata();
            cboitemno.SelectedValueChanged += query_data;
            button2.Enabled = false;
            awmp.BeginInit();
            this.Controls.Add(awmp);
            awmp.uiMode = "Full";
            awmp.EndInit();
            if (session.listenuser.type != "1")
            {
                tsmi_data.Enabled = false;
            }

            timer1.Interval = 100;
            timer1.Tick += Timer1_Tick;
            timer1.Start();

            progressBar1.MouseClick += ProgressBar1_Click;
            label14.MouseClick += Label_MouseClick;
            label15.MouseClick += Label_MouseClick;
            cbopici.SelectedValueChanged += Cbopici_SelectedValueChanged;
            setbtn(cbopici.Text,1);
        }

        private void Cbopici_SelectedValueChanged(object sender, EventArgs e)
        {
            setbtn(((ComboBox)sender).Text,1);
        }

        private void setbtn(string batchdesc="",int type=0)
        {
            if (type == 0)
            {
               batchdesc = session.listenScore.batchdesc;
            }
            switch (batchdesc)
            {
                case "客观0主观有":
                    btnrecheck.Enabled = false;
                    btnzero.Enabled = true;
                    btnex.Enabled = false;
                    break;
                case "主观0客观有":
                    btnrecheck.Enabled = true;
                    btnzero.Enabled = false;
                    btnex.Enabled = false;
                    break;
                case "客观低简述高":
                    btnrecheck.Enabled = false;
                    btnzero.Enabled = true;
                    btnex.Enabled = false;
                    break;
                case "定标":
                    btnrecheck.Enabled = false;
                    btnzero.Enabled = false;
                    btnex.Enabled = true;
                    break;
                case "0分题":
                    btnrecheck.Enabled = true;
                    btnzero.Enabled = false;
                    btnex.Enabled = false;
                    break;
                default:
                    btnrecheck.Enabled = true;
                    btnzero.Enabled = true;
                    btnex.Enabled = true;
                    break;
            }


        }

        private void Label_MouseClick(object sender, MouseEventArgs e)
        {
            var a = (Label)sender;
            //textBox1.Text = a.Text;
        }


        private void query_combo(object sender, EventArgs e)

        {
            session.batchno = Convert.ToInt32(cbopici.SelectedValue);         
            var quesnolist = MySqlHelper.GetDataSet(CommandType.Text, string.Format("SELECT itemno FROM AI_listenScore WHERE batchno={0} and validflag='2'  GROUP BY itemno ORDER BY itemno", session.batchno)).Tables[0];
            cboitemno.DataSource = quesnolist;
            cboitemno.DisplayMember = "itemno";
            querydata();

        }


        private void ProgressBar1_Click(object sender, MouseEventArgs e)
        {
            awmp.Ctlcontrols.stop();
            var pb = ((ProgressBar)sender);
            int width = pb.Width;
            int x = e.X;
            progressBar1.Value = Convert.ToInt32( x *100/width);
            awmp.Ctlcontrols.currentPosition = x * awmp.currentMedia.duration / width;
            Console.WriteLine(progressBar1.Value+"  ----------------   "+ awmp.Ctlcontrols.currentPosition);
            awmp.Ctlcontrols.play();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (awmp.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                progressBar1.Value = Convert.ToInt32(awmp.Ctlcontrols.currentPosition * 100 / awmp.currentMedia.duration);
                label16.Text = awmp.Ctlcontrols.currentPositionString + " / " + awmp.currentMedia.durationString;
            }
            else if (awmp.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                label16.Text = awmp.currentMedia.durationString+" / " + awmp.currentMedia.durationString;
            }
        }

        string voice_path = "";
        private void query_data(object sender, EventArgs e)
        {
            querydata();
        }

        private void tsmi_import_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件*.txt|*.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string filepath = ofd.FileName;
                txt_edit te = new txt_edit();
                //import_data(filepath);
                imptmpdata(filepath);
                te.Show();
                //MessageBox.Show("插入成功！！！");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {       
            getonedata();
        }

     
        List<tmpdata> blist = session.blist;
        #region 将txt插入到tmplist    blist
        private void imptmpdata(string filepath)
        {
            List<string> aList = File.ReadAllLines(filepath, Encoding.UTF8).ToList();
            session.first_array = aList.First().Split('\t');
            int len = aList.First().Split('\t').Length;
            foreach (var item1 in aList)
            {
                string[] a = item1.Split('\t');
                tmpdata td = new tmpdata();
                System.Type t = td.GetType();
                try
                {
                    for (int i = 0; i < len; i++)
                    {
                        string tmpfiled = string.Format("tmpfiled{0}", i+1);
                        PropertyInfo filed = t.GetProperty(tmpfiled);
                        filed.SetValue(td,a[i],null);
                    }
                    blist.Add(td);
                }
                catch (Exception)
                {
                    blist.Add(td);
                }
            }
        }
        #endregion


        #region 数量
        private void querydata()
        {
            string totalnum_sql = string.Format("SELECT count(Id) FROM AI_listenScore WHERE  itemno='{0}' and validflag='2' and batchno={1} AND (Statu='0' or Statu='9')",  cboitemno.Text,session.batchno);
            int totalnum = Convert.ToInt32(MySqlHelper.ExecuteScalar(CommandType.Text, totalnum_sql));
            string complete_sql = string.Format("SELECT count(Id) FROM AI_listenScore WHERE  itemno='{0}' and validflag='2' AND Statu='9' and batchno={1}",  cboitemno.Text, session.batchno);
            int completenum = Convert.ToInt32(MySqlHelper.ExecuteScalar(CommandType.Text, complete_sql));
            label7.Text = completenum.ToString() + "/" + totalnum.ToString();
        }
        #endregion

        #region 取题
        private int Id = 0;
        private void getonedata()
        {
            try
            {
                string sqlstr = string.Format("SELECT * FROM AI_listenScore WHERE  itemno='{1}' and validflag='2'  AND Statu='0' and batchno={0} order by papercode,itemno limit 1", Convert.ToInt32(cbopici.SelectedValue), cboitemno.Text.Trim());
                List<AI_listenScore>  singlerecord1 = MySqlHelper.GetDataSet(CommandType.Text,sqlstr).Tables[0].ToModels<AI_listenScore>();
                AI_listenScore singlerecord = MySqlHelper.GetDataSet(CommandType.Text, sqlstr).Tables[0].ToModels<AI_listenScore>().First();
                session.listenScore = singlerecord;
                string sql_upd = string.Format("UPDATE AI_listenScore SET statu='9',listenDT=now(),listener='{1}' WHERE Id={0}", singlerecord.Id,session.listenuser.UserName);

                MySqlHelper.ExecuteScalar(CommandType.Text, sql_upd);
                label8.Text = singlerecord.papercode;
                label9.Text = singlerecord.itemno;
                label14.Text = singlerecord.Score1;
                label15.Text = singlerecord.Score2;
                setbtn();
                voice_path =session.filead+ singlerecord.filename;
                Id = singlerecord.Id;
                button2.Text = string.Format("试卷号：{0} | 题目号：{1}", singlerecord.papercode, singlerecord.itemno);                
                if (voice_path != "")
                {
                    awmp.URL = voice_path;
                    awmp.Ctlcontrols.play();
                }
                querydata();
                try
                {
                    string sqltxt = string.Format("SELECT t1.itemno,t2.papercode,t1.itemtext,t1.answertext,t1.modeltext FROM AI_item t1, AI_paper t2 WHERE t1.itemno='{0}' AND t2.papercode='{1}' AND t1.paperid=t2.paperid", session.listenScore.itemno, session.listenScore.papercode);
                    item item1 = MySqlHelper.GetDataSet(CommandType.Text, sqltxt).Tables[0].ToModels<item>().First();
                    textBox1.Text = item1.itemtext;
                    textBox2.Text = item1.answertext;
                }
                catch (Exception)
                {
                    MessageBox.Show("取不到题目对应信息!!!");
                }
                
                session.taskIdlist.Add(Id);
            }
            catch (Exception)
            {
                MessageBox.Show("取不到题目了！！");
            }
          
        }
        #endregion

        private void getpredata()
        {          
            int index = session.taskIdlist.FindIndex(a => a == Id);
            if (index >=1)
            {
                int b = session.taskIdlist[index - 1];
                string sqlstr = string.Format("SELECT * FROM ExamScore WHERE Id={0}", b);

                AI_listenScore singlerecord = SQLHelper.GetDbTable(sqlstr).ToModels<AI_listenScore>().First();
                label8.Text = singlerecord.papercode;
                label9.Text = singlerecord.itemno;
                label14.Text = singlerecord.Score1;
                label15.Text = singlerecord.Score2;
                voice_path = singlerecord.filename;
                Id = singlerecord.Id;
                button2.Text = string.Format("试卷号：{0} | 题目号：{1}", singlerecord.papercode, singlerecord.itemno);
                if (voice_path != "")
                {
                    awmp.URL = voice_path;
                    awmp.Ctlcontrols.play();

                }

            }
            
        }

        private void Awmp_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (awmp.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                Application.DoEvents();
                
            }            
        }


        private void initdata_Click(object sender, EventArgs e)
        {
            DialogResult dialogClose = MessageBox.Show("确定初始化？？？初始化会清空所有数据！！！", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dialogClose == DialogResult.OK)
            {
                try
                {
                    string sqlstr = "TRUNCATE TABLE AI_listenScore";
                    MySqlHelper.ExecuteScalar(CommandType.Text, sqlstr);
                    MessageBox.Show("初始化成功!!!");
                    refreshgui();
                }
                catch (Exception)
                {
                    MessageBox.Show("初始化失败!!!");
                }
            }
           

        }

        private void tsmi_export_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            awmp.Ctlcontrols.stop();
            DialogResult dr = MessageBox.Show("确认标记异常并进入下一题吗？", "提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                //标记并进入下一题
                string sqlstr = string.Format("UPDATE AI_listenScore SET sign='1' WHERE Id={1}", session.listenuser.UserName, session.listenScore.Id);
                MySqlHelper.ExecuteScalar(CommandType.Text, sqlstr);
                getonedata();
            }


        }

        private void btnstartstop_Click(object sender, EventArgs e)
        {
            if (awmp.status.Contains("正在播放"))
            {
                awmp.Ctlcontrols.pause();

            }
            else if (awmp.status.Contains("暂停"))
            {
                awmp.Ctlcontrols.play();

            }
            else if (awmp.status.Contains("停止"))
            {
                awmp.Ctlcontrols.play();

            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            //getpredata();
        }

        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            refreshgui();
        }


        private void refreshgui()
        {
            var batchlist = MySqlHelper.GetDataSet(CommandType.Text, "SELECT batchno,batchdesc FROM AI_listenScore where validflag='2' GROUP BY batchno,batchdesc ORDER BY batchno").Tables[0];
            cbopici.DataSource = batchlist;
            cbopici.DisplayMember = "batchdesc";
            cbopici.ValueMember = "batchno";
            session.batchno = Convert.ToInt32(cbopici.SelectedValue);
            cbopici.SelectedValueChanged += query_combo;

            var quesnolist = MySqlHelper.GetDataSet(CommandType.Text, string.Format("SELECT itemno FROM AI_listenScore WHERE batchno={0} and validflag='2' GROUP BY itemno ORDER BY itemno", session.batchno)).Tables[0];
            comboBox1.DisplayMember = "papercode";
            cboitemno.DataSource = quesnolist;
            cboitemno.DisplayMember = "itemno";
            querydata();
        }

        private void tsmijdxq_Click(object sender, EventArgs e)
        {
            List<AI_listenScore> pcjd = MySqlHelper.GetDataSet(CommandType.Text, string.Format("select * from AI_listenScore where validflag='2'")).Tables[0].ToModels<AI_listenScore>();
            var pcjdlist = pcjd.GroupBy(a => a.batchdesc).Select(b => new { b.First().batchdesc, totalcount = b.Count(), ycount = b.Where(c => c.Statu == "9").Count(), wcount = b.Where(c => c.Statu == "0").Count() });
         }

        private void btnzero_Click(object sender, EventArgs e)
        {

            DialogResult dr = MessageBox.Show("确认置为0分并进入下一题吗？", "提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                if (session.listenScore.scoreid != 0)
                {
                    try
                    {
                        string sql = string.Format("UPDATE AI_score SET scorelast=0,listener='{0}',listenDT=now(),scoretype='L' WHERE scoreid={1}", session.listenuser.UserName, session.listenScore.scoreid);
                        MySqlHelper.ExecuteScalar(CommandType.Text, sql);
                        getonedata();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
                else
                {
                    MessageBox.Show("当前题目不能置为0分!!!");
                }
            }
            
        }

        private void btnrecheck_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确认设为复核卷并进入下一题吗？", "提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                try
                {
                    string sql = string.Format("UPDATE AI_listenScore SET recheck='1' WHERE Id={0}",session.listenScore.Id);
                    MySqlHelper.ExecuteScalar(CommandType.Text, sql);
                    getonedata();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }
    }
}
