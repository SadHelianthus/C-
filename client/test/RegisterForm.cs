using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace test
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
            CustomizeUI();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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

            string studentID = textBoxStudentID.Text;
            string userName = textBoxUsername.Text;
            string passWard = textBoxPassword.Text;
            var registerInfo = new { studentID = studentID, name = userName, pw = passWard };
            string jsonDataRegister = JsonConvert.SerializeObject(registerInfo);
            byte[] sendByte = Encoding.UTF8.GetBytes(jsonDataRegister);
            socket.Send(sendByte);

            byte[] receiveByte = new byte[2048];
            int count = socket.Receive(receiveByte);

            string msg = Encoding.UTF8.GetString(receiveByte, 0, count);
            if (msg == "注册成功")
            {
                MessageBox.Show("注册成功，请用刚注册的用户名和密码登录");
                this.Close();
            }
            else if (msg == "注册失败")
            {
                MessageBox.Show("注册失败，请重试");
            }
        }

        private void CustomizeUI()
        {
            // 设置背景颜色
            this.BackColor = Color.FromArgb(255, 239, 239, 244);

            // 设置按钮样式
            btnSubmit.BackColor = Color.FromArgb(255, 100, 149, 237);
            btnSubmit.ForeColor = Color.White;
            btnSubmit.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.FlatAppearance.BorderSize = 0;

            // 设置文本框样式
            textBoxStudentID.BackColor = Color.White;
            textBoxStudentID.ForeColor = Color.FromArgb(255, 60, 60, 60);
            textBoxStudentID.Font = new Font("Segoe UI", 12);

            textBoxUsername.BackColor = Color.White;
            textBoxUsername.ForeColor = Color.FromArgb(255, 60, 60, 60);
            textBoxUsername.Font = new Font("Segoe UI", 12);

            textBoxPassword.BackColor = Color.White;
            textBoxPassword.ForeColor = Color.FromArgb(255, 60, 60, 60);
            textBoxPassword.Font = new Font("Segoe UI", 12);

            // 设置标签样式
            labelStudentID.ForeColor = Color.FromArgb(255, 100, 149, 237);
            labelStudentID.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            labelUsername.ForeColor = Color.FromArgb(255, 100, 149, 237);
            labelUsername.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            labelPassword.ForeColor = Color.FromArgb(255, 100, 149, 237);
            labelPassword.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        }
    }
}
