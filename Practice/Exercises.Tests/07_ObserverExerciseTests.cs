using Exercises.Observer;
using Xunit;

namespace Exercises.Tests;

public class ObserverExerciseTests
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

        Assert.Single(email.SentEmails);
        Assert.Equal("Don hang DH0001 chuyen sang trang thai: Cancelled", email.SentEmails[0]);
        Assert.Single(inventory.RestockedOrders);
        Assert.Equal("DH0001", inventory.RestockedOrders[0]);
    }

    [Fact]
    public void InventoryUpdater_OnlyReactsToCancelledStatus()
    {
        var subject = new OrderSubject();
        var inventory = new InventoryUpdater();
        subject.Subscribe(inventory);

        subject.ChangeStatus("DH0002", "Shipped");

        Assert.Empty(inventory.RestockedOrders);
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
