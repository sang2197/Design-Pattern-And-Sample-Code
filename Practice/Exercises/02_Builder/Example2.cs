namespace Exercises.Builder.Example2;

// Builder - Vi du 2: Fluent Builder dung cau lenh SQL
// Xem lai: Builder-Pattern.md - Vi du 2
//
// Viet lai TU DAU cac thanh phan sau:
//
// - class SqlQuery (Product)
//     string Table { get; set; } = ""
//     List<string> Columns { get; } = new List<string>()
//     List<string> Conditions { get; } = new List<string>()
//     string ToSql() -> "SELECT {cot, hoac * neu rong} FROM {Table}", them " WHERE {dk1 AND dk2 ...}" neu co Conditions
// - class SqlQueryBuilder
//     Select(params string[] columns), From(string table), Where(string condition) -> return this
//     Build() -> neu Table rong thi throw InvalidOperationException("Table is required"), nguoc lai tra ve query
