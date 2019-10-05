using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGeneratorNS
{
    namespace ItemNS
    {
        public class Key : Item
        {
            public override string Name { get; } = "Key";
        }
        public class StartPortal : Item
        {
            public override string Name { get; } = "StartPortal";
        }
        public class EndPortal : Item
        {
            public override string Name { get; } = "EndPortal";
        }
    }

    public abstract class Item
    {
        public abstract string Name { get; }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Item other = (Item) obj;
                return Name == other.Name;
            }
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}