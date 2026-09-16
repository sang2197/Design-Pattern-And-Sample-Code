namespace Exercises.Strategy.Example2;

// Strategy - Vi du 2: Chon chien luoc tinh phi van chuyen theo dieu kien don hang
// Xem lai: Strategy-Pattern.md - Vi du 2
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface IShippingFeeStrategy (Strategy) { decimal Calculate(decimal orderTotal, decimal weightKg); }
// - class StandardShipping : IShippingFeeStrategy -> Calculate() -> 15000 + weightKg * 3000
// - class ExpressShipping : IShippingFeeStrategy -> Calculate() -> 30000 + weightKg * 5000
// - class FreeShipping : IShippingFeeStrategy -> Calculate() -> 0
// - class ShippingContext (Context)
//     constructor nhan IShippingFeeStrategy strategy
//     CalculateFee(orderTotal, weightKg) -> uy quyen cho _strategy.Calculate(orderTotal, weightKg)
