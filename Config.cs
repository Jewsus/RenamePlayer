using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RenamePlayer
{
    public class Config : Dictionary<string, string>
    {

        public bool LoadConfig(string configFile)
        {
            string[] file;
            if (File.Exists(configFile))
            {
                file = File.ReadAllLines(configFile);
            } // if
            else
            {
                return false;
            } // else

            foreach (var t in file.Where(t => !t.StartsWith("//")
                                             && !t.StartsWith("#"))
                                                 .Where(t => t.Split('=').Length == 2))
            {
                Add(t.Split('=')[0].ToLower(), t.Split('=')[1]);
            } // for

            return CheckKeys();
        } // Load


        private bool CheckKeys()
        {
            var enableWordReplace = false;

            foreach (var pair in this)
            {
                switch (pair.Key)
                {
                    case "enablewordreplace":
                        if (bool.TryParse(pair.Value, out enableWordReplace))
                        {
                            enableWordReplace = true;
                        }
                        break;
                } // switch

            } // foreach

            return enableWordReplace;
        } // CheckKeys

    } // Settings

} // TerrariaIRC
