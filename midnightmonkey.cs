using MelonLoader;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Weapons;
using UnityEngine;
using Random = System.Random;
using System.Collections.Generic;
using System.Linq;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.TowerSets;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Towers;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using BTD_Mod_Helper.Api.Display;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.TowerFilters;
using Il2CppAssets.Scripts.Models.Map;
using Il2CppAssets.Scripts.Models.Audio;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using HarmonyLib;
using Il2Cpp;
using System;
using BTD_Mod_Helper.Api.Components;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using UnityEngine.Assertions;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Track;
using midnightmonkey;
using BTD_Mod_Helper.Api.ModOptions;
using Il2CppAssets.Scripts.Models.Profile;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Unity.Utils;
using Il2CppAssets.Scripts.Utils;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Unity.Utils.ElasticSearch;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity.Towers;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors;
using static MelonLoader.MelonLogger;
using Il2CppTMPro;

[assembly: MelonInfo(typeof(midnightmonkey.midnightmonkey), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace midnightmonkey;

public class midnightmonkey : BloonsTD6Mod
{
    public int midnightDurationFrames = 0;
    public static SpriteReference mrBeast => ModContent.GetSpriteReference<midnightmonkey>("midnightblur");
    public bool canActivate = true;
    public bool activeability = false;
    public override void OnApplicationStart()
    {
        ModHelper.Msg<midnightmonkey>("midnightmonkey loaded!");
    }
    public override void OnAbilityCast(Ability ability)
    {
        if (InGame.instance == null || InGame.instance.bridge == null) { return; }
        InGame game = InGame.instance;
        RectTransform rect = game.uiRect;
        var tower = ability.tower;
        if (tower.model.name.Contains("midnightmonke"))
        {
            var time = ability.CooldownRemaining.ToString();
            if (canActivate == true)
            {
                if (activeability == false)
                {
                    midnightDurationFrames = ((int)ability.CooldownRemaining);
                    activeability = true;
                    MenuUi.CreateUpgradeMenu(rect);
                }
            }
        }
    }
    public override void OnUpdate()
    {
        if (InGame.instance == null || InGame.instance.bridge == null) { return; }
        InGame game = InGame.instance;
        RectTransform rect = game.uiRect;
        if (activeability == true)
        {
            canActivate = false;
            midnightDurationFrames -= 3;
            if (midnightDurationFrames <= 0)
            {
                midnightDurationFrames = 0;
                MenuUi.instance.CloseMenu();
                canActivate = true;
                activeability = false;
            }
        }
    }
    [RegisterTypeInIl2Cpp(false)]
    public class MenuUi : MonoBehaviour
    {

        public static MenuUi instance;
        public ModHelperInputField input;
        public void CloseMenu()
        {
            Destroy(gameObject);
        }
        public static void CreateUpgradeMenu(RectTransform rect)
        {
            ModHelperPanel panel = rect.gameObject.AddModHelperPanel(new Info("Panel_", 500, 500, 1, 1, new UnityEngine.Vector2()), VanillaSprites.BrownInsertPanel);
            MenuUi upgradeUi = panel.AddComponent<MenuUi>();
            instance = upgradeUi;
            var image = panel.AddImage(new Info("SacrificeIcon", 12000), mrBeast.guidRef);
            var newColor = new UnityEngine.Color(image.Image.color.r, image.Image.color.g, image.Image.color.b, 0.85f);
            image.Image.color = newColor;
        }
    }
        public override void OnMatchStart()
    {
        canActivate = true;
        activeability = false;
        midnightDurationFrames = 0;
    }

    public class midnightmonke : ModTower
    {


        public override TowerSet TowerSet => TowerSet.Primary;
        public override string BaseTower => "DartMonkey-002";
        public override int Cost => 500;
        public override int TopPathUpgrades => 5;
        public override int MiddlePathUpgrades => 5;
        public override int BottomPathUpgrades => 5;
        public override string Description => "Activate your midnight ability to deal increased short range damage with the power of the night.";
        public override string DisplayName => "Midnight Monkey";

        public override ParagonMode ParagonMode => ParagonMode.Base555;
        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            var abilityModel = new AbilityModel("AbilityModel_Midnight", "Midnight",
                "Activates midnight", 1, 0,
                GetSpriteReference("midnightIcon"), 20f, null, false, false, null,
                0, 0, 9999999, false, false);
            towerModel.AddBehavior(abilityModel);
            var activateAttackModel = new ActivateAttackModel("ActivateAttackModel_Execute", 10, true,
            new Il2CppReferenceArray<AttackModel>(1), true, false, false, false, false);
            abilityModel.AddBehavior(activateAttackModel);
            var attackModel = activateAttackModel.attacks[0] = Game.instance.model.GetTower(TowerType.Sauda).GetAttackModel().Duplicate();
            attackModel.range += 20;
            activateAttackModel.AddChildDependant(attackModel);
            var targetFirstModel = attackModel.GetBehavior<TargetFirstModel>();
            attackModel.targetProvider = targetFirstModel;
            attackModel.attackThroughWalls = true;
            var weapon = attackModel.weapons[0];
            weapon.emission.AddBehavior(
                new EmissionRotationOffBloonDirectionModel("EmissionRotationOffBloonDirectionModel", false, false));
            abilityModel.dontShowStacked = true;
            var projectileModel = weapon.projectile;
            projectileModel.GetDamageModel().damage = 8;
            projectileModel.GetDamageModel().immuneBloonProperties = BloonProperties.None;
            var soundModel2 = Game.instance.model.GetTower(TowerType.BoomerangMonkey, 0, 4, 0).GetAbility().GetBehavior<CreateSoundOnAbilityModel>().Duplicate();
            soundModel2.sound.assetId = GetAudioSourceReference<midnightmonkey>("midnightActivate");
            abilityModel.AddBehavior(soundModel2);
            var soundModel = weapon.GetBehavior<CreateSoundOnProjectileCreatedModel>();
            soundModel.sound1.assetId = GetAudioSourceReference<midnightmonkey>("midnightStrike");
            soundModel.sound2.assetId = GetAudioSourceReference<midnightmonkey>("midnightStrike2");
            soundModel.sound3.assetId = GetAudioSourceReference<midnightmonkey>("midnightStrike3");
            soundModel.sound4.assetId = GetAudioSourceReference<midnightmonkey>("midnightStrike");
            soundModel.sound5.assetId = GetAudioSourceReference<midnightmonkey>("midnightStrike3");
            projectileModel.AddBehavior(soundModel);
            var genuineAttackModel = towerModel.GetAttackModel();
            towerModel.ApplyDisplay<midnightMonkeyBaseDisplay>();
            genuineAttackModel.weapons[0].projectile.display = Game.instance.model.GetTowerFromId("SuperMonkey-003").GetWeapon().projectile.display;
        }
    }
    public class midnightMonkeyBaseDisplay : ModDisplay
    {
        public override string BaseDisplay => GetDisplay(TowerType.DartMonkey);
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            node.RemoveBone("DartMonkeyDart");
            SetMeshTexture(node, "displayshit", 2);
        }
    }
    public class piercingmidnight : ModUpgrade<midnightmonke>
    {
        public override int Path => TOP;
        public override int Tier => 1;
        public override int Cost => 400;
        public override string Icon => "durableIcon";
        public override string DisplayName => "Durable Blades";

        public override string Description => "Night blades have more pierce. Midnight ability has more pierce.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetAttackModel().weapons[0].projectile.pierce += 4f;
            var abilityAttack = towerModel.GetAbility().GetBehavior<ActivateAttackModel>().attacks[0];
            abilityAttack.weapons[0].projectile.pierce += 6f;
        }
    }
    public class leadmidnight : ModUpgrade<midnightmonke>
    {
        public override int Path => TOP;
        public override int Tier => 2;
        public override int Cost => 650;
        public override string Icon => "sharpIcon";
        public override string DisplayName => "Sharper Blades";

        public override string Description => "Night blades have more pierce and cut through lead. Midnight ability has even more pierce.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetAttackModel().weapons[0].projectile.pierce += 6f;
            towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
            var abilityAttack = towerModel.GetAbility().GetBehavior<ActivateAttackModel>().attacks[0];
            abilityAttack.weapons[0].projectile.pierce += 8f;
        }
    }
    public class slighthoming : ModUpgrade<midnightmonke>
    {
        public override int Path => TOP;
        public override int Tier => 3;
        public override int Cost => 1450;
        public override string Icon => "crosshair";
        public override string DisplayName => "Richocheting Blades";

        public override string Description => "Night blades slightly home onto enemies and bounce from them.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(new RetargetOnContactModel("erridkstoplookingatme>:(",300f,250,"Close",0.05f,true));
            towerModel.GetAttackModel().weapons[0].projectile.pierce += 10f;
        }
    }
    public class explosiveblades : ModUpgrade<midnightmonke>
    {
        public override int Path => TOP;
        public override int Tier => 4;
        public override int Cost => 5000;
        public override string Icon => "explosion";
        public override string DisplayName => "Explosive Blades";

        public override string Description => "Night blades explode on contact. They also deal +3 damage.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().damage += 3;
            var projectileModel = towerModel.GetAttackModel().weapons[0].projectile;
            projectileModel.AddBehavior(Game.instance.model.GetTowerFromId("BombShooter").GetWeapon().projectile.GetBehavior<CreateProjectileOnContactModel>().Duplicate());
            projectileModel.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage = 2;
            projectileModel.AddBehavior(Game.instance.model.GetTowerFromId("BombShooter").GetWeapon().projectile.GetBehavior<CreateSoundOnProjectileCollisionModel>().Duplicate());
            projectileModel.AddBehavior(Game.instance.model.GetTowerFromId("BombShooter").GetWeapon().projectile.GetBehavior<CreateEffectOnContactModel>().Duplicate());
        }
    }
    public class darkmoonblades : ModUpgrade<midnightmonke>
    {
        public override int Path => TOP;
        public override int Tier => 5;
        public override int Cost => 26000;
        public override string Icon => "darkaura";
        public override string DisplayName => "Dark Moon Blades";

        public override string Description => "Dark Moon Blades have heavily increased damage and pierce.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().damage += 18;
            var projectileModel = towerModel.GetAttackModel().weapons[0].projectile;
            projectileModel.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage = 8;
            towerModel.GetAttackModel().weapons[0].projectile.pierce += 40f;
        }
    }
    public class Darkhands : ModUpgrade<midnightmonke>
    {
        public override int Path => MIDDLE;
        public override int Tier => 1;
        public override int Cost => 400;
        public override string Icon => "darkHands";
        public override string DisplayName => "Dark Hands";

        public override string Description => "Faster throwing and midnight ability has faster attack speed.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetAttackModel().weapons[0].rate *= 0.6f;
            var abilityAttack = towerModel.GetAbility().GetBehavior<ActivateAttackModel>().attacks[0];
            abilityAttack.weapons[0].rate *= 0.6f;
        }
    }
    public class DarkRange : ModUpgrade<midnightmonke>
    {
        public override int Path => MIDDLE;
        public override int Tier => 2;
        public override int Cost => 500;
        public override string Icon => "darkRange";
        public override string DisplayName => "Increased Range";

        public override string Description => "Further range. Midnight ability is better at hitting bloons.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetAttackModel().range += 20;
            towerModel.range += 20;
            var abilityAttack = towerModel.GetAbility().GetBehavior<ActivateAttackModel>().attacks[0];
            abilityAttack.weapons[0].projectile.radius *= 2;
            abilityAttack.weapons[0].projectile.scale *= 2;
        }
    }
    public class NocturneDamage : ModUpgrade<midnightmonke>
    {
        public override int Path => MIDDLE;
        public override int Tier => 3;
        public override int Cost => 1700;
        public override string Icon => "moon";
        public override string DisplayName => "Nocturne Damage";

        public override string Description => "Both the ability and blades deal nocturnal damage, which deal increased damage to MOAB class and ceramic.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("dmgUp","Moabs",1f,10f,false,false));
            towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("dmgUp2", "Ceramic", 1f, 2f, false, false));
            towerModel.GetAttackModel().weapons[0].projectile.hasDamageModifiers = true;
            var abilityAttack = towerModel.GetAbility().GetBehavior<ActivateAttackModel>().attacks[0];
            abilityAttack.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("dmgUp", "Moabs", 2f, 10f, false, false));
            abilityAttack.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("dmgUp2", "Ceramic", 1f, 2f, false, false));
            abilityAttack.weapons[0].projectile.hasDamageModifiers = true;
        }
    }
    public class SecretStun : ModUpgrade<midnightmonke>
    {
        public override int Path => MIDDLE;
        public override int Tier => 4;
        public override int Cost => 6000;
        public override string Icon => "dizzyIcon";
        public override string DisplayName => "Evening Stun";

        public override string Description => "During midnight, each hit stuns bloons briefly";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var abilityAttack = towerModel.GetAbility().GetBehavior<ActivateAttackModel>().attacks[0];
            var bombthang = Game.instance.model.GetTower(TowerType.BombShooter, 5);
            var weapon = abilityAttack.weapons[0];
            var slow = bombthang.GetDescendant<SlowModel>().Duplicate();
            slow.lifespan = 0.2f;
            slow.lifespanFrames = 120;
            weapon.projectile.collisionPasses = new[] { -1, 0 };
            weapon.projectile.AddBehavior(slow);
        }
    }
    public class DarkPrism : ModUpgrade<midnightmonke>
    {
        public override int Path => MIDDLE;
        public override int Tier => 5;
        public override int Cost => 56000;
        public override string Icon => "darknesss";
        public override string DisplayName => "Permanent Darkness";

        public override string Description => "Always in midnight mode. Nocturne damage is more effective. Midnight ability increases damage for a few seconds.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var abilityAttack = towerModel.GetAbility().GetBehavior<ActivateAttackModel>().attacks[0];
            abilityAttack.weapons[0].projectile.GetDamageModel().damage += 25;
            towerModel.AddBehavior(abilityAttack);
            towerModel.GetAbility().RemoveBehavior<ActivateAttackModel>();
            var turbo = Game.instance.model.GetTowerFromId("BoomerangMonkey-040").GetAbility().GetBehavior<TurboModel>().Duplicate();
            turbo.projectileDisplay.assetPath = CreatePrefabReference<stuffff>();
            turbo.multiplier = 1;
            turbo.extraDamage += 25;
        }
    }
    public class stuffff : ModDisplay
    {
        public override string BaseDisplay => Game.instance.model.GetTowerFromId("Sauda").GetWeapon().projectile.display.guidRef;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            
        }
    }
    public class ShadowAssistance : ModUpgrade<midnightmonke>
    {
        public override int Path => BOTTOM;
        public override int Tier => 1;
        public override int Cost => 500;
        public override string Icon => "hand";
        public override string DisplayName => "Dark Assitance";

        public override string Description => "Nearby towers gain +1 MOAB damage";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.AddBehavior(new DamageModifierSupportModel("NinjaMOABIncrease22", true, "NinjaMOABincrease22", null, true, new DamageModifierForTagModel("Moabs", "Moabs", 1, 2, false, true)));
        }
    }
    public class Energy : ModUpgrade<midnightmonke>
    {
        public override int Path => BOTTOM;
        public override int Tier => 2;
        public override int Cost => 800;
        public override string Icon => "nightTime";
        public override string DisplayName => "Lasting Nighttime";

        public override string Description => "Midnight ability lasts longer";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetAbility().GetBehavior<ActivateAttackModel>().lifespan = 15;
            towerModel.GetAbility().GetBehavior<ActivateAttackModel>().lifespanFrames = 900;
        }
    }
    public class DarkMarket : ModUpgrade<midnightmonke>
    {
        public override int Path => BOTTOM;
        public override int Tier => 3;
        public override int Cost => 2700;
        public override string Icon => "darkMarket";
        public override string DisplayName => "Dark Market";

        public override string Description => "Makes cash secretly and gives nearby towers discounts.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.AddBehavior(Game.instance.model.GetTowerFromId("BananaFarm-005").GetBehavior<PerRoundCashBonusTowerModel>().Duplicate());
            towerModel.GetBehavior<PerRoundCashBonusTowerModel>().cashPerRound = 250;
            towerModel.AddBehavior(new DiscountZoneModel("discount", 0.2f, 3, "urmomGay", "urmomGay", false, 4, "shutUp", "shutUp2"));
        }
    }
    public class BareMidnight : ModUpgrade<midnightmonke>
    {
        public override int Path => BOTTOM;
        public override int Tier => 4;
        public override int Cost => 5600;
        public override string Icon => "human";
        public override string DisplayName => "Bare Night";

        public override string Description => "Bloons hit by blades take extra damage.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(new AddBonusDamagePerHitToBloonModel("stinky", "bareNightDamage", 60f, 8f, 9, true, true, false));

        }
    }
    public class TheDarkSide : ModUpgrade<midnightmonke>
    {
        public override int Path => BOTTOM;
        public override int Tier => 5;
        public override int Cost => 75000;
        public override string Icon => "darkside";
        public override string DisplayName => "The Dark Side";

        public override string Description => "All stats increase with the immense power of night";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<AddBonusDamagePerHitToBloonModel>().perHitDamageAddition += 10;
            towerModel.GetBehavior<PerRoundCashBonusTowerModel>().cashPerRound += 1250;
            towerModel.GetBehavior<DamageModifierSupportModel>().increase += 12f;

        }
    }
    public class NightMongerer : ModParagonUpgrade<midnightmonke>
    {
        public override int Cost => 600000;
        public override string Description => "One with the night...";
        public override string DisplayName => "Night Longerer";
        public override string Portrait => "midnightIcon";
        public override string Icon => "midnightmonke-Icon";
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.range += 60;
            var attackModel2 = towerModel.GetAttackModel();
            attackModel2.range += 60;
            attackModel2.weapons[0].rate *= 0.001f;
            var abilityModel = new AbilityModel("AbilityModel_Midnight", "Midnight",
                "Activates midnight", 1, 0,
                GetSpriteReference("midnightIcon"), 40f, null, false, false, null,
                0, 0, 9999999, false, false);
            towerModel.AddBehavior(abilityModel);
            var activateAttackModel = new ActivateAttackModel("ActivateAttackModel_Execute", 20, true,
            new Il2CppReferenceArray<AttackModel>(1), true, false, false, false, false);
            abilityModel.AddBehavior(activateAttackModel);
            var attackModel = activateAttackModel.attacks[0] = Game.instance.model.GetTower(TowerType.Sauda).GetAttackModel().Duplicate();
            attackModel.range += 20;
            activateAttackModel.AddChildDependant(attackModel);
            var targetFirstModel = attackModel.GetBehavior<TargetFirstModel>();
            attackModel.targetProvider = targetFirstModel;
            attackModel.attackThroughWalls = true;
            var weapon = attackModel.weapons[0];
            weapon.emission.AddBehavior(
                new EmissionRotationOffBloonDirectionModel("EmissionRotationOffBloonDirectionModel", false, false));
            abilityModel.dontShowStacked = true;
            var projectileModel = weapon.projectile;
            projectileModel.GetDamageModel().damage = 1000f;
            weapon.rate *= 0.12f;
            projectileModel.GetDamageModel().immuneBloonProperties = BloonProperties.None;
            var soundModel2 = Game.instance.model.GetTower(TowerType.BoomerangMonkey, 0, 4, 0).GetAbility().GetBehavior<CreateSoundOnAbilityModel>().Duplicate();
            soundModel2.sound.assetId = GetAudioSourceReference<midnightmonkey>("midnightActivate");
            abilityModel.AddBehavior(soundModel2);
            var soundModel = weapon.GetBehavior<CreateSoundOnProjectileCreatedModel>();
            soundModel.sound1.assetId = GetAudioSourceReference<midnightmonkey>("midnightStrike");
            soundModel.sound2.assetId = GetAudioSourceReference<midnightmonkey>("midnightStrike2");
            soundModel.sound3.assetId = GetAudioSourceReference<midnightmonkey>("midnightStrike3");
            soundModel.sound4.assetId = GetAudioSourceReference<midnightmonkey>("midnightStrike");
            soundModel.sound5.assetId = GetAudioSourceReference<midnightmonkey>("midnightStrike3");
            projectileModel.AddBehavior(soundModel);
        }
    }
}