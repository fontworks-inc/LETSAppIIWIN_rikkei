using System;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace setup
{
    class Program
    {
        static void Main(string[] args)
        {
            // 管理者権限を持つユーザで実行されているか確認する
            if (!IsAdministratorsMember())
            {
                System.Windows.Forms.MessageBox.Show("管理者権限を持つユーザで実行してください");
                return;
            }

            // ホームドライブの取得
            string winDir = System.Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string homedrive = winDir.Substring(0, winDir.IndexOf("\\"));

            // LETSフォルダ
            string letsfolder = $@"{homedrive}\ProgramData\Fontworks\LETS";

            System.Diagnostics.Process pro = new System.Diagnostics.Process();

            pro.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            pro.StartInfo.Arguments = @"/c ver";
            pro.StartInfo.CreateNoWindow = true;
            pro.StartInfo.UseShellExecute = false;
            pro.StartInfo.RedirectStandardOutput = true;

            pro.Start();
            string output = pro.StandardOutput.ReadToEnd();
            string[] versions = null;

            MatchCollection matches = Regex.Matches(output, @"\d+\.\d+\.\d+(\.\d+)?");
            foreach (Match match in matches)
            {
                versions = match.Value.Split('.');
            }
            if (versions != null)
            {
                try
                {
                    int major = int.Parse(versions[0]);
                    if (major < 10)
                    {
                        System.Windows.Forms.MessageBox.Show("Windows10 未満の OS では LETS をご利用できません。");
                        return;
                    }
                }
                catch (Exception)
                {
                    // NOP
                }
            }

            // インストーラ開始日時を取得する
            DateTime inststt = DateTime.Now;

            // Setupプログラムから実行していることを示す一時ファイル
            string tmpfile = $@"{homedrive}\ProgramData\.runfromletssetup";

            try
            {
                //  一時ファイルを作成する
                FileStream s = File.Create(tmpfile);
                s.Close();

                //  LETSがインストールされているか確認
                //  レジストリ「コンピューター\HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{805BF39F-7BBE-445F-B56B-A6FD34C4D817}」
                Microsoft.Win32.RegistryKey rkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{805BF39F-7BBE-445F-B56B-A6FD34C4D817}");
                if (rkey != null)
                {
                    //  存在する場合
                    // ユーザ情報クリアバッチを削除
                    // ・clearuserdata*.bat
                    string[] batfiles = Directory.GetFiles(letsfolder, "clearuserdata*.bat");
                    if(batfiles != null && batfiles.Length > 0)
                    {
                        foreach(string batfile in batfiles)
                        {
                            File.Delete(batfile);
                        }
                    }
                    // ・uninstallfonts*.bat
                    batfiles = Directory.GetFiles(letsfolder, "uninstallfonts*.bat");
                    if (batfiles != null && batfiles.Length > 0)
                    {
                        foreach (string batfile in batfiles)
                        {
                            File.Delete(batfile);
                        }
                    }
                    // ・uninstreg*.bat
                    batfiles = Directory.GetFiles(letsfolder, "uninstreg*.bat");
                    if (batfiles != null && batfiles.Length > 0)
                    {
                        foreach (string batfile in batfiles)
                        {
                            File.Delete(batfile);
                        }
                    }

                    // アンインストーラ実行
                    Process pu = Process.Start("MsiExec.exe", "/X{805BF39F-7BBE-445F-B56B-A6FD34C4D817} /passive");
                    pu.WaitForExit();

                    if(pu.ExitCode != 0)
                    {
                        // アンインストーラが(UAC)キャンセルされたら終了
                        return;
                    }

                    // ショートカット(uninstallfonts.bat)の削除
                    string uninstbat = $@"{homedrive}\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp\uninstallfonts.bat";
                    if (File.Exists(uninstbat))
                    {
                        File.Delete(uninstbat);
                    }
                }

                // ショートカットにバッチ(uninstallfonts.bat)があるか確認
                string uninstshortcut = $@"{homedrive}\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp\uninstallfonts.bat";
                if (File.Exists(uninstshortcut))
                {
                    System.Windows.Forms.MessageBox.Show("アンインストール処理が完了していません。");
                    return;
                }

                // 実行ファイルパスの取得(圧縮フォルダ内でも取得できる)
                System.Reflection.Assembly executionAsm = System.Reflection.Assembly.GetExecutingAssembly();
                string actualPath = System.IO.Path.GetDirectoryName(executionAsm.Location);

                // インストーラの起動
                string installer = actualPath + @"\LETS-Installer.msi";
                Process p1 = Process.Start(installer);
                p1.WaitForExit();

                if(p1.ExitCode != 0)
                {
                    // インストーラがキャンセルされたら終了
                    return;
                }

                // LETSアプリの起動(ショートカットを実行する)
                string shortcut = $@"{homedrive}\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp\LETS デスクトップアプリ.lnk";
                if(!File.Exists(shortcut))
                {
                    // ショートカットがなければ終了する
                    return;
                }

                Process p2 = Process.Start(shortcut);

                // チュートリアル画面の起動
                Thread.Sleep(5000); // アプリの起動待ち
                string updator = $@"{homedrive}\ProgramData\Fontworks\LETS\LETSUpdater.exe";
                Process p3 = Process.Start(updator);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"{ex.Message}：{ex.StackTrace}");
            }
            finally
            {
                //System.Windows.Forms.MessageBox.Show("finally Delete："+tmpfile);
                File.Delete(tmpfile);
            }
        }

        /// <summary>
        /// 現在のユーザーがローカルAdministratorsグループのメンバーか調べる
        /// </summary>
        /// <returns>メンバーであればtrue。</returns>
        public static bool IsAdministratorsMember()
        {
            ////現在のユーザーを表すWindowsIdentityオブジェクトを取得する
            //System.Security.Principal.WindowsIdentity wi =
            //    System.Security.Principal.WindowsIdentity.GetCurrent();
            ////WindowsPrincipalオブジェクトを作成する
            //System.Security.Principal.WindowsPrincipal wp =
            //    new System.Security.Principal.WindowsPrincipal(wi);
            ////Administratorsグループに属しているか調べる
            //return wp.IsInRole(
            //    System.Security.Principal.WindowsBuiltInRole.Administrator);

            try
            {
                //ローカルコンピュータストアのPrincipalContextオブジェクトを作成する
                using (PrincipalContext pc = new PrincipalContext(ContextType.Machine))
                {
                    //現在のユーザーのプリンシパルを取得する
                    UserPrincipal up = UserPrincipal.Current;
                    //ローカルAdministratorsグループを探す
                    //"S-1-5-32-544"はローカルAdministratorsグループを示すSID
                    GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, "S-1-5-32-544");
                    //グループのメンバーであるか調べる
                    return up.IsMemberOf(gp);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.StackTrace);
                return true;
            }
        }
    }


}
