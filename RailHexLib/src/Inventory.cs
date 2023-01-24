using System.Collections.Generic;
using System.Diagnostics;

namespace RailHexLib
{
    public class Inventory
    {
        public void AddResource(Resource name, int count)
        {
            if (!resources.ContainsKey(name))
            {
                resources[name] = 0;
            }
            resources[name] += count;
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
            return existCount >= count ? count: existCount % count;
        }

        public int ResourceCount(Resource name)
        {
            return resources.GetValueOrDefault(name, 0);
        }

        Dictionary<Resource, int> resources = new Dictionary<Resource, int>();

        public Dictionary<Resource, int> Resources => resources;
    }
}