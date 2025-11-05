using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO;



[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}


[Serializable]
public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}


[Serializable]
public class DalInvalidOperationException : Exception
{
    public DalInvalidOperationException(string? message) : base(message) { }
}


[Serializable]
public class DalDataCorruptionException : Exception
{
    public DalDataCorruptionException(string? message) : base(message) { }
}


[Serializable]
public class DalNullReferenceException : Exception
{
    public DalNullReferenceException(string? message) : base(message) { }
}
