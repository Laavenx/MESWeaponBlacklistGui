using System.Collections.Generic;
using System.IO;
using System.Text;
using CoreSystems.Api;
using Sandbox.Definitions;
using Sandbox.Game.Gui;
using Sandbox.Graphics.GUI;
using VRage.Game;
using VRage.Library.Collections;
using VRage.Utils;
using VRageMath;
using Sandbox.ModAPI;
using System.Xml;

// ReSharper disable VirtualMemberCallInConstructor

namespace Blacklist.Gui
{
    class WeaponDialog : MyGuiScreenDebugBase
    {
        private MyGuiControlButton saveButton;
        private MyGuiControlTable toolbarTable;
        private readonly string caption;
        public WeaponDialog(string caption)
             : base(new Vector2(0.5f, 0.5f), new Vector2(1f, 0.975f), new Color(100, 255, 255, 0), true)
        {
            this.caption = caption;

            RecreateControls(true);

            CanBeHidden = true;
            CanHideOthers = true;
            CloseButtonEnabled = true;
        }

        public override void RecreateControls(bool constructor)
        {
            base.RecreateControls(constructor);

            AddCaption(caption, Color.Red.ToVector4(), new Vector2(0.0f, 0.003f), 1.9f);

            CreateListBox();
            CreateButtons();
        }

        private Vector2 DialogSize => m_size ?? Vector2.One;

        private void CreateListBox()
        {
            toolbarTable = new MyGuiControlTable
            {
                Position = new Vector2(0.001f, -0.5f * DialogSize.Y + 0.1f),
                Size = new Vector2(0.85f * DialogSize.X, DialogSize.Y - 0.25f),
                OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP,
                ColumnsCount = 3,
                VisibleRowsCount = 20,
            };
            
            toolbarTable.SetCustomColumnWidths(new[] { 0.8f, 0.1f, 0.1f });
            toolbarTable.SetColumnName(0, new StringBuilder("Name"));
            toolbarTable.SetColumnName(1, new StringBuilder("Grid Size"));
            toolbarTable.SetColumnName(2, new StringBuilder("Check"));
            toolbarTable.SortByColumn(0);
            
            ListWeapons();
            Controls.Add(toolbarTable);
        }
        private void CreateButtons()
        {
            saveButton = new MyGuiControlButton(
                visualStyle: MyGuiControlButtonStyleEnum.Default,
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                text: new StringBuilder("Save"), onButtonClick: OnSave);

            var xs = 0.85f * DialogSize.X;
            var y = 0.5f * (DialogSize.Y - 0.15f);
            saveButton.Position = new Vector2(-0.39f * xs, y);

            saveButton.SetToolTip("Saves config");
            Controls.Add(saveButton);
        }
        private void OnSave(MyGuiControlButton _)
        {
            var dir = Path.Combine(MyAPIGateway.Session.CurrentPath, "Storage\\1521905890.sbm_ModularEncountersSystems\\Config-Grids.xml");
            if (File.Exists(dir))
            {
                var brokenWeapons = new List<string>() {
                        "1380830774",
                        "Large_SC_LaserDrill_HiddenStatic",
                        "Large_SC_LaserDrill_HiddenTurret",
                        "Large_SC_LaserDrill",
                        "Large_SC_LaserDrillTurret",
                        "Spotlight_Turret_Large",
                        "Spotlight_Turret_Light_Large",
                        "Spotlight_Turret_Small",
                        "SmallSpotlight_Turret_Small",
                        "ShieldChargerBase_Large",
                        "LDualPulseLaserBase_Large",
                        "AegisLargeBeamBase_Large",
                        "AegisMediumeamBase_Large",
                        "XLGigaBeamGTFBase_Large",
                        "XLDualPulseLaserBase_Large",
                        "1817300677"
                };

                var xmlfile = new XmlDocument();
                var indexmax = toolbarTable.RowsCount;
                xmlfile.LoadXml(File.ReadAllText(dir));
                XmlNodeList targetElement = xmlfile.GetElementsByTagName("WeaponReplacerBlacklist");
                XmlElement target = (XmlElement)targetElement[0];
                target.RemoveAll();

                foreach(string weapon in brokenWeapons)
                {
                    XmlElement elem = xmlfile.CreateElement("string");
                    XmlText text = xmlfile.CreateTextNode(weapon);
                    target.AppendChild(elem);
                    target.LastChild.AppendChild(text);
                }

                for (int i = 0; i < indexmax; i++)
                {
                    var data = toolbarTable.GetRow(i).GetCell(2);
                    var weapondata = toolbarTable.GetRow(i).GetCell(0).UserData.ToString();
                    var castedData = (MyGuiControlCheckbox)data.Control;
                    var Check = castedData.IsChecked;
                    if(Check)
                    {
                        XmlElement elem = xmlfile.CreateElement("string");
                        XmlText text = xmlfile.CreateTextNode(weapondata);
                        target.AppendChild(elem);
                        target.LastChild.AppendChild(text);
                    }
                }
                xmlfile.Save(dir);
                CreateDialog("Weapon blacklist saved");
            }
            else
            {
                CreateDialog("Weapon blacklist couldn't be saved");
            }
        }

