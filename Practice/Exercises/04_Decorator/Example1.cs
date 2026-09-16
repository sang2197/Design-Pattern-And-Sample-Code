namespace Exercises.Decorator.Example1;

// Decorator - Vi du 1: Goi mon ca phe, them topping tu do (Milk / Sugar)
// Xem lai: Decorator-Pattern.md - Vi du 1
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface ICoffee (Component) { string Describe(); decimal Cost(); }
// - class SimpleCoffee : ICoffee (ConcreteComponent) -> Describe() "Coffee", Cost() 20000
// - abstract class CoffeeDecorator : ICoffee (Decorator)
//     protected readonly ICoffee Inner; constructor nhan ICoffee inner, gan vao Inner
//     virtual Describe() => Inner.Describe(); virtual Cost() => Inner.Cost();
// - class MilkDecorator : CoffeeDecorator (ConcreteDecorator)
//     Describe() -> "{Inner.Describe()} + Milk"; Cost() -> Inner.Cost() + 5000
// - class SugarDecorator : CoffeeDecorator (ConcreteDecorator)
//     Describe() -> "{Inner.Describe()} + Sugar"; Cost() -> Inner.Cost() + 2000
