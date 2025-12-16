using System;

namespace FashionShopApp.Core.Exceptions
{
    // Kế thừa từ Exception
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}