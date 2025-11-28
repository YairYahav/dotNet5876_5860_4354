using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO;


[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
        : base(message, innerException) { }
}


[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException) { }
}


[Serializable]
public class BlDataValidationException : Exception
{
    public BlDataValidationException(string? message) : base(message) { }
}


[Serializable]
public class BlInvalidOperationException : Exception
{
    public BlInvalidOperationException(string? message) : base(message) { }
}


[Serializable]
public class BlConfigException : Exception
{
    public BlConfigException(string msg) : base(msg) { }
    public BlConfigException(string msg, Exception ex) : base(msg, ex) { }
}


[Serializable]
public class BlNullReferenceException : Exception
{
    public BlNullReferenceException(string? message) : base(message) { }
}

[Serializable]
public class BlUnauthorizedAccessException : Exception
{
    public BlUnauthorizedAccessException(string? message) : base(message) { }
}
