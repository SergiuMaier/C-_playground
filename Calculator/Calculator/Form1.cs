using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator
{
    public partial class Form1 : Form
    {
        Double valoare = 0;
        String operatie = "";
        bool calcul = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, EventArgs e)  
        {
            if ((afisare.Text == "0") || (calcul))
                afisare.Clear();

            calcul = false;
            Button b = (Button)sender;

            if (b.Text == ".")  
            {
                if (!afisare.Text.Contains("."))
                    afisare.Text = afisare.Text + b.Text;
            }
            else
                afisare.Text = afisare.Text + b.Text;
        }

        private void button5_Click(object sender, EventArgs e)  
        {
            afisare.Text = "0";
        }

        private void operatie_click(object sender, EventArgs e)  
        {
            Button b = (Button)sender;
            
            if(valoare != 0)
            {
                button19.PerformClick();
                calcul = true;
                operatie = b.Text;
                ecuatie.Text = valoare + " " + operatie + " "; //Label-ul din partea stanga
            }   
            else
            { 
                operatie = b.Text;
                valoare = Double.Parse(afisare.Text);
                calcul = true;
                ecuatie.Text = valoare + " " + operatie + " ";
            }
        }

        private void button19_Click(object sender, EventArgs e)  
        {
            ecuatie.Text = "";
            switch (operatie)
            {
                case "+":
                    afisare.Text = (valoare + Double.Parse(afisare.Text)).ToString();
                    break;
                case "-":
                    afisare.Text = (valoare - Double.Parse(afisare.Text)).ToString();
                    break;
                case "*":
                    afisare.Text = (valoare * Double.Parse(afisare.Text)).ToString();
                    break;
                case "/":
                    afisare.Text = (valoare / Double.Parse(afisare.Text)).ToString();
                    break;
                default: break;
            }
            valoare = Int32.Parse(afisare.Text);
            operatie = "";
            
        }
        private void button10_Click(object sender, EventArgs e) 
        {
            afisare.Clear();
            afisare.Text = "0";
        }

        
    }
}