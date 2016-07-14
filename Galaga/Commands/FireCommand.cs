using System;
using System.Collections.Generic;
using System.Text;

using edu.CiclosFormativos.Games.DIDAM.Entities;
using edu.CiclosFormativos.Games.DIDAM.Commands;
using edu.CiclosFormativos.Games.DIDAM.Scenes;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Commands
{
    /// <summary>
    /// Encapsula un comando que dispara un misíl
    /// </summary>
    public class FireCommand : Command
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public FireCommand()
        {
        }

        public override void Execute(SceneNode subject, SFML.System.Time dt)
        {
            if (subject.GetType() == typeof(Entities.PlayerShip))
                ((Entities.PlayerShip)subject).Fire();
        }
    }
}
