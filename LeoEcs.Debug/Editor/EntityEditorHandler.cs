namespace UniGame.LeoEcs.Debug.Editor
{
    using Converter.Runtime.Editor;
    using UnityEditor;

    public static class EntityEditorHandler
    {
        [InitializeOnLoadMethod]
        public static void EditorInitialize()
        {
            EntityEditorCommands.OnEntityInfoRequested -= OpenEntityInfo;
            EntityEditorCommands.OnEntityInfoRequested += OpenEntityInfo;
        }

        public static void OpenEntityInfo(int entity)
        {
            EntityDataWindow.OpenPopupWindow(entity);
        }
    }
}