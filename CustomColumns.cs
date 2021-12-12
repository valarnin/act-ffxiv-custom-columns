using System.Windows.Forms;
using Advanced_Combat_Tracker;
using System.Reflection;
using System.Collections.Generic;

[assembly: AssemblyTitle("Custom columns")]
[assembly: AssemblyDescription("Adds custom columns for FFXIV")]
[assembly: AssemblyCompany("valarnin")]
[assembly: AssemblyVersion("1.0.0.1")]

namespace ACT_Plugin
{
  public class CustomColumnDef
  {
    public List<string> IncomingAbilities = new List<string>();
    public List<string> OutgoingAbilities = new List<string>();
    public List<string> EitherAbilities = new List<string>();
    public string ExportName;
    public string Name;
    public string Description;

    private List<string> calculatedIncomingAbilities = new List<string>();
    private List<string> calculatedOutgoingAbilities = new List<string>();

    public void Initialize()
    {
      foreach (var ability in this.IncomingAbilities)
        if (!this.calculatedIncomingAbilities.Contains(ability))
          this.calculatedIncomingAbilities.Add(ability);

      foreach (var ability in this.OutgoingAbilities)
        if (!this.calculatedOutgoingAbilities.Contains(ability))
          this.calculatedOutgoingAbilities.Add(ability);

      foreach (var ability in this.EitherAbilities) {
        if (!this.calculatedIncomingAbilities.Contains(ability))
          this.calculatedIncomingAbilities.Add(ability);
        if (!this.calculatedOutgoingAbilities.Contains(ability))
          this.calculatedOutgoingAbilities.Add(ability);
      }

      CombatantData.ColumnDefs.Add(Name,
          new CombatantData.ColumnDef(Name, true, "INT", Name,
            (Data) => {
              return GetCount(Data).ToString();
            },
            (Data) => {
              return GetCount(Data).ToString();
            },
            (Left, Right) => {
              return GetCount(Left).CompareTo(GetCount(Right));
            }
          )
        );

        // Make sure the new column is in the Options tab
        ActGlobals.oFormActMain.ValidateTableSetup();

        CombatantData.ExportVariables.Add(ExportName,
          new CombatantData.TextExportFormatter(ExportName, Name, Description,
            (Data, Extra) => {
              return GetCount(Data).ToString();
            }
          )
        );
    }

    public int GetCount(CombatantData Data) {
      int count = 0;
      AttackType atk = null;

      foreach (var ability in this.calculatedIncomingAbilities)
      {
        if (Data.AllInc.TryGetValue(ability, out atk))
          count += atk.Swings;
      }
      foreach (var ability in this.calculatedOutgoingAbilities)
      {
        if (Data.AllOut.TryGetValue(ability, out atk))
          count += atk.Swings;
      }
      return count;
    }
  }

  public class CustomColumns : IActPluginV1
  {
    Label lblStatus;  // The status label that appears in ACT's Plugin tab

    List<CustomColumnDef> Columns;

    public void DeInitPlugin()
    {
      lblStatus.Text = "Plugin Exited";
    }

    public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
    {
      lblStatus = pluginStatusText;  // Hand the status label's reference to our local var
      this.Columns = new List<CustomColumnDef>();
      Columns.Add(new CustomColumnDef()
      {
        ExportName = "rezes",
        Name = "Rezes",
        Description = "Number of resurrections cast by the combatant.",
        OutgoingAbilities = new List<string>(){
          "Resurrection",
          "Raise",
          "Ascend",
          "Angel Whisper",
          "Verraise",
          "Lost Sacrifice",
          "Lost Arise",
          "Resistance Phoenix",
          "Egeiro"
        },
      });
      Columns.Add(new CustomColumnDef()
      {
        ExportName = "dmgdown",
        Name = "Dmg Down",
        Description = "Number of Damage Downs received by the combatant.",
        IncomingAbilities = new List<string>(){
          "Damage Down"
        },
      });
      Columns.Add(new CustomColumnDef()
      {
        ExportName = "vulns",
        Name = "Vulns",
        Description = "Number of Vulnerability Ups received by the combatant.",
        IncomingAbilities = new List<string>(){
          "Vulnerability Up"
        },
      });
      InitColumns();
      ActGlobals.oFormActMain.ValidateLists();  // Make sure the new variable appears in the preset creator
      lblStatus.Text = "Plugin Started";
    }

    private void InitColumns()
    {
      foreach (var column in this.Columns)
      {
        column.Initialize();
      }
    }
  }
}
