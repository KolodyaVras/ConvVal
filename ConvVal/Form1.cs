using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConvVal.DailyInfo;

namespace ConvVal
{
    public partial class Form1 : Form
    {
        Dictionary<string, double> dict = new Dictionary<string, double>();

        public Form1()
        {
            InitializeComponent();
            FillDict(ref dict);
            AddItems(ref boxValToConv, dict);
            AddItems(ref boxCelVal, dict);

            boxValToConv.SelectedIndex = 11;
            boxCelVal.SelectedIndex = 0;            
        }

        public static Dictionary<string, double> FillDict(ref Dictionary<string, double> dict)
        {
            DailyInfoSoapClient client = new DailyInfoSoapClient();
            dict.Add("Российский рубль", 1);
            foreach (DataTable table in client.GetCursOnDate(On_date: DateTime.Now).Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    dict.Add(row[0].ToString().Substring(0, 50), Convert.ToDouble(row[2]) / Convert.ToDouble(row[1]));
                }
            }

            client.Close();
            return dict;
        }

        public static ComboBox AddItems(ref ComboBox box, Dictionary<string, double> dict)
        {
            foreach (var val in dict)
                box.Items.Add(val.Key);

            return box;
        }

        private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && number != 8 && number != 44) // цифры, клавиша BackSpace и запятая
            {
                e.Handled = true;
            }
        }

        private void TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtInput.Text == "")
                {
                    txtResult.Text = "0";
                    return;
                }

                double toConv = 1, celVal = 1;
                foreach (var val in dict)
                {
                    if (val.Key == boxValToConv.SelectedItem.ToString())
                        toConv = val.Value;
                    if (val.Key == boxCelVal.SelectedItem.ToString())
                        celVal = val.Value;

                    txtResult.Text = (toConv * Convert.ToDouble(txtInput.Text) / celVal).ToString();
                }
            }
            catch
            {
                txtResult.Text = "ERROR";
            }
        }
    }
}
