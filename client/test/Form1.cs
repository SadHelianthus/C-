using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace test
{
    public partial class Form1 : Form
    {
        private Socket socket;

        public Form1()
        {
            InitializeComponent();
            CustomizeUI();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
            try
            {
                socket.Connect(endPoint);
                Console.WriteLine("与服务器连接成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("请先在服务器端启动服务");
                return;
            }

            string userName = textBox1.Text;
            string passWard = textBox2.Text;
            loginInfo login = new loginInfo { name = userName, pw = passWard };
            string jsonDataLogin = JsonConvert.SerializeObject(login);
            byte[] sendByte = Encoding.UTF8.GetBytes(jsonDataLogin);
            socket.Send(sendByte);

            byte[] sendReceiveByte = new byte[2048];
            int count = socket.Receive(sendReceiveByte);

            string msg = Encoding.UTF8.GetString(sendReceiveByte, 0, count);
            if (msg == "验证成功")
            {
                this.Hide();
                kaoshi examForm = new kaoshi();
                examForm.ShowDialog();
                this.Show();
            }
            else if (msg == "服务器端断开连接了")
            {
                MessageBox.Show("服务器端断开连接了");
            }
            else
            {
                MessageBox.Show("用户名或者密码错误，请重试");
            }
        }

        public class loginInfo
        {
            public string name { get; set; }
            public string pw { get; set; }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show();
        }

        private void CustomizeUI()
        {
            // 设置背景颜色
            this.BackColor = Color.FromArgb(255, 240, 240, 240);

            // 设置按钮样式
            btnLogin.BackColor = Color.FromArgb(255, 100, 149, 237);
            btnLogin.ForeColor = Color.White;
            btnLogin.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;

            button2.BackColor = Color.FromArgb(255, 100, 149, 237);
            button2.ForeColor = Color.White;
            button2.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0;

            btnClose.BackColor = Color.FromArgb(255, 255, 69, 58);
            btnClose.ForeColor = Color.White;
            btnClose.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;

            // 设置文本框样式
            textBox1.BackColor = Color.White;
            textBox1.ForeColor = Color.FromArgb(255, 60, 60, 60);
            textBox1.Font = new Font("Segoe UI", 10);
            textBox1.BorderStyle = BorderStyle.FixedSingle;

            textBox2.BackColor = Color.White;
            textBox2.ForeColor = Color.FromArgb(255, 60, 60, 60);
            textBox2.Font = new Font("Segoe UI", 10);
            textBox2.BorderStyle = BorderStyle.FixedSingle;

            // 设置标签样式
            label1.ForeColor = Color.FromArgb(255, 60, 60, 60);
            label1.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            label2.ForeColor = Color.FromArgb(255, 60, 60, 60);
            label2.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        }
    }
}
