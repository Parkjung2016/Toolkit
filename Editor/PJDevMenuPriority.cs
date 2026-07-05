namespace PJDev.DevelopKit.Editors
{
    public static class PJDevMenuPriority
    {
        public const int MenuBarRoot = -10000;
        public const int GameObjectRoot = 0;

        public const int Hub = MenuBarRoot;
        public const int Inventory = MenuBarRoot + 100;
        public const int GameplayTags = MenuBarRoot + 200;
        public const int AnimMontage = MenuBarRoot + 250;
        public const int Ui = MenuBarRoot + 300;
        public const int CDebug = MenuBarRoot + 400;
        public const int Addressable = MenuBarRoot + 500;
    }
}
