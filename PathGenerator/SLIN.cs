using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathGenerator
{
    class SLIN
    {
        double speed;
        E6POS e6pos;
        FDAT fdat;
        LDAT ldat;
        bool cont = false;

        public SLIN(double speed, E6POS e6pos, FDAT fdat, LDAT ldat, bool cont = true)
        {
            this.speed = speed;
            this.e6pos = e6pos;
            this.fdat = fdat;
            this.ldat = ldat;
            this.cont = cont;
        }

        //;FOLD SLIN KLB_01_02 CONT Vel=0.1 m/s L Tool[1]:GUN_01 Base[10]:WorkBase;%{PE}%R 8.3.48,%MKUKATPBASIS,%CSPLINE,%VSLIN_SB,%P 1:SLIN_SB, 2:KLB_01_02, 3:C_SPL, 5:0.1, 7:L
        //SLIN XKLB_01_02 WITH $VEL=SVEL_CP( 0.1, , LL), $TOOL=STOOL2( FKLB_01_02), $BASE= SBASE( FKLB_01_02.BASE_NO),$IPO_MODE=SIPO_MODE( FKLB_01_02.IPO_FRAME), $LOAD=SLOAD( FKLB_01_02.TOOL_NO), $ACC=SACC_CP( LL), $APO=SAPO( LL), $ORI_TYPE=SORI_TYP( LL), $JERK=SJERK( LL) C_SPL
        //;ENDFOLD

        public override string ToString()
        {
            //SLIN XKLB_01_02 WITH $VEL=SVEL_CP( 0.1, , LL), $TOOL=STOOL2( FKLB_01_02), $BASE= SBASE( FKLB_01_02.BASE_NO),$IPO_MODE=SIPO_MODE( FKLB_01_02.IPO_FRAME), $LOAD=SLOAD( FKLB_01_02.TOOL_NO), $ACC=SACC_CP( LL), $APO=SAPO( LL), $ORI_TYPE=SORI_TYP( LL), $JERK=SJERK( LL) C_SPL
            string slin = String.Format("SLIN {0} WITH $VEL=SVEL_CP( {1}, , {2}), $TOOL=STOOL2( {3}), $BASE= SBASE({3}.BASE_NO),$IPO_MODE=SIPO_MODE( {3}.IPO_FRAME), $LOAD=SLOAD( {3}.TOOL_NO), $ACC=SACC_CP( {2}), $APO=SAPO( {2}), $ORI_TYPE=SORI_TYP( {2}), $JERK=SJERK( {2}) C_SPL",
                e6pos.Name, speed, ldat.Name, fdat.Name);

            string fold = String.Format(";FOLD SLIN {0} CONT Vel={1} m/s L Tool[{2}]:GUN_01 Base[{3}]:WorkBase;%{{PE}}%R 8.3.48,%MKUKATPBASIS,%CSPLINE,%VSLIN_SB,%P 1:SLIN_SB, 2:{0}, 3:C_SPL, 5:{1}, 7:L",
                e6pos.Name.Substring(1),
                speed,
                fdat.Tool_no,
                fdat.Base_no);

            string endfold = String.Format(";ENDFOLD");


            return fold + "\r\n" + slin + "\r\n" + endfold;
        }

    }
}
