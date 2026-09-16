namespace Exercises.Adapter.Example3;

// Adapter - Vi du 3: Thanh toan qua SDK nuoc ngoai (khac don vi tinh)
// Xem lai: Adapter-Pattern.md - Vi du 3
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface IPaymentGateway (Target) { string Pay(decimal amount); }
// - class ForeignPaymentSdk (Adaptee)
//     string Charge(int amountInCents, string currency) -> tra ve "[ForeignSDK] Charged {amountInCents} {currency} cents"
// - class ForeignPaymentAdapter : IPaymentGateway (Adapter)
//     constructor nhan ForeignPaymentSdk
//     Pay(decimal amount) -> quy doi amount (vd 19.99) sang amountInCents (int, vd 1999) bang (int)(amount * 100),
//       roi goi _sdk.Charge(amountInCents, "USD")
