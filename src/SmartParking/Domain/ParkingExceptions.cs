namespace SmartParking.Domain;

public class InvalidParkingSessionException : Exception
{
    public InvalidParkingSessionException()
    {
    }

    public InvalidParkingSessionException(string message)
        : base(message)
    {
    }

    public InvalidParkingSessionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class IncompatibleSpotException : Exception
{
    public IncompatibleSpotException()
    {
    }

    public IncompatibleSpotException(string message)
        : base(message)
    {
    }

    public IncompatibleSpotException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
