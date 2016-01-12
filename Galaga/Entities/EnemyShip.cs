using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Entities
{
    /// <summary>
    /// Encapsula la funcionalidad de las naves enemigas.
    /// </summary>
    /// <remarks>
    /// - Puede disparar
    /// - Dan puntos al usuario
    /// - Pueden colisionar con los disparos del jugador o con la propia nave del jugador
    /// - Tiene movimientos controlados por la IA
    /// </remarks>
    class EnemyShip : Entity
    {
        public EnemyShip()
            : base()
        {

        }
    }
}
