using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Solution {
    public partial class Form1 : Form {

        private static Random random = new Random();
        private double currentWeight = 0.0;
        private List<string> codes = new List<string>(); 
        private double remainingWeight;
        private double targetWeight = 0.0;
        private ListBox listBoxCodes = new ListBox();
        private static int maxWeight = 0;

        public Form1() { 
            InitializeComponent();
            InitializeListBox();
        }

        private void InitializeListBox(){
            listBoxCodes.Location = new System.Drawing.Point(220, 220);
            listBoxCodes.Size = new System.Drawing.Size(300, 100);
            Controls.Add(listBoxCodes);
        }


        public static string GenerateRandomCode() {
            StringBuilder sb = new StringBuilder();
            sb.Append("(01)54610268019390(3103)");
            Console.WriteLine(maxWeight);
            int randomValue = random.Next(1, (int)maxWeight);
            sb.Append(randomValue.ToString());
            sb.Append("(11)230630(10)3999(21)0001");
            return sb.ToString();
        }

        private void button1_Click(object sender, EventArgs e) {
            if (double.TryParse(textBox1.Text, out targetWeight)){
                int.TryParse(textBox2.Text, out maxWeight);
                currentWeight = 0.0;

                for (int i = 0; i < 1; i++){
                    string randomCode = GenerateRandomCode();
                    listBoxCodes.Items.Add(randomCode);
                    codes.Add(randomCode);
                }
                remainingWeight = targetWeight - currentWeight;
                label4.Text = remainingWeight.ToString();

                ProcessProductCodes(codes);
            }
        }
        private void ProcessProductCodes(List<string> productCodes) {
            foreach (string code in productCodes) {
                if (code.StartsWith("(01)") && code.Length >= 15) {
                    int startIndex = code.IndexOf("(3103)") + 6;
                    int endIndex = code.IndexOf("(11)", startIndex);

                    if (endIndex != -1 && int.TryParse(code.Substring(startIndex, endIndex - startIndex), out int weight)) {
                        Console.WriteLine(weight);
                        currentWeight += weight;
                    }
                }
            }
            remainingWeight = targetWeight - currentWeight;
            label4.Text = remainingWeight.ToString();

            if (currentWeight >= targetWeight){
                MessageBox.Show("Достигнут заданный вес продукта! Сброс!", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetValues();
                return;
            }

            if (remainingWeight - maxWeight <= 0) {
                MessageBox.Show("Возможно переполнение, так как максимальный вес продукта (" + maxWeight + ") " +
                    "больше или равен оставшемуся весу (" + remainingWeight + ").");
            }
        }

        private void ResetValues() {
            currentWeight = 0.0;
            textBox1.Clear();
            textBox2.Clear();
            remainingWeight = targetWeight;
            codes.Clear();
            listBoxCodes.Items.Clear();
            label4.Text = "0";
        }
    }
}

