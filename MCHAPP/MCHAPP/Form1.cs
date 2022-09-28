using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using libzkfpcsharp;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using Sample;

namespace MCHAPP
{
    public partial class Form1 : Form
    {
        private IntPtr mDevHandle = IntPtr.Zero;
        private IntPtr mDBHandle = IntPtr.Zero;
        private IntPtr FormHandle = IntPtr.Zero;
        private bool bIsTimeToDie = false;
        private bool IsRegister = false;
        private bool IsChanging = false;
        private bool bIdentify = true;
        private byte[] FPBuffer;
        private int RegisterCount = 0;
        private string fileNamePrefix = null;
        private const int REGISTER_FINGER_COUNT = 3;

        private byte[][] RegTmps = new byte[3][];
        private byte[] RegTmp = new byte[2048];
        private byte[] CapTmp = new byte[2048];
        private int cbCapTmp = 2048;
        private int cbRegTmp = 0;
        private int iFid = 1;

        private int mfpWidth = 0;
        private int mfpHeight = 0;
        private ApplicationDbContext db = new ApplicationDbContext();
        private const int MESSAGE_CAPTURED_OK = 0x0400 + 6;

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private HouseholdReg mother = new HouseholdReg();
        private Change change = new Change();

        public Form1()
        {
            InitializeComponent();
            lblHealthFacilityName.Text = Program.FacilityName;
        }

        private void bnInit_Click(object sender, EventArgs e)
        {
            cmbIdx.Items.Clear();
            int ret = zkfperrdef.ZKFP_ERR_OK;
            if ((ret = zkfp2.Init()) == zkfperrdef.ZKFP_ERR_OK)
            {
                int nCount = zkfp2.GetDeviceCount();
                if (nCount > 0)
                {
                    for (int i = 0; i < nCount; i++)
                    {
                        cmbIdx.Items.Add(i.ToString());
                    }
                    cmbIdx.SelectedIndex = 0;
                    bnInit.Enabled = false;
                    bnFree.Enabled = true;
                    bnOpen.Enabled = true;
                }
                else
                {
                    zkfp2.Terminate();
                    MessageBox.Show("No device connected!");
                }
            }
            else
            {
                MessageBox.Show("Initialize fail, ret=" + ret + " !");
            }

            ret = zkfp.ZKFP_ERR_OK;
            if (IntPtr.Zero == (mDevHandle = zkfp2.OpenDevice(cmbIdx.SelectedIndex)))
            {
                MessageBox.Show("OpenDevice fail");
                return;
            }
            if (IntPtr.Zero == (mDBHandle = zkfp2.DBInit()))
            {
                MessageBox.Show("Init DB fail");
                zkfp2.CloseDevice(mDevHandle);
                mDevHandle = IntPtr.Zero;
                return;
            }
            bnInit.Enabled = false;
            bnFree.Enabled = true;
            bnOpen.Enabled = false;
            bnClose.Enabled = true;
            bnEnroll.Enabled = true;
            btnChangeFingerPrint.Enabled = true;
            bnVerify.Enabled = true;
            bnIdentify.Enabled = true;
            RegisterCount = 0;
            cbRegTmp = 0;
            iFid = 1;
            for (int i = 0; i < 3; i++)
            {
                RegTmps[i] = new byte[2048];
            }
            byte[] paramValue = new byte[4];
            int size = 4;
            zkfp2.GetParameters(mDevHandle, 1, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpWidth);

            size = 4;
            zkfp2.GetParameters(mDevHandle, 2, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpHeight);

            FPBuffer = new byte[mfpWidth * mfpHeight];

            Thread captureThread = new Thread(new ThreadStart(DoCapture));
            captureThread.IsBackground = true;
            captureThread.Start();
            bIsTimeToDie = false;
            textRes.Text = @"Device ready for finger print taking.";
        }

        private void bnFree_Click(object sender, EventArgs e)
        {
            zkfp2.Terminate();
            cbRegTmp = 0;
            bnInit.Enabled = true;
            bnFree.Enabled = false;
            bnOpen.Enabled = false;
            bnClose.Enabled = false;
            bnEnroll.Enabled = false;
            btnChangeFingerPrint.Enabled = false;
            bnVerify.Enabled = false;
            bnIdentify.Enabled = false;
        }

