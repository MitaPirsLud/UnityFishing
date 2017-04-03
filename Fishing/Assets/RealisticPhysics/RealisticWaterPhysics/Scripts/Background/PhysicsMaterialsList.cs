using UnityEngine;
using System.Collections;

public enum MaterialTypes
{
	Liquids,
	Gases,
	Solid
}

public enum LiquidsMaterialList
{
	WaterFresh,
	WaterSalt,
	Liquidhydrogen,
	Tetrachloroethene,
	Mercury,
	Gasoline,
	Petrol,
	DieselFuel,
	Methanol,
	Turpentine,
	Milk,
	CrudeOil,
	CoconutOil,
	OliveOil,
	SunflowerOil,
	SoyaBeanOil,
	Beer,
}

public enum GasesMaterialList
{
	Air,
	Acetylene,
	Ammonia,
	Argon,
	Benzene,
	BlastFurnaceGas,
	Butane,
	CarbonDioxide,
	Chlorine,
	Hydrogen,
	HydrogenSulfide,
	HydrogenChloride,
	Methane,
	NaturalGas,
	NitricOxide,
	Nitrogen,
	Oxygen,
	Propane,
	SulfurDioxide,
	WaterVapor,
	Helium,
	TungstenHexafluoride,
}

public enum SolidsMaterialList
{
	Aerogel,
	Aerographite,
	Aluminium,
	Antimony,
	Beryllium,
	Bismuth,
	Brass,
	Cadmium,
	Chromium,
	Cobalt,
	Concrete,
	Copper,
	Cork,
	Diamond,
	Diiodomethane,
	Glycerol,
	Gold,
	Human,
	Ice,
	Iridium,
	Iron,
	Lead,
	Lithium,
	Magnesium,
	Manganese,
	MetallicMicrolattice,
	Molybdenum,
	Nickel,
	Niobium,
	Nylon,
	Oak,
	Osmium,
	Pine,
	Plastics,
	Platinum,
	Plutonium,
	Potassium,
	Rhodium,
	Selenium,
	Silicon,
	Silver,
	Sodium,
	Steel,
	Styrofoam,
	Tantalum,
	Thorium,
	Tin,
	Titanium,
	Tungsten,
	Uranium,
	Vanadium,
	Wood,
	Zinc,
	Dama,
}

public static class PhysicsMaterialsList
{
	public static float getLiquidsMaterialValue(LiquidsMaterialList _LiquidsMaterialList)
	{
		switch (_LiquidsMaterialList)
		{
		case LiquidsMaterialList.WaterFresh: return 1000f;
		case LiquidsMaterialList.WaterSalt: return 1030f;
		case LiquidsMaterialList.Liquidhydrogen: return 70.8f;
		case LiquidsMaterialList.Tetrachloroethene: return 1622f;
		case LiquidsMaterialList.Mercury: return 13546f;
		case LiquidsMaterialList.Gasoline: return 711f;
		case LiquidsMaterialList.Petrol: return 711f;
		case LiquidsMaterialList.DieselFuel: return 920f;
		case LiquidsMaterialList.Methanol: return 465f;
		case LiquidsMaterialList.Turpentine: return 868.2f;
		case LiquidsMaterialList.Milk: return 1020f;
		case LiquidsMaterialList.CrudeOil: return 790f;
		case LiquidsMaterialList.CoconutOil: return 924f;
		case LiquidsMaterialList.OliveOil: return 860f;
		case LiquidsMaterialList.SunflowerOil: return 920f;
		case LiquidsMaterialList.SoyaBeanOil: return 925f;
		case LiquidsMaterialList.Beer: return 1010f;
		default: return 0.0f;break;
		}
	}

