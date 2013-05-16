using System;

namespace PachubeDataAccess
{
    public interface IFeedItem
    {
        string ToJson(int FeedId);
    }
}
