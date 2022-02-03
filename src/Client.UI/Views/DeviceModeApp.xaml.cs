using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ApplicationService.Interfaces;
using Client.UI.Wrappers;
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

        private IDeviceModeService deviceModeService = null;

        /// <summary>
        /// 指定のプロセスを実施するサービス
        /// </summary>
        private IStartProcessService startProcessService;

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
            this.deviceModeService = container.Resolve<IDeviceModeService>();
            this.startProcessService = container.Resolve<IStartProcessService>();

            // ファイルパスクリア
            this.FilePathClear();

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
        private string LicenseInfoDisp()
        {
            List<string> letsKindList = new List<string>();
            string letsKinds = string.Empty;
            this.paragraph = new Paragraph();
            this.paragraph.FontFamily = new FontFamily("ＭＳ ゴシック");
            this.LicenseTerm.Document = new FlowDocument(this.paragraph);
            if (this.deviceModeLicenseInfoRepository != null)
            {
                var now = DateTime.Now;
                DeviceModeLicenseInfo deviceModeLicenseInfo = this.deviceModeLicenseInfoRepository.GetDeviceModeLicenseInfo();
                if (this.letsKindMap != null)
                {
                    this.letsKindMap.Clear();
                }

                foreach (DeviceModeLicense deviceModeLicense in deviceModeLicenseInfo.DeviceModeLicenceList)
                {
                    this.letsKindMap.Add((int)deviceModeLicense.LetsKind, deviceModeLicense.LetsKindName);
                    letsKindList.Add(deviceModeLicense.LetsKindName);
                    Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                    string fontname = $"{deviceModeLicense.LetsKindName}";
                    int bytes = sjisEnc.GetByteCount(fontname);
                    for (int n = bytes; n < 24; n++)
                    {
                        fontname += " ";
                    }

                    if (now > deviceModeLicense.ExpireDate.AddDays(1))
                    {
                        this.paragraph.Inlines.Add(new Run($"{fontname} 期限切れ") { Foreground = new SolidColorBrush(Colors.Red) });
                    }
                    else
                    {
                        string expireDate = deviceModeLicense.ExpireDate.ToString("yyyy.MM.dd");
                        this.paragraph.Inlines.Add($"{fontname} {expireDate}まで");
                    }

                    this.paragraph.Inlines.Add(new LineBreak());
                }
            }

            return string.Join("、", letsKindList);
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
                    if ((bool)fontInfo.IsRemove)
                    {
                        continue;
                    }

                    if (!letsKindList.Contains((int)fontInfo.LetsKind))
                    {
                        var chkitem = new CheckBox();

                        if (this.letsKindMap.ContainsKey((int)fontInfo.LetsKind))
                        {
                            chkitem.Content = this.letsKindMap[(int)fontInfo.LetsKind];
                            this.UninstallListBox.Items.Add(chkitem);
                            letsKindList.Add((int)fontInfo.LetsKind);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ライセンス情報登録メニュー選択
        /// </summary>
        private void LicenseRegistButton_Click(object sender, RoutedEventArgs e)
        {
            this.FilePathClear();

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
            this.FilePathClear();

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
            this.FilePathClear();

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

        private void FilePathClear()
        {
            if (this.deviceModeSettingRepository == null)
            {
                return;
            }

            var deviceModeSetting = this.deviceModeSettingRepository.GetDeviceModeSetting();
            deviceModeSetting.FontFilePath = string.Empty;
            deviceModeSetting.LicenceFileKeyPath = string.Empty;
            this.deviceModeSettingRepository.SaveDeviceModeSetting(deviceModeSetting);

            this.FontFilePath.Content = string.Empty;
            this.LicenseKeyFilePath.Content = string.Empty;
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
            var deviceModeSetting = this.deviceModeSettingRepository.GetDeviceModeSetting();
            this.ErrorMessage.Text = string.Empty;
            this.ErrorMessage.Visibility = Visibility.Hidden;

            try
            {
                DeviceModeLicenseInfo deviceModeLicenseInfo = this.deviceModeLicenseInfoRepository.GetDeviceModeLicenseInfo(true, deviceModeSetting.OfflineDeviceID, deviceModeSetting.IndefiniteAccessToken, null, deviceModeSetting.LicenseDecryptionKey);

                this.deviceModeLicenseInfoRepository.SaveDeviceModeLicenseInfo(deviceModeLicenseInfo);

                // ライセンス情報を表示する。
                string letsKinds = this.LicenseInfoDisp();
                if (string.IsNullOrEmpty(letsKinds))
                {
                   this.ToastShow("ライセンス登録", "デバイスに対応するライセンスがありません。ライセンスキーファイルが適切であるか確認してください。");
                }
                else
                {
                    this.ToastShow("ライセンス登録", $"ライセンスを更新しました。 LETS種別：{letsKinds}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
                this.ErrorMessage.Text = "通信エラーが発生しました。";
                this.ErrorMessage.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// ライセンスキーを選択ボタン処理
        /// </summary>
        private void LicenseKeyFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "データファイル (*.csv)|*.csv|全てのファイル (*.*)|*.*";
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

            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    string license = string.Empty;
                    string jsonText = string.Empty;

                    // ファイルからライセンス情報を作成する
                    string[] csvLines = File.ReadAllLines(filePath);

                    string ownDevId = deviceModeSetting.OfflineDeviceID;
                    foreach (string line in csvLines)
                    {
                        string[] fields = line.Split(",");
                        string devid = fields[0].Trim(new char[] { '"' });

                        if (devid == ownDevId)
                        {
                            string licenseBase64 = fields[1].Trim(new char[] { '"' });

                            // ライセンスキーを解凍する
                            byte[] encrypted = Convert.FromBase64String(licenseBase64);

                            // AES復号化を行う
                            RijndaelManaged aes = new RijndaelManaged();
                            aes.BlockSize = 128;
                            aes.KeySize = 128;
                            aes.Padding = PaddingMode.Zeros;
                            aes.Mode = CipherMode.ECB;
                            aes.Key = Convert.FromBase64String(deviceModeSetting.LicenseDecryptionKey);

                            ICryptoTransform decryptor = aes.CreateDecryptor();

                            byte[] planeText = new byte[encrypted.Length];

                            MemoryStream memoryStream = new MemoryStream(encrypted);
                            CryptoStream cryptStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                            cryptStream.Read(planeText, 0, planeText.Length);
                            jsonText = System.Text.Encoding.UTF8.GetString(planeText);
                            int lastClosingParenthesis = jsonText.LastIndexOf('}');
                            if (lastClosingParenthesis > 0)
                            {
                                jsonText = jsonText.Substring(0, lastClosingParenthesis + 1);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(jsonText))
                    {
                        DeviceModeLicenseInfo licenseInfo = this.deviceModeLicenseInfoRepository.CreateLicenseInfoFromJsonText(jsonText);

                        // [デバイスモードライセンス]に保存する
                        this.deviceModeLicenseInfoRepository.SaveDeviceModeLicenseInfo(licenseInfo);

                        // ライセンス情報を表示する。
                        string letsKinds = this.LicenseInfoDisp();
                        if (string.IsNullOrEmpty(letsKinds))
                        {
                            this.ToastShow("ライセンス登録", "デバイスに対応するライセンスがありません。ライセンスキーファイルが適切であるか確認してください。");
                        }
                        else
                        {
                            this.ToastShow("ライセンス登録", $"ライセンスを更新しました。 LETS種別：{letsKinds}");
                        }
                    }
                    else
                    {
                        this.ToastShow("ライセンス登録", "デバイスに対応するライセンスがありません。ライセンスキーファイルが適切であるか確認してください。");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
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
                        try
                        {
                            SZManager.ExtractWithPassword(filePath, tempPath, zipPassword);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.StackTrace);

                            // 解凍に失敗した場合、通知を表示する。
                            this.ToastShow("フォントインストール失敗", "フォントのインストールに失敗しました。正しいフォントファイルを選択してください。");
                            return;
                        }

                        IList<string> messageList = this.deviceModeService.InstallFonts(tempPath);
                        if (messageList.Count > 0)
                        {
                            foreach (string message in messageList)
                            {
                                this.ToastShow("フォントインストール", message);
                            }
                        }

                        // アンインストール対象リストを更新する
                        this.UninstallListDisp();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.StackTrace);
                    }
                    finally
                    {
                        // 一時フォルダを削除する
                        this.deviceModeService.DeleteFolder(tempPath);
                    }
                }
            }
        }

        /// <summary>
        /// アンインストールボタン処理
        /// </summary>
        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            // ライセンス情報から、LEST種別名-LETS種別マップを作成する
            DeviceModeLicenseInfo deviceModeLicenseInfo = this.deviceModeLicenseInfoRepository.GetDeviceModeLicenseInfo();
            IDictionary<string, int> letsNameKindMap = this.deviceModeService.LetsNameKindMap();

            // アンインストール対象LETS種別をアンインストールする
            IList<int> uninstallLetsKindList = new List<int>();
            List<string> uninstallFontList = new List<string>();
            var items = this.UninstallListBox.Items;
            foreach (CheckBox item in items)
            {
                if ((bool)item.IsChecked)
                {
                    string letsKindName = item.Content.ToString();
                    if (letsNameKindMap.ContainsKey(letsKindName))
                    {
                        int letsKind = letsNameKindMap[letsKindName];
                        DeviceModeFontList deviceModeFontList = this.deviceModeFontListRepository.GetDeviceModeFontList();

                        // 削除対象フォントリストを作成する
                        foreach (DeviceModeFontInfo deviceModeFontInfo in deviceModeFontList.Fonts)
                        {
                            if (deviceModeFontInfo.LetsKind == letsKind)
                            {
                                string filePath = deviceModeFontInfo.FontFilePath;
                                string regkey = deviceModeFontInfo.RegistryKey;

                                uninstallFontList.Add(string.Join("\t", filePath, regkey, letsKindName, "Uninstall"));
                            }
                        }
                    }
                }
            }

            if (uninstallFontList.Count > 0)
            {
                IList<string> messageList = this.deviceModeService.UninstallFonts(uninstallFontList, "フォントのアンインストールに成功しました。");

                if (messageList.Count > 0)
                {
                    foreach (string message in messageList)
                    {
                        this.ToastShow("フォントアンインストール", message);
                    }

                    this.UninstallListDisp();
                }
            }
        }

        /// <summary>
        /// 通知表示処理
        /// </summary>
        private void ToastShow(string title, string message)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding enc = Encoding.GetEncoding("Shift_JIS");
            int titleBytes = enc.GetByteCount(title);
            int msgMax = 144 - titleBytes;
            if (enc.GetByteCount(message) > msgMax)
            {
                message = message.Substring(0, enc.GetString(enc.GetBytes(message), 0, msgMax - 2).Length) + "…";
            }

            ToastNotificationWrapper.Show(title, message);
        }

        /// <summary>
        /// ヘルプボタン処理
        /// </summary>
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            string helpPath = System.IO.Path.Combine(this.applicationSettingFolder, "LETS-for-Device-help.pdf");

            Process.Start(new ProcessStartInfo("cmd", $"/c start {helpPath}") { CreateNoWindow = true });
        }
    }
}
