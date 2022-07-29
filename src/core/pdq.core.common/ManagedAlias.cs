[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("pdq.core-tests")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("pdq.state")]
namespace pdq.common
{
    internal class ManagedAlias
    {
        private ManagedAlias(string name, string relation)
        {
            Name = name;
            Relation = relation;
        }

        /// <summary>
        /// The name of the Alias.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The relation that this alias is associated with.
        /// </summary>
        public string Relation { get; set; }

        /// <summary>
        /// Create a <see cref="ManagedAlias"/> for a given relation.
        /// </summary>
        /// <param name="name">The name of the alias.</param>
        /// <param name="relation">The relation that the alias is associated with.</param>
        /// <returns>A new instance of <see cref="ManagedAlias"/>.</returns>
        public static ManagedAlias Create(string name, string relation) => new ManagedAlias(name, relation);

        /// <summary>
        /// /// Create a <see cref="ManagedAlias"/> for a given relation.
        /// </summary>
        /// <param name="relation">The relation to generate an alias for.</param>
        /// <returns>A new instance of <see cref="ManagedAlias"/>.</returns>
        public static ManagedAlias Create(string relation) => new ManagedAlias(null, relation);
    }
}

