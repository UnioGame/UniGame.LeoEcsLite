namespace UniGame.LeoEcs.Converter.Runtime.Editor
{
    using System;

    public static class EntityEditorCommands
    {

        public static Action<int> OnEntityInfoRequested;

        public static void OpenEntityInfo(int entityId)
        {
            OnEntityInfoRequested?.Invoke(entityId);
        }
        
    }
}