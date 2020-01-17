using System.Text;

namespace SoulCollector.Entities
{

    public class Party
    {
        public Entity[] Members;
        public Party(Entity[] members)
        {
            Members = members;
        }

        public bool IsAlive()
        {
            foreach (Entity ent in Members)
            {
                if (ent.IsAlive())
                {
                    return true;
                }

            }

            return false;
        }

        public void UpdateParty(long gameTick)
        {
            foreach (Entity ent in Members)
            {
                ent.Update(gameTick);
            }
        }

        public string GetWinText()
        {
            StringBuilder bld = new StringBuilder(ToString());
            if (Members.Length == 1)
                bld.Append(" has won");
            else
                bld.Append(" have won.");
            return bld.ToString();
        }

        public override string ToString()
        {
            StringBuilder bld = new StringBuilder();


            for (int i = 0; i < Members.Length - 2; ++i)
            {
                bld.Append(Members[i].Name);
                bld.Append(", ");
            }

            if (Members.Length >= 2)
            {
                bld.Append(Members[Members.Length - 2].Name);
                bld.Append(" and ");
            }

            bld.Append(Members[Members.Length - 1].Name);
            return bld.ToString();
        }
    }
}