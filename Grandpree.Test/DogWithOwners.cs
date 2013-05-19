using System.Collections.Generic;

namespace Grandpree.Test
{
    public class DogWithOwners
    {
        public string DogName { get; set; }

        public override string ToString()
        {
            return string.Format("DogName: {0}, Owners: {1}", DogName, Owners);
        }

        public IEnumerable<string> Owners { get; set; }
    }
}