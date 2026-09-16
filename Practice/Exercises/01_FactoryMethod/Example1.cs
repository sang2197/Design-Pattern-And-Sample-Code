namespace Exercises.FactoryMethod.Example1;

// Factory Method - Vi du 1: Xu ly thanh toan theo phuong thuc (VNPay / Momo)
// Xem lai: Factory-Method-Pattern.md - Vi du 1
//
// Viet lai TU DAU (khong xem code mau) cac thanh phan sau, dung EXACT ten va chu ky nhu duoi day
// de test bien dich va chay duoc:
//
// - interface IPaymentMethod { string Pay(decimal amount); }
// - class VnPayPayment : IPaymentMethod
//     Pay() tra ve "Thanh toan {amount:N0} qua VNPay"
// - class MomoPayment : IPaymentMethod
//     Pay() tra ve "Thanh toan {amount:N0} qua Momo"
// - abstract class CheckoutProcessor
//     protected abstract IPaymentMethod CreatePaymentMethod();
//     public string Checkout(decimal amount) -> goi CreatePaymentMethod() roi tra ve "[Checkout] {ket qua Pay()}"
// - class VnPayCheckoutProcessor : CheckoutProcessor -> CreatePaymentMethod() tra ve VnPayPayment
// - class MomoCheckoutProcessor : CheckoutProcessor -> CreatePaymentMethod() tra ve MomoPayment
