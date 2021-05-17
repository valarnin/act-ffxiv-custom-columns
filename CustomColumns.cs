using System.Windows.Forms;
using Advanced_Combat_Tracker;
using System.Reflection;

[assembly: AssemblyTitle("Custom columns")]
[assembly: AssemblyDescription("Adds custom columns for FFXIV")]
[assembly: AssemblyCompany("valarnin")]
[assembly: AssemblyVersion("1.0.0.1")]

namespace ACT_Plugin
{
  public class PluginSample : IActPluginV1
  {
    Label lblStatus;  // The status label that appears in ACT's Plugin tab

    public void DeInitPlugin()
    {
      lblStatus.Text = "Plugin Exited";
    }

    public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
    {
      lblStatus = pluginStatusText;  // Hand the status label's reference to our local var
      InitRezesColumn();
      InitDamageDownColumn();
      InitVulnerabilityUpColumn();
      ActGlobals.oFormActMain.ValidateLists();  // Make sure the new variable appears in the preset creator
      lblStatus.Text = "Plugin Started";
    }

    private void InitRezesColumn() {
      CombatantData.ColumnDefs.Add("Rezes",
        new CombatantData.ColumnDef("Rezes", true, "INT", "Rezes",
          (Data) => {
            return GetRessurectionCount(Data).ToString();
          },
          (Data) => {
            return GetRessurectionCount(Data).ToString();
          },
          (Left, Right) => {
            return GetRessurectionCount(Left).CompareTo(GetRessurectionCount(Right));
          }
        )
      );

      // Make sure the new column is in the Options tab
      ActGlobals.oFormActMain.ValidateTableSetup();

      CombatantData.ExportVariables.Add("rezes",
        new CombatantData.TextExportFormatter("rezes", "Ressurections", "Number of ressurections cast by the combatant.",
          (Data, Extra) => {
            return GetRessurectionCount(Data).ToString();
          }
        )
      );
    }

    private int GetRessurectionCount(CombatantData Combatant)
    {
      int count = 0;

      AttackType atk = null;

      if (Combatant.AllOut.TryGetValue("Resurrection", out atk))
        count += atk.Swings;

      if (Combatant.AllOut.TryGetValue("Raise", out atk))
        count += atk.Swings;

      if (Combatant.AllOut.TryGetValue("Ascend", out atk))
        count += atk.Swings;

      return count;
    }

    private void InitDamageDownColumn() {
      CombatantData.ColumnDefs.Add("Dmg Down",
        new CombatantData.ColumnDef("Dmg Down", true, "INT", "Dmg Down",
          (Data) => {
            return GetDamageDownCount(Data).ToString();
          },
          (Data) => {
            return GetDamageDownCount(Data).ToString();
          },
          (Left, Right) => {
            return GetDamageDownCount(Left).CompareTo(GetDamageDownCount(Right));
          }
        )
      );

      // Make sure the new column is in the Options tab
      ActGlobals.oFormActMain.ValidateTableSetup();

      CombatantData.ExportVariables.Add("dmgdown",
        new CombatantData.TextExportFormatter("dmgdown", "Damage Down", "Number of Damage Downs received by the combatant.",
          (Data, Extra) => {
            return GetDamageDownCount(Data).ToString();
          }
        )
      );
    }

    private int GetDamageDownCount(CombatantData Combatant)
    {
      int count = 0;

      AttackType atk = null;

      if (Combatant.AllInc.TryGetValue("Damage Down", out atk))
        count += atk.Swings;

      return count;
    }

    private void InitVulnerabilityUpColumn() {
      CombatantData.ColumnDefs.Add("Vulns",
        new CombatantData.ColumnDef("Vulns", true, "INT", "Vulns",
          (Data) => {
            return GetVulnerabilityUpCount(Data).ToString();
          },
          (Data) => {
            return GetVulnerabilityUpCount(Data).ToString();
          },
          (Left, Right) => {
            return GetVulnerabilityUpCount(Left).CompareTo(GetVulnerabilityUpCount(Right));
          }
        )
      );

      // Make sure the new column is in the Options tab
      ActGlobals.oFormActMain.ValidateTableSetup();

      CombatantData.ExportVariables.Add("vulns",
        new CombatantData.TextExportFormatter("vulns", "Vulnerability Up", "Number of Vulnerability Ups received by the combatant.",
          (Data, Extra) => {
            return GetVulnerabilityUpCount(Data).ToString();
          }
        )
      );
    }

    private int GetVulnerabilityUpCount(CombatantData Combatant)
    {
      int count = 0;

      AttackType atk = null;

      if (Combatant.AllInc.TryGetValue("Vulnerability Up", out atk))
        count += atk.Swings;

      return count;
    }
  }
}
