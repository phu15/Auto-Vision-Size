using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Attach
{
    class UserLogin
    {
        public string[] user = new string[] { "Operator", "Engineer", "Developer" };//new string[] { "JPTOpr", "JPTEng", "JPTDev" };
        static string[] accessLvl = new string[] { "Operator", "Engineer", "Developer" };
        static string[] password = { "", "PE", "MEG" };

        public int GetAccess(string userName, string userPassword)
        {
            int access = 0;

            for (int i = 0; i < user.Length; i++)
            {
                if (user[i].ToLower() == userName.ToLower())
                {
                    if (password[i].ToLower() == userPassword.ToLower())
                        return i;
                }
            }
            return access; // 0=Operator, 1=Engineer, 2=Developer
        }
    }
}
