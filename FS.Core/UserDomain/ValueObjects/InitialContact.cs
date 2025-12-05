using FS.Core.UserDomain.Enums;

namespace FS.Core.UserDomain.ValueObjects;

public record InitialContact(
    ContactType ContactType,
    string Url
);