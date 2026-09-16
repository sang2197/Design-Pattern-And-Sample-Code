using Exercises.Observer.Example1;
using Xunit;

namespace Observer.Tests;

public class Example1Tests
{
    [Fact]
    public void ChangeStatus_NotifiesAllSubscribedObservers()
    {
        var subject = new OrderSubject();
        var email = new EmailNotifier();
        var inventory = new InventoryUpdater();

        subject.Subscribe(email);
        subject.Subscribe(inventory);

        subject.ChangeStatus("DH0001", "Cancelled");

        Assert.Equal("Don hang DH0001 chuyen sang trang thai: Cancelled", email.SentEmails[0]);
        Assert.Equal("DH0001", inventory.RestockedOrders[0]);
    }

    [Fact]
    public void Unsubscribe_StopsReceivingFurtherUpdates()
    {
        var subject = new OrderSubject();
        var email = new EmailNotifier();
        subject.Subscribe(email);
        subject.Unsubscribe(email);

        subject.ChangeStatus("DH0003", "Cancelled");

        Assert.Empty(email.SentEmails);
    }
}
