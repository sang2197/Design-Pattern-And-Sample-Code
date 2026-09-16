namespace Exercises.Proxy.Example2;

// Proxy - Vi du 2: Protection Proxy - kiem tra quyen truoc khi xoa tai lieu
// Xem lai: Proxy-Pattern.md - Vi du 2
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface IDocumentService (Subject) { void DeleteDocument(string documentId); }
// - class DocumentService : IDocumentService (RealSubject)
//     List<string> DeletedDocuments { get; } = new List<string>();
//     DeleteDocument(documentId) -> them documentId vao DeletedDocuments
// - class DocumentServiceProxy : IDocumentService (Proxy)
//     private readonly DocumentService _realService; private readonly string _currentUserRole;
//     List<string> DeniedAttempts { get; } = new List<string>();
//     constructor nhan (DocumentService realService, string currentUserRole)
//     DeleteDocument(documentId) -> neu _currentUserRole != "Admin" thi them documentId vao DeniedAttempts roi return;
//       nguoc lai goi _realService.DeleteDocument(documentId)
