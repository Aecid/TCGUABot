using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Data.Models
{
    public class TelegramUser
    {
        [Key]
        public long Id { get; set; }

        public string EmojiStatus { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [NotMapped]
        public string Name
        {
            get
            {
                var pname = string.Empty;
                if (FirstName != string.Empty)
                    pname += FirstName;
                if (LastName != string.Empty)
                    pname += " " + LastName;
                if (LastName == string.Empty && FirstName == string.Empty && Username != string.Empty)
                    pname = Username;

                return pname;
            }
        }
    }
}
