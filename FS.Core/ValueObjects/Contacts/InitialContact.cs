using FS.Core.Enums;

namespace FS.Core.ValueObjects.Contacts;

public record InitialContact(
    ContactType ContactType,
    string Url
);