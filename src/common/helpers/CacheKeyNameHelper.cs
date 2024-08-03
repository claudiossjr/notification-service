namespace Common.Helpers;

public static class CacheKeyNameHelper
{
    public static string GetNotificationRuleKey(string sender)
    {
        return $"RuleSender:{sender}";
    }

    public static string GetNotificationTokenBucketKey(string sender, string recipient)
    {
        return $"Bucket:{sender}:{recipient}";
    }

}
