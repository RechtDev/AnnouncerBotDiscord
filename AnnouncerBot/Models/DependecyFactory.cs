using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnouncerBot.Models
{
    class DependecyFactory
    {
        public Announcer announcer { get; set; }
        public DependecyFactory(CommandContext context)
        {
            announcer = new(context);
        }
    }
}
