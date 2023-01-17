using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace DoseMetricExample.Models
{
    public class DoseMetricModel
    {
        private PlanSetup _plan;

        public string Structure { get; set; }
        public string Metric { get; set; }
        public double InputValue { get; set; }
        public List<string> InputUnits { get; set; }
        public string InputUnit { get; set; }
        public List<string> OutputUnits { get; set; }
        public string OutputUnit { get; set; }
        public double OutputValue { get; set; }
        public string Tolerance { get; set; }
        public bool ToleranceMet { get; set; }
        public DoseMetricModel(PlanSetup plan)
        {
            _plan = plan;
        }

        internal void GetOutputValue()
        {
            //put methods here.
            if (Metric == "Dose At Volume")
            {
                OutputValue = _plan.GetDoseAtVolume(_plan.StructureSet.Structures.FirstOrDefault(x => x.Id == Structure),
                    InputValue,
                    InputUnit == "%" ? VolumePresentation.Relative : VolumePresentation.AbsoluteCm3,
                    OutputUnit == "%" ? DoseValuePresentation.Relative : DoseValuePresentation.Absolute).Dose;

            }
            else if (Metric == "Volume At Dose")
            {
                OutputValue = _plan.GetVolumeAtDose(_plan.StructureSet.Structures.FirstOrDefault(x => x.Id == Structure),
                    new DoseValue(InputValue, (InputUnit == "%" ? DoseValue.DoseUnit.Percent : DoseValue.DoseUnit.cGy)),
                    OutputUnit == "%" ? VolumePresentation.Relative : VolumePresentation.AbsoluteCm3);
            }
            else if (Metric == "Volume")
            {
                InputValue = Math.Round(_plan.StructureSet.Structures.FirstOrDefault(x => x.Id == Structure).Volume, 2);
                OutputValue = Math.Round(_plan.StructureSet.Structures.FirstOrDefault(x => x.Id == Structure).Volume,2);
            }
            else if(Metric == "MU")
            {
                double test = 100;
                foreach(Beam x in _plan.Beams)
                {
                    if(x.Meterset.Value != 100)
                    {
                        test = x.Meterset.Value;
                    }
                }
                InputValue = test;
                OutputValue = test;
            }
            else if (Metric == "EffectiveDepth")
            {

                InputValue = Math.Round(_plan.Beams.FirstOrDefault().FieldReferencePoints.FirstOrDefault().EffectiveDepth, 2);
                OutputValue = Math.Round(_plan.Beams.FirstOrDefault().FieldReferencePoints.FirstOrDefault().EffectiveDepth, 2);
            }

            else if (Metric == "HUOverride")
            {

                Structure St = _plan.StructureSet.Structures.FirstOrDefault(x => x.Id == Structure);

                St.GetAssignedHU(out double huvalue);

                InputValue = huvalue;
                OutputValue = huvalue;
            }

            else { throw new ApplicationException("Could not determine metric"); }
            if (Tolerance.Contains("<"))
            {
                ToleranceMet = OutputValue < Convert.ToDouble(Tolerance.TrimStart('<'));
            }
            else if (Tolerance.Contains(">"))
            {
                ToleranceMet = OutputValue > Convert.ToDouble(Tolerance.TrimStart('>'));
            }
            else if (Tolerance.Contains(">="))
            {
                ToleranceMet = OutputValue >= Convert.ToDouble(Tolerance.Trim(new Char[] { '>', '=' }));
            }
            else if (Tolerance.Contains("<="))
            {
                ToleranceMet = OutputValue <= Convert.ToDouble(Tolerance.Trim(new Char[] {'<','='}));
            }
            else if (Tolerance.Contains("="))
            {
                ToleranceMet = Math.Round(OutputValue,2) == Convert.ToDouble(Tolerance.TrimStart('='));
            }
            else if (Tolerance.Contains("%"))
            {
                var test = true;

                if(OutputUnit == "cGy" && OutputValue <= Convert.ToDouble(Tolerance.TrimStart('%')) * 1.01 && OutputValue >= Convert.ToDouble(Tolerance.TrimStart('%')) * 0.99)
                {
                    test = true;
                }
                else if(OutputUnit == "%" && OutputValue <= Convert.ToDouble(Tolerance.TrimStart('%')) +1 && OutputValue >= Convert.ToDouble(Tolerance.TrimStart('%')) - 1)
                {
                    test = true;
                }
                else
                {
                    test = false;
                }
  
                ToleranceMet = test;
            }

            else { throw new ApplicationException("No Tolerance Specified"); }
        }
    }
}
