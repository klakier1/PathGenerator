using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathGenerator
{
    class LDAT
    {
        string name;

        public LDAT(string name)
        {
            this.Name = name;
        }

        public string Name { get => name; set => name = value; }

        public override string ToString()
        {
            //DECL LDAT LL={VEL 2.00000,ACC 100.000,APO_DIST 2.00000,APO_FAC 50.0000,AXIS_VEL 100.000,AXIS_ACC 100.000,ORI_TYP #VAR,CIRC_TYP #BASE,JERK_FAC 50.0000,GEAR_JERK 50.0000,EXAX_IGN 0}

            return String.Format("DECL LDAT {0}={{VEL 2.00000,ACC 100.000,APO_DIST 2.00000,APO_FAC 50.0000,AXIS_VEL 100.000,AXIS_ACC 100.000,ORI_TYP #VAR,CIRC_TYP #BASE,JERK_FAC 50.0000,GEAR_JERK 50.0000,EXAX_IGN 0}}", Name);
        }
    }
}
