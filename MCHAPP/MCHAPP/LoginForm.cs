using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCHAPP
{
    public partial class LoginForm : Form
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public LoginForm()
        {
            InitializeComponent();
            lblLoggingIn.Visible = false;
            string path =
                AppDomain.CurrentDomain.BaseDirectory + "Dp676767766.txt";
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line = sr.ReadLine();
                    txtEmailAddress.Text = line;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtEmailAddress.Text = txtPassword.Text = "";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEmailAddress.Text))
            {
                MessageBox.Show(@"Enter your email address.");
            }
            //else if (string.IsNullOrEmpty(txtPassword.Text))
            //{
            //    MessageBox.Show(@"Enter your password.");
            //}
            else
            {
                groupBox1.Cursor = Cursors.WaitCursor;
                lblLoggingIn.Visible = true;
                btnLogin.Visible = btnClear.Visible = false;

                try
                {
                    Thread threadInput = new Thread(DisplayData);
                    threadInput.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void DisplayData()
        {
            try
            {
                string firstMacAddress = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();
                db = new ApplicationDbContext();
                var user = db.Users.Include(i => i.HealthFacility)
                    .SingleOrDefault(i => i.Email == txtEmailAddress.Text);

                if (user != null)
                {
                    if (user.IsLoggedIn /*&& user.MacAddress == firstMacAddress*/)
                    {
                        if (user.HealthFacility == null)
                        {
                            MessageBox.Show(@"User not assigned to any health facility.");
                            this.Invoke((MethodInvoker)delegate
                            {
                                btnLogin.Visible = btnClear.Visible = true;
                                lblLoggingIn.Visible = false;
                                groupBox1.Cursor = Cursors.Arrow;
                            });
                        }
                        else
                        {
                            Program.FacilityName = user.HealthFacility.Name;
                            Program.FacilityId = user.HealthFacility.Id;
                            Program.UserId = user.Id;
                            var loginForm = new Form1()
                            {
                                Text = Program.FacilityName,
                                MdiParent = this.ParentForm.MdiParent,
                                AutoScaleMode = AutoScaleMode.Dpi,
                                //  WindowState = FormWindowState.Maximized
                            };
                            string path =
                                AppDomain.CurrentDomain.BaseDirectory + "Dp676767766.txt";

                            // Write the string array to a new file named "WriteLines.txt".
                            using (StreamWriter outputFile = new StreamWriter(path))
                            {
                                outputFile.WriteLine(txtEmailAddress.Text);
                            }
                            this.Invoke((MethodInvoker)delegate
                            {
                                loginForm.MdiParent = this.MdiParent;
                                loginForm.Show();
                                groupBox1.Cursor = Cursors.Arrow;
                                this.Close();
                            });
                        }
                    }
                    else
                    {
                        MessageBox.Show(@"User not logged in the MIS. Login to the MIS first.");
                        this.Invoke((MethodInvoker)delegate
                        {
                            btnLogin.Visible = btnClear.Visible = true;
                            lblLoggingIn.Visible = false;
                            groupBox1.Cursor = Cursors.Arrow;
                        });
                    }
                }
                else
                {
                    // MessageBox.Show(@"Email or Password incorrect.");
                    MessageBox.Show(@"Email address not registered.");
                    this.Invoke((MethodInvoker)delegate
                    {
                        btnLogin.Visible = btnClear.Visible = true;
                        lblLoggingIn.Visible = false;
                        groupBox1.Cursor = Cursors.Arrow;
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Invoke((MethodInvoker)delegate
                {
                    btnLogin.Visible = btnClear.Visible = true;
                    lblLoggingIn.Visible = false;
                    groupBox1.Cursor = Cursors.Arrow;
                });
            }
        }

        private void progressBar_Click(object sender, EventArgs e)
        {
        }
    }
}