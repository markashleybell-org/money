using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Design",
    "RCS1170: Use read-only auto-implemented property.",
    Justification = "Dapper needs a setter to hydrate objects")]

[assembly: SuppressMessage(
    "Design",
    "RCS1090: Add call to 'ConfigureAwait' (or vice versa).",
    Justification = "This isn't a library/no sync context in ASP.NET Core")]
