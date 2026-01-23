namespace CatRunner.Core
{
    public enum GameState
    {
        Idle,           // Ovillo colgando - menu
        CutYarn,        // Corte del ovillo
        FastTravel,     // Viaje rápido fake
        SlowDown,       // Frenada del viaje
        ArrivalIntro,   // Gato baja de la caja
        Playing,        // Endless runner activo
        GameOver,       // Game over, trasncisión al ovillo colgando
        ReturnToMenu    // Transición de regreso al menú principal
    }
}
