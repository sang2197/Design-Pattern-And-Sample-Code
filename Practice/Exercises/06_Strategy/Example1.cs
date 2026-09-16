namespace Exercises.Strategy.Example1;

// Strategy - Vi du 1: Chien luoc giam gia don hang, doi Strategy ngay tai runtime
// Xem lai: Strategy-Pattern.md - Vi du 1
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface IDiscountStrategy (Strategy) { decimal Apply(decimal orderTotal); }
// - class NoDiscount : IDiscountStrategy -> Apply() tra ve nguyen orderTotal
// - class PercentageDiscount : IDiscountStrategy
//     constructor nhan decimal percent
//     Apply(orderTotal) -> orderTotal - (orderTotal * _percent / 100)
// - class FixedAmountDiscount : IDiscountStrategy
//     constructor nhan decimal amount
//     Apply(orderTotal) -> Math.Max(0, orderTotal - _amount)
// - class OrderContext (Context)
//     constructor nhan IDiscountStrategy discountStrategy
//     SetDiscountStrategy(IDiscountStrategy) -> gan lai _discountStrategy (cho phep doi Strategy tai runtime)
//     CalculateTotal(orderTotal) -> uy quyen cho _discountStrategy.Apply(orderTotal)
