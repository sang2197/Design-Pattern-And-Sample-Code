namespace Exercises.Builder.Example3;

// Builder - Vi du 3: Builder kem Director dung san cong thuc Pizza
// Xem lai: Builder-Pattern.md - Vi du 3
//
// Viet lai TU DAU cac thanh phan sau:
//
// - class Pizza (Product)
//     string Size { get; set; } = "", List<string> Toppings { get; } = new List<string>(), bool ExtraCheese { get; set; }
//     string Describe() -> "Pizza {Size}: {toppings, hoac 'khong topping' neu rong}{' + extra cheese' neu ExtraCheese}"
// - class PizzaBuilder
//     WithSize(string), AddTopping(string), WithExtraCheese() -> return this
//     Build() -> tra ve pizza
// - class PizzaMenuDirector
//     MakeMargherita(PizzaBuilder builder) -> Size "M", topping "Tomato", "Mozzarella"
//     MakePepperoniSupreme(PizzaBuilder builder) -> Size "L", topping "Pepperoni", "Mushroom", co ExtraCheese
