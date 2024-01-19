using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Solution
{
    public partial class ParseProductCode : Form
    {
        private double totalWeight = 0;
        private double currentWeight = 0.0;
        private ListBox listBoxCodes = new ListBox();
        private Timer timer = new Timer();
        private int absoluteTime = 2;
        private string currentCodeFormat = @"^\(01\)\d+\(3103\)\d+\(11\)\d+\(10\)\d+\(21\)\d+$";
        private int remainingTime = 0;
        private System.Windows.Forms.Button buttonSettings;
        private ContextMenuStrip contextMenuStripSettings; 
        private ToolStripMenuItem toolStripMenuItemTime; 
        private TimeSettingsForm timeSettingsForm;
        private ToolStripMenuItem toolStripMenuItemTimeSettings;
        private ToolStripMenuItem toolStripMenuItemCodeFormatSettings;

        public ParseProductCode()
        {
            InitializeComponent();
            InitializeListBox();

            buttonSettings = new System.Windows.Forms.Button();
            buttonSettings.Location = new Point(20, 20);
            buttonSettings.Text = "Настройки";
            buttonSettings.Click += ButtonSettings_Click;
            Controls.Add(buttonSettings);

            contextMenuStripSettings = new ContextMenuStrip();
            toolStripMenuItemTimeSettings = new ToolStripMenuItem("Настроить время");
            toolStripMenuItemTimeSettings.Click += ToolStripMenuItemTime_Click;
            contextMenuStripSettings.Items.Add(toolStripMenuItemTimeSettings);

            toolStripMenuItemCodeFormatSettings = new ToolStripMenuItem("Настроить формат кода");
            toolStripMenuItemCodeFormatSettings.Click += ToolStripMenuItemCodeFormat_Click;
            contextMenuStripSettings.Items.Add(toolStripMenuItemCodeFormatSettings);

            remainingTime = 2;

            timer.Interval = 1000;
            timer.Tick += (s, e) =>
            {
                remainingTime--;
                label6.Text = remainingTime.ToString();

                if (remainingTime <= 0)
                {
                    Timer_Tick(s, e);
                }
            };

            textBox1.TextChanged += TextBox1_TextChanged;
        }

        private void ToolStripMenuItemCodeFormat_Click(object sender, EventArgs e)
        {
            using (var codeFormatSettingsForm = new CodeFormatSettingsForm(GetCodeFormat()))
            {
                if (codeFormatSettingsForm.ShowDialog() == DialogResult.OK)
                {
                    SetCodeFormat(codeFormatSettingsForm.CodeFormat);
                }
            }
        }
        private string GetCodeFormat()
        {
            return currentCodeFormat;
        }

        private void SetCodeFormat(string codeFormat)
        {
            currentCodeFormat = codeFormat;
        }

        private void ButtonFormatSettings_Click(object sender, EventArgs e)
        {
            using (var codeFormatSettingsForm = new CodeFormatSettingsForm(GetCodeFormat()))
            {
                if (codeFormatSettingsForm.ShowDialog() == DialogResult.OK)
                {
                    SetCodeFormat(codeFormatSettingsForm.CodeFormat); 
                }
            }
        }

        private void ButtonSettings_Click(object sender, EventArgs e)
        {
            contextMenuStripSettings.Show(buttonSettings, new Point(0, buttonSettings.Height));
        }

        private void ToolStripMenuItemTime_Click(object sender, EventArgs e)
        {
            using (var timeSettingsForm = new TimeSettingsForm())
            {
                if (timeSettingsForm.ShowDialog() == DialogResult.OK)
                {
                   
                    int timeInSeconds = timeSettingsForm.TimeInSeconds;
                   
                    absoluteTime = timeInSeconds;
                    label6.Text = remainingTime.ToString();

                    if (timer.Enabled)
                    {
                        timer.Stop();
                        timer.Start();
                    }
                }
            }
        }

        private void InitializeListBox()
        {
            listBoxCodes.Location = new System.Drawing.Point(220, 240);
            listBoxCodes.Size = new System.Drawing.Size(300, 100);
            Controls.Add(listBoxCodes);
        }

        private void UpdateTotalWeight()
        {
            label4.Text = totalWeight.ToString();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            string code = textBox1.Text;
            double weight = GetProductWeight(code);
            if (weight > 0)
            {
                if (string.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBox.Show("Поле максимального веса товаров не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                double newTotalWeight = totalWeight + weight;
                if (newTotalWeight > Convert.ToDouble(textBox2.Text))
                {
                    MessageBox.Show("Превышен максимальный вес товаров!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    listBoxCodes.Items.Add(code);
                    totalWeight = newTotalWeight;
                    UpdateTotalWeight();
                }
            }
            textBox1.Clear();
            textBox1.BackColor = SystemColors.Window;
            errorProvider.SetError(textBox1, "");
            remainingTime = absoluteTime;
            label6.Text = remainingTime.ToString();
        }

       private void TextBox1_TextChanged(object sender, EventArgs e)
        {
        string code = textBox1.Text;
        bool isValid = IsProductCodeValid(code);

            if (isValid)
            {
                textBox1.BackColor = Color.LightGreen;
                errorProvider.SetError(textBox1, "");
                timer.Stop();
                remainingTime = absoluteTime;
                label6.Text = remainingTime.ToString();
                timer.Start();
            }
            else
            {
                textBox1.BackColor = SystemColors.Window;
                errorProvider.SetError(textBox1, "Неверный формат кода товара");
                timer.Stop();
                remainingTime = 0;
                label6.Text = remainingTime.ToString();
            }
        }

        private bool IsProductCodeValid(string code)
        {
            string pattern = @"^\(01\)\d+\(3103\)\d+\(11\)\d+\(10\)\d+\(21\)\d+$";
            return Regex.IsMatch(code, pattern);
        }

        private double GetProductWeight(string code)
        {
            currentWeight = 0.0;
            int weight = 0;
            if (code.StartsWith("(01)") && code.Length >= 15)
            {
                int startIndex = code.IndexOf("(3103)") + 6;
                int endIndex = code.IndexOf("(11)", startIndex);

                if (endIndex != -1 && int.TryParse(code.Substring(startIndex, endIndex - startIndex), out weight))
                {
                    Console.WriteLine(weight);
                    currentWeight += weight;
                }
            }
            return weight;
        }

        private void ResetValues()
        {
            currentWeight = 0.0;
            textBox1.Clear();
            textBox2.Clear();
            listBoxCodes.Items.Clear();
            totalWeight = 0.0;
            UpdateTotalWeight();
            textBox1.BackColor = SystemColors.Window;
            errorProvider.SetError(textBox1, "");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ResetValues();
        }

    }
    public class TimeSettingsForm : Form
    {
        private NumericUpDown numericUpDownTime;
        private System.Windows.Forms.Button buttonOK;

        public int TimeInSeconds => (int)numericUpDownTime.Value;

        public TimeSettingsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            numericUpDownTime = new NumericUpDown();
            buttonOK = new System.Windows.Forms.Button();

            numericUpDownTime.Location = new Point(20, 20);
            numericUpDownTime.Width = 100;
            numericUpDownTime.Minimum = 1;
            numericUpDownTime.Maximum = 60;

            buttonOK.Location = new Point(20, 50);
            buttonOK.Text = "OK";
            buttonOK.DialogResult = DialogResult.OK;
            buttonOK.Click += ButtonOK_Click;

            Controls.Add(numericUpDownTime);
            Controls.Add(buttonOK);

            Text = "Настройки времени";
            Width = 200;
            Height = 120;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            AcceptButton = buttonOK;
            CancelButton = buttonOK;
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
        
            Close();
        }
    }

    public class CodeFormatSettingsForm : Form
    {
        private System.Windows.Forms.TextBox textBoxFormat;
        private System.Windows.Forms.Button buttonOK;

   

        public string CodeFormat { get; private set; }

        public CodeFormatSettingsForm(string codeFormat)
        {
            CodeFormat = codeFormat;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            textBoxFormat = new System.Windows.Forms.TextBox();
            buttonOK = new System.Windows.Forms.Button();

            textBoxFormat.Location = new Point(20, 20);
            textBoxFormat.Width = 200;

            buttonOK.Location = new Point(20, 50);
            buttonOK.Text = "OK";
            buttonOK.DialogResult = DialogResult.OK;
            buttonOK.Click += ButtonOK_Click;

            Controls.Add(textBoxFormat);
            Controls.Add(buttonOK);

            Text = "Настройка формата кода";
            Width = 250;
            Height = 120;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            AcceptButton = buttonOK;
            CancelButton = buttonOK;
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            CodeFormat = textBoxFormat.Text;
            Close();
        }
    }
}