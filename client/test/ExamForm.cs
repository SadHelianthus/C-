using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace test
{
    public partial class ExamForm : Form
    {
        private int currentQuestionIndex = 0;
        private List<Question> questions = new List<Question>(); // 存储所有问题
        private int timeLeft = 20; // 考试时间为20秒
        private Timer timer;
        private int score = 0;
        private Random random = new Random();

        public ExamForm()
        {
            InitializeComponent();
            LoadQuestions();
            DisplayQuestion(currentQuestionIndex);
            StartTimer();
            CustomizeUI();
        }

        private void LoadQuestions()
        {
            // 添加随机生成的示例问题
            for (int i = 0; i < 5; i++)
            {
                questions.Add(GenerateRandomQuestion());
            }
        }

        private Question GenerateRandomQuestion()
        {
            int num1 = random.Next(1, 101); // 生成1到100之间的随机数
            int num2 = random.Next(1, 101); // 生成1到100之间的随机数
            string questionText = $"{num1} + {num2} = ?";
            string answer = (num1 + num2).ToString();
            return new Question { Text = questionText, Answer = answer };
        }

        private void DisplayQuestion(int questionIndex)
        {
            if (questionIndex >= 0 && questionIndex < questions.Count)
            {
                lblQuestion.Text = $"题号 {questionIndex + 1}: {questions[questionIndex].Text}";
                textBoxAnswer.Text = ""; // 清除上题的答案
            }
        }

        private void StartTimer()
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft--;
                this.Text = "剩余时间：" + timeLeft.ToString() + "秒";
            }
            else
            {
                timer.Stop();
                MessageBox.Show("考试时间到，请提交试卷。");
                DisplayScore();
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentQuestionIndex > 0)
            {
                string currentAnswer = GetCurrentAnswer(); // 获取当前答案
                CheckAnswer(currentAnswer);
                currentQuestionIndex--;
                DisplayQuestion(currentQuestionIndex);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentQuestionIndex < questions.Count - 1)
            {
                string currentAnswer = GetCurrentAnswer(); // 获取当前答案
                CheckAnswer(currentAnswer);
                currentQuestionIndex++;
                DisplayQuestion(currentQuestionIndex);
            }
        }

        private string GetCurrentAnswer()
        {
            return textBoxAnswer.Text;
        }

        private void CheckAnswer(string currentAnswer)
        {
            if (questions[currentQuestionIndex].Answer == currentAnswer)
            {
                score++;
            }
        }

        private void DisplayScore()
        {
            MessageBox.Show("考试结束，你的得分是：" + score.ToString());
        }

        private void ExamForm_Load(object sender, EventArgs e)
        {
            // 初始化时显示第一题
            DisplayQuestion(currentQuestionIndex);
        }

        private void CustomizeUI()
        {
            // 设置背景颜色
            this.BackColor = Color.FromArgb(255, 239, 239, 244);

            // 设置按钮样式
            btnPrevious.BackColor = Color.FromArgb(255, 100, 149, 237);
            btnPrevious.ForeColor = Color.White;
            btnPrevious.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnPrevious.FlatStyle = FlatStyle.Flat;
            btnPrevious.FlatAppearance.BorderSize = 0;

            btnNext.BackColor = Color.FromArgb(255, 100, 149, 237);
            btnNext.ForeColor = Color.White;
            btnNext.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.FlatAppearance.BorderSize = 0;

            // 设置标签样式
            lblQuestion.BackColor = Color.Transparent;
            lblQuestion.ForeColor = Color.FromArgb(255, 60, 60, 60);
            lblQuestion.Font = new Font("Segoe UI", 14, FontStyle.Bold);

            // 设置文本框样式
            textBoxAnswer.BackColor = Color.White;
            textBoxAnswer.ForeColor = Color.FromArgb(255, 60, 60, 60);
            textBoxAnswer.Font = new Font("Segoe UI", 12);
            textBoxAnswer.BorderStyle = BorderStyle.FixedSingle;
        }
    }

    public class Question
    {
        public string Text { get; set; }
        public string Answer { get; set; }
    }
}
