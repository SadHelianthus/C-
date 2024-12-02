using System;
using System.Drawing;
using System.Windows.Forms;

namespace test
{
    public partial class kaoshi : Form
    {
        public kaoshi()
        {
            InitializeComponent();
            CustomizeUI();
        }

        private void kaoshi_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = "欢迎使用小熊考试系统\n";
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
            richTextBox1.SelectAll();
            richTextBox1.AppendText("1.考试准备：\n考生需要提前20分钟登录考试系统，确保网络畅通，避免因网络问题导致无法参加考试。\n");
            richTextBox1.AppendText("2.考试过程：\n若发生设备或网络故障，考生应主动与工作人员联系，寻求帮助。\n");
            richTextBox1.AppendText("3.考试结束：\n考生完成考试后，须经监考教师确认后，方能退出考试考场。\n");
            richTextBox1.Font = new Font("Segoe UI", 12);
            richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
            richTextBox1.SelectionColor = Color.DarkSlateBlue;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            ExamForm examForm = new ExamForm();
            examForm.ShowDialog();
        }

        private void CustomizeUI()
        {
            // 设置背景颜色
            this.BackColor = Color.FromArgb(239, 244, 255);

            // 设置按钮样式
            btnStart.BackColor = Color.FromArgb(70, 130, 180);
            btnStart.ForeColor = Color.White;
            btnStart.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.FlatAppearance.BorderSize = 0;

            // 设置富文本框样式
            richTextBox1.BackColor = Color.White;
            richTextBox1.ForeColor = Color.FromArgb(60, 60, 60);
            richTextBox1.Font = new Font("Segoe UI", 12);
            richTextBox1.BorderStyle = BorderStyle.None;
        }
    }
}
