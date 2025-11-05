namespace Pharmacy.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string name) : base($"{name} Was Not Found.") { }
        public NotFoundException(string name, object Key) : base($"{name} With Key '{Key}' Was Not Found.") { }

    }
}