        private void bnOpen_Click(object sender, EventArgs e)
        {
            lblName.Text = "";
            int ret = zkfp.ZKFP_ERR_OK;
            if (IntPtr.Zero == (mDevHandle = zkfp2.OpenDevice(cmbIdx.SelectedIndex)))
            {
                MessageBox.Show("OpenDevice fail");
                return;
            }
            if (IntPtr.Zero == (mDBHandle = zkfp2.DBInit()))
            {
                MessageBox.Show("Init DB fail");
                zkfp2.CloseDevice(mDevHandle);
                mDevHandle = IntPtr.Zero;
                return;
            }
            bnInit.Enabled = false;
            bnFree.Enabled = true;
            bnOpen.Enabled = false;
            bnClose.Enabled = true;
            bnEnroll.Enabled = true;
            btnChangeFingerPrint.Enabled = true;
            bnVerify.Enabled = true;
            bnIdentify.Enabled = true;
            RegisterCount = 0;
            cbRegTmp = 0;
            iFid = 1;
            for (int i = 0; i < 3; i++)
            {
                RegTmps[i] = new byte[2048];
            }
            byte[] paramValue = new byte[4];
            int size = 4;
            zkfp2.GetParameters(mDevHandle, 1, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpWidth);

            size = 4;
            zkfp2.GetParameters(mDevHandle, 2, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpHeight);

            FPBuffer = new byte[mfpWidth * mfpHeight];

            Thread captureThread = new Thread(new ThreadStart(DoCapture));
            captureThread.IsBackground = true;
            captureThread.Start();
            bIsTimeToDie = false;
            textRes.Text = @"Device Opened. Select Enrol or Verify";
        }

