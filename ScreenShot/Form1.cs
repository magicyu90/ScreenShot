using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenShot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            timer1.Tick += Timer1_Tick;
            var intervalMinute = int.Parse(ConfigurationManager.AppSettings["intervalMinutes"]);
            timer1.Interval = 60 * 1000 * intervalMinute;
        }

        private static readonly string screenShotDir = @"D://screenshots//";

        private void btnStart_Click(object sender, EventArgs e)
        {

            Task.Factory.StartNew(() =>
            {

            }).ContinueWith(task =>
            {
                btnStart.Enabled = false;
                btnStop.Enabled = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());

            SaveImage();
            timer1.Start();

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {

            SaveImage();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            Task.Factory.StartNew(() =>
            {

            }).ContinueWith(task =>
            {
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void SaveImage()
        {
            if (!Directory.Exists(screenShotDir))
            {
                Directory.CreateDirectory(screenShotDir);
            }
            var currentDayStr = DateTime.Now.ToString("yyyyMMdd");
            var dayDir = screenShotDir + currentDayStr + "_Screen" + "//";
            if (!Directory.Exists(dayDir))
            {
                Directory.CreateDirectory(dayDir);
            }

            var name = DateTimeToUnixTimestamp(DateTime.Now);
            var filePath = dayDir + name + ".jpg";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                bitmap.Save(filePath, ImageFormat.Jpeg);
            }

        }

        private long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (Int64)(TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }
    }
}
