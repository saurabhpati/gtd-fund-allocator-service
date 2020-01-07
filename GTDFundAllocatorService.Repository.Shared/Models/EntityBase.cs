using System;

namespace GTDFundAllocatorService.Repository.Shared
{
    public class EntityBase : EntityBase<int>
    {
    }

    public class EntityBase<TKey> where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
    }
}
