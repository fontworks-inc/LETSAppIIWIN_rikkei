using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Core.Entities;
using Core.Interfaces;
using NLog;
using OS.Interfaces;
using Prism.Ioc;
using Prism.Unity;
using CheckBox = System.Windows.Controls.CheckBox;

namespace Client.UI.Views
{
    /// <summary>
    /// DeviceModeApp.xaml の相互作用ロジック
    /// </summary>
    public partial class DeviceModeApp : Page
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        private IDeviceModeSettingRepository deviceModeSettingRepository = null;

        private IDeviceModeFontListRepository deviceModeFontListRepository = null;

        private IDeviceModeLicenseInfoRepository deviceModeLicenseInfoRepository = null;

        private IFontActivationService fontActivationService = null;

        private IFontFileRepository fontInfoRepository = null;

        private Paragraph paragraph;

        private IDictionary<int, string> letsKindMap = new Dictionary<int, string>();

        private string applicationSettingFolder = System.IO.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "..", "config");

        /// <summary>
        /// デバイスモードアプリ画面
        /// </summary>
        public DeviceModeApp()
        {
            this.InitializeComponent();

            IContainerProvider container = (System.Windows.Application.Current as PrismApplication).Container;
            this.deviceModeSettingRepository = container.Resolve<IDeviceModeSettingRepository>();
            this.deviceModeFontListRepository = container.Resolve<IDeviceModeFontListRepository>();
            this.deviceModeLicenseInfoRepository = container.Resolve<IDeviceModeLicenseInfoRepository>();
            this.fontActivationService = container.Resolve<IFontActivationService>();
            this.fontInfoRepository = container.Resolve<IFontFileRepository>();

            // ライセンスキーファイルパス初期表示
            if (this.deviceModeSettingRepository != null)
            {
                this.LicenseKeyFilePath.Content = System.IO.Path.GetFileName(this.deviceModeSettingRepository.GetDeviceModeSetting().LicenceFileKeyPath);
            }
            else
            {
                this.LicenseKeyFilePath.Content = string.Empty;
            }

            // フォントファイルパス初期表示
            if (this.deviceModeSettingRepository != null)
            {
                this.FontFilePath.Content = System.IO.Path.GetFileName(this.deviceModeSettingRepository.GetDeviceModeSetting().FontFilePath);
            }
            else
            {
                this.FontFilePath.Content = string.Empty;
            }

            // ライセンス情報初期表示
            this.LicenseInfoDisp();

            // アンインストール選択リスト初期表示
            this.UninstallListDisp();
        }

        /// <summary>
        /// ライセンス情報表示
        /// </summary>
        private void LicenseInfoDisp()
        {
            this.paragraph = new Paragraph();
            this.LicenseTerm.Document = new FlowDocument(this.paragraph);
            if (this.deviceModeLicenseInfoRepository != null)
            {
                var now = DateTime.Now;
                DeviceModeLicenseInfo deviceModeLicenseInfo = this.deviceModeLicenseInfoRepository.GetDeviceModeLicenseInfo();
                foreach (DeviceModeLicense deviceModeLicense in deviceModeLicenseInfo.DeviceModeLicenceList)
                {
                    this.letsKindMap.Add((int)deviceModeLicense.LetsKind, deviceModeLicense.LetsKindName);
                    if (now > deviceModeLicense.ExpireDate)
                    {
                        this.paragraph.Inlines.Add(new Run($"{deviceModeLicense.LetsKindName}     期限切れ") { Foreground = new SolidColorBrush(Colors.Red) });
                    }
                    else
                    {
                        string expireDate = deviceModeLicense.ExpireDate.ToString("yyyy.MM.dd");
                        this.paragraph.Inlines.Add($"{deviceModeLicense.LetsKindName} {expireDate}まで");
                    }

                    this.paragraph.Inlines.Add(new LineBreak());
                }
            }
        }

        /// <summary>
        /// アンインストール対象リスト表示
        /// </summary>
        private void UninstallListDisp()
        {
            IList<int> letsKindList = new List<int>();
            this.UninstallListBox.Items.Clear();
            if (this.deviceModeFontListRepository != null)
            {
                DeviceModeFontList deviceModeFontList = this.deviceModeFontListRepository.GetDeviceModeFontList();
                foreach (DeviceModeFontInfo fontInfo in deviceModeFontList.Fonts)
                {
                    if (!letsKindList.Contains((int)fontInfo.LetsKind))
                    {
                        var chkitem = new CheckBox();
                        chkitem.Content = this.letsKindMap[(int)fontInfo.LetsKind];
                        this.UninstallListBox.Items.Add(chkitem);
                        letsKindList.Add((int)fontInfo.LetsKind);
                    }
                }
            }
        }

        /// <summary>
        /// ライセンス情報登録メニュー選択
        /// </summary>
        private void LicenseRegistButton_Click(object sender, RoutedEventArgs e)
        {
            this.MenuButtonClear();
            this.LicenseRegistButton.Background = new SolidColorBrush(Colors.LightGray);

            this.AllPanelHIdden();
            this.LicenseRegistPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// フォントインストールメニュー選択
        /// </summary>
        private void FontInstallButton_Click(object sender, RoutedEventArgs e)
        {
            this.MenuButtonClear();
            this.FontInstallButton.Background = new SolidColorBrush(Colors.LightGray);

            this.AllPanelHIdden();
            this.FontInstallPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// フォントアンインストールメニュー選択
        /// </summary>
        private void FontUninstallButton_Click(object sender, RoutedEventArgs e)
        {
            this.MenuButtonClear();
            this.FontUninstallButton.Background = new SolidColorBrush(Colors.LightGray);

            this.AllPanelHIdden();
            this.FontUninstallPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// メニューボタン背景クリア
        /// </summary>
        private void MenuButtonClear()
        {
            this.LicenseRegistButton.Background = new SolidColorBrush(Colors.White);
            this.FontInstallButton.Background = new SolidColorBrush(Colors.White);
            this.FontUninstallButton.Background = new SolidColorBrush(Colors.White);
        }

        /// <summary>
        /// 機能別パネル非表示
        /// </summary>
        private void AllPanelHIdden()
        {
            this.LicenseRegistPanel.Visibility = Visibility.Hidden;
            this.FontInstallPanel.Visibility = Visibility.Hidden;
            this.FontUninstallPanel.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// ライセンス情報を取得ボタン処理
        /// </summary>
        private void LicenseOnlineButton_Click(object sender, RoutedEventArgs e)
        {
            // オンラインでライセンス情報を取得する
            // ファイルを所定の場所に保存する
            var deviceModeSetting = this.deviceModeSettingRepository.GetDeviceModeSetting();
            string filePath = deviceModeSetting.LicenceFileKeyPath;
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = System.IO.Path.Combine(this.applicationSettingFolder, "LicenseKeFile.dat");
                this.LicenseKeyFilePath.Content = System.IO.Path.GetFileName(filePath);
                deviceModeSetting.LicenceFileKeyPath = filePath;
                this.deviceModeSettingRepository.SaveDeviceModeSetting(deviceModeSetting);
            }
        }

        /// <summary>
        /// ライセンスキーを選択ボタン処理
        /// </summary>
        private void LicenseKeyFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "データファイル (*.dat)|*.dat|全てのファイル (*.*)|*.*";
            if (!string.IsNullOrEmpty(this.LicenseKeyFilePath.Content.ToString()))
            {
                dialog.FileName = this.LicenseKeyFilePath.Content.ToString();
            }

            // ダイアログを表示する
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // 選択されたファイル名 (ファイルパス) を画面に表示
                this.LicenseKeyFilePath.Content = System.IO.Path.GetFileName(dialog.FileName);
                var deviceModeSetting = this.deviceModeSettingRepository.GetDeviceModeSetting();
                deviceModeSetting.LicenceFileKeyPath = dialog.FileName;
                this.deviceModeSettingRepository.SaveDeviceModeSetting(deviceModeSetting);
            }
        }

        /// <summary>
        /// 登録ボタン処理
        /// </summary>
        private void RegistButton_Click(object sender, RoutedEventArgs e)
        {
            // ライセンスファイルからライセンス情報を取得し、設定ファイル保存と画面表示をおこなう
            var deviceModeSetting = this.deviceModeSettingRepository.GetDeviceModeSetting();
            string filePath = deviceModeSetting.LicenceFileKeyPath;

            if (!string.IsNullOrEmpty(filePath))
            {
                DeviceModeLicenseInfo licenseInfo = new DeviceModeLicenseInfo();

                // ファイルからライセンス情報を作成する

                // [デバイスモードライセンス]に保存する
                this.deviceModeLicenseInfoRepository.SaveDeviceModeLicenseInfo(licenseInfo);

                // ライセンス情報を再表示する
                this.LicenseInfoDisp();
            }
        }

        /// <summary>
        /// フォントを選択ボタン処理
        /// </summary>
        private void SelectFontFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "ZIPファイル (*.zip)|*.zip|全てのファイル (*.*)|*.*";
            if (!string.IsNullOrEmpty(this.FontFilePath.Content.ToString()))
            {
                dialog.FileName = this.FontFilePath.Content.ToString();
            }

            // ダイアログを表示する
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // 選択されたファイル名 (ファイルパス) を画面に表示
                this.FontFilePath.Content = System.IO.Path.GetFileName(dialog.FileName);
                var deviceModeSetting = this.deviceModeSettingRepository.GetDeviceModeSetting();
                deviceModeSetting.FontFilePath = dialog.FileName;
                this.deviceModeSettingRepository.SaveDeviceModeSetting(deviceModeSetting);
            }
        }

        /// <summary>
        /// インストールボタン処理
        /// </summary>
        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            // フォントファイルのパスを取得する
            var deviceModeSetting = this.deviceModeSettingRepository.GetDeviceModeSetting();
            string filePath = deviceModeSetting.FontFilePath;
            if (!string.IsNullOrEmpty(filePath))
            {
                string zipPassword = this.deviceModeLicenseInfoRepository.GetDeviceModeLicenseInfo().ZipPassword;

                if (!string.IsNullOrEmpty(zipPassword))
                {
                    // 一時フォルダを作成する
                    string tempPath = System.IO.Path.GetTempPath();
                    tempPath = System.IO.Path.Combine(tempPath, Guid.NewGuid().ToString());
                    Directory.CreateDirectory(tempPath);
                    try
                    {
                        // フォントファイルをUNZIPする
                        System.IO.Compression.ZipFile.ExtractToDirectory(zipPassword, tempPath);

                        IDictionary<int, DateTime> fontExpireMap = new Dictionary<int, DateTime>();
                        DateTime now = DateTime.Now;

                        // 展開されたフォルダ(/font/device, /font/windows/device)にあるフォントをインストールする
                        DeviceModeLicenseInfo deviceModeLicenseInfo = this.deviceModeLicenseInfoRepository.GetDeviceModeLicenseInfo();
                        string commonFontPath = System.IO.Path.Combine(tempPath, "/font/device");
                        string[] commonFiles = Directory.GetFiles(commonFontPath, "*");
                        foreach (string fontPath in commonFiles)
                        {
                            var fontIdInfo = this.fontInfoRepository.GetFontInfo(fontPath);
                            DeviceModeFontInfo deviceModeFontInfo = this.FontInstall(deviceModeLicenseInfo, fontExpireMap, now, fontIdInfo, fontPath);
                            if (deviceModeFontInfo != null)
                            {
                                DeviceModeFontList deviceModeFontList = this.deviceModeFontListRepository.GetDeviceModeFontList();
                                deviceModeFontList.Fonts.Add(deviceModeFontInfo);
                                this.deviceModeFontListRepository.SaveDeviceModeFontList(deviceModeFontList);
                            }
                        }

                        string winFontPath = System.IO.Path.Combine(tempPath, "/font/device");
                        string[] winFiles = Directory.GetFiles(winFontPath, "*");
                        foreach (string fontPath in winFiles)
                        {
                            var fontIdInfo = this.fontInfoRepository.GetFontInfo(fontPath);
                            DeviceModeFontInfo deviceModeFontInfo = this.FontInstall(deviceModeLicenseInfo, fontExpireMap, now, fontIdInfo, fontPath);
                            if (deviceModeFontInfo != null)
                            {
                                DeviceModeFontList deviceModeFontList = this.deviceModeFontListRepository.GetDeviceModeFontList();
                                deviceModeFontList.Fonts.Add(deviceModeFontInfo);
                                this.deviceModeFontListRepository.SaveDeviceModeFontList(deviceModeFontList);
                            }
                        }

                        // アンインストール対象リストを更新する
                        this.UninstallListDisp();
                    }
                    finally
                    {
                        // 一時フォルダを削除する
                        this.DeleteFolder(tempPath);
                    }
                }
            }
        }

        private DeviceModeFontInfo FontInstall(DeviceModeLicenseInfo deviceModeLicenseInfo, IDictionary<int, DateTime> fontExpireMap, DateTime now, FontIdInfo fontIdInfo, string filePath)
        {
            int letsKind = fontIdInfo.LetsKind;
            DateTime expireDate = DateTime.Now.AddDays(-1);

            if (fontExpireMap.ContainsKey(letsKind))
            {
                expireDate = fontExpireMap[letsKind];
            }
            else
            {
                foreach (DeviceModeLicense deviceModeLicense in deviceModeLicenseInfo.DeviceModeLicenceList)
                {
                    if (deviceModeLicense.LetsKind == letsKind)
                    {
                        expireDate = deviceModeLicense.ExpireDate;
                        fontExpireMap.Add(letsKind, expireDate);
                    }
                }
            }

            if (expireDate > now)
            {
                DeviceModeFontInfo deviceModeFontInfo = this.fontActivationService.InstallDeviceMode(filePath);
                return deviceModeFontInfo;
            }

            return null;
        }

        /// <summary>
        /// アンインストールボタン処理
        /// </summary>
        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            // ライセンス情報から、LEST種別名-LETS種別マップを作成する
            DeviceModeLicenseInfo deviceModeLicenseInfo = this.deviceModeLicenseInfoRepository.GetDeviceModeLicenseInfo();
            IDictionary<string, int> letsNameKindMap = new Dictionary<string, int>();
            foreach (DeviceModeLicense deviceModeLicense in deviceModeLicenseInfo.DeviceModeLicenceList)
            {
                letsNameKindMap.Add(deviceModeLicense.LetsKindName, (int)deviceModeLicense.LetsKind);
            }

            // アンインストール対象LETS種別をアンインストールする
            IList<int> uninstallLetsKindList = new List<int>();
            var items = this.UninstallListBox.Items;
            foreach (CheckBox item in items)
            {
                if ((bool)item.IsChecked)
                {
                    string letsKindName = item.Content.ToString();
                    if (letsNameKindMap.ContainsKey(letsKindName))
                    {
                        int letsKind = letsNameKindMap[letsKindName];
                        this.fontActivationService.UninstallDeviceMode(letsKind);
                    }
                }
            }
        }

        /// <summary>
        /// ヘルプボタン処理
        /// </summary>
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            string helpPath = System.IO.Path.Combine(this.applicationSettingFolder, "LETS-for-Device-help.pdf");

            Process.Start(new ProcessStartInfo("cmd", $"/c start {helpPath}") { CreateNoWindow = true });
        }

        /// <summary>
        /// 指定したディレクトリとその中身を全て削除する
        /// </summary>
        private void DeleteFolder(string targetDirectoryPath)
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                return;
            }

            // ディレクトリ以外の全ファイルを削除
            string[] filePaths = Directory.GetFiles(targetDirectoryPath);
            foreach (string filePath in filePaths)
            {
                try
                {
                    System.IO.File.SetAttributes(filePath, FileAttributes.Normal);
                    System.IO.File.Delete(filePath);
                }
                catch (Exception)
                {
                    // 消せないファイルは無視する
                }
            }

            // ディレクトリの中のディレクトリも再帰的に削除
            string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
            foreach (string directoryPath in directoryPaths)
            {
                this.DeleteFolder(directoryPath);
            }

            // 中が空になったらディレクトリ自身も削除
            try
            {
                Directory.Delete(targetDirectoryPath, false);
            }
            catch (Exception)
            {
                // 消せないフォルダも無視する
            }
        }
    }
}
