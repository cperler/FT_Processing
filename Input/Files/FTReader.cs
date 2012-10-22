using System;
using System.IO;
using System.Text.RegularExpressions;
using log4net;
using FT.Input.Data;

namespace FT.Input.Files
{
    public class FTReader
    {
        private readonly string title = @"^.*#(?<id>\d*):\s(?<cost>[^a-zA-Z]*)\s(?<style>[^,]*),\s(?<table>[^-]*)\s-\s(?<sb_bb>[^\s]*)\s-\s(?<game>[^-]*)\s-\s(?<time>[^-]*)\s-\s(?<date>[^-]*)$";
        private readonly string seat = @"^.*(?<seat>\d+):\s(?<user>[^\(]*)\s\((?<amount>[^\)]*)\)$";
        private readonly string post = @"^(?<user>.*) posts the (?<blind>small|big) blind of (?<amount>\d*)$";
        private readonly string btn = @"^.*button.*#(?<button_pos>\d)$";
        private readonly string deal = @"^Dealt to (?<user>[^\[]*)\s\[(?<card1>\w\w)\s(?<card2>\w\w)\]$";
        private readonly string act_fold = @"^(?<user>.*)\s(?<action>folds|checks)$";
        private readonly string act_call = @"^(?<user>.*)\s(?<action>bets|calls|raises\sto)\s(?<amount>[\d|,]*)$";
        private readonly string flop = @".*FLOP.*\[(?<flop1>\w\w)\s(?<flop2>\w\w)\s(?<flop3>\w\w)\]$";
        private readonly string turn = @".*TURN.*\[(?<flop1>\w\w)\s(?<flop2>\w\w)\s(?<flop3>\w\w)\]\s\[(?<turn>\w\w)\]$";
        private readonly string river = @".*RIVER.*\[(?<flop1>\w\w)\s(?<flop2>\w\w)\s(?<flop3>\w\w)\s(?<turn>\w\w)\]\s\[(?<river>\w\w)\]$";
        private readonly string showing = @"^(?<user>.*)\sshows\s\[(?<card1>\w\w)\s(?<card2>\w\w)\]\s(?<summary>.*)$";
        private readonly string tie = @"^(?<user>.*)\sties.*\((?<tie_pot>[\d|,]*)\)\swith\s(?<summary>.*)$";
        private readonly string summary = @"^.*pot\s(?<total_pot>[^\s]*).*Rake\s(?<rake>[\d|,]*)$";
        private readonly string board3 = @"^.*\[(?<flop1>\w\w)\s(?<flop2>\w\w)\s(?<flop3>\w\w)\]$";
        private readonly string board4 = @"^.*\[(?<flop1>\w\w)\s(?<flop2>\w\w)\s(?<flop3>\w\w)\s(?<turn>\w\w)\]$";
        private readonly string board5 = @"^.*\[(?<flop1>\w\w)\s(?<flop2>\w\w)\s(?<flop3>\w\w)\s(?<turn>\w\w)\s(?<river>\w\w)\]$";
        private readonly string preflopfold = @"^.*(?<seat>\d+):\s(?<pre_flop_fold>[^\(]*).*folded\sbefore.*$";
        private readonly string showandwin = @"^.*(?<seat>\d+):\s(?<showdown_win>[^\(]*).*(?<card1>\w\w)\s(?<card2>\w\w).*won\s\((?<amount>[\d|,]*)\)\swith\s(?<summary>.*)$";
        private readonly string foldonflop = @"^.*(?<seat>\d+):\s(?<pre_flop_fold>[^\(]*).*folded.*Flop$";
        private readonly string noshowandwin = @"^(?<user>.*)\swins.*\((?<won_pot>[\d|,]*)\)\swith\s(?<summary>.*)$";
        private readonly string nobet = @"^.*(?<seat>\d+):\s(?<no_bet>.*)\sdidn't\sbet.*$";        
        private readonly string showandlose = @"^.*(?<seat>\d+):\s(?<showdown_lose>[^\(]*).*(?<card1>\w\w)\s(?<card2>\w\w).*lost\swith\s(?<summary>.*)$";
        private readonly string uncalledbet = @"^Uncalled.*(?<amount>[\d|,]+).*\sto\s(?<user>.*)$";
        private readonly string muck = @"(?<muck_user>.*)\smucks$";
        private readonly string win = @"(?<win_user>.*)\swins.*(?<pot>[\d|,]+)\)$";
        private readonly string collect = @"(?<collecting_user>.*)\scollect.*(?<pot>[\d|,]+)\).*$";
        private readonly string allin = @"^(?<allin_user>.*)\s(?<action>bets|calls|raises\sto)\s(?<amount>[\d|,]*).*all.*$";
        private readonly string showscards = @"(?<user>.*)\sshows\s\[(?<card1>\w\w)\s(?<card2>\w\w)\]$";
        private readonly string showssummary = @"(?<user>.*)\sshows\s(?<summary>[^\[]*)$";
        private readonly string stands = @"(?<standing_user>.*).*stands.*$";
        private readonly string blinds = @".*blinds.*(?<small>[\d|,]+).(?<large>[\d|,]+)$";
        private readonly string sitout = @"^(?<sitout_user>.*).*sitting.*$";
        private readonly string returns = @"^(?<sitout_user>.*).*returned.*$";

        private readonly string[] exclusions = new string[]
                {
                    "*** HOLE CARDS ***",
                    "*** SHOW DOWN ***",
                    "*** SUMMARY ***",
                    "15 seconds left to act"
                };

        private static ILog log = LogManager.GetLogger(typeof(FTReader));
        private readonly string[] checks;

        public FTReader()
        {
            checks = new string[]
                {
                    nobet, title, seat, post, btn, deal, act_fold, act_call, flop,
                    turn, river, showing, tie, summary, board3, preflopfold,
                    showandwin, foldonflop, noshowandwin, showandlose,
                    uncalledbet, muck, win, board4, board5, collect, allin,
                    showscards, showssummary, stands, blinds, sitout, returns
                };
        }

        public void Read(string location)
        {
            TextReader tr = new StreamReader(location);
            
            string line;
            while ((line = tr.ReadLine()) != null)
            {
                Parse(line);                
            }            
        }        

        private void Parse(string line)
        {            
            foreach (string check in checks)
            {
                Regex RE = new Regex(check);
                Match m = RE.Match(line);
                if (m.Success)
                {
                    Print(RE, m.Groups);
                    return;
                }
            }

            foreach (string exclusion in exclusions)
            {
                if (line.EndsWith(exclusion) || line.Trim().Equals(String.Empty))
                {
                    return;
                }
            }
        }

        private static void Print(Regex RE, GroupCollection GC)
        {
            for (int i = 1; i < GC.Count; i++)
            {
                log.Info(RE.GroupNameFromNumber(i) + " = " + GC[i]);
                if (RE.GroupNameFromNumber(i) == "user")
                {
                    object playerExists = 
                        Transaction.ExecuteScalar("select count(*) from players where name = '" + GC[i] + "'");
                    if (int.Parse(playerExists.ToString()) == 0)
                    {
                        Transaction.ExecuteNonQuery("insert into players (name) values ('" + GC[i] + "')");
                    }
                }                
            }
        }
    }
}
