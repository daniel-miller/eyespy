using System;

namespace TestDiagnostics;

public class UnwantedIdentifierSuffix
{
    // This should trigger SPY02 because _myID should instead be _myId.
    private readonly Guid _myID;

    public UnwantedIdentifierSuffix()
    {
        _myID = Guid.NewGuid();
    }

    public Guid GetID() => _myID;
}