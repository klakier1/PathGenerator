using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathGenerator
{
    class FDAT
    {
        int tool_no;
        int base_no;
        string name;

        public FDAT(int tool_no, int base_no, string name)
        {
            this.Tool_no = tool_no;
            this.Base_no = base_no;
            this.Name = name;
        }

        public int Tool_no { get => tool_no; set => tool_no = value; }
        public int Base_no { get => base_no; set => base_no = value; }
        public string Name { get => name; set => name = value; }

        public override string ToString()
        {
            //DECL FDAT FKLB_09_04={TOOL_NO 1,BASE_NO 10,IPO_FRAME #BASE,POINT2[] " ",TQ_STATE FALSE}
            return String.Format("DECL FDAT {0}={{TOOL_NO {1},BASE_NO {2},IPO_FRAME #BASE,POINT2[] \" \",TQ_STATE FALSE}}", Name, Tool_no, Base_no);
        }
    }
}
