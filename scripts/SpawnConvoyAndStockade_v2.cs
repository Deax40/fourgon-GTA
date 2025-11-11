
// SpawnConvoyAndStockade_v2.cs
// GTA V (Single-player) - ScriptHookVDotNet C# script
// Changes in v2:
// - Robust convoy spawn with guaranteed BLIPs on lead + optional on escorts
// - Configurable via INI (scripts/SpawnConvoyAndStockade.ini)
// - Extra null-safety & model loading, better ground placement
// - Optional debug notifications
//
// WARNING: SOLO uniquement. N'utilisez JAMAIS ce mod en GTA Online.

using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;
using System.Windows.Forms;

public class SpawnConvoyAndStockade_v2 : Script
{
    // State
    private bool enabled = false;
    private int lastSingleCheck = 0;
    private int lastConvoyCheck = 0;
    private Random rng = new Random();

    // Config (defaults; can be overridden by INI)
    private Keys toggleKey = Keys.F7;
    private float spawnDistance = 30.0f;
    private float existCheckRadius = 90.0f;
    private int stockadeIntervalMs = 4500;
    private int convoyCheckIntervalMs = 18000;
    private int convoyChancePercent = 20;
    private int convoyMinLength = 2;
    private int convoyMaxLength = 4;
    private bool blipsOnEscort = true;
    private bool debugNotifies = true;

    private string iniName = "scripts/SpawnConvoyAndStockade.ini";

    public SpawnConvoyAndStockade_v2()
    {
        LoadIni();
        Interval = 100;
        Tick += OnTick;
        KeyDown += OnKeyDown;
        Notify("SpawnConvoy v2 chargé — " + toggleKey + " pour activer/désactiver");
    }