        private void DoCapture()
        {
            while (!bIsTimeToDie)
            {
                cbCapTmp = 2048;
                int ret = zkfp2.AcquireFingerprint(mDevHandle, FPBuffer, CapTmp, ref cbCapTmp);
                if (ret == zkfp.ZKFP_ERR_OK)
                {
                    SendMessage(FormHandle, MESSAGE_CAPTURED_OK, IntPtr.Zero, IntPtr.Zero);
                }
                Thread.Sleep(200);
            }
        }

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case MESSAGE_CAPTURED_OK:
                    {
                        MemoryStream ms = new MemoryStream();
                        BitmapFormat.GetBitmap(FPBuffer, mfpWidth, mfpHeight, ref ms);
                        Bitmap bmp = new Bitmap(ms);
                        this.picFPImg.Image = bmp;
                        var dir = Application.StartupPath + "\\images\\";
                        if (IsRegister)
                        {
                            var facilityId = Program.FacilityId;
                            var userId = Program.UserId;

                            int ret = zkfp.ZKFP_ERR_OK;
                            int fid = 0, score = 0;
                            ret = zkfp2.DBIdentify(mDBHandle, CapTmp, ref fid, ref score);
                            /* if (zkfp.ZKFP_ERR_OK == ret)
                             {
                                 textRes.Text = "This finger was already register by " + fid + "!";
                                 return;
                             }
                             */
                            if (RegisterCount > 0 && zkfp2.DBMatch(mDBHandle, CapTmp, RegTmps[RegisterCount - 1]) <= 0)
                            {
                                textRes.Text = @"Please press the same finger 3 times for the enrolment";
                                return;
                            }
                            Array.Copy(CapTmp, RegTmps[RegisterCount], cbCapTmp);
                            String strBase64 = zkfp2.BlobToBase64(CapTmp, cbCapTmp);
                            byte[] blob = zkfp2.Base64ToBlob(strBase64);

                            if (fileNamePrefix == null)
                            {
                                fileNamePrefix = DateTime.Now.ToString("yyyymmddhhmmss") + ".bmp";
                            }

                            var fileName = "";
                            if (IsChanging) // Changing Fingerprint
                            {
                                fileName = change.Household.Mother.FullName.Replace(" ", "_") + "_" + RegisterCount + "_" + fileNamePrefix;
                            }
                            else
                            {
                                fileName = mother.Mother.FullName.Replace(" ", "_") + "_" + RegisterCount + "_" + fileNamePrefix;
                            }

                            // File.WriteAllBytes(dir + fileName, ms.ToArray());

                            RegisterCount++;
                            if (RegisterCount >= REGISTER_FINGER_COUNT)
                            {
                                RegisterCount = 0;
                                if (zkfp.ZKFP_ERR_OK == (ret = zkfp2.DBMerge(mDBHandle, RegTmps[0], RegTmps[1], RegTmps[2], RegTmp, ref cbRegTmp)) &&
                                       zkfp.ZKFP_ERR_OK == (ret = zkfp2.DBAdd(mDBHandle, iFid, RegTmp)))
                                {
                                    iFid++;
                                    lblName.Text = @"Enrolled";

                                    if (IsChanging)
                                        textRes.Text = @"Fingerprint taken successful.";
                                    else
                                        textRes.Text = @"Fingerprint enrolled successfully. " + Environment.NewLine + Environment.NewLine + @"You can go back to the MIS and continue with the registration process.";

                                    fileNamePrefix = DateTime.Now.ToString("yyyymmddhhmmss") + ".bmp";

                                    // File.WriteAllBytes(dir + fileName, RegTmp.ToArray());
                                    if (IsChanging) // Changing Fingerprint
                                    {
                                        // fileName = change.Household.Mother.FullName.Replace(" ", "_") + "_Template_" + fileNamePrefix;
                                        change.FingerPrint = RegTmp.ToArray();
                                        change.TakingFingerPrint = false;
                                        change.FingerPrintVerified = true;
                                        change.RawFingerPrint = RegTmps[2];
                                    }
                                    else
                                    {
                                        // fileName = mother.Mother.FullName.Replace(" ", "_") + "_Template_" + fileNamePrefix;
                                        mother.FingerPrint = RegTmp.ToArray();
                                        mother.RawFingerPrint = RegTmps[2];
                                        mother.VerifyingFingerPrint = false;
                                    }

                                    db.SaveChanges();
                                    picFPImg.Image = null;
                                }
                                else
                                {
                                    textRes.Text = @"Sorry, enrolment failed. " + Environment.NewLine + @"Restart the finger print application and try again.";
                                }
                                IsRegister = false;
                                return;
                            }
                            else
                            {
                                textRes.Text = @"Press finger " + (REGISTER_FINGER_COUNT - RegisterCount) + " more time" + (RegisterCount > 1 ? "" : "s");
                            }
                        }
                        else
                        {
                            //if (cbRegTmp <= 0)
                            //{
                            //    textRes.Text = "Please register your finger first!";
                            //    return;
                            //}
                            if (bIdentify)
                            {
                                int ret = zkfp.ZKFP_ERR_OK;
                                int fid = 0, score = 0;
                                ret = zkfp2.DBIdentify(mDBHandle, CapTmp, ref fid, ref score);
                                if (zkfp.ZKFP_ERR_OK == ret)
                                {
                                    textRes.Text = "Identify succ, fid= " + fid + ",score=" + score + "!";
                                    return;
                                }
                                else
                                {
                                    textRes.Text = "Identify fail, ret= " + ret;
                                    return;
                                }
                            }
                            else
                            {
                                cbCapTmp = 2048;

                                //int var1= zkfp2.ExtractFromImage(mDBHandle,dir + "0-20182713022709.bmp",8,RegTmps[0], ref cbCapTmp);
                                //int var2=zkfp2.ExtractFromImage(mDBHandle,dir + "1-20182713022709.bmp",8,RegTmps[1], ref cbCapTmp);
                                //int var3= zkfp2.ExtractFromImage(mDBHandle,dir + "2-20182713022709.bmp",8,RegTmps[2], ref cbCapTmp);
                                //using (FileStream file = new FileStream(dir + "0-20182713022709.bmp", FileMode.Open, FileAccess.Read))
                                //{
                                //    byte[] bytes = new byte[2048];
                                //    file.Read(bytes, 0, (int)file.Length);
                                //    ms.Write(bytes, 0, (int)file.Length);
                                //    RegTmps[0] = bytes;
                                //}
                                var facilityId = Program.FacilityId;
                                var userId = Program.UserId;
                                var verification = db.FingerPrintVerifications
                                    .FirstOrDefault(i => i.IsVerifying &&
                                    i.HealthFacilityId == facilityId);

                                var mother = db.HouseholdReg.SingleOrDefault(i => i.Id == verification.HouseholdId
                                && i.HealthFacilityId == facilityId);

                                using (MemoryStream ms2 = new MemoryStream(mother.FingerPrint))
                                {
                                    RegTmp = ms2.ToArray();
                                }
                                // RegTmp =File.ReadAllBytes (dir + "Combined-20183713033728.bmp");

                                int ret = zkfp2.DBMatch(mDBHandle, CapTmp, RegTmp);

                                // RegTmp

                                if (0 < ret)
                                {
                                    textRes.Text = @"Finger print matched, score=" + ret + @"!" + Environment.NewLine + Environment.NewLine + @"You can go back to the MIS and continue with the registration process.";
                                    lblName.Text = @"Verified";

                                    verification.Verified = true;
                                    verification.IsVerifying = false;
                                    db.FingerPrintVerifications.AddOrUpdate(verification);
                                    db.SaveChanges();
                                    picFPImg.Image = null;
                                }
                                else
                                {
                                    textRes.Text = @"Finger print does not match with the one registered, score= " + ret;
                                    return;
                                }
                            }
                        }
                    }
                    break;

                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FormHandle = this.Handle;
        }

