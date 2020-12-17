using ApplicationService.Interfaces;
using Core.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ApplicationService.Fonts.Tests
{
    /// <summary>
    /// フォント通知サービスのテスト
    /// </summary>
    [TestClass]
    public class FontNotificationServiceTests
    {
        /// <summary>
        /// フォント通知のアクティベート通知の呼び出し確認
        /// </summary>
        [TestMethod]
        public void FontNotification_Activate()
        {
            // フォント情報
            var font = new ActivateFont("111", "フォント表示", 300.55f, "1.1.1");

            // モックを作成
            var fontManagerServiceMock = new Mock<IFontManagerService>();

            fontManagerServiceMock.Setup(x => x.Synchronize(font));
            var service = new FontNotificationService(
                fontManagerServiceMock.Object);

            service.Activate(font);

            // 下記のような設定だと例外が発生する（正しいと発生しない）
            // var font1= new ActivateFont("222", "フォント表示", 300.55f, "1.1.1");
            fontManagerServiceMock.Verify(x => x.Synchronize(font), Times.AtLeastOnce());
        }
    }
}