namespace Exercises.Mediator.Example3;

// Mediator - Vi du 3: Dai kiem soat khong luu dieu phoi may bay ha canh
// Xem lai: Mediator.md - Vi du 3 (ban goc dung Console.WriteLine; ban luyen tap nay
// doi RequestLanding()/Land() de tra ve bool thay vi void, cho de viet test)
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface IControlTower (Mediator) { bool RequestLanding(Airplane airplane); void NotifyLanded(Airplane airplane); }
// - abstract class Airplane (Colleague)
//     protected readonly IControlTower Tower; string FlightCode { get; }
//     constructor nhan (IControlTower tower, string flightCode)
//     bool RequestLanding() -> tra ve Tower.RequestLanding(this)
//     Land() -> goi Tower.NotifyLanded(this)
// - class PassengerPlane : Airplane (ConcreteColleague) -> khong them gi, chi ke thua constructor
// - class ControlTower : IControlTower (ConcreteMediator)
//     private Airplane? _runwayOccupiedBy;
//     RequestLanding(airplane) -> neu _runwayOccupiedBy != null thi tra ve false;
//       nguoc lai gan _runwayOccupiedBy = airplane roi tra ve true
//     NotifyLanded(airplane) -> neu _runwayOccupiedBy == airplane thi gan _runwayOccupiedBy = null
