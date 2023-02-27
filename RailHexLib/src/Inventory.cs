using System.Collections.Generic;
using System.Diagnostics;

namespace RailHexLib
{
    public class Inventory
    {
        public Dictionary<Resource, int> Resources => resources;

        public void AddResource(Resource name, int count)
        {
            Debug.Assert(canAcceptResource(name, count));
            // TODO: overflow of resources should prevent addition
            if (!resources.ContainsKey(name))
            {
                resources[name] = 0;
            }
            resources[name] += count;
        }

        public bool canAcceptResource(Resource name, int count)
        {
            return ResourceCount(name) + count <= MaxResourceCapacity(name);
        }

        public int PickResource(Resource name, int count)
        {
            if (!Resources.ContainsKey(name))
            {
                return 0;
            }
            int existCount = resources[name];

            resources[name] -= count;
            if (resources[name] < 0)
            {
                resources[name] = 0;
            }
            return existCount >= count ? count : existCount % count;
        }

        public int ResourceCount(Resource name)
        {
            return resources.GetValueOrDefault(name, 0);
        }

        public int MaxResourceCapacity(Resource name)
        {
            return 100;
        }

        Dictionary<Resource, int> resources = new Dictionary<Resource, int>();
    }
}