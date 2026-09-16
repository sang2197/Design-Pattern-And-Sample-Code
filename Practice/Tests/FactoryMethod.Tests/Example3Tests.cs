using Exercises.FactoryMethod.Example3;
using Xunit;

namespace FactoryMethod.Tests;

public class Example3Tests
{
    [Fact]
    public void EmailNotificationService_Notify_UsesEmailNotifier()
    {
        NotificationService service = new EmailNotificationService();
        Assert.Equal("[Email] Xin chao", service.Notify("Xin chao"));
    }

    [Fact]
    public void SmsNotificationService_Notify_UsesSmsNotifier()
    {
        NotificationService service = new SmsNotificationService();
        Assert.Equal("[SMS] Xin chao", service.Notify("Xin chao"));
    }
}
