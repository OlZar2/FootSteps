using FS.Core.Enums;

namespace FS.Core.ValueObjects.Contacts;

public record InitialContact(
    ContactType Type,
    string Url
);