    private void LoadIni()
    {
        try
        {
            var s = ScriptSettings.Load(iniName);

            string toggle = s.GetValue("General", "ToggleKey", "F7");
            toggleKey = (Keys)Enum.Parse(typeof(Keys), toggle, true);

            spawnDistance = s.GetValue("Spawn", "SpawnDistance", 30.0f);
            existCheckRadius = s.GetValue("Spawn", "ExistCheckRadius", 90.0f);

            stockadeIntervalMs = s.GetValue("Timing", "StockadeIntervalMs", 4500);
            convoyCheckIntervalMs = s.GetValue("Timing", "ConvoyCheckIntervalMs", 18000);
            convoyChancePercent = s.GetValue("Convoy", "ConvoyChancePercent", 20);
            convoyMinLength = s.GetValue("Convoy", "ConvoyMinLength", 2);
            convoyMaxLength = s.GetValue("Convoy", "ConvoyMaxLength", 4);

            blipsOnEscort = s.GetValue("UI", "BlipsOnEscort", true);
            debugNotifies = s.GetValue("UI", "DebugNotifies", true);
        }
        catch
        {
            // keep defaults
        }
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == toggleKey)
        {
            enabled = !enabled;
            Notify("Spawn automatique " + (enabled ? "~g~ACTIVÉ" : "~r~DÉSACTIVÉ"));
        }
    }

    private void OnTick(object sender, EventArgs e)
    {
        if (!enabled) return;
        var player = Game.Player?.Character;
        if (player == null || !player.Exists() || !player.IsAlive) return;

        int now = Game.GameTime;

        // Single Stockade generation
        if (now - lastSingleCheck >= stockadeIntervalMs)
        {
            lastSingleCheck = now;
            TrySpawnSingleStockadeNear(player);
        }

        // Convoy generation
        if (now - lastConvoyCheck >= convoyCheckIntervalMs)
        {
            lastConvoyCheck = now;
            int roll = rng.Next(100);
            if (roll < convoyChancePercent)
            {
                SpawnConvoyNear(player);
            }
        }
    }

    private void TrySpawnSingleStockadeNear(Ped player)
    {
        int nearby = CountNearbyStockades(player.Position, existCheckRadius);
        if (nearby > 0) return;

        Vector3 desired = player.Position + player.ForwardVector * spawnDistance;
        Vector3 spawnPos = FindGroundedPosition(desired);

        var stockade = CreateVehicle("stockade", spawnPos, player.Heading);
        if (stockade == null) return;

        var driver = PutDriver(stockade, "s_m_m_security_01");
        if (driver != null)
        {
            TaskWander(driver, stockade, 8f);
        }

        AddBlip(stockade, BlipSprite.ArmoredTruck, "Fourgon blindé");
        Debug("Stockade apparu près du joueur.");
    }

    private void SpawnConvoyNear(Ped player)
    {
        int n = rng.Next(Math.Max(1, convoyMinLength), Math.Max(convoyMinLength, convoyMaxLength) + 1);
        Vector3 basePos = player.Position + player.ForwardVector * (spawnDistance + 25f) + player.RightVector * rng.Next(-18, 18);
        basePos = FindGroundedPosition(basePos);

        List<Vehicle> vehicles = new List<Vehicle>();

        // Lead armored
        var lead = CreateVehicle("stockade", basePos, player.Heading);
        if (lead == null) return;
        vehicles.Add(lead);

        // Escorts
        string[] escorts = { "granger", "rumpo3", "sheriff", "sentinel", "landstalker2" };
        for (int i = 1; i < n; i++)
        {
            Vector3 p = basePos - player.ForwardVector * (4.0f * i) + player.RightVector * rng.Next(-2, 3);
            p = FindGroundedPosition(p);
            var v = CreateVehicle(escorts[rng.Next(escorts.Length)], p, player.Heading);
            if (v != null) vehicles.Add(v);
        }

        // Drivers + behaviors
        foreach (var v in vehicles)
        {
            var d = PutDriver(v, "s_m_m_security_01");
            if (d == null) continue;

            int behavior = rng.Next(100);
            if (behavior < 65)
            {
                DrivePastPlayer(d, v, player.Position + player.ForwardVector * (spawnDistance + 200f), 12f);
            }
            else
            {
                TaskWander(d, v, 0.1f);
                // simulate stop and guards
                int delay = rng.Next(2500, 6000);
                DelayedAction(delay, () => OpenDoorsAndSpawnGuards(v));
            }
        }

        // BLIPs: always on lead, optional on escorts
        AddBlip(lead, BlipSprite.ArmoredTruck, "Convoi blindé (Lead)");
        if (blipsOnEscort)
        {
            for (int i = 1; i < vehicles.Count; i++)
            {
                AddBlip(vehicles[i], BlipSprite.PersonalVehicleCar, "Escort");
            }
        }

        Notify($"Convoi ~b~spawné~s~ : ~y~{vehicles.Count}~s~ véhicules. Repéré sur carte.");
    }

    // --- Helpers ---

    private int CountNearbyStockades(Vector3 pos, float radius)
    {
        int c = 0;
        foreach (var v in World.GetAllVehicles())
        {
            try
            {
                if (v != null && v.Exists() && v.Position.DistanceTo(pos) <= radius)
                {
                    if (v.Model.Hash == new Model("stockade").Hash) c++;
                }
            }
            catch { }
        }
        return c;
    }

    private Vehicle CreateVehicle(string modelName, Vector3 pos, float heading)
    {
        try
        {
            var m = new Model(modelName);
            if (!m.IsInCdImage) return null;
            if (!m.Request(1000)) return null;
            var v = World.CreateVehicle(m, pos);
            if (v == null || !v.Exists()) return null;
            v.Heading = heading;
            v.IsPersistent = true;
            v.NeedsToBeHotwired = false;
            v.LockStatus = VehicleLockStatus.Unlocked;
            return v;
        }
        catch { return null; }
    }

    private Ped PutDriver(Vehicle v, string pedModel)
    {
        try
        {
            var m = new Model(pedModel);
            if (!m.IsInCdImage) return null;
            if (!m.Request(800)) return null;
            var d = v.CreatePedOnSeat(VehicleSeat.Driver, m);
            if (d == null) return null;
            d.BlockPermanentEvents = true;
            Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, d.Handle, 5, true);
            return d;
        }
        catch { return null; }
    }

    private void TaskWander(Ped d, Vehicle v, float speed)
    {
        try
        {
            Function.Call(Hash.TASK_VEHICLE_DRIVE_WANDER, d.Handle, v.Handle, speed, 786603);
        }
        catch { }
    }

    private void DrivePastPlayer(Ped d, Vehicle v, Vector3 dest, float speed)
    {
        try
        {
            Function.Call(Hash.TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE, d.Handle, v.Handle, dest.X, dest.Y, dest.Z, speed, 786603, 10f);
        }
        catch { }
    }

    private void OpenDoorsAndSpawnGuards(Vehicle v)
    {
        try
        {
            if (v == null || !v.Exists()) return;
            if (v.Doors != null)
            {
                try { v.Doors[VehicleDoor.FrontLeft]?.Open(); } catch { }
                try { v.Doors[VehicleDoor.FrontRight]?.Open(); } catch { }
                try { v.Doors[VehicleDoor.BackLeft]?.Open(); } catch { }
                try { v.Doors[VehicleDoor.BackRight]?.Open(); } catch { }
            }

            int guards = rng.Next(1, 3);
            for (int i = 0; i < guards; i++)
            {
                Vector3 p = v.Position + new Vector3(rng.Next(-2, 3), rng.Next(-2, 3), 0);
                p = FindGroundedPosition(p);
                var g = CreatePed("s_m_m_security_01", p);
                if (g == null) continue;
                g.Weapons.Give(WeaponHash.Pistol, 120, true, true);
                g.RelationshipGroup = World.AddRelationshipGroup("SECURITY");
                World.SetRelationshipBetweenGroups(Relationship.Hate, g.RelationshipGroup, Game.Player.Character.RelationshipGroup);
                Function.Call(Hash.TASK_COMBAT_PED, g.Handle, Game.Player.Character.Handle, 0, 16);
            }
        }
        catch { }
    }

    private Ped CreatePed(string modelName, Vector3 pos)
    {
        try
        {
            var m = new Model(modelName);
            if (!m.IsInCdImage) return null;
            if (!m.Request(800)) return null;
            var p = World.CreatePed(m, pos);
            if (p == null || !p.Exists()) return null;
            p.BlockPermanentEvents = true;
            return p;
        }
        catch { return null; }
    }

    private Blip AddBlip(Entity e, BlipSprite sprite, string name)
    {
        try
        {
            var b = e.AddBlip();
            if (b == null) return null;
            b.Sprite = sprite;
            b.Name = name;
            b.IsShortRange = false;
            return b;
        }
        catch { return null; }
    }

    private Vector3 FindGroundedPosition(Vector3 near)
    {
        Vector3 test = near;
        for (int i = 0; i < 10; i++)
        {
            if (World.GetGroundHeight(test, out float groundZ))
            {
                test.Z = groundZ + 1.0f;
                return test;
            }
            test.Z += 1.0f;
        }
        return near;
    }

    private void DelayedAction(int ms, Action act)
    {
        try
        {
            // Run within script fiber using Wait
            int start = Game.GameTime;
            while (Game.GameTime - start < ms) Script.Wait(0);
            act?.Invoke();
        }
        catch { }
    }

    private void Notify(string message)
    {
        UI.Notify(message);
    }
    private void Debug(string message)
    {
        if (debugNotifies) UI.Notify("[DEBUG] " + message);
    }
}