        private void CreateDialog(string name)
        {
        MyGuiSandbox.AddScreen(
            MyGuiSandbox.CreateMessageBox(buttonType: MyMessageBoxButtonsType.OK,
                size: new Vector2(0.3f, 0.2f),
                messageText: new StringBuilder(name)));
        }

        private static List<string> CheckedWeapons()
        {
            var dir = Path.Combine(MyAPIGateway.Session.CurrentPath, "Storage\\1521905890.sbm_ModularEncountersSystems\\Config-Grids.xml");
            if (File.Exists(dir))
            {
                var xmlfile = new XmlDocument();
                xmlfile.LoadXml(File.ReadAllText(dir));
                XmlNodeList targetElement = xmlfile.GetElementsByTagName("WeaponReplacerBlacklist");
                XmlElement target = (XmlElement)targetElement[0];
                List<string> CheckedList = new List<string>();
                foreach (XmlNode node in target)
                {
                    CheckedList.Add(node.InnerText);
                }
                return CheckedList;
            }
            else
            {
                return null;
            }

        }

        private void ListWeapons()
        {
            var CheckedList = CheckedWeapons();
            if(CheckedList != null)
            {
                var weaponList = WeaponList();
                foreach (var weapon in weaponList)
                {
                    var WeaponDef = MyDefinitionManager.Static.GetDefinition(weapon);
                    var CubeDef = MyDefinitionManager.Static.GetCubeBlockDefinition(weapon);
                    //var CubeSize = MyDefinitionManager.Static.GetCubeSize(CubeDef.CubeSize);
                    var row = new MyGuiControlTable.Row();
                    MyGuiControlTable.Cell disabledCell = new MyGuiControlTable.Cell();
                    var Check = CheckedList.Contains(weapon.SubtypeName);
                    MyGuiControlCheckbox disabledBox = new MyGuiControlCheckbox(isChecked: Check);
                    toolbarTable.Controls.Add(disabledBox);
                    disabledCell.Control = disabledBox;
                    row.AddCell(new MyGuiControlTable.Cell(WeaponDef.DisplayNameText, WeaponDef.Id.SubtypeName, WeaponDef.ToString()));
                    row.AddCell(new MyGuiControlTable.Cell(CubeDef.CubeSize.ToString()));
                    row.AddCell(disabledCell);
                    toolbarTable.Add(row);
                }
            }
            else
            {
                var row = new MyGuiControlTable.Row();
                row.AddCell(new MyGuiControlTable.Cell("Couldn't load blacklist"));
                row.AddCell(new MyGuiControlTable.Cell(""));
                row.AddCell(new MyGuiControlTable.Cell(""));
                toolbarTable.Add(row);
            }

        }

        private static ICollection<MyDefinitionId> WeaponList()
        {
            var defTypes = new List<string>()
            {
                "MyObjectBuilder_SmallMissileLauncher",
                "MyObjectBuilder_SmallMissileLauncherReload",
                "MyObjectBuilder_SmallGatlingGun",
                "MyObjectBuilder_LargeGatlingTurret",
                "MyObjectBuilder_LargeMissileTurret",
                "MyObjectBuilder_InteriorTurret"
            };
            var definitions = MyDefinitionManager.Static.GetAllDefinitions();
            var weaponArray = new List<string>();
            var GetWeapons = new WcApi();
            GetWeapons.Load();
            var weaponList = new MyList<MyDefinitionId>();
            GetWeapons.GetAllCoreWeapons(weaponList);
            GetWeapons.Unload();
            foreach (MyDefinitionBase definition in definitions)
            {
                if(defTypes.Contains(definition.Id.TypeId.ToString()) && !weaponList.Contains(definition.Id))
                {
                    weaponList.Add(definition.Id);
                }
            }
            return weaponList;
        }

        private string SelectedName => toolbarTable.SelectedRow?.GetCell(0)?.Text?.ToString() ?? "";

        public override string GetFriendlyName() => "ListDialog";
    }
}