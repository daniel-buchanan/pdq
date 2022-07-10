﻿using System;
namespace pdq.services
{
    public class Entity : IEntity { }

    public abstract class Entity<TKey> : IEntity<TKey>
    {
        protected Entity(string keyName)
        {
            Key = KeyMetadata<TKey>.Create(keyName);
        }

        public IKeyMetadata Key { get; }
    }

    public abstract class Entity<TKey1, TKey2> : IEntity<TKey1, TKey2>
    {
        protected Entity(string componentOne, string componentTwo)
        {
            Key = new CompositeKey
            {
                ComponentOne = KeyMetadata<TKey1>.Create(componentOne),
                ComponentTwo = KeyMetadata<TKey2>.Create(componentTwo)
            };
        }

        public ICompositeKey Key { get; }
    }

    public abstract class Entity<TKey1, TKey2, TKey3> : IEntity<TKey1, TKey2, TKey3>
    {
        protected Entity(string componentOne, string componentTwo, string componentThree)
        {
            Key = new CompositeKeyTriple
            {
                ComponentOne = KeyMetadata<TKey1>.Create(componentOne),
                ComponentTwo = KeyMetadata<TKey2>.Create(componentTwo),
                ComponentThree = KeyMetadata<TKey3>.Create(componentThree)
            };
        }

        public ICompositeKeyTriple Key { get; }
    }
}

