using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Commands
{
    /// <summary>
    /// Encapsula un comando que mueve de manera lineal una entidad
    /// </summary>
    class FireCommand : Command
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