        private void bnClose_Click(object sender, EventArgs e)
        {
            bIsTimeToDie = true;
            RegisterCount = 0;
            Thread.Sleep(1000);
            zkfp2.CloseDevice(mDevHandle);
            bnInit.Enabled = false;
            bnFree.Enabled = true;
            bnOpen.Enabled = true;
            bnClose.Enabled = false;
            bnEnroll.Enabled = false;
            btnChangeFingerPrint.Enabled = false;
            bnVerify.Enabled = false;
            bnIdentify.Enabled = false;
        }

        private void bnEnroll_Click(object sender, EventArgs e)
        {
            try
            {
                db = new ApplicationDbContext();
                IsChanging = false;
                IsRegister = false;
                var facilityId = Program.FacilityId;
                var userId = Program.UserId;
                if (!IsRegister)
                {
                    mother = db.HouseholdReg.Include(i => i.Mother)
                       .SingleOrDefault(i => i.VerifyingFingerPrint && i.HealthFacilityId == facilityId);

                    if (mother == null)
                    {
                        IsRegister = false;
                        RegisterCount = 0;
                        cbRegTmp = 0;
                        // textRes.Text = @"There is no mother ready for enrolment";
                        MessageBox.Show(@"There is no mother ready for enrolment");
                    }
                    else
                    {
                        lblName.Text = mother.Mother.FullName;
                        lblName.Visible = true;
                        IsRegister = true;
                        RegisterCount = 0;
                        cbRegTmp = 0;
                        textRes.Text = @"Enrolling " + mother.Mother.FullName + Environment.NewLine + @"Press finger 3 times!";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnChangeFingerPrint_Click(object sender, EventArgs e)
        {
            try
            {
                db = new ApplicationDbContext();
                IsRegister = false;
                IsChanging = true;
                var facilityId = Program.FacilityId;
                var userId = Program.UserId;

                if (!IsRegister)
                {
                    change = db.Changes.Include(i => i.Household.Mother)
                        .SingleOrDefault(i => i.TakingFingerPrint && i.Household.HealthFacilityId == facilityId);

                    if (change == null)
                    {
                        IsRegister = false;
                        RegisterCount = 0;
                        cbRegTmp = 0;
                        // textRes.Text = @"There is no mother ready for enrolment";
                        MessageBox.Show(@"There is no mother ready for change of fingerprints");
                    }
                    else
                    {
                        lblName.Text = change.Household.Mother.FullName;
                        lblName.Visible = true;
                        IsRegister = true;
                        RegisterCount = 0;
                        cbRegTmp = 0;
                        textRes.Text = @"Changing fingerprints for " + change.Household.Mother.FullName + Environment.NewLine + Environment.NewLine + @"Press finger 3 times!";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bnIdentify_Click(object sender, EventArgs e)
        {
            if (!bIdentify)
            {
                bIdentify = true;
                textRes.Text = "Please press your finger!";
            }
        }

        private void bnVerify_Click(object sender, EventArgs e)
        {
            try
            {
                db = new ApplicationDbContext();
                groupBox1.Cursor = Cursors.Arrow;
                lblName.Text = "";

                var facilityId = Program.FacilityId;
                var userId = Program.UserId;
                var verification = db.FingerPrintVerifications
                    .FirstOrDefault(i => i.IsVerifying &&
                                         i.HealthFacilityId == facilityId);
                if (verification != null)
                {
                    var mother = db.HouseholdReg.Include(i => i.Mother)
                        .SingleOrDefault(i => i.Id == verification.HouseholdId && i.HealthFacilityId == facilityId);
                    if (mother == null)
                    {
                        MessageBox.Show(@"No mother found for verification");
                    }
                    else
                    {
                        lblName.Text = mother.Mother.FullName;
                        lblName.Visible = true;
                        bIdentify = false;

                        textRes.Text = @"Please press finger!";
                    }
                    groupBox1.Cursor = Cursors.Default;
                }
                else
                {
                    textRes.Text = @"";
                    MessageBox.Show(@"No mother found ready for finger print verification");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbIdx_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
        }
    }
}