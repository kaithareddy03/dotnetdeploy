namespace RSign.Common.Enums
{
    /// <summary>
    /// Envelope nodes enum
    /// This enum is used to create/update Envelope.XML file on temporary location.
    /// </summary>
    public enum EnvelopeNodes
    {
        CreatedDateTime,
        ExpiryDate,
        IsEnvelopeDiscarded,
        IsEnvelopeSaved,
        IsEnvelopeRejected,
        IsEnvelopeCompleted,
        IsSequenceCheck
    }
}