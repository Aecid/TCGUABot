using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Models.TcgPlayerModels
{
    public class TcgPlayerProductExtendedDataItem
    {
        string name { get; set; }
        string displayName { get; set; }
        string value { get; set; }

                //        {
                //    "name": "Rarity",
                //    "displayName": "Rarity",
                //    "value": "M"
                //},
                //{
                //    "name": "Number",
                //    "displayName": "#",
                //    "value": "62"
                //},
                //{
                //    "name": "SubType",
                //    "displayName": "Creature Type or Sub Type",
                //    "value": "Planeswalker — Jace"
                //},
                //{
                //    "name": "OracleText",
                //    "displayName": "Rules Text Contains",
                //    "value": "[+2]: Look at the top card of target player's library. You may put that card on the bottom of that player's library\r\n<br>[0]: Draw three cards, then put two cards from your hand on top of your library in any order.\r\n<br>[-1]: Return target creature to its owner's hand.\r\n<br>[-12]: Exile all cards from target player's library, then that player shuffles his or her hand into his or her library."
                //}
    }
}
