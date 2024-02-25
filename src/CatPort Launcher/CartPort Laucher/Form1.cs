using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CartPort_Laucher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await DownLoad(sender, e);
        }


        public async Task DownLoad(object sender, EventArgs e)
        {
            // 指定要下載的檔案的 URL 和要保存的檔案名稱
            string downloadUrl = "https://github.com/Nickyangtpe/CatPort/files/14395861/CatPort.zip";
            string fileName = "cartport.zip";
            string downloadPath = @"C:\temp"; // 保存下載的檔案的路徑
            string extractPath = Path.Combine(downloadPath, "CartPort"); // 保存提取的檔案的路徑

            // 檢查以前是否已下載過檔案
            bool oldVersionExists = File.Exists(Path.Combine(downloadPath, fileName));

            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                // 沒有網路連線
                MessageBox.Show("無法連線至網路。請檢查您的網路連線。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 創建 WebClient 實例來下載檔案
            using (WebClient client = new WebClient())
            {
                try
                {
                    Console.WriteLine("正在下載檔案...");
                    // 確保保存下載的檔案的目錄存在
                    Directory.CreateDirectory(downloadPath);
                    await client.DownloadFileTaskAsync(downloadUrl, Path.Combine(downloadPath, fileName));
                    Console.WriteLine("檔案下載成功。");

                    // 確保保存提取的檔案的目錄存在
                    Directory.CreateDirectory(extractPath);

                    // 提取下載的檔案
                    Console.WriteLine("正在提取檔案...");
                    using (ZipArchive archive = ZipFile.OpenRead(Path.Combine(downloadPath, fileName)))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            string entryFullPath = Path.Combine(extractPath, entry.FullName);
                            // 確保已創建目錄結構
                            Directory.CreateDirectory(Path.GetDirectoryName(entryFullPath));
                            // 提取條目（檔案）
                            entry.ExtractToFile(entryFullPath, true);
                        }
                    }
                    Console.WriteLine("提取完成。");

                    // 打開提取的資料夾
                    Process.Start(extractPath + @"\CatPort");
                    Close();
                }
                catch (Exception ex)
                {
                    if (oldVersionExists)
                    {
                        // 詢問用戶是否要使用舊版本
                        DialogResult result = MessageBox.Show("下載過程中發生錯誤。您要使用本地檔案嗎？", "錯誤", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (result == DialogResult.Yes)
                        {
                            // 使用本地檔案
                            Console.WriteLine("使用舊版本...");
                            // 打開提取的資料夾
                            Process.Start("explorer.exe", extractPath);
                        }
                    }
                    else
                    {
                        // 顯示錯誤訊息
                        MessageBox.Show($"下載過程中發生錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

    }
}
