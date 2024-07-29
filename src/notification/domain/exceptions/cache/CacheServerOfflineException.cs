namespace Notification.Domain.Exceptions.Cache;
using System;

public class CacheServerOfflineException : Exception
{
    public CacheServerOfflineException(string cacheServiceName) : base($"CacheService [{cacheServiceName}] is down.")
    {

    }
}