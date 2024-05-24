using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP = Tekla.Structures.Plugins;
using TSM = Tekla.Structures.Model;
using TSMUI = Tekla.Structures.Model.UI;
using TSD = Tekla.Structures.Dialog;
using System.Collections;
using TSG = Tekla.Structures.Geometry3d;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using System.Windows.Forms;

namespace Input_Beam
{
    public class UI_Data
    {
        [TSP.StructuresField(nameof(Ui_Class))]
        public int Ui_Class = 6;
    }
    [TSP.PluginUserInterface(nameof(Input_Beam) + "." + nameof(Preset))]
    [TSP.Plugin(nameof(Input_Beam))]
    public class Input_Beam : TSP.PluginBase
    {
        public TSM.Model Model { get; }
        public UI_Data UI_Data { get; }
        public Input_Beam(UI_Data data)
        {
            Model = new TSM.Model();
            UI_Data = data;
        }
        public override List<InputDefinition> DefineInput()
        {
            var picker = new TSMUI.Picker();
            var message = "";
            List<TSG.Point> pp;
            do
            {
                pp = picker.PickPoints(TSMUI.Picker.PickPointEnum.PICK_TWO_POINTS, message + "Укажите точки").Cast<TSG.Point>().ToList();
                message = "Неправильный выбор. ";
            }
            while (pp.Count < 2);
            TSM.Operations.Operation.DisplayPrompt("Выбраны точки");
            return new List<InputDefinition>
            {
                new InputDefinition(pp[0], pp[1]),
            };
        }
        void GetValuesFromDialog()
        {
            var default_data = new UI_Data();
            if (this.IsDefaultValue(this.UI_Data.Ui_Class)) this.UI_Data.Ui_Class = default_data.Ui_Class;
        }
        void SetValuesFromDialog()
        {
            var plug = Model.SelectModelObject(this.Identifier);
            if (plug != null)
            {
                plug.SetUserProperty(nameof(UI_Data.Ui_Class), this.UI_Data.Ui_Class);
            }
        }
        public override bool Run(List<InputDefinition> Input)
        {
            try
            {
                GetValuesFromDialog();
                var points = (Input[0].GetInput() as ArrayList).Cast<TSG.Point>().ToArray();
                Create_Beam(points);
                SetValuesFromDialog();
                Model.CommitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        bool Create_Beam(params TSG.Point[] points)
        {
            return new TSM.Beam(points[0], points[1])
            {
                Profile = new TSM.Profile { ProfileString = "L100*50*10" },
                Material = new TSM.Material { MaterialString = "Steel_Undefinded"},
                Class  = this.UI_Data.Ui_Class.ToString(),
            }.Insert();
        }
        public static void Main()
        {
            Tekla.Structures.Datatype.Settings.SetValue("language", "RUSSIAN");
            var i = 3;
            Action
                ai = delegate
                {
                    var f = new Preset();
                    f.Show();
                    Application.Run(f);
                },
                ar = delegate
                {
                    var data = new UI_Data();
                    var plagin = new Input_Beam(data);
                    plagin.Run(plagin.DefineInput());
                };
            switch (i)
            {
                case 0:
                    ai();
                    break;
                case 1:
                    ai();
                    ar();
                    break;
                case 2:
                    ar();
                    ai();
                    break;
                case 3:
                    ar();
                    break;
            }
        }
    }
    public class Preset : TSD.PluginFormBase
    {
        SWF.TableLayoutPanel Main_Panel
        {
            get
            {
                if(_Main_Panel == null)
                {
                    _Main_Panel = new SWF.TableLayoutPanel
                    {
                        AutoSize = true,
                        AutoSizeMode = SWF.AutoSizeMode.GrowAndShrink,
                    };
                    _Main_Panel.Controls.Add(SaveLoad, 0, 0);
                    _Main_Panel.Controls.Add(Ui_Class_CheckBox, 0, 1);
                    _Main_Panel.Controls.Add(Ui_Class_TextBox, 1, 1);
                    _Main_Panel.Controls.Add(OkApplyModifyGetOnOffCancel, 0, 2);
                    _Main_Panel.SetColumnSpan(SaveLoad, 3);
                    _Main_Panel.SetColumnSpan(OkApplyModifyGetOnOffCancel, 3);
                }
                return _Main_Panel;
            }
        }
        SWF.TableLayoutPanel _Main_Panel = null;
        SWF.CheckBox Ui_Class_CheckBox
        {
            get
            {
                if(_Ui_Class_CheckBox == null)
                {
                    _Ui_Class_CheckBox = new SWF.CheckBox
                    {
                        AutoSize = true,
                        Text = "Класс",
                        CheckState = SWF.CheckState.Checked,
                    };
                    structuresExtender.SetAttributeName(_Ui_Class_CheckBox, nameof(UI_Data.Ui_Class));
                    structuresExtender.SetAttributeTypeName(_Ui_Class_CheckBox, null);
                    structuresExtender.SetBindPropertyName(_Ui_Class_CheckBox, null);
                    structuresExtender.SetIsFilter(_Ui_Class_CheckBox, true);
                }
                return _Ui_Class_CheckBox;
            }
        }
        SWF.CheckBox _Ui_Class_CheckBox = null;
        SWF.TextBox Ui_Class_TextBox
        {
            get
            {
                if (_Ui_Class_TextBox == null)
                {
                    _Ui_Class_TextBox = new SWF.TextBox
                    {
                        Size = new SD.Size(150, 25),                        
                    };
                    structuresExtender.SetAttributeName(_Ui_Class_TextBox, nameof(UI_Data.Ui_Class));
                    structuresExtender.SetAttributeTypeName(_Ui_Class_TextBox, "Integer");
                    structuresExtender.SetBindPropertyName(_Ui_Class_TextBox, null);
                }
                return _Ui_Class_TextBox;
            }
        }
        SWF.TextBox _Ui_Class_TextBox = null;
        TSD.UIControls.OkApplyModifyGetOnOffCancel OkApplyModifyGetOnOffCancel
        {
            get
            {
                if(_OkApplyModifyGetOnOffCancel == null)
                {
                    _OkApplyModifyGetOnOffCancel = new TSD.UIControls.OkApplyModifyGetOnOffCancel
                    {
                        MinimumSize = new SD.Size(519, 29),
                        AutoSize = true,
                        AutoSizeMode = SWF.AutoSizeMode.GrowAndShrink,
                    };
                    _OkApplyModifyGetOnOffCancel.ApplyClicked += delegate
                    {
                        this.Apply();
                    };
                    _OkApplyModifyGetOnOffCancel.ModifyClicked += delegate
                    {
                        this.Modify();
                    };
                    _OkApplyModifyGetOnOffCancel.CancelClicked += delegate
                    {
                        this.Close();
                    };
                    _OkApplyModifyGetOnOffCancel.GetClicked += delegate
                    {
                        this.Get();
                    };
                    _OkApplyModifyGetOnOffCancel.OnOffClicked += delegate
                    {
                        this.ToggleSelection();
                    };
                    _OkApplyModifyGetOnOffCancel.OkClicked += delegate
                    {
                        this.Apply();
                        this.Close();
                    };
                }
                return _OkApplyModifyGetOnOffCancel;
            }
        } 

        TSD.UIControls.OkApplyModifyGetOnOffCancel _OkApplyModifyGetOnOffCancel = null;
        TSD.UIControls.SaveLoad SaveLoad
        {
            get
            {
                if(_SaveLoad == null)
                {
                    _SaveLoad = new TSD.UIControls.SaveLoad
                    {
                        MinimumSize = new SD.Size(519, 43),
                        AutoSize = true,
                        AutoSizeMode = SWF.AutoSizeMode.GrowAndShrink,
                    };
                }
                return _SaveLoad;
            }
        }
        TSD.UIControls.SaveLoad _SaveLoad = null;
        public Preset()
        {
            this.Controls.Add(Main_Panel);
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Text = "Плагин для вставки балки";
        }
    }
}