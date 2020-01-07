using System;

namespace GTDFundAllocatorService.Domain.Shared
{
    public class Key : Key<int>
    {
    }

    public class Key<TKey> where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
    }
}
