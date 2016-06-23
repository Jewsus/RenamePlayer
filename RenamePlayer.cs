using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using Terraria;
using TShockAPI;
using System.Text.RegularExpressions;
using TerrariaApi.Server;

namespace RenamePlayer
{
    [ApiVersion(1, 23)]
    public class RenamePlayer : TerrariaPlugin
    {
        private TSPlayer _player;
        private string _newName;
        private bool _nameChanged = false;
        private static string configFile = Path.Combine(TShock.SavePath, "RenamePlayer.cfg");
        public static Config _config = new Config();
        public static bool _replaceWords = false;

        public override void Initialize()
        {
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer);

            if (_config.LoadConfig(configFile))
            {
                try
                {
                    _replaceWords = bool.Parse(_config["enablewordreplace"]);
                }
                catch (KeyNotFoundException)
                {
                    Console.WriteLine("! Unable to parse 'enableWordReplace' from config file: " + configFile);
                    Thread.Sleep(5000);
                }
            }
            else
            {
                Console.WriteLine("! Config failed to load, skipping name replacement.");
                Thread.Sleep(5000);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreetPlayer);
                ServerApi.Hooks.ServerConnect.Deregister(this, OnJoin);
                base.Dispose(disposing);
            }
        }
        void OnJoin(int playerId, HandledEventArgs eventArgs)
        {
            try
            {
                _player = TShock.Players[playerId];
                ConvertEncoding();
                ReplaceWords();

                if (_player.Name != _newName)
                {
                    TShock.Utils.Broadcast(string.Format("Player '{0}' has been renamed to '{1}'.", _player.Name, _newName),
                                            Color.IndianRed);
                    TShock.Log.Info(string.Format("Player '{0}' has been renamed to '{1}'.", _player.Name, _newName));

                    _player.TPlayer.name = _newName;
                    _nameChanged = true;
                }
            }
            catch (Exception exception)
            {
                TShock.Log.Error(string.Format("! Error: {0}: {1}", exception.Message, exception.StackTrace));
            }
        }
        void OnGreetPlayer(GreetPlayerEventArgs args)
        //void OnGreetPlayer(int playerId, HandledEventArgs eventArgs)
        {
            if (_nameChanged)
            {
                _player.SendMessage("Warning: Your name had invalid characters, it has been changed to: " + _newName,
                                     Color.OrangeRed);
            }

        }
        private void ConvertEncoding()
        {
            byte[] byteArray = Encoding.Unicode.GetBytes(_player.Name);
            byte[] asciiArray = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, byteArray);

            _newName = Encoding.ASCII.GetString(asciiArray);
            if (_player.Name != _newName)
            {
                _newName = "guest_" + _player.Name.ToString().GetHashCode().ToString("x").Substring(0, 4);
            }
        }
        private void ReplaceWords()
        {

            if (_replaceWords)
            {
                foreach (var entry in _config)
                {
                    if (_newName.ToLower().Contains(entry.Key.ToLower()))
                    {
                        _newName = Regex.Replace(_newName, entry.Key, entry.Value, RegexOptions.IgnoreCase);
                        //            _newName = _newName.ToLower().Replace( entry.Key.ToLower(), entry.Value );
                    }
                }
            }

        }
        public override string Name
        {
            get { return "RenamePlayer"; }
        }
        public override string Author
        {
            get { return "Scavenger, _Jon"; }
        }
        public override string Description
        {
            get { return "Renames players with Invalid Characters"; }
        }
        public override Version Version
        {
            get { return new Version(1, 0, 3, 0); }
        }
        public RenamePlayer(Main game) : base(game)
        {
            Order = -11;
        }
    }
}