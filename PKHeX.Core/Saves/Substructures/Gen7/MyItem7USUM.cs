using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class MyItem7USUM : MyItem
{
    private const int HeldItem = 0; // 427 (Case 0)
    private const int KeyItem = HeldItem + (4 * 427); // 198 (Case 4)
    private const int TMHM = KeyItem + (4 * 198); // 108 (Case 2)
    private const int Medicine = TMHM + (4 * 108); // 60 (Case 1)
    private const int Berry = Medicine + (4 * 60); // 67 (Case 3)
    private const int ZCrystals = Berry + (4 * 67); // 35 (Case 5)
    private const int BattleItems = ZCrystals + (4 * 35); // 11 (Case 6)

    public MyItem7USUM(SaveFile SAV, int offset) : base(SAV) => Offset = offset;

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            InventoryPouch7[] pouch =
            {
                new(InventoryType.Medicine, Legal.Pouch_Medicine_SM, 999, Medicine),
                new(InventoryType.Items, Legal.Pouch_Items_SM, 999, HeldItem),
                new(InventoryType.TMHMs, Legal.Pouch_TMHM_SM, 1, TMHM),
                new(InventoryType.Berries, Legal.Pouch_Berries_SM, 999, Berry),
                new(InventoryType.KeyItems, Legal.Pouch_Key_USUM, 1, KeyItem),
                new(InventoryType.ZCrystals, Legal.Pouch_ZCrystal_USUM, 1, ZCrystals),
                new(InventoryType.BattleItems, Legal.Pouch_Roto_USUM, 999, BattleItems),
            };
            return pouch.LoadAll(Data);
        }
        set => value.SaveAll(Data);
    }
}
