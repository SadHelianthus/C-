using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CustomizeUI();
        }

        Socket serverSocket;
        Thread receiveThread;
        Thread acceptThread;

        private void btnStart_Click(object sender, EventArgs e)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEnd = new IPEndPoint(ip, 8080);
            serverSocket.Bind(ipEnd);
            Console.WriteLine("服务端启动");
            serverSocket.Listen(0);
            Console.WriteLine("开启监听");
            acceptThread = new Thread(new ParameterizedThreadStart(AcceptClientConnect));
            acceptThread.Start(serverSocket);
            lblStatus.Text = "服务已启动";
            lblStatus.ForeColor = Color.Green;
        }

        public void AcceptClientConnect(object socket)
        {
            Socket serverSocket = socket as Socket;
            while (serverSocket != null)
            {
                try
                {
                    Socket tempSocket = serverSocket.Accept();
                    Console.WriteLine("客户端连接 " + tempSocket.RemoteEndPoint);
                    receiveThread = new Thread(ListenClientConnect);
                    receiveThread.Start(tempSocket);
                }
                catch (Exception ex)
                {
                    if (serverSocket != null)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                }
            }
        }

        private void ListenClientConnect(Object tempSocket)
        {
            bool flag = false;
            Socket mySocket = tempSocket as Socket;
            while (true)
            {
                try
                {
                    byte[] receiveByte = new byte[2048];
                    int count = mySocket.Receive(receiveByte);
                    Console.WriteLine("服务器接收到：" + Encoding.UTF8.GetString(receiveByte, 0, count));

                    string receiveStr = Encoding.UTF8.GetString(receiveByte, 0, count);
                    Console.WriteLine("服务端验证" + receiveStr);
                    JObject person = JsonConvert.DeserializeObject<JObject>(receiveStr);

                    if (person["studentID"] != null)
                    {
                        // 注册处理
                        string studentID = person["studentID"].ToString();
                        string tempName = person["name"].ToString();
                        string tempPW = person["pw"].ToString();
                        flag = RegisterUser(studentID, tempName, tempPW);

                        if (flag == true)
                        {
                            byte[] backByte = Encoding.UTF8.GetBytes("注册成功");
                            mySocket.Send(backByte);
                        }
                        else
                        {
                            byte[] backByte = Encoding.UTF8.GetBytes("注册失败");
                            mySocket.Send(backByte);
                        }
                    }
                    else
                    {
                        // 登录处理
                        string tempName = person["name"].ToString();
                        string tempPW = person["pw"].ToString();
                        flag = LogInCheck(tempName, tempPW);

                        if (flag == true)
                        {
                            byte[] backByte = Encoding.UTF8.GetBytes("验证成功");
                            mySocket.Send(backByte);
                        }
                        else
                        {
                            byte[] backByte = Encoding.UTF8.GetBytes("验证失败");
                            mySocket.Send(backByte);
                        }
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        private bool LogInCheck(string username, string password)
        {
            bool flag = false;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
            using (StreamReader sr = new StreamReader(path))
            {
                using (JsonTextReader jtr = new JsonTextReader(sr))
                {
                    JObject temp = JObject.Load(jtr);
                    JArray users = (JArray)temp["user"];
                    foreach (var user in users)
                    {
                        UserPassword u = JsonConvert.DeserializeObject<UserPassword>(user.ToString());
                        if (u.Name == username && u.Password == password)
                        {
                            flag = true;
                            break;
                        }
                        else if (u.Name != username || u.Password != password)
                        {
                            flag = false;
                        }
                    }
                }
            }
            return flag;
        }

        private bool RegisterUser(string studentID, string username, string password)
        {
            bool flag = false;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
            JObject usersData;

            try
            {
                Console.WriteLine("尝试读取 users.json 文件：" + path);

                using (StreamReader sr = new StreamReader(path))
                {
                    using (JsonTextReader jtr = new JsonTextReader(sr))
                    {
                        usersData = JObject.Load(jtr);
                    }
                }

                Console.WriteLine("成功读取 users.json 文件");

                JArray users = usersData["user"] as JArray;
                if (users == null)
                {
                    users = new JArray();
                    usersData["user"] = users;
                    Console.WriteLine("users.json 文件中没有用户数据，创建新的用户列表");
                }

                foreach (var user in users)
                {
                    var userNameToken = user["Name"];
                    if (userNameToken != null && userNameToken.ToString() == username)
                    {
                        Console.WriteLine("用户名已存在：" + username);
                        return false; // 用户名已存在
                    }
                }

                JObject newUser = new JObject
                {
                    { "StudentID", studentID },
                    { "Name", username },
                    { "Password", password }
                };

                users.Add(newUser);
                usersData["user"] = users;

                using (StreamWriter sw = new StreamWriter(path))
                using (JsonTextWriter jwt = new JsonTextWriter(sw))
                {
                    usersData.WriteTo(jwt);
                    flag = true;
                }

                Console.WriteLine("成功注册用户：" + username);
            }
            catch (Exception ex)
            {
                Console.WriteLine("注册时出错：" + ex.Message);
            }

            return flag;
        }

        internal class UserPassword
        {
            public string Name { get; set; }
            public string Password { get; set; }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (receiveThread != null && receiveThread.IsAlive)
                {
                    receiveThread.Abort();
                }

                if (serverSocket != null)
                {
                    serverSocket.Close();
                    serverSocket = null;
                    Console.WriteLine("服务端断开连接了");
                    lblStatus.Text = "服务已断开";
                    lblStatus.ForeColor = Color.Red;
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        private void CustomizeUI()
        {
            // 设置背景颜色
            this.BackColor = Color.LightSteelBlue;

            // 设置按钮样式
            btnStart.BackColor = Color.DarkSlateBlue;
            btnStart.ForeColor = Color.White;
            btnStart.Font = new Font("Arial", 14, FontStyle.Bold);

            btnClose.BackColor = Color.DarkSlateBlue;
            btnClose.ForeColor = Color.White;
            btnClose.Font = new Font("Arial", 14, FontStyle.Bold);

            // 设置状态标签样式
            lblStatus.ForeColor = Color.DarkSlateBlue;
            lblStatus.Font = new Font("Arial", 14, FontStyle.Bold);
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
        }
    }
}
