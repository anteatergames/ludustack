using Microsoft.AspNetCore.Identity;

namespace LuduStack.Infra.CrossCutting.Identity.Model
{
    public class Role : IdentityRole
    {
        public Role()
        {
        }

        public Role(string name)
        {
            Name = name;
            NormalizedName = name.ToUpperInvariant();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}