	public static float getGasesMaterialValue(GasesMaterialList _GasesMaterialList)
	{
		switch (_GasesMaterialList)
		{
		case GasesMaterialList.Acetylene: return 1.0921f;
		case GasesMaterialList.Air: return 1.2051f;
		case GasesMaterialList.Ammonia: return 0.7171f;
		case GasesMaterialList.Argon: return 1.6611f;
		case GasesMaterialList.Benzene: return 3.486f;
		case GasesMaterialList.BlastFurnaceGas: return 1.2502f;
		case GasesMaterialList.Butane: return 2.4891f;
		case GasesMaterialList.CarbonDioxide: return 1.8421f;
		case GasesMaterialList.Chlorine: return 2.9941f;
		case GasesMaterialList.Hydrogen: return 0.08992f;
		case GasesMaterialList.HydrogenSulfide: return 1.4341f;
		case GasesMaterialList.HydrogenChloride: return 1.5281f;
		case GasesMaterialList.Methane: return 0.6681f;
		case GasesMaterialList.NaturalGas: return 0.92f;
		case GasesMaterialList.NitricOxide: return 1.2491f;
		case GasesMaterialList.Nitrogen: return 1.1651f;
		case GasesMaterialList.Oxygen: return 1.3311f;
		case GasesMaterialList.Propane: return 1.8821f;
		case GasesMaterialList.SulfurDioxide: return 2.2791f;
		case GasesMaterialList.WaterVapor: return 0.804f;
		case GasesMaterialList.Helium: return 0.179f;
		case GasesMaterialList.TungstenHexafluoride: return 12.4f;
		default: return 0.0f;break;
		}
	}
		
	public static float getSolidMaterialValue(SolidsMaterialList _SolidsMaterialList)
	{
		switch (_SolidsMaterialList) 
		{
		case SolidsMaterialList.Aerographite: return 0.2f;
		case SolidsMaterialList.MetallicMicrolattice: return 0.9f;
		case SolidsMaterialList.Aerogel: return 1.0f;
		case SolidsMaterialList.Styrofoam: return 75f;
		case SolidsMaterialList.Cork: return 240f;
		case SolidsMaterialList.Pine: return 373f;
		case SolidsMaterialList.Lithium: return 535f;
		case SolidsMaterialList.Wood: return 700f;
		case SolidsMaterialList.Oak: return 710f;
		case SolidsMaterialList.Potassium: return 860f;
		case SolidsMaterialList.Sodium: return 970f;
		case SolidsMaterialList.Ice: return 916.7f;
		case SolidsMaterialList.Nylon: return 1150f;
		case SolidsMaterialList.Plastics: return 1175f;
		case SolidsMaterialList.Magnesium: return 1740f;
		case SolidsMaterialList.Beryllium: return 1850f;
		case SolidsMaterialList.Glycerol: return 1261f;
		case SolidsMaterialList.Concrete: return 2000f;
		case SolidsMaterialList.Silicon: return 2330f;
		case SolidsMaterialList.Aluminium: return 2700f;
		case SolidsMaterialList.Diiodomethane: return 3325f;
		case SolidsMaterialList.Diamond: return 3500f;
		case SolidsMaterialList.Titanium: return 4540f;
		case SolidsMaterialList.Selenium: return 4800f;
		case SolidsMaterialList.Vanadium: return 6100f;
		case SolidsMaterialList.Antimony: return 6690f;
		case SolidsMaterialList.Zinc: return 7000f;
		case SolidsMaterialList.Chromium: return 7200f;
		case SolidsMaterialList.Tin: return 7310f;
		case SolidsMaterialList.Manganese: return 7325f;
		case SolidsMaterialList.Iron: return 7870f;
		case SolidsMaterialList.Steel: return 8050f;
		case SolidsMaterialList.Niobium: return 8570f;
		case SolidsMaterialList.Brass: return 8600f;
		case SolidsMaterialList.Cadmium: return 8650f;
		case SolidsMaterialList.Cobalt: return 8900f;
		case SolidsMaterialList.Nickel: return 8900f;
		case SolidsMaterialList.Copper: return 8940f;
		case SolidsMaterialList.Bismuth: return 9750f;
		case SolidsMaterialList.Molybdenum: return 10220f;
		case SolidsMaterialList.Silver: return 10500f;
		case SolidsMaterialList.Lead: return 11340f;
		case SolidsMaterialList.Thorium: return 11700f;
		case SolidsMaterialList.Rhodium: return 12410f;
		case SolidsMaterialList.Tantalum: return 16600f;
		case SolidsMaterialList.Uranium: return 18800f;
		case SolidsMaterialList.Tungsten: return 19300f;
		case SolidsMaterialList.Gold: return 19320f;
		case SolidsMaterialList.Plutonium: return 19840f;
		case SolidsMaterialList.Platinum: return 21450f;
		case SolidsMaterialList.Iridium: return 22420f;
		case SolidsMaterialList.Osmium: return 22570;
		case SolidsMaterialList.Dama: return 1450;
		case SolidsMaterialList.Human: return 970f; // yes we even got human on the menu!
		default: return 0.0f;break;
		}
	}
